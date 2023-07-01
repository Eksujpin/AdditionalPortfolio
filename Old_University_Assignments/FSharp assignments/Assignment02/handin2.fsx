open System

// Assignment 2

// Exercise 2.1
// if-then-else
let rec downto1 n = 
    if      n = 0 then []
    else    n::downto1(n-1)

// pattern matching
let rec downto2 =
    function
    | 0 -> []
    | n -> n::downto2(n-1)

// Exercise 2.2   
let rec removeOddIdx lst =
    match lst with
    | [] -> []
    | x::y::xs -> x::removeOddIdx xs 
    | [x] -> [x]
 
// Exercise 2.3
let rec combinePair lst =
    match lst with
    | [] -> []
    | x::y::xs -> (x,y)::combinePair xs
    | [x] -> []

// Exercise 2.4
type complex = (float * float)

let mkComplex x y = (x,y):complex

let complexToPair (comp:complex) = (fst comp, snd comp)

let addComplex (n1:complex) (n2:complex) =
    let a, b = n1
    let c, d = n2
    (a + c, b + d)

let (|+|) a b = addComplex a b

let mulComplex (n1:complex) (n2:complex) =
    let a, b = n1
    let c, d = n2
    (a*c - b*d, b*c + a*d)

let (|*|) a b = mulComplex a b

let subComplex (n1:complex) (n2:complex) =
    let a, b = n1
    let c, d = n2
    (a-c, b-d)

let (|-|) a b = subComplex a b

let divComplex (n1:complex) (n2:complex) =
    let a, b = n1
    let c, d = n2
    let div = (c*c + d*d)
    ((a*c + b*d)/div, ((b*c - a*d)/div))

let (|/|) a b = divComplex a b

// Exercise 2.7
// Non-recursive
//let explode1 (str:string) =  [for letter in str -> letter]
//Non-recursive V2
let explode1 (str:string) = str.ToCharArray() |> List.ofArray

// Recursive
let rec explode2 (str:string) =
    if (str.Length > 0) then
        str.[0]::explode2(str.Remove(0,1))
    else
        []
        
// Exercise 2.8
//Naive / probs not the solution they are looking for
//let implode (cs:char list) = new string(Array.ofList(cs))
let implode (cs: char list) = new string(Array.ofList(List.foldBack(fun acc x -> acc :: x) [] cs))
//reversed 
let implodeRev(cs:char list) = new string(Array.ofList(List.fold(fun acc x -> x :: acc) [] cs))

// Exercise 2.9
let toUpper (s:string) = s |> Seq.map System.Char.ToUpper |> Seq.map string |> String.concat ""

// Exercise 2.10
let rec ack (m, n) = 
    match (m,n) with
    |(0,n) -> n+1
    |(m,0) when m >0 -> ack(m-1,1) 
    |(m,n) when m > 0 && n > 0 -> ack(m-1,ack(m,n-1))

//exercise 2.11
// given
let time f = 
    let start = System.DateTime.Now
    let res = f ()
    let finish = System.DateTime.Now
    (res, finish - start)

//new
let timeArg1 f a = 
    let start = DateTime.Now
    let res = f (a)
    let finish = DateTime.Now
    (res, finish - start)

//exercise 2.12
let downto3 f n e =
    if n > 0 then
        let range = [1..n]
        List.foldBack f range e
    else 
     e

let fac x = downto3 (*) x 1

let range g n = downto3 (fun a b -> g a::b) n []


//Scrabble assignments !!
type word = (char * int) list
//exercise 2.13
let hello = [('H',4);('E',1);('L',1);('L',1);('O',1)]

//exercise 2.14
type squareFun = word -> int -> int -> int

let singleLetterScore (word:word) pos acc = 
    match word.[pos] with
    | (c, v) -> acc + v

let doubleLetterScore (word:word) pos acc = 
    match word.[pos] with
    | (c, v) -> acc + v*2

let tripleLetterScore (word:word) pos acc = 
    match word.[pos] with
    | (c, v) -> acc + v*3

//exercise 2.15
let doubleWordScore (word:word) pos acc = acc*2

let tripleWordScore (word:word) pos acc = acc*3

//exercise 2.16
let containsNumbers (word:word) pos acc = 
    if List.exists (fun (elm1, elm2) -> System.Char.IsNumber(elm1)) word then 
        -acc
    else 
        acc

//exercise 2.17 (non-mandatory)
type square = (int * squareFun) list

let SLS : square = [(0, singleLetterScore)];;
let DLS : square = [(0, doubleLetterScore)];;
let TLS : square = [(0, tripleLetterScore)];;
let DWS : square = SLS @ [(1, doubleWordScore)];;
let TWS : square = SLS @ [(1, tripleWordScore)];;

let calculatePoints (square:square list) (word: word) = 
    match word with
   // | square list |> List.mapi 