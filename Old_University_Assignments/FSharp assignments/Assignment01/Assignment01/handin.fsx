

// exercise 1.1
let sqr x = x*x;;
// 1.1 demo
sqr 3;;
sqr 7;;

// exercise 1.2
let pow x n = System.Math.Pow(x,n);;
// 1.2 demo
pow 2.2 5.3;;
pow 1.2 2.0;;


// exercise 1.3
let rec sum = function
|   0 -> 0
|   x -> x+(sum(x-1))
// 1.3 demo
sum 3;;
sum 4;;

// exercise 1.4
let rec fib = function
| 0 -> 0
| 1 -> 1
| x -> fib(x-1)+fib(x-2)
// 1.4 demo
fib 4;;
fib 6;;

(* exercise 1.5

(System.Math.PI, fact -1) = StackOverflowException.

fact(fact 4) = -int (overflow)

power(System.Math.PI, fact 2) = float

(power, fact) = float * int

*)

// exercise 1.6
let dup x = string x+x;;
// 1.6 demo
dup "hi ";;
dup "yay ";;

// exercise 1.7
let dupn s n = String.replicate n s
// 1.7 demo
dupn "hi " 3;;
dupn "yay " 10;;

// exercise 1.8
let rec bin (n, k) =
    match (n, k) with
    | (n, 0) -> 1
    | (n, k) when k = n -> 1
    | (n, k) -> bin(n-1, k-1) + bin(n-1, k)
// 1.8 demo
bin (2,1);;
bin (4,2);;

(* exercise 1.9
 1. int * int -> int
 2. when x = 0
 3. f (2, 3)
    -> f (2-1, 2*3)
    -> f (1, 6)
    -> f (1-1, 1*6)
    -> f (0, 6)
    -> 6
 4. an exponential function
 *)

 (* exercise 1.10
 let test(c,e) = if c then e else 0
 1. bool * int -> int
 2. StackOverflowException.
 3. this would return 0
 *)

// exercise 1.11
let curry f = fun a -> fun b -> f(a,b);;
let uncurry g = fun (a,b) -> g a b;;

//scrabble here we go!
//exercise 1.12
let empty (letter, pointValue) = fun pos -> letter, pointValue;;
// 1.12 demo
empty ('Q', 10) 34;;
empty ('O', 2) -21123;;


//exercise 1.13
type word = int -> char * int;;
let add newPos (letter, pointValue) (word:word) = fun pos ->
    match newPos with
    |   x when x = pos -> (letter,pointValue)
    |   x -> word pos

//exercise 1.14
let hello = empty(char 0,0) |> add 0 ('H',4) |> add 1 ('E',1)  |> add 2 ('L',1)
            |> add 3 ('L',1) |> add 4 ('O',1)

//exercise 1.15
let singleLetterScore (word:word) pos = 
    match (empty(word pos) pos) with
    (x,y) -> y

let doubleLetterScore (word:word) pos = 
    match (empty(word pos) pos) with
    (x,y) -> y*2

let trippleLetterScore (word:word) pos = 
    match (empty(word pos) pos) with
    (x,y) -> y*3

    