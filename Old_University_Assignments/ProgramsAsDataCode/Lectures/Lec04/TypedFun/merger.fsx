let list1 = [ 3; 5; 12]
let list2 = [ 2; 3; 4; 7]


let rec merge lst lst2 = 
    match (lst, lst2) with
    |(x::xs, y::ys) -> if x > y then y::(merge lst ys) else x::(merge xs lst2)
    |(rst, [])
    |([], rst) -> rst