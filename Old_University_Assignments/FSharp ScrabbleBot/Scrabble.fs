namespace scumbotBH

open ScrabbleUtil
open ScrabbleUtil.ServerCommunication

open System.IO

open ScrabbleUtil.DebugPrint

// The RegEx module is only used to parse human input. It is not used for the final product.

module RegEx =
    open System.Text.RegularExpressions

    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None

    let parseMove ts =
        let pattern = @"([-]?[0-9]+[ ])([-]?[0-9]+[ ])([0-9]+)([A-Z]{1})([0-9]+)[ ]?" 
        Regex.Matches(ts, pattern) |>
        Seq.cast<Match> |> 
        Seq.map 
            (fun t -> 
                match t.Value with
                | Regex pattern [x; y; id; c; p] ->
                    ((x |> int, y |> int), (id |> uint32, (c |> char, p |> int)))
                | _ -> failwith "Failed (should never happen)") |>
        Seq.toList

 module Print =

    let printHand pieces hand =
        hand |>
        MultiSet.fold (fun _ x i -> forcePrint (sprintf "%d -> (%A, %d)\n" x (Map.find x pieces) i)) ()

module State = 
    // Make sure to keep your state localised in this module. It makes your life a whole lot easier.
    // Currently, it only keeps track of your hand, your player numer, your board, and your dictionary,
    // but it could, potentially, keep track of other useful
    // information, such as number of players, player turn, etc.
    type placed = coord * (uint32 * (char * int))
    
    type state = {
        board         : Parser.board
        dict          : ScrabbleUtil.Dictionary.Dict
        playerNumber  : uint32
        hand          : MultiSet.MultiSet<uint32>
        tilesPlaced   : Map<coord,char>
        tiles         : Map<uint32, tile>
    }

    let mkState b d pn h tp t = {board = b; dict = d;  playerNumber = pn; hand = h; tilesPlaced = tp; tiles = t}

    let board st         = st.board
    let dict st          = st.dict
    let playerNumber st  = st.playerNumber
    let hand st          = st.hand

module Scrabble =
    open System.Threading
    open State
    open Dictionary
    
    type Direction = 
    |Right 
    |Down

    let nextCoord ((x,y):coord) = function
        |Right -> (x+1,y) 
        |Down -> (x,y+1)

    let prevCoord ((x,y):coord) = function
        |Right -> (x-1,y) 
        |Down -> (x,y-1)

    let bestWord word1 word2 = if List.length word1 > List.length word2 then word1 else word2

    // fix and use if we want to look for crossing words instead of ignoring them
    (*let isWord (start: coord) (s: state) (dir: Direction) (ogDict: Dict) =
        let realDir = 
            match dir with
            |Right -> Down
            |Down  -> Right
        let rec aux (coord: coord) (reversed: bool) (dict: Dict) (isWordBool: bool) (first: bool) = 
            let next = if reversed then prevCoord coord realDir else nextCoord coord realDir
            match Map.tryFind next s.tilesPlaced with 
            |Some a -> 
                match step a dict with 
                |Some (b,di) -> aux next reversed di b false
                |None -> first
            |None -> 
                match reversed with
                |true  -> aux coord false dict isWordBool false
                |false -> isWordBool
                //match reverse dict with
                //|Some (b,di) -> aux (nextCoord start realDir) false di b false
                //|None -> isWordBool
        aux start true ogDict false true*)

    let isClear (curr: coord) (s: state) (dir: Direction) =
        match dir with
        |Right -> match Map.tryFind (nextCoord curr Down) s.tilesPlaced with
                  |Some a -> false
                  |None -> match Map.tryFind (prevCoord curr Down) s.tilesPlaced with
                           |Some a ->  false
                           |None -> true
        |Down  -> match Map.tryFind (nextCoord curr Right) s.tilesPlaced with
                  |Some a -> false
                  |None -> match Map.tryFind (prevCoord curr Right) s.tilesPlaced with
                           |Some a ->  false
                           |None -> true
     
    let lastClear (curr: coord) (s: state) (dir: Direction) =
        match dir with
        |Right -> match Map.tryFind (nextCoord curr Right) s.tilesPlaced with
                  |Some a -> false
                  |None -> true
        |Down -> match Map.tryFind (nextCoord curr Down) s.tilesPlaced with
                  |Some a -> false
                  |None -> true

    let moveFromPos (state : state) (start : coord) (direction : Direction) = 

        let rec aux (reversed:bool) (current : placed list) (best : placed list)  (coord : coord) (dict: Dict) (hand : MultiSet.MultiSet<uint32>)  =
            let next = if reversed then prevCoord coord direction else nextCoord coord direction
            match state.board.squares coord with 
            |Some a -> 
                match Map.tryFind coord state.tilesPlaced with
                |Some a -> 
                    match step a dict with
                    |Some (b,di) -> 
                        if (lastClear coord state direction) then
                            let newBest = 
                                if b then bestWord best current 
                                else best
                            aux reversed current newBest next di hand
                        else best
                    |None -> best
                |None -> 
                    let searchHand = MultiSet.fold (fun acc cha r ->
                        let (cv, pv) = Set.minElement (Map.find cha state.tiles)
                        match step cv dict with
                        |Some (b,di) -> 
                            if (isClear coord state direction) then 
                                    let newAcc = (coord,(cha,(cv,pv)))::current
                                    let newBest =
                                        if b then if lastClear coord state direction then  bestWord best newAcc else best 
                                        else best
                                    aux reversed newAcc newBest next di (MultiSet.removeSingle cha hand) |> bestWord acc
                                else best
                        |None -> acc ) best hand
                
                    let findSpecial = 
                        match Dictionary.reverse dict with
                        |Some (b,di) -> 
                            if (isClear coord state direction) then 
                                if (lastClear (nextCoord start direction) state direction) then
                                    let newBest = 
                                        if b then bestWord best current 
                                        else best
                                    aux false current newBest (nextCoord start direction) di hand
                                else best
                            else best
                        |None -> best
                    bestWord searchHand findSpecial
            |None -> best    
        aux true [] [] start state.dict state.hand

    
    let playsomething (state : state) = 
        match state.tilesPlaced.IsEmpty with
        |true  -> moveFromPos state state.board.center Down
        |false -> 
            let res =
                Map.fold (fun (acc : Map<int,placed list>) coord c -> 
                let best = bestWord (moveFromPos state coord Down) (moveFromPos state coord Right)
                acc.Add ((List.length best), best)) Map.empty state.tilesPlaced
            let longest (mp : Map<int, placed list>) =
                Map.fold (fun acc x y -> if x > acc then x else acc) 0 mp
            res.Item(longest res)


    let playGame cstream pieces (st : State.state) =

        let rec aux (st : State.state) =
            Print.printHand pieces (State.hand st)

            // remove the force print when you move on from manual input (or when you have learnt the format)
            //forcePrint "Input move (format '(<x-coordinate> <y-coordinate> <piece id><character><point-value> )*', note the absence of space between the last inputs)\n\n"
            //let input =  System.Console.ReadLine()
            //let move = RegEx.parseMove input
            let move = match playsomething st with
                       |[] -> SMPlay [] // potentialy call something like SMForfeit to aovid empty move loop
                       | x -> SMPlay x


            debugPrint (sprintf "Player %d -> Server:\n%A\n" (State.playerNumber st) move) // keep the debug lines. They are useful.
            send cstream (move)

            let msg = recv cstream
            debugPrint (sprintf "Player %d <- Server:\n%A\n" (State.playerNumber st) move) // keep the debug lines. They are useful.

            match msg with
            | RCM (CMPlaySuccess(ms, points, newPieces)) ->
                (* Successful play by you. Update your state (remove old tiles, add the new ones, change turn, etc) *)
                let st' = 
                    State.mkState
                                  st.board
                                  st.dict 
                                  st.playerNumber 
                                  (List.fold (fun acc (id,count) -> MultiSet.add id count acc) (List.fold (fun acc (c, id) -> MultiSet.removeSingle (fst id) acc ) st.hand ms) newPieces)
                                  (List.fold (fun acc (id, count) -> Map.add id (fst(snd count)) acc  ) st.tilesPlaced ms)
                                  st.tiles
                aux st'
            | RCM (CMPlayed (pid, ms, points)) ->
                (* Successful play by other player. Update your state * does not matter to play solo ourself*)
                let st' = 
                    State.mkState
                                  st.board
                                  st.dict 
                                  st.playerNumber 
                                  st.hand
                                  st.tilesPlaced
                                  st.tiles
                aux st'
            | RCM (CMPlayFailed (pid, ms)) ->
                (* Failed play. Update your state *)
                let st' = 
                    State.mkState
                                  st.board
                                  st.dict 
                                  st.playerNumber 
                                  st.hand
                                  st.tilesPlaced
                                  st.tiles
                aux st'
            | RCM (CMGameOver _) -> ()
            | RCM a -> failwith (sprintf "not implmented: %A" a)
            | RGPE err -> printfn "Gameplay Error:\n%A" err; aux st
        aux st

    let startGame 
            (boardP : boardProg) 
            (dictf : bool -> Dictionary.Dict) 
            (numPlayers : uint32) 
            (playerNumber : uint32) 
            (playerTurn  : uint32) 
            (hand : (uint32 * uint32) list)
            (tiles : Map<uint32, tile>)
            (timeout : uint32 option) 
            (cstream : Stream) =
        debugPrint 
            (sprintf "Starting game!
                      number of players = %d
                      player id = %d
                      player turn = %d
                      hand =  %A
                      timeout = %A\n\n" numPlayers playerNumber playerTurn hand timeout)

        let dict = dictf true // Uncomment if using a gaddag for your dictionary
        //let dict = dictf false // Uncomment if using a trie for your dictionary
        let board = Parser.parseBoardProg boardP
                  
        let handSet = List.fold (fun acc (x, k) -> MultiSet.add x k acc) MultiSet.empty hand

        fun () -> playGame cstream tiles (State.mkState board dict playerNumber handSet Map.empty tiles)
        