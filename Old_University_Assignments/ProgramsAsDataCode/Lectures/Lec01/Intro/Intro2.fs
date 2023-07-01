(* Programming language concepts for software developers, 2010-08-28 *)

(* Evaluating simple expressions with variables *)

module Intro2

(* Association lists map object language variables to their values *)

let env = [("a", 3); ("c", 78); ("baf", 666); ("b", 111)];;

let emptyenv = []; (* the empty environment *)

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

let cvalue = lookup env "c";;


(* Object language expressions with variables *)

type expr = 
  | CstI of int
  | Var of string
  | Prim of string * expr * expr
  | If of expr * expr * expr;;
  



let e1 = CstI 17;;

let e2 = Prim("+", CstI 3, Var "a");;

let e3 = Prim("+", Prim("*", Var "b", CstI 9), Var "a");;

let e4 = Prim("max", Var "a", Var "b" )

let e5 = Prim("min", Var "a", Var "b" )

let e6 = Prim("==", Var "a", Var "b" )

let e7 = Prim("==", Var "a", Var "a" )

let e8 = If( CstI 1, Prim("+", Var "a", Var "b"), Prim("-", Var "a", Var "b"))

let e9 = If( CstI 0, Prim("+", Var "a", Var "b"), Prim("-", Var "a", Var "b"))



(* Evaluation within an environment *)
let rec oldEval e (env : (string * int) list) : int = //
    match e with
    | CstI i                -> i
    | Var x                 -> lookup env x
    | Prim("+", e1, e2)     -> oldEval e1 env + oldEval e2 env
    | Prim("*", e1, e2)     -> oldEval e1 env * oldEval e2 env
    | Prim("-", e1, e2)     -> oldEval e1 env - oldEval e2 env
    (* ex 1.1 I & V*)
    | Prim("max", e1, e2)   -> max (oldEval e1 env) (oldEval e2 env)
    | Prim("min", e1, e2)   -> min (oldEval e1 env) (oldEval e2 env)
    | Prim("==", e1, e2)    -> if  (oldEval e1 env) =  (oldEval e2 env) then 1 else 0
    | If(e1, e2, e3)        -> if oldEval e1 env <> 0 then oldEval e2 env else oldEval e3 env
    | Prim _                -> failwith "unknown primitive"

    (* ex 1.1 III-IV*)
let rec eval e (env : (string * int) list) : int =
    match e with
    | CstI i            -> i
    | Var x             -> lookup env x
    | Prim(ope, e1, e2) -> 
        let i1 = eval e1 env 
        let i2 = eval e2 env
        match ope with
        | "+" -> i1 + i2
        | "*" -> i1 * i2
        | "-" -> i1 - i2
        | "max" -> max i1 i2
        | "min" -> min i1 i2
        | "==" -> if i1 = i2 then 1 else 0
        | _ -> failwith "unkonwPrimitive"
    | If(e1, e2, e3) ->
        if eval e1 env <> 0 then eval e2 env else eval e3 env


let eo1v   = oldEval e1 env;;
let eo2v1  = oldEval e2 env;;
let eo2v2  = oldEval e2 [("a", 314)];;
let eo3v   = oldEval e3 env;;
let eo4v    = oldEval e4 env;;
let eo5v    = oldEval e5 env;;
let eo6v    = oldEval e6 env;;
let eo7v    = oldEval e7 env;;

let e1v   = eval e1 env;;
let e2v1  = eval e2 env;;
let e2v2  = eval e2 [("a", 314)];;
let e3v   = eval e3 env;;
let e4v    = eval e4 env;;
let e5v    = eval e5 env;;
let e6v    = eval e6 env;;
let e7v    = eval e7 env;;
let e8v = eval e8 env;;
let e9v = eval e9 env;;

