open System

// Assignment 5.1
let sum (m: int) n =
    let mutable result = m

    for value in 1 .. n do
        result <- result + (m + value)

    result

// Assignment 5.2

let length lst =
    List.fold (fun acc lst -> acc + 1) 0 lst

// Assignment 5.3

let foldBack f lst acc =
    let rec Loop lst c =
        match lst with
        | [] -> c acc
        | x :: xs -> Loop xs (fun r -> c (f x r))

    Loop lst id

// Assignment 5.4

let factA x =
    let rec aux acc =
        function
        | 0 -> acc
        | x -> aux (x * acc) (x - 1)

    aux 1 x

let factC x =
    let rec aux x cont =
        match x with
        | 0 -> cont 1
        | x -> aux (x - 1) (fun r -> cont r * x)

    aux x id


(* TODO: *)
(* Compare the running time between factA and factC. Which solution is faster and why?
   When we compare the running time between factA and factC with 30 as argument we see no difference. Both run at 00:00:00:00
*)

// Assignment 5.5

let fibW x =
    let mutable res1 = 0
    let mutable res2 = 1
    let mutable i = 1

    while (i <= x) do
        let temp = res1
        res1 <- res2
        res2 <- temp + res2
        i <- i + 1

    res1

let fibA x =
    let rec aux acc1 acc2 =
        function
        | 0 -> acc1
        | 1 -> acc2
        | x -> aux acc2 (acc1 + acc2) (x - 1)

    aux 0 1 x

let fibC x =
    let rec aux x cont =
        match x with
        | 0 -> cont 0
        | 1 -> cont 1
        | x -> aux (x - 2) (fun fib1 -> aux (x - 1) (fun fib2 -> cont (fib1 + fib2)))

    aux x id

(* TODO: *)
(* Compare the running time of fibW, fibA and fibC with 40 as input
   fibW: 00:00:00:00
   fibA: 00:00:00:00
   fibC: 00:00:03.682
*)

// Assignment 5.6

let rec bigListK c =
    function
    | 0 -> c []
    | n -> bigListK (fun res -> 1 :: c res) (n - 1)

(* TODO *)
(* The call bigListK id 130000 causes a stack overflow.
   Analyse the problem and describe exactly why this happens.
   Why is this not an iterative function?

   The overflow occours because the function keeps filling the stack and not sends any values to the heap (until it is done).
   So when the stack is full the overflow occours.
   This is not an iterative function since we are calling the whole function at every cycle. There is no auxilliary function that needs
   to be satisfied before the next step.
*)

// Assignment 5.7

type word = (char * int) list

type aExp =
    | N of int (* Integer literal *)
    | V of string (* Variable reference *)
    | WL (* Word length *)
    | PV of aExp (* Point value lookup at word index *)
    | Add of aExp * aExp (* Addition *)
    | Sub of aExp * aExp (* Subtraction *)
    | Mul of aExp * aExp (* Multiplication *)
    | CharToInt of cExp (* NEW: Cast to integer *)

and cExp =
    | C of char (* Character literal *)
    | CV of aExp (* Character lookup at word index *)
    | ToUpper of cExp (* Convert character to upper case *)
    | ToLower of cExp (* Convert character to lower case *)
    | IntToChar of aExp (* NEW: Cast to character *)


let rec arithEvalSimple (a: aExp) (w: word) s =
    match a with
    | N n           -> n
    | V v           ->
                       match Map.tryFind v s with //= None then 0 else Map.find v s
                       | None -> 0
                       | Some x -> x
    | WL            -> List.length (w)
    | PV x          -> snd (w.[arithEvalSimple a w s])
    | Add (a, b)    -> (+) (arithEvalSimple a w s) (arithEvalSimple b w s)
    | Sub (a, b)    -> (-) (arithEvalSimple a w s) (arithEvalSimple b w s)
    | Mul (a, b)    -> (*) (arithEvalSimple a w s) (arithEvalSimple b w s)
    | CharToInt x   -> int (charEvalSimple x w s)
and charEvalSimple (c: cExp) (w: word) s =
    match c with
    | C c           -> c
    | CV c          -> fst (w.[arithEvalSimple c w s])
    | ToUpper c     -> System.Char.ToUpper(charEvalSimple c w s)
    | ToLower c     -> System.Char.ToLower(charEvalSimple c w s)
    | IntToChar x   -> char (arithEvalSimple x w s)

// Assigment 5.8
let rec arithEvalTail (a: aExp) (w: word) s cont =
    match a with
    | N n           -> cont n
    | V v           ->
                       match Map.tryFind v s with //= None then 0 else Map.find v s
                       | None      -> cont 0
                       | Some x    -> cont x
    | WL            -> cont (List.length (w))
    | PV x          -> arithEvalTail x w s (fun value -> snd w.[value] |> cont)
    | Add (x, y)    -> arithEvalTail x w s (fun val1 -> arithEvalTail y w s (fun val2 -> val1+val2 |> cont))
    | Sub (x, y)    -> arithEvalTail x w s (fun val1 -> arithEvalTail y w s (fun val2 -> val1-val2 |> cont))
    | Mul (x, y)    -> arithEvalTail x w s (fun val1 -> arithEvalTail y w s (fun val2 -> val1*val2 |> cont))
    | CharToInt x   -> charEvalTail x w s (fun c -> int c |> cont)
and charEvalTail (c: cExp) (w: word) s cont =
    match c with
    | C c           -> cont c
    | CV x          -> arithEvalTail x w s (fun eval -> fst w.[eval] |> cont)
    | ToUpper c     -> charEvalTail c w s (fun eval -> Char.ToUpper eval |> cont)
    | ToLower c     -> charEvalTail c w s (fun eval -> Char.ToLower eval |> cont)
    | IntToChar x   -> arithEvalTail x w s (fun i -> char i |> cont)

let arithEval a w s = arithEvalTail a w s id
let charEval c w s = charEvalTail c w s id
