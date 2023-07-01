module MultiSet    
    type MultiSet
   
    val empty : MultiSet

    val isEmpty : MultiSet -> bool

    val size : MultiSet -> uint32
    
    val contains : 'a -> MultiSet -> bool

    val numItems : 'a -> MultiSet -> uint32

    val add : 'a -> uint32 -> MultiSet -> MultiSet

    val addSingle : 'a -> MultiSet -> MultiSet

    val remove : 'a -> uint32 -> MultiSet -> MultiSet

    val removeSingle : 'a -> MultiSet -> MultiSet

    val fold : ('a -> 'b -> uint32 -> 'a) -> 'a -> MultiSet -> 'a

    val foldBack : ('a -> uint32 -> 'b -> 'b) -> MultiSet -> 'b -> 'b

    val map : ('a -> 'b) -> MultiSet -> MultiSet

    val ofList : 'a list -> MultiSet

    val toList : MultiSet -> 'a list

    val union : MultiSet -> MultiSet -> MultiSet

    val sum : MultiSet -> MultiSet -> MultiSet

    val subtract : MultiSet -> MultiSet -> MultiSet

    val intersection : MultiSet -> MultiSet -> MultiSet

    val toString : MultiSet-> string
    