// Insert your updated Eval.fs file here from Assignment 7. All modules must be internal.

module internal Eval

    open StateMonad

    (* Code for testing *)

    let hello = [('H',4); ('E',1); ('L',1); ('L',1); ('O',1);]
    let state = mkState [("x", 5); ("y", 42)] hello ["_pos_"; "_result_"]
    let emptyState = mkState [] [] []
    
    let add a b = 
        a >>= fun x ->
        b >>= fun y -> 
        ret(x+y)

    let div a b = 
        a >>= fun x ->
        b >>= fun y ->
        if y <> 0 then ret (x/y) else fail DivisionByZero

    let sub a b =
        a >>= fun x ->
        b >>= fun y -> 
        ret(x-y)

    let mul a b =
        a >>= fun x ->
        b >>= fun y -> 
        ret(x*y)

    let modu a b = 
        a >>= fun x ->
        b >>= fun y ->
        if y <> 0 then ret (x%y) else fail DivisionByZero

    let neg a =
        a >>= fun x ->
        ret (-1*x)

    let binop f a b s =
        f (a s) (b s)

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
        | Neg of aExp
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
        | N n -> ret n
        | V x -> lookup x
        | WL -> wordLength
        | PV a1 -> (arithEval a1) >>= fun x -> pointValue x
        | Add (a1, a2) -> add (arithEval a1) (arithEval a2)
        | Sub (a1, a2) -> sub (arithEval a1) (arithEval a2)
        | Mul (a1, a2) -> mul (arithEval a1) (arithEval a2)
        | Div (a1, a2) -> div (arithEval a1) (arithEval a2)
        | Mod (a1, a2) -> modu (arithEval a1) (arithEval a2)
        | Neg a1 -> neg (arithEval a1)
        | CharToInt cexp -> charEval cexp >>= fun x -> ret ((int) x)
    
    and charEval c : SM<char> = 
        match c with
        | C n -> ret n
        | CV cv -> (arithEval cv) >>= fun x -> characterValue x
        | ToUpper cexp ->  charEval cexp >>= fun x -> ret (System.Char.ToUpper(x))
        | ToLower cexp -> charEval cexp >>= fun x -> ret (System.Char.ToLower(x))
        | IntToChar aexp -> arithEval aexp >>= fun x -> ret ((char) x)


    let rec boolEval b : SM<bool> = 
        match b with
        | TT -> ret true
        | FF -> ret false
        | AEq (aexp1, aexp2) -> arithEval aexp1 >>= fun x -> arithEval aexp2 >>= fun y -> ret(x=y)
        | ALt (aexp1, aexp2) -> arithEval aexp1 >>= fun x -> arithEval aexp2 >>= fun y -> ret(x<y)
        | Not bexp -> boolEval bexp >>= fun x -> ret(not x)
        | Conj (bexp1, bexp2) -> boolEval bexp1 >>= fun x -> boolEval bexp2 >>= fun y -> ret(x && y)
        | IsLetter cexp -> charEval cexp >>= fun x -> ret (System.Char.IsLetter(x))
        | IsDigit cexp -> charEval cexp >>= fun x -> ret (System.Char.IsDigit(x))


    type stm =                (* statements *)
    | Declare of string       (* variable declaration *)
    | Ass of string * aExp    (* variable assignment *)
    | Skip                    (* nope *)
    | Seq of stm * stm        (* sequential composition *)
    | ITE of bExp * stm * stm (* if-then-else statement *)
    | While of bExp * stm     (* while statement *)

    let rec stmntEval stmnt : SM<unit> = 
        match stmnt with
        | Declare dc -> declare dc
        | Ass (var, aexp) -> (arithEval aexp >>= fun x -> update var x)
        | Skip -> ret ()
        | Seq (stm1, stm2) -> (stmntEval stm1 >>>= stmntEval stm2)
        | ITE (bexp, stm1, stm2) -> boolEval bexp >>= fun x -> if x then push >>>= stmntEval stm1 >>>= pop else push >>>= stmntEval stm2 >>>= pop
        | While (bexp, stm1) -> boolEval bexp >>= fun x -> if x then push >>>= stmntEval stm1 >>>= stmntEval (While (bexp, stm1)) >>>= pop else ret ()

(* Part 3 (Optional)*)

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

(* Part 4 *) 

    type word = (char * int) list
    type squareFun = word -> int -> int -> int

    let stmntToSquareFun stm = 
        fun (w:word) pos acc -> 
              stmntEval stm >>>= lookup "_result_" |> evalSM (mkState [("_pos_", pos); ("_result_", 0); ("_acc_", acc)] w  ["_pos_";"_result_";"_acc_"]) |>
              function
              | Success a-> a
              | Failure b -> 0

    type coord = int * int

    let stmntToBoardFun stm m = 
        fun (x,y) -> 
            stmntEval stm >>>= lookup "_result_" >>= (fun x -> Map.tryFind x m |> ret)|> evalSM (mkState [("_x_", x);("_y_", y);("_result_", 0)] [] ["_x_";"_y_";"_result_"]) |>
                function
                | Success s -> s
                | Failure f -> None
