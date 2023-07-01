
type aexpr = 
  | CstI of int
  | Var of string
  | Add of aexpr * aexpr
  | Mul of aexpr * aexpr
  | Sub of aexpr * aexpr;;

let ae1 = Sub(Var "v", Add(Var "w", Var "z"))
  
let ae2 = Mul(CstI 2, ae1)
  
let ae3 = Add(Add(Add( Var "x", Var "y"), Var "z"), Var "v")

let ae4 = Add(Var "x", CstI 0)
let ae5 = Add(CstI 0, CstI 1)
let ae6 = Add(CstI 1, CstI 0)
let ae7 = Mul(Add(CstI 1, CstI 0), Add(Var "x", CstI 0))

let rec fmt a =
    match a with
    | CstI i -> string i 
    | Var x -> x
    | Add (a1, a2) ->   "(" + fmt a1 + " + " + fmt a2 + ")"
    | Mul (a1, a2) ->   "(" + fmt a1 + " * " + fmt a2 + ")" 
    | Sub (a1, a2) ->   "(" + fmt a1 + " - " + fmt a2 + ")"

let rec simplify a =
    match a with 
    | Add(CstI 0, b)
    | Add (b, CstI 0)
    | Sub(CstI 0, b)
    | Mul(CstI 1, b) 
    | Mul(b, CstI 1) -> simplify b
    | Mul(CstI 0, b)
    | Mul(b, CstI 0) -> CstI 0
    | Sub(b, c) when b=c -> CstI 0
    | Add(b, c) -> simplify (Add(simplify b, simplify c))
    | Sub(b, c) -> simplify (Sub(simplify b, simplify c))
    | Mul(b, c) -> simplify (Mul(simplify b, simplify c))
    | _ -> a

let rec SDEval exp : aexpr =
    let derive = 
        match exp with
        | Var (x)       -> CstI 1
        | CstI (i)      -> CstI 0
        | Add(e1, e2) -> Add(SDEval(e1), SDEval(e2))
        | Sub(e1, e2) -> Sub(SDEval(e1), SDEval(e2))
        | Mul(e1, e2) -> Add(Mul(SDEval(e1), e2), Mul(e1, SDEval(e2)))
        | _ -> CstI 0
    simplify derive

let a1f = fmt ae1;;
let a2f = fmt ae2;;
let a3f = fmt ae3;;

let a4s = simplify ae4;;
let a5s = simplify ae5;;
let a6s = simplify ae6;;
let a7s = simplify ae7;;

let a8sd = SDEval (Add(Mul(CstI 4, Var "x"), CstI 2))