public class Merger {


public static void main(String[] args) {
    int[] xs = {3,5,12};
    int[] ys = {2,3,4,7};
    int[] res = merge(ys, xs);
    for (int i : res) {
       System.out.println(i); 
    }
 }
    
public static int[] merge (int[] xs, int[] ys) {
    int[] res = new int[xs.length+ ys.length];
    int countX = 0;
    int countY = 0;

    for (int i = 0; i < res.length; i++ ) {
        if (countX >= xs.length) {
            res[i] = ys[countY];
            countY ++;
        } else if (countY >= ys.length){
            res[i] = xs[countX];
            countX ++;
        }else if ( xs[countX] < ys[countY] ){
            res[i] = xs[countX];
            countX ++;
        }else {
            res[i] = ys[countY];
            countY ++;
        }
    }
    return res;
}

}
