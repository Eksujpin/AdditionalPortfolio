module ImpParser

open Eval
open System

(*
    The interfaces for JParsec and FParsecLight are identical and the implementations should always produce the same output
    for successful parses although running times and error messages will differ. Please report any inconsistencies.
    *)

open JParsec.TextParser // Example parser combinator library. Use for CodeJudge.
// open FParsecLight.TextParser     // Industrial parser-combinator library. Use for Scrabble Project.

let pIntToChar = pstring "intToChar"
let pPointValue = pstring "pointValue" 

let pCharToInt = pstring "charToInt"
let pToUpper = pstring "toUpper"
let pToLower = pstring "toLower"
let pCharValue = pstring "charValue"

let pTrue = pstring "true"
let pFalse = pstring "false" 
let pIsDigit = pstring "isDigit" 
let pIsLetter = pstring "isLetter" 

let pif = pstring "if"
let pthen = pstring "then"
let pelse = pstring "else"
let pwhile = pstring "while"
let pdo = pstring "do" 
let pdeclare = pstring "declare" 

let whitespaceChar = satisfy Char.IsWhiteSpace 
let pletter = satisfy Char.IsLetter 
let palphanumeric = satisfy Char.IsLetterOrDigit 

let spaces = many (satisfy Char.IsWhiteSpace) 
let spaces1 = many1 (satisfy Char.IsWhiteSpace) 

let (.>*>.) p1 p2 = p1 .>>. (spaces >>. p2) 
let (.>*>) p1 p2 = p1 .>>. (spaces >>. p2) |>> fst 
let (>*>.) p1 p2 = p1 .>>. (spaces >>. p2) |>> snd 

let parenthesise p =
    (pchar '(' >*>. spaces >*>. p
     .>*> spaces
     .>*> pchar ')')

let tuborgclamps p =
    (pchar '{' >*>. spaces >*>. p
     .>*> spaces
     .>*> pchar '}')



let pid =
    (((pchar '_' <|> pletter)
      .>>. (many (palphanumeric <|> pchar '_')))
     |>> fun (x, y) -> x.ToString() + String.Concat(Array.ofList (y)))


let unop op a = op >*>. a 

let binop op p1 p2 = p1 .>*> op .>>. (spaces >>. p2)

//ref's for aexp
let TermParse, tref = createParserForwardedToRef<aExp> ()
let ProdParse, pref = createParserForwardedToRef<aExp> ()
let AtomParse, aref = createParserForwardedToRef<aExp> ()

//ref's for cexp
let cParse, cref    = createParserForwardedToRef<cExp> ()

//ref's for bexp
let SlashParse, sref    = createParserForwardedToRef<bExp> ()
let CompareParse, coref = createParserForwardedToRef<bExp> ()
let IsParse, iref       = createParserForwardedToRef<bExp> ()

//ref's for stms
let sParseNoSeq, ssref = createParserForwardedToRef<stm>()
let sParseSeq, ssref2 = createParserForwardedToRef<stm>()

// aexp parsers:
let AddParse = binop (pchar '+') ProdParse TermParse |>> Add <?> "Add"
let SubParse = binop (pchar '-') ProdParse TermParse |>> Sub <?> "Sub"
do tref := choice [ AddParse; SubParse; ProdParse ]

let MulParse = binop (pchar '*') AtomParse ProdParse |>> Mul <?> "Mul"
let DivParse = binop (pchar '/') AtomParse ProdParse |>> Div <?> "Div"
let ModParse = binop (pchar '%') AtomParse ProdParse |>> Mod <?> "Mod"
do pref := choice [ MulParse; DivParse; ModParse; AtomParse ]

let NParse      = pint32 |>> N <?> "Int"
let NegParse    = unop (pchar '-') TermParse |>> (fun x ->  Mul ((N -1),x)) <?> "Neg"

let PVParse     = pPointValue >*>. parenthesise TermParse |>> PV <?> "PV" 
let VarParse    = pid |>> V <?> "string"
let ParParse    = parenthesise TermParse
let CIParse     = pCharToInt >*>. parenthesise cParse |>> CharToInt <?> "CharToInt"
do aref := choice [ParParse; NegParse; PVParse; NParse; CIParse; VarParse]

//cexp parsers:
let CharParse       = pchar ''' >>. anyChar .>> pchar ''' |>> C <?> "Char"
let CharValParse    = pCharValue >*>. parenthesise TermParse |>> CV <?> "CV"
let ICParse         = pIntToChar >*>. parenthesise TermParse |>> IntToChar <?> "IntToChar" 
let UpParse         = pToUpper >*>. parenthesise cParse |>> ToUpper <?> "ToUpper"
let DownParse       = pToLower >*>. parenthesise cParse |>> ToLower <?> "ToLower"
do cref := choice [CharParse; CharValParse; UpParse; DownParse; ICParse]

//bexp parsers:
let TrueParse       = pTrue |>> (fun x -> TT) <?> "True"
let FalseParse      = pFalse |>> (fun x -> FF) <?> "False"
let ConjParse       = binop (pstring "/\\") CompareParse SlashParse  |>> (fun (x,y) -> x.&&.y) <?> "Conj"
let DisjParse       = binop (pstring "\\/")  CompareParse SlashParse |>> (fun (x,y) -> x.||.y) <?> "Disj"                       
do sref := choice [TrueParse; FalseParse; ConjParse; DisjParse; CompareParse]

let EqualParse          = binop (pchar '=') TermParse TermParse     |>> (fun (x,y) -> x .=. y)    <?> "Equal"
let NotEqualParse       = binop (pstring "<>") TermParse TermParse  |>> (fun (x,y) -> x .<>. y)   <?> "NotEqual"

let SmallerParse        = binop (pchar '<') TermParse TermParse     |>> (fun (x,y) -> x .<. y)    <?> "Smaller"
let GreaterParse        = binop (pchar '>') TermParse TermParse     |>> (fun (x,y) -> x .>. y )  <?> "Greater"

let SmallerEqualParse   = binop (pstring "<=") TermParse TermParse  |>> (fun (x,y) -> x .<=. y)   <?> "SmallerEqual"
let GreaterEqualParse   = binop (pstring ">=") TermParse TermParse  |>> (fun (x,y) -> x .>=. y)   <?> "GreaterEqual"
do coref := choice [EqualParse;  SmallerParse; NotEqualParse; SmallerEqualParse; GreaterEqualParse; GreaterParse; IsParse]

let bParParse          = parenthesise SlashParse
let TildeParse         = unop (pchar '~') SlashParse |>> Not <?> "Not"
let IsLetterParse      = pIsLetter >*>.  parenthesise cParse |>> IsLetter <?> "isLetter"
let IsDigitParse       = pIsDigit >*>. parenthesise cParse |>> IsDigit <?> "isDigit"
do iref := choice [bParParse; TildeParse; IsLetterParse; IsDigitParse]

//Statement parsers:

let AssParse = binop (pstring ":=") pid TermParse |>> (fun (x,y) -> Ass (x,y)) <?> "stm"

let DeclareParse = pdeclare >>. spaces1 >>. pid |>> (fun x -> Declare x) <?> "declare"

let SeqParse = binop (pstring ";") sParseNoSeq sParseSeq |>> (fun (x,y) -> Seq (x,y))  <?> "Seperation"

let IfThenElseParse = pif >*>. parenthesise SlashParse .>*> pthen .>*>. tuborgclamps sParseSeq .>*> pelse .>*>. tuborgclamps sParseSeq |>> (fun ((b,s1), s2) -> ITE (b, s1, s2)) <?> "IfTheElse"

let IfThenParse = pif >*>. parenthesise SlashParse .>*> pthen .>*>. tuborgclamps sParseSeq |>> (fun (b,s1) -> ITE (b, s1, Skip)) <?> "IfThen"

let WhileParse = pwhile >*>. parenthesise SlashParse .>*> pdo .>*>. tuborgclamps sParseSeq |>> (fun (b,s1) -> While (b, s1)) <?> "While"


do ssref := choice [AssParse; DeclareParse;IfThenElseParse; IfThenParse; WhileParse]
do ssref2 := choice [SeqParse; sParseNoSeq]

let AexpParse = TermParse

let CexpParse = cParse

let BexpParse = SlashParse

let stmntParse = sParseSeq

(* These five types will move out of this file once you start working on the project *)
type coord = int * int

type squareProg = Map<int, string>

type boardProg =
    { prog: string
      squares: Map<int, squareProg>
      usedSquare: int
      center: coord

      isInfinite: bool // For pretty-printing purposes only
      ppSquare: string } // For pretty-printing purposes only

type word = (char * int) list
type square = Map<int, word -> int -> int -> int>

let parseSquareFun _ = failwith "not implemented"

let parseBoardFun _ = failwith "not implemented"

type boardFun = coord -> square option

type board =
    { center: coord
      defaultSquare: square
      squares: boardFun }

let parseBoardProg (bp: boardProg) = failwith "not implemented"
