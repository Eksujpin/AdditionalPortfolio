
//exercise 11.1
let rec len xs =
    match xs with
    | [] -> 0
    | x::xr -> 1 + len xr

let rec lenc xs con =
    match xs with
    | [] -> con 0
    | x::xr -> lenc xr (fun v -> con (v+1))

let rec leni xs acc =
    match xs with
    | [] -> acc
    | x::xr -> leni xr (acc+1)


lenc [2;5;7] id;;

lenc [2;5;7] (printf "the answer is %d\n");;

lenc [2;5;7] (fun v -> 2*v);;

leni [2;5;7] 0;;


//exercise 11.2

let rec rev xs = 
    match xs with
    |[] -> []
    |x::xr -> rev xr @ [x]

let rec revc xs con = 
    match xs with
    |[] -> con []
    |x::xr -> revc xr (fun v -> con (v@[x]))

let rec revi xs acc = 
    match xs with
    |[] -> acc
    |x::xr -> revi xr (x::acc)

revc [2;5;7] id;;

revc [2;5;7] (fun v -> v@v);;

revi [2;5;7] [];;

//exercise 11.3
let rec prod xs = 
    match xs with
    |[] -> 1
    |x::xr -> x* prod xr

let rec prodc xs con = 
    match xs with
    |[] -> con 1
    |x::xr -> prodc xr (fun v -> con (x*v))

let rec prodc2 xs con = 
    match xs with
    |[] -> con 1
    |0::_ -> 0
    |x::xr -> prodc2 xr (fun v -> con (x*v))

let rec prodi xs acc = 
    match xs with
    |[] -> acc
    |0::_ -> 0
    |x::xr -> prodi xr (acc*x)

prodc [2;5;7] id;;
prodc2 [2;0;7;5;3;8;2;2;] id;;
prodc2 [2;5;7;5;3;8;2;2;] id;;
prodi [2;0;7;5;3;8;2;2;] 1;;
prodi [2;5;7;5;3;8;2;2;] 1;;