import edu.princeton.cs.algs4.UF;
import edu.princeton.cs.algs4.StdIn;
import edu.princeton.cs.algs4.StdOut;

class Week1 {
    public static void main(String[] args) {
        int initial = StdIn.readInt();
        var UFSet = new UF(initial);
        StdIn.readInt();
        while (!StdIn.isEmpty()) {
            int action = StdIn.readInt();
            int p = StdIn.readInt();
            int q = StdIn.readInt();
            if(action == 0){
                if (UFSet.find(p) == UFSet.find(q)) StdOut.println("1");
                else{StdOut.println("0");}
            }else if(action == 1){
                UFSet.union(p, q);
            }else if(action == 2){
                if(UFSet.find(p) != UFSet.find(q)){

                }
            }
        }
    }

}