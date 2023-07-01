

/* This is allocator is based on boundary tag coalescing, 
 * using first fir placement and implicit free lists.
 * Much of the code is heavely inspired from the course book, and its sourc code
 * available at at http://csapp.cs.cmu.edu/3e/code.html
 * I choose to take use this code to easier understand malloc
 * as well as to have more time to play with the code. 
 * I have althought removed the comments from the book source code,
 * and used my own thoughts and understandiong instead 
 *
 * The Blocks is aligned to doublewords with 8 byte boundries 
 * with a minimum block size of 16 bytes.
 */

#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <unistd.h>
#include <string.h>
#include "mm.h"
#include "memlib.h"

/*********************************************************
 * NOTE TO STUDENTS: Before you do anything else, please
 * provide your team information in the following struct.
 ********************************************************/
team_t team = {
    /* Team name */
    "",
    /* First member's full name */
    "",
    /* First member's email address */
    "",
    /* Second member's full name (leave blank if none) */
    "",
    /* Second member's email address (leave blank if none) */
    ""
};

// single word (4) or double word (8) alignment
#define ALIGNMENT 8

// rounds up to the nearest multiple of ALIGNMENT 
#define ALIGN(size) (((size) + (ALIGNMENT-1)) & ~0x7)

#define SIZE_T_SIZE (ALIGN(sizeof(size_t)))


// define using next fit, else use first fit search
#define NEXT_FITx


//constants and macros, again mostly from the books code
#define WSIZE       4
#define DSIZE       8
#define CHUNKSIZE  (1<<12)

#define MAX(x, y) ((x) > (y)? (x) : (y))

//put size and bit into a word 
#define PACK(size, alloc)  ((size) | (alloc))

//read-write / a word at location p
#define GET(p)       (*(unsigned int *)(p))
#define PUT(p, val)  (*(unsigned int *)(p) = (val))

//get size and allocatet fields at p 
#define GET_SIZE(p)  (GET(p) & ~0x7)
#define GET_ALLOC(p) (GET(p) & 0x1)

//compute address of head and foot
#define HDRP(bp)       ((char *)(bp) - WSIZE)
#define FTRP(bp)       ((char *)(bp) + GET_SIZE(HDRP(bp)) - DSIZE)

//compute the address of the next and previous blocks
#define NEXT_BLKP(bp)  ((char *)(bp) + GET_SIZE(((char *)(bp) - WSIZE)))
#define PREV_BLKP(bp)  ((char *)(bp) - GET_SIZE(((char *)(bp) - DSIZE)))


//global variables
static char *heap_listp = 0;
#ifdef NEXT_FIT
static char *rover;
#endif

//Helper functions
static void *extend_heap(size_t words);
static void place(void *bp, size_t asize);
static void *find_fit(size_t asize);
static void *coalesce(void *bp);
static void printblock(void *bp);
static void checkheap(int verbose);
static void checkblock(void *bp);

//initialization of memory manager
int mm_init(void) 
{
    // We start by making an empthy heap
    if ((heap_listp = mem_sbrk(4*WSIZE)) == (void *)-1) return -1;

    PUT(heap_listp, 0);                          
    PUT(heap_listp + (1*WSIZE), PACK(DSIZE, 1));  
    PUT(heap_listp + (2*WSIZE), PACK(DSIZE, 1));  
    PUT(heap_listp + (3*WSIZE), PACK(0, 1));     
    heap_listp += (2*WSIZE);                       

#ifdef NEXT_FIT
    rover = heap_listp;
#endif

    //extend heap with free block
    if (extend_heap(CHUNKSIZE/WSIZE) == NULL) return -1;
    return 0;
}


 
//allocate the block with the lowest size
void *mm_malloc(size_t size) 
{
    size_t asize;      
    size_t extsize;
    char *bp;      

    if (heap_listp == 0) mm_init();

    if (size == 0) return NULL;

    if (size <= DSIZE) asize = 2*DSIZE;                                        
    else asize = DSIZE * ((size + (DSIZE) + (DSIZE-1)) / DSIZE); 

    if ((bp = find_fit(asize)) != NULL) {  
        place(bp, asize);                  
        return bp;
    }

    extsize = MAX(asize,CHUNKSIZE);                 
    if ((bp = extend_heap(extsize/WSIZE)) == NULL) return NULL;                             
    place(bp, asize);                                 
    return bp;
} 


// free a block
void mm_free(void *bp)
{
    if (bp == 0) return;

    size_t size = GET_SIZE(HDRP(bp));

    if (heap_listp == 0) mm_init();

    PUT(HDRP(bp), PACK(size, 0));
    PUT(FTRP(bp), PACK(size, 0));
    coalesce(bp);
}


//boundary tag coalescing, returning pointer to the coalesced block
static void *coalesce(void *bp) 
{
    size_t prev_alloc = GET_ALLOC(FTRP(PREV_BLKP(bp)));
    size_t next_alloc = GET_ALLOC(HDRP(NEXT_BLKP(bp)));
    size_t size = GET_SIZE(HDRP(bp));

    if (prev_alloc && next_alloc) return bp;   //The previous and next blocks are both allocated

    else if (prev_alloc && !next_alloc) {      //The previous block is allocated and the next block is free
        size += GET_SIZE(HDRP(NEXT_BLKP(bp)));
        PUT(HDRP(bp), PACK(size, 0));
        PUT(FTRP(bp), PACK(size,0));
    }

    else if (!prev_alloc && next_alloc) {      //The previous block is free and the next block is allocated
        size += GET_SIZE(HDRP(PREV_BLKP(bp)));
        PUT(FTRP(bp), PACK(size, 0));
        PUT(HDRP(PREV_BLKP(bp)), PACK(size, 0));
        bp = PREV_BLKP(bp);
    }

    else {                                     //The previous and next blocks are both free
        size += GET_SIZE(HDRP(PREV_BLKP(bp))) + GET_SIZE(FTRP(NEXT_BLKP(bp)));
        PUT(HDRP(PREV_BLKP(bp)), PACK(size, 0));
        PUT(FTRP(NEXT_BLKP(bp)), PACK(size, 0));
        bp = PREV_BLKP(bp);
    }

    // ensure that the rover does not point to the block we just coaleced
#ifdef NEXT_FIT
    if ((rover > (char *)bp) && (rover < NEXT_BLKP(bp))) rover = bp;
#endif

    return bp;
}


 // Relocation implementation
void *mm_realloc(void *ptr, size_t size)
{
    size_t oldsize;
    void *newptr;

    //if the size is 0, we can just return null as the block is already free
    if(size == 0) {
        mm_free(ptr);
        return 0;
    }

    //if *ptr is null, we can confirm this is malloc
    if(ptr == NULL) {
        return mm_malloc(size);
    }

    //assign newptr
    newptr = mm_malloc(size);

    // if pointing to a new place fails, leave everything untouched
    if(!newptr) {
        return 0;
    }

    //copying old data
    oldsize = GET_SIZE(HDRP(ptr));
    if(size < oldsize) oldsize = size;
    memcpy(newptr, ptr, oldsize);

    //free block of old data
    mm_free(ptr);

    return newptr;
}

//check the heap for correctness
void mm_check(int verbose)  
{ 
    checkheap(verbose);
}


//extend the heap and return is pointer
static void *extend_heap(size_t words) 
{
    char *bp;
    size_t size;

    size = (words % 2) ? (words+1) * WSIZE : words * WSIZE; 

    if ((long)(bp = mem_sbrk(size)) == -1) return NULL;                                        

    PUT(HDRP(bp), PACK(size, 0));         
    PUT(FTRP(bp), PACK(size, 0));
    PUT(HDRP(NEXT_BLKP(bp)), PACK(0, 1));

    return coalesce(bp);                                          
}


//Places a block at the start of the free block bp 
static void place(void *bp, size_t asize)
{
    size_t currsize = GET_SIZE(HDRP(bp));   

    if ((currsize - asize) >= (2*DSIZE)) { 
        PUT(HDRP(bp), PACK(asize, 1));
        PUT(FTRP(bp), PACK(asize, 1));
        bp = NEXT_BLKP(bp);
        PUT(HDRP(bp), PACK(currsize-asize, 0));
        PUT(FTRP(bp), PACK(currsize-asize, 0));
    }
    else { 
        PUT(HDRP(bp), PACK(currsize, 1));
        PUT(FTRP(bp), PACK(currsize, 1));
    }
}


 //Find a fit for a block with a given bytesize 
static void *find_fit(size_t asize)
{
#ifdef NEXT_FIT 
    char *oldrover = rover;

    for ( ; GET_SIZE(HDRP(rover)) > 0; rover = NEXT_BLKP(rover))
        if (!GET_ALLOC(HDRP(rover)) && (asize <= GET_SIZE(HDRP(rover))))return rover;
    for (rover = heap_listp; rover < oldrover; rover = NEXT_BLKP(rover))
        if (!GET_ALLOC(HDRP(rover)) && (asize <= GET_SIZE(HDRP(rover)))) return rover;
    return NULL;
#else 
    void *bp;

    for (bp = heap_listp; GET_SIZE(HDRP(bp)) > 0; bp = NEXT_BLKP(bp)) {
        if (!GET_ALLOC(HDRP(bp)) && (asize <= GET_SIZE(HDRP(bp)))) return bp;
    }
    return NULL; 
#endif
}


static void printblock(void *bp) 
{
    size_t hsize, halloc, fsize, falloc;
    checkheap(0);
    hsize = GET_SIZE(HDRP(bp));
    halloc = GET_ALLOC(HDRP(bp));  
    fsize = GET_SIZE(FTRP(bp));
    falloc = GET_ALLOC(FTRP(bp));  

    if (hsize == 0) {
        printf("%p: EOL\n", bp);
        return;
    }

    printf("%p: header: [%ld:%c] footer: [%ld:%c]\n",
            bp, hsize, (halloc ? 'a' : 'f'), 
            fsize, (falloc ? 'a' : 'f')); 
}

static void checkblock(void *bp) 
{
    if ((size_t)bp % 8) printf("Error! %p is not doubleword aligned\n", bp);
    
    if (GET(HDRP(bp)) != GET(FTRP(bp)))printf("Error! The header does not match the footer\n");
}


 //minimal heap consistency check
void checkheap(int verbose) 
{
    char *bp = heap_listp;

    if (verbose) printf("Heap (%p):\n", heap_listp);

    if ((GET_SIZE(HDRP(heap_listp)) != DSIZE) || !GET_ALLOC(HDRP(heap_listp))) 
        printf("Bad prologue header\n");

    checkblock(heap_listp);

    for (bp = heap_listp; GET_SIZE(HDRP(bp)) > 0; bp = NEXT_BLKP(bp)) {
        if (verbose) printblock(bp);
        checkblock(bp);
    }

    if (verbose) printblock(bp);
    
    if ((GET_SIZE(HDRP(bp)) != 0) || !(GET_ALLOC(HDRP(bp))))
        printf("Bad epilogue header\n");
}
