module Ass3_1

    (* Exercise 3.1 *)

    type aExp =
        | N of int
        | Add of aExp * aExp
        | Sub of aExp * aExp
        | Mul of aExp * aExp

    let (.+.) a b = Add (a, b)
    let (.-.) a b = Sub (a, b)
    let (.*.) a b = Mul (a, b)


    let rec arithEvalSimple aExp = 
        match aExp with
        | N n -> n
        | Add (a,b) -> (arithEvalSimple a) + (arithEvalSimple b) 
        | Sub (a,b) -> (arithEvalSimple a) - (arithEvalSimple b)
        | Mul (a,b) -> (arithEvalSimple a) * (arithEvalSimple b)
