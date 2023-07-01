module Ass3_2

type aExp =
    | N of int
    | V of string
    | Add of aExp * aExp
    | Sub of aExp * aExp
    | Mul of aExp * aExp

let (.+.) a b = Add (a, b)
let (.-.) a b = Sub (a, b)
let (.*.) a b = Mul (a, b)


type state = Map<string, int>

    let rec arithEvalState aExp (s:state) = 
        match aExp with
        | N n -> n
        | V v ->  match Map.tryFind v s with //= None then 0 else Map.find v s
                        | None -> 0
                        | Some x -> x
        | Add (a,b) -> (+) (arithEvalState a s) (arithEvalState b s)
        | Sub (a,b) -> (-) (arithEvalState a s) (arithEvalState b s)
        | Mul (a,b) -> (*) (arithEvalState a s) (arithEvalState b s)
    
    