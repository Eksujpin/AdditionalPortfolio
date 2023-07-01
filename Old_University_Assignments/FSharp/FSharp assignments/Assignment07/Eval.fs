﻿module Eval

    open StateMonad
    open System
    (* Code for testing *)

    let hello = [('H',4);('E',1);('L',1);('L',1);('O',1)]
    let state = mkState [("x", 5); ("y", 42)] hello ["_pos_"; "_result_"]
    let emptyState = mkState [] [] []
    
    let add (a:SM<int>) (b:SM<int>) = a >>= fun x -> b >>= fun y -> ret (x+y)
    let div (a:SM<int>) (b:SM<int>) = a >>= fun x -> b >>= fun y -> if y = 0 then fail DivisionByZero else ret (x/y)

    type aExp =
        | N of int
        | V of string
        | WL
        | PV of aExp
        | Add of aExp * aExp
        | Sub of aExp * aExp
        | Mul of aExp * aExp
        | Div of aExp * aExp
        | Mod of aExp * aExp
        | CharToInt of cExp

    and cExp =
       | C  of char  (* Character value *)
       | CV of aExp  (* Character lookup at word index *)
       | ToUpper of cExp
       | ToLower of cExp
       | IntToChar of aExp

    type bExp =             
       | TT                   (* true *)
       | FF                   (* false *)

       | AEq of aExp * aExp   (* numeric equality *)
       | ALt of aExp * aExp   (* numeric less than *)

       | Not of bExp          (* boolean not *)
       | Conj of bExp * bExp  (* boolean conjunction *)

       | IsLetter of cExp     (* check for letter *)
       | IsDigit of cExp      (* check for digit *)

    let (.+.) a b = Add (a, b)
    let (.-.) a b = Sub (a, b)
    let (.*.) a b = Mul (a, b)
    let (./.) a b = Div (a, b)
    let (.%.) a b = Mod (a, b)

    let (~~) b = Not b
    let (.&&.) b1 b2 = Conj (b1, b2)
    let (.||.) b1 b2 = ~~(~~b1 .&&. ~~b2)       (* boolean disjunction *)
    let (.->.) b1 b2 = (~~b1) .||. b2           (* boolean implication *) 
       
    let (.=.) a b = AEq (a, b)   
    let (.<.) a b = ALt (a, b)   
    let (.<>.) a b = ~~(a .=. b)
    let (.<=.) a b = a .<. b .||. ~~(a .<>. b)
    let (.>=.) a b = ~~(a .<. b)                (* numeric greater than or equal to *)
    let (.>.) a b = ~~(a .=. b) .&&. (a .>=. b) (* numeric greater than *)    

    let rec arithEval a : SM<int> = 
        match a with
        | N n           -> ret n
        | V c           -> lookup c
        | WL            -> wordLength
        | PV x          -> arithEval x >>= fun y -> pointValue y
        | Add (a,b)     -> add (arithEval a) (arithEval b)
        | Sub (a,b)     -> arithEval a >>= (fun x -> arithEval b >>= (fun y -> ret (x-y)))
        | Mul (a,b)     -> arithEval a >>= (fun x -> arithEval b >>=(fun y -> ret (x*y))) 
        | Div (a,b)     -> div (arithEval a) (arithEval b)
        | Mod (a,b)     -> arithEval a >>= (fun x -> arithEval b >>=(function | 0 -> fail DivisionByZero | y -> ret(x%y)))
        | CharToInt c   -> charEval c >>= fun x -> ret (int (x))
    
    and charEval c : SM<char> = 
        match c with
        | C c           -> ret c
        | CV c          -> arithEval c >>= fun x -> characterValue x  
        | ToUpper c     -> charEval c >>= fun x -> ret(Char.ToUpper(x))
        | ToLower c     -> charEval c >>= fun x -> ret(Char.ToLower(x))
        | IntToChar x   -> arithEval x >>= fun y -> ret((char) y)

    let rec boolEval b : SM<bool> =
        match b with
        | TT            -> ret true
        | FF            -> ret false
        | AEq (a,b)     -> arithEval a >>= (fun x -> arithEval b >>= (fun y -> ret (x=y)))
        | ALt (a,b)     -> arithEval a >>= (fun x -> arithEval b >>= (fun y -> ret (x<y)))
        | Not b         -> boolEval b >>= fun x -> ret (not x)
        | Conj (a,b)    -> boolEval a >>= (fun x -> boolEval b >>= (fun y -> ret (x&&y)))
        | IsLetter c    -> charEval c >>= (fun x -> ret (Char.IsLetter x))
        | IsDigit c     -> charEval c >>= fun x -> ret (Char.IsDigit x)

    type stm =                (* statements *)
    | Declare of string       (* variable declaration *)
    | Ass of string * aExp    (* variable assignment *)
    | Skip                    (* nop *)
    | Seq of stm * stm        (* sequential composition *)
    | ITE of bExp * stm * stm (* if-then-else statement *)
    | While of bExp * stm     (* while statement *)

    let rec stmntEval stmnt : SM<unit> = 
        match stmnt with
        | Declare x                 -> declare x
        | Ass (x,a)                 -> arithEval a >>= update x
        | Skip                      -> ret ()
        | Seq (stm1, stm2)          -> stmntEval stm1 >>>= stmntEval stm2 
        | ITE (guard, stm1, stm2)   -> push >>>= boolEval guard >>= (fun x -> if x then stmntEval stm1 else stmntEval stm2 ) >>>= pop
        | While(guard, stm)         -> boolEval guard >>= (fun x -> if x then push >>>= stmntEval stm >>>= pop >>>= stmntEval (While (guard,stm)) else ret ())


(* Part 3 (Optional) 

    type StateBuilder() =

        member this.Bind(f, x)    = f >>= x
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Delay(f)      = f ()
        member this.Combine(a, b) = a >>= (fun _ -> b)
        
    let prog = new StateBuilder()

    let arithEval2 a = failwith "Not implemented"
    let charEval2 c = failwith "Not implemented"
    let rec boolEval2 b = failwith "Not implemented"
    let stmntEval2 stm = failwith "Not implemented"
*)

(* Part 4 *) 

    type word = (char * int) list
    type squareFun = word -> int -> int -> Result<int, Error>

    let stmntToSquareFun stm = fun w pos acc -> 
        stmntEval stm >>>= lookup "_result_" |>  
        evalSM (mkState [("_pos_", pos); ("_acc_", acc); ("_result_", 0)] w ["_acc_";"_pos_"; "_result_"])

    type coord = int * int

    type boardFun = coord -> Result<squareFun option, Error> 
    
    let stmntToBoardFun stm squares = fun (x,y) -> 
        stmntEval stm >>>= lookup "_result_" >>= (fun whatever -> 
        ret (Map.tryFind whatever squares)) |> 
        evalSM (mkState [("_x_", x); ("_y_", y); ("_result_", 0)] [] ["_x_";"_y_"; "_result_"])

    type board = {
        center        : coord
        defaultSquare : squareFun
        squares       : boardFun
    }

    let mkBoard c defaultSq boardStmnt ids = 
        {   center = c;
            defaultSquare = stmntToSquareFun defaultSq ;
            squares = stmntToBoardFun boardStmnt (Map.ofList(List.map(fun (k, sq) -> (k, stmntToSquareFun sq)) ids ))}