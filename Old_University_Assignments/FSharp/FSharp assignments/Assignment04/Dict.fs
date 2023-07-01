module Dict
    
    type Dict = 
        |Dic of List<string>
    
    let empty = 
        function 
        | () -> Dic List.Empty
    
    let insert (s:string) (Dic(d)) = Dic(s::d)
    
    let lookup (s:string) (Dic(d)) = List.contains s d
 