module MultiSet
    type MultiSet<'a when 'a : comparison> =
        | MS of Map<'a,uint32>
    
    let unpack (s:MultiSet<'a>) = 
        match s with
        |MS m -> m

    let empty = MS Map.empty

    let isEmpty (s:MultiSet<'a>) = Map.isEmpty (unpack s)
    
    //for every element/key sum the value
    let size (s:MultiSet<'a>) = List.sum(Map.fold(fun acc _ value -> value::acc ) [] (unpack s))

    let contains a (s:MultiSet<'a>) = unpack(s).ContainsKey(a)
    
    let numItems a (MS(m)) = match m.TryFind(a) with
                                       |Some x -> x
                                       |None -> uint32(0)
                                       
    //simplified works but still keeping the old ones
    //let add a (n:uint32) (s:MultiSet<'a>) = MS((unpack s).Add(a,n))
    let add a (n:uint32) (s:MultiSet<'a>) = match unpack(s).TryFind(a) with
                                                |Some x -> MS((unpack s).Add(a,uint32(x+uint32(n))))
                                                |None -> MS((unpack s).Add(a,uint32(uint32(n))))                                      
    
    //simplified works but still keeping the old ones
    //let addSingle a (s:MultiSet<'a>) = MS((unpack s).Add(a,uint32(1)))
    let addSingle a (s:MultiSet<'a>) = match unpack(s).TryFind(a) with
                                            |Some x -> MS((unpack s).Add(a,uint32(x+uint32(1))))
                                            |None -> MS((unpack s).Add(a,uint32(uint32(1))))       
    
    let remove a (n:uint32) (s:MultiSet<'a>) = match unpack(s).TryFind(a) with
                                                |Some x -> if x <= n then MS((unpack s).Remove(a)) else MS((unpack s).Add(a,uint32(x-uint32(n))))
                                                |None -> s
    
    let removeSingle a (s:MultiSet<'a>) = match unpack(s).TryFind(a) with
                                              |Some x -> if x <= uint32(1) then MS((unpack s).Remove(a)) else MS((unpack s).Add(a,uint32(x-uint32(1))))
                                              |None -> s
    
    let fold f acc (s:MultiSet<'a>) = Map.fold f acc (unpack s)

    let foldBack f (s:MultiSet<'a>) acc = Map.foldBack f (unpack s) acc 

    let map f (s:MultiSet<'a>) = fold (fun acc key value -> add (f key) value acc) empty s 
    
    let ofList lst = List.fold (fun map element -> addSingle element map) empty lst 

    //let toList2 (s:MultiSet<'a>) = Map.fold (fun acc key _ -> key::acc) [] (unpack s)

    //let toList (s:MultiSet<'a>) = List.fold (fun acc key value -> for i 

    let toList (s:MultiSet<'a>) = fun (lst:List<'a>) -> for item in (unpack s) do for i = 0 to int(item.Value) do item.Key::lst
    
    //let toList (s:MultiSet<'a>) = fold(fun lst theSet -> for item in (unpack theSet) do for i=0 to item.value ) [] s
    

    (*
    
    MS (x -> 2, y -> 1)
        return lst [x,]
    
    
    *)

    // let union = failwith("notImplemtedYet")

    // let sum = failwith("notImplemtedYet")

    // let subtract = failwith("notImplemtedYet")

    // let intersection = failwith("notImplemtedYet")

    // let toString = failwith("notImplemtedYet")

    // static member add a n (M(s)) =
    //     Map.add a (n + numItems a (M(s))) s
    //      |> M