// ScrabbleUtil contains the types coord, boardProg, and SquareProg. Remove these from your file before proceeding.
// Also note that the modulse Ass7 and ImpParser have been merged to one module called Parser.

// Insert your Parser.fs file here from Assignment 7. All modules must be internal.

module internal Parser

    open ScrabbleUtil // NEW. KEEP THIS LINE.
    open System
    open Eval
    open FParsecLight.TextParser     // Industrial parser-combinator library. Use for Scrabble Project.
    
    let pIntToChar  = pstring "intToChar"
    let pPointValue = pstring "pointValue"

    let pCharToInt  = pstring "charToInt"
    let pToUpper    = pstring "toUpper"
    let pToLower    = pstring "toLower"
    let pCharValue  = pstring "charValue"

    let pTrue       = pstring "true"
    let pFalse      = pstring "false"
    let pIsDigit    = pstring "isDigit"
    let pIsLetter   = pstring "isLetter"

    let pif         = pstring "if"
    let pthen       = pstring "then"
    let pelse       = pstring "else"
    let pwhile      = pstring "while"
    let pdo         = pstring "do"
    let pdeclare    = pstring "declare"

    let whitespaceChar = satisfy System.Char.IsWhiteSpace
    let pletter        = satisfy System.Char.IsLetter
    let palphanumeric  = satisfy System.Char.IsLetterOrDigit 

    let spaces         = many whitespaceChar
    let spaces1        = many1 whitespaceChar

    let (.>*>.) p1 p2 = p1 .>> spaces .>>. p2
    let (.>*>)  p1 p2 = p1 .>> spaces .>> p2 
    let (>*>.)  p1 p2 = p1 >>. spaces >>. p2

    let parenthesise p = pchar '(' >*>. p .>*> pchar ')'

    let bracketise   p = pchar '{' >*>. p .>*> pchar '}'

    let apostrophe   p = pchar ''' >>. p .>> pchar '''

    let pid22 = pletter <|> pchar '_' .>>. many (palphanumeric <|> pchar '_') |>> 
                fun (c, list) ->  c::list |> Array.ofList |> System.String.Concat

    let pid  = pletter <|> pchar '_' .>>. many (palphanumeric <|> pchar '_') |>> 
                fun (c, list) -> c.ToString() + System.String.Concat(Array.ofList list)
    
    let unop op p = op >*>. p 
    let binop op p1 p2 = p1 .>*> op .>*>. p2

    let TermParse,  tref  = createParserForwardedToRef<aExp>()
    let ProdParse,  pref  = createParserForwardedToRef<aExp>()
    let AtomParse,  aref  = createParserForwardedToRef<aExp>()

    let CharParse,  cref  = createParserForwardedToRef<cExp>()

    let JunctParse, jref  = createParserForwardedToRef<bExp>()
    let CompParse,  cmref = createParserForwardedToRef<bExp>()
    let BoolParse,  bref  = createParserForwardedToRef<bExp>()

    let SParse,     sref  = createParserForwardedToRef<stm>()
    let NSParse,    nsref = createParserForwardedToRef<stm>()
    
    let AddParse  = binop (pchar '+') ProdParse TermParse       |>> Add                            <?> "Add"
    let SubParse  = binop (pchar '-') ProdParse TermParse       |>> Sub                            <?> "Sub"
                                                                                                   
    let DivParse  = binop (pchar '/') AtomParse ProdParse       |>> Div                            <?> "Div"
    let MulParse  = binop (pchar '*') AtomParse ProdParse       |>> Mul                            <?> "Mul"
    let ModParse  = binop (pchar '%') AtomParse ProdParse       |>> Mod                            <?> "Mod"
    let NegParse  = unop  (pchar '-') AtomParse                 |>> (fun x -> Mul(N -1, x))        <?> "Neg"
    let NParse    = pint32                                      |>> N                              <?> "Int"
    let ParParse  = parenthesise TermParse                                                         
    let PVParse   = unop pPointValue ParParse                   |>> PV                             <?> "PointValue"
    let VarParse  = pid                                         |>> V                              <?> "Var"
    let CTIParse  = unop pCharToInt (parenthesise CharParse)    |>> CharToInt                      <?> "CharToInt"
                                                                                                   
    let CParse    = apostrophe (palphanumeric <|> pchar ' ')    |>> C                              <?> "Char"
    let CVParse   = unop pCharValue (parenthesise TermParse)    |>> CV                             <?> "CharValue"
    let ITCParse  = unop pIntToChar (parenthesise TermParse)    |>> IntToChar                      <?> "IntToChar"
    let TUParse   = unop pToUpper   (parenthesise CharParse)    |>> ToUpper                        <?> "ToUpper"
    let TLParse   = unop pToLower   (parenthesise CharParse)    |>> ToLower                        <?> "ToLower"
                                                                                                   
    let ConjParse = binop (pstring "/\\") CompParse JunctParse  |>> Conj                           <?> "Conj"
    let DisjParse = binop (pstring "\\/") CompParse JunctParse  |>> (fun (x,y) -> ((.||.) x y))    <?> "Disj"
    let AEqParse  = binop (pchar '=')     TermParse TermParse   |>> AEq                            <?> "Equal"
    let AGoEParse = binop (pstring ">=")  TermParse TermParse   |>> (fun (x,y) -> (.>=.) x y)      <?> "Greater or Equal"
    let AGtParse  = binop (pchar '>')     TermParse TermParse   |>> (fun (x,y) -> (.>.) x y)       <?> "Greater Than"
    let ALoEParse = binop (pstring "<=")  TermParse TermParse   |>> (fun (x,y) -> (.<=.) x y)      <?> "Less or Equal"
    let ALtParse  = binop (pchar '<')     TermParse TermParse   |>> ALt                            <?> "Less Than"
    let ANEqParse = binop (pstring "<>")  TermParse TermParse   |>> (fun (x,y) -> (.<>.) x y)      <?> "Not Equal"
    let BTParse   = pTrue                                       |>> (fun _ -> TT)                  <?> "True"
    let BFParse   = pFalse                                      |>> (fun _ -> FF)                  <?> "False"
    let ILParse   = unop pIsLetter  (parenthesise CharParse)    |>> IsLetter                       <?> "IsLetter"
    let IDParse   = unop pIsDigit   (parenthesise CharParse)    |>> IsDigit                        <?> "IsDigit"
    let NotParse  = unop (pchar '~') JunctParse                 |>> Not                            <?> "Not"
    let BPParse   = parenthesise JunctParse                                                        
                                                                                                   
    let AssParse  = binop (pstring ":=") pid TermParse          |>> (fun (x,y) -> Ass (x,y))       <?> "Stm"
    let DecParse  = pdeclare >>. spaces1 >>. pid                |>> (fun x -> Declare x)           <?> "Declare"
    let SeqParse  = binop (pchar ';') NSParse SParse            |>> (fun (x,y) -> Seq (x,y))       <?> "Seq"
    let ITEParse  = pif >*>. parenthesise JunctParse .>*> 
                    pthen .>*>. bracketise SParse .>*> 
                    pelse .>*>. bracketise SParse               |>> (fun ((b,x),y) -> ITE (b,x,y)) <?> "ITE"
    let ITParse   = pif >*>. parenthesise JunctParse .>*> 
                    pthen .>*>. bracketise SParse               |>> (fun (b,x) -> ITE (b,x,Skip))  <?> "IT"
    let WHLParse  = pwhile >*>. parenthesise JunctParse .>*> 
                    pdo .>*>. bracketise SParse                 |>> (fun (b,x) -> While (b,x))     <?> "While"

    do tref  := choice [AddParse ; SubParse  ; ProdParse]
    do pref  := choice [MulParse ; DivParse  ; ModParse ; AtomParse]
    do aref  := choice [ParParse ; NegParse  ; PVParse  ; NParse   ; CTIParse ; VarParse]

    do cref  := choice [CParse   ; CVParse   ; TUParse  ; TLParse  ; ITCParse]
             
    do jref  := choice [BTParse  ; BFParse   ; ConjParse; DisjParse; CompParse]
    do cmref := choice [AEqParse ; ANEqParse ; ALtParse ; ALoEParse; AGoEParse; AGtParse; BoolParse]
    do bref  := choice [BPParse  ; NotParse  ; ILParse  ; IDParse]

    do sref  := choice [SeqParse ; NSParse]
    do nsref := choice [AssParse ; DecParse  ; ITEParse  ; ITParse  ; WHLParse]


    


    let AexpParse = TermParse 

    let CexpParse = CharParse

    let BexpParse = JunctParse

    let stmntParse = SParse


(* These five types will move out of this file once you start working on the project *)
    // type coord      = int * int
    // type squareProg = Map<int, string>
    // type boardProg  = {
    //         prog       : string;
    //         squares    : Map<int, squareProg>
    //         usedSquare : int
    //         center     : coord
    
    //         isInfinite : bool   // For pretty-printing purposes only
    //         ppSquare   : string // For pretty-printing purposes only
    //     }

    type word   = (char * int) list
    type square = Map<int, word -> int -> int -> int>


    let parseSquareFun (sqp:squareProg) = Map.map (fun x y-> run stmntParse y |> getSuccess |> stmntToSquareFun) sqp

    let parseBoardFun s m = stmntToBoardFun (getSuccess (run stmntParse s)) m


    type boardFun = coord -> square option
    type board = {
        center        : coord
        defaultSquare : square
        squares       : boardFun
    }

    let parseBoardProg (bp : boardProg) = Map.map (fun k v -> parseSquareFun v) bp.squares |> (fun m' -> 
        {   center = bp.center;
            defaultSquare = Map.find bp.usedSquare m';
            squares = parseBoardFun bp.prog m';
        }               
    )

