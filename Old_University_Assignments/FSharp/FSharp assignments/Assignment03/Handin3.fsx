module Ass3_3
open System

type state = Map<string, int>
type word = (char * int) list
type squareFun = word -> int -> int -> int
type aExp =
    | N of int
    | V of string
    | WL
    | PV of aExp
    | Add of aExp * aExp
    | Sub of aExp * aExp
    | Mul of aExp * aExp

let (.+.) a b = Add (a, b)
let (.-.) a b = Sub (a, b)
let (.*.) a b = Mul (a, b)
    
let hello = [('H',4);('E',1);('L',1);('L',1);('O',1)]

let arithSingleLetterScore = PV (V "_pos_") .+. (V "_acc_")
let arithDoubleLetterScore = ((N 2) .*. PV (V "_pos_")) .+. (V "_acc_")
let arithTripleLetterScore = ((N 3) .*. PV (V "_pos_")) .+. (V "_acc_")

let arithDoubleWordScore = N 2 .*. V "_acc_"
let arithTripleWordScore = N 3 .*. V "_acc_"
    
let rec arithEval (aExp:aExp) (w:word) (s:state) =
    match aExp with
        | N n       -> n
        | V v       ->  match Map.tryFind v s with // = None then 0 else Map.find v s
                        | None -> 0
                        | Some x -> x
        | WL        -> List.length(w) 
        | PV x      -> snd(w.[arithEval x w s])
        | Add (a,b) -> (+) (arithEval a w s) (arithEval b w s)
        | Sub (a,b) -> (-) (arithEval a w s) (arithEval b w s)
        | Mul (a,b) -> (*) (arithEval a w s) (arithEval b w s)

type cExp =
    | C of char       (* Character value *)
    | ToUpper of cExp (* Converts lower case to upper case character, non-characters unchanged *)
    | ToLower of cExp (* Converts upper case to lower case character, non characters unchanged *)
    | CV of aExp      (* Character lookup at word index *)

let rec charEval (cExp:cExp) (word:word) (state:state) = 
    match cExp with
    | C c           -> c
    | ToUpper c     -> Char.ToUpper(charEval c word state)
    | ToLower c     -> Char.ToLower(charEval c word state)
    | CV c          -> fst(word.[arithEval c word state])
    
type bExp =             
   | TT                   (* true *)
   | FF                   (* false *)

   | AEq of aExp * aExp   (* numeric equality *)
   | ALt of aExp * aExp   (* numeric less than *)

   | Not of bExp          (* boolean not *)
   | Conj of bExp * bExp  (* boolean conjunction *)

   | IsLetter of cExp     (* check for letter *)
   | IsDigit  of cExp     (* check for constant *)

let (~~) b = Not b
let (.&&.) b1 b2 = Conj (b1, b2)
let (.||.) b1 b2 = ~~(~~b1 .&&. ~~b2)       (* boolean disjunction *)
   
let (.=.) a b = AEq (a, b)   
let (.<.) a b = ALt (a, b)   
let (.<>.) a b = ~~(a .=. b)
let (.<=.) a b = a .<. b .||. ~~(a .<>. b)
let (.>=.) a b = ~~(a .<. b)                (* numeric greater than or equal to *)
let (.>.) a b = ~~(a .=. b) .&&. (a .>=. b) (* numeric greater than *)


let rec boolEval bExp (word:word) (state:state) =
    match bExp with
    | TT            -> true
    | FF            -> false
    | AEq (a,b)     -> (arithEval a word state) = (arithEval b word state)
    | ALt (a,b)     -> (arithEval a word state) < (arithEval b word state)
    | Not b         -> not (boolEval b word state)
    | Conj (b1,b2)  -> (boolEval b1 word state) && (boolEval b2 word state)
    | IsLetter c    -> Char.IsLetter(charEval c word state)
    | IsDigit c     -> Char.IsDigit(charEval c word state)

type stmnt =
   | Skip                        (* does nothing *)
   | Ass of string * aExp        (* variable assignment *)
   | Seq of stmnt * stmnt        (* sequential composition *)
   | ITE of bExp * stmnt * stmnt (* if-then-else statement *)    
   | While of bExp * stmnt       (* while statement *)

let rec evalStmnt (stm:stmnt) (word:word) (state:state) = 
    match stm with
    | Skip                      -> state
    | Ass (x,a)                 -> state.Add(x, arithEval a word state)
    | Seq (stm1, stm2)          -> evalStmnt stm2 word (evalStmnt stm1 word state)
    | ITE (guard, stm1, stm2)   -> if (boolEval guard word state) then evalStmnt stm1 word state
                                   else evalStmnt stm2 word state
    | While(guard, stm)         -> if (boolEval guard word state) then 
                                       evalStmnt (While (guard, stm)) word (evalStmnt stm word state) 
                                       else state

// Assignment 3.7
let stmntToSquareFun (stm:stmnt) = fun w pos acc -> (evalStmnt stm w (state [("_pos_", pos); ("_acc_", acc)])).["_result_"]

let singleLetterScore = stmntToSquareFun (Ass ("_result_", arithSingleLetterScore))
let doubleLetterScore = stmntToSquareFun (Ass ("_result_", arithDoubleLetterScore))
let tripleLetterScore = stmntToSquareFun (Ass ("_result_", arithTripleLetterScore))
let doubleWordScore = stmntToSquareFun (Ass ("_result_", arithDoubleWordScore))
let tripleWordScore = stmntToSquareFun (Ass ("_result_", arithTripleWordScore))

let containsNumbers = stmntToSquareFun (Seq (Ass ("_result_", V "_acc_"), While (V "i" .<. WL, ITE (IsDigit (CV (V "i")),
                                        Seq (Ass ("_result_", V "_result_" .*. N -1), Ass ("i", WL)), Ass ("i", V "i" .+. N 1)))))
