(* File Cont/Icon.fs 

   Abstract syntax and interpreter for micro-Icon, a language where an 
   expression can produce more than one result.  

   sestoft@itu.dk * 2010-05-18

   ---

   For a description of micro-Icon, see Chapter 11: Continuations, in
   Programming Language Concepts for Software Developers.

   As described there, the interpreter (eval e cont econt) has two
   continuations:

      * a success continuation cont, that is called on the result v of
        the expression e, in case it has one;

      * a failure continuation econt, that is called on () in case the
        expression e has no result.
 *)

module Icon

(* Micro-Icon abstract syntax *)

type expr = 
  | CstI of int
  | CstS of string
  | FromTo of int * int
  | Write of expr
  | If of expr * expr * expr
  | Prim of string * expr * expr
  | Prim1 of string * expr  // Exercise 11.8.iii
  | And of expr * expr
  | Or  of expr * expr
  | Seq of expr * expr
  | Every of expr 
  | Fail;;

(* Runtime values and runtime continuations *)

type value = 
  | Int of int
  | Str of string;;

type econt = unit -> value;;

type cont = value -> econt -> value;;

(* Print to console *)

let write v =
    match v with 
    | Int i -> printf "%d " i
    | Str s -> printf "%s " s;;

(* Expression evaluation with backtracking *)

let rec eval (e : expr) (cont : cont) (econt : econt) = 
    match e with
    | CstI i -> cont (Int i) econt
    | CstS s  -> cont (Str s) econt
    | FromTo(i1, i2) -> 
      let rec loop i = 
          if i <= i2 then 
              cont (Int i) (fun () -> loop (i+1))
          else 
              econt ()
      loop i1
    | Write e -> 
      eval e (fun v -> fun econt1 -> (write v; cont v econt1)) econt
    | If(e1, e2, e3) -> 
      eval e1 (fun _ -> fun _ -> eval e2 cont econt)
              (fun () -> eval e3 cont econt)
    | Prim(ope, e1, e2) -> 
      eval e1 (fun v1 -> fun econt1 ->
          eval e2 (fun v2 -> fun econt2 -> 
              match (ope, v1, v2) with
              | ("+", Int i1, Int i2) -> 
                  cont (Int(i1+i2)) econt2 
              | ("*", Int i1, Int i2) -> 
                  cont (Int(i1*i2)) econt2
              | ("<", Int i1, Int i2) -> 
                  if i1<i2 then 
                      cont (Int i2) econt2
                  else
                      econt2 ()
              | _ -> Str "unknown prim2")
              econt1)
          econt
    | Prim1(ope, e) -> // Exercise 11.8.iii
      eval e (fun v -> fun econt1 ->
          match (ope, v) with
          | ("sqr", Int i) -> cont (Int(i * i)) econt1
          | ("even", Int i) -> 
              if i % 2 = 0 then
                  cont (Int i) econt1
              else
                  econt1 ()
          | ("multiples", Int i) ->
              let rec loop j = 
                  cont (Int j) (fun () -> loop (j+i))
              loop i
          | _ -> Str "unknown prim1")
        econt
    | And(e1, e2) -> 
      eval e1 (fun _ -> fun econt1 -> eval e2 cont econt1) econt
    | Or(e1, e2) -> 
      eval e1 cont (fun () -> eval e2 cont econt)
    | Seq(e1, e2) -> 
      eval e1 (fun _ -> fun econt1 -> eval e2 cont econt)
              (fun () -> eval e2 cont econt)
    | Every e -> 
      eval e (fun _ -> fun econt1 -> econt1 ())
             (fun () -> cont (Int 0) econt)
    | Fail -> econt ()

let run e = eval e (fun v -> fun _ -> v) (fun () -> (printfn "Failed"; Int 0));


(* Examples in abstract syntax *)

// (write(1 to 3)) ; fail
let ex1 = Seq(Write (FromTo(1, 3)), Fail);

// (write(1 to 3)) & fail
let ex2 = And(Write (FromTo(1, 3)), Fail);

// (write((1 to 3) & (4 to 6))) & fail
let ex3and = And(Write(And(FromTo(1, 3), FromTo(4, 6))), Fail);

// (write((1 to 3) | (4 to 6))) & fail
let ex3or  = And(Write(Or(FromTo(1, 3), FromTo(4, 6))), Fail);

// (write((1 to 3) ; (4 to 6))) & fail
let ex3seq = And(Write(Seq(FromTo(1, 3), FromTo(4, 6))), Fail);

// write((1 to 3) & ((4 to 6) & "found"))
let ex4 = Write(And(FromTo(1, 3), And(FromTo(4, 6), CstS "found")));

// every(write(1 to 3))
let ex5 = Every (Write (FromTo(1, 3)));

// (every(write(1 to 3)) & (4 to 6))
let ex6 = And(Every (Write (FromTo(1, 3))), FromTo(4, 6));

// every(write((1 to 3) + (4 to 6)))
let ex7 = Every(Write(Prim("+", FromTo(1,3), FromTo(4, 6))));

// write(4 < (1 to 10))
let ex8 = Write(Prim("<", CstI 4, FromTo(1, 10)));

// every(write(4 < (1 to 10)))
let ex9 = Every(Write(Prim("<", CstI 4, FromTo(1, 10))));


//exercise 11.8(i)
// every(write(1 + 2 * (1 to 4)))
let ex11_8i1 = Every(Write(Prim("+", CstI 1, Prim("*", CstI 2, FromTo(1,4)))));

// every(write(10 * (2 to 4) + (1 to 2)))
let ex11_8i2 = Every(Write(Prim("+", Prim("*", CstI 10, FromTo(2,4)), FromTo(1,2))));


//exercise 11.8(ii)
// write(50 < (7 * (1 to 50)))
let ex11_8ii = Write(Prim("<", CstI 50, Prim("*", CstI 7, FromTo(1, 50))));


//exercise 11.8(iii)
// every(write(square(3 to 6)))
let ex11_8iii1 = Every(Write(Prim1("sqr", FromTo(3, 6))))

// every(write(even(1 to 7)))
let ex11_8iii2 = Every(Write(Prim1("even", FromTo(1, 7))))


//exercise 11.8(iv)
// write(50 < (7 * (multiples(1))))
let ex11_8iv = Write(Prim("<", CstI 50, Prim("*", CstI 7, Prim1("multiples", CstI 1))));