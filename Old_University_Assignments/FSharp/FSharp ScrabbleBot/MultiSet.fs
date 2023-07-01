// Insert your MultiSet.fs file here. All modules must be internal

module internal MultiSet

    type MultiSet<'a when 'a : comparison> = 
    | M of Map<'a, uint32>
        override q.ToString() = 
            match q with 
            |M mp -> (Map.fold (fun acc key value -> acc + sprintf "(%A, #%d), " key value) "{" mp) 
            |> fun z -> sprintf "%s" (z.[0..z.Length-3] + "}")
    
    let empty = M Map.empty

    let isEmpty (M s) = Map.isEmpty s

    let size s = 
        match s with
        |M s -> Map.fold (fun acc k v -> acc + v) 0u s

    let contains a (M s) = s.ContainsKey a 

    let numItems a (M s) = 
           match s.TryFind a with
           | Some x -> x
           | None -> 0u

    let add a n (M s) = 
           match s.TryFind a with
           | Some x -> M (s.Add (a, x+n)) 
           | _ -> M (s.Add (a, n))


    let addSingle a s = add a 1u s

    let remove a n (M s) = 
           match s.TryFind a with
           | Some x when x > n -> M (s.Add (a, x-n)) 
           | Some x -> M (s.Remove a) 
           | _ -> M s

    let removeSingle a s = remove a 1u s
   
    let fold f acc (M s) =  Map.fold f acc s

    let foldBack f (M s) acc = Map.foldBack f s acc

    let map f s = fold (fun acc x n -> add (f x) n acc) empty s

    let ofList lst = List.fold (fun acc x -> addSingle x acc) empty lst

    let toList s = fold (fun acc x n -> acc@(List.ofArray (Array.init ((int) n) (fun z -> x)))) [] s

    let union s1 s2 = fold (fun acc k v -> 
        match s2 with
        | M m -> 
            match m.TryFind k with
                | Some x when x > v -> remove k v acc |> add k x
                | Some x -> acc
                | None -> acc
                        ) s1 s2

    let sum s1 s2 = fold (fun acc k v -> add k v acc) s1 s2

    let subtract s1 s2 = fold (fun acc k v -> remove k v acc) s1 s2 

    let intersection (M s1) (M s2) = 
        fold (fun (M acc) k v -> match acc.TryFind k with 
                                    | Some x when x >= v -> remove k (x-v) (M acc)
                                    | Some x -> remove k (v-x) (M acc)
                                    | None -> remove k v (M acc)) (M s1) (M s2) 
                                    |> fun (M x) -> M (Map.filter (fun k _ -> s2.ContainsKey k) x)
            