import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.Insertion;
import edu.princeton.cs.algs4.StdIn;
import edu.princeton.cs.algs4.StdOut;

public class week4 {
    public static void main(String[] args) {
        int amount = StdIn.readInt();
        StdIn.readLine();
        String[] names = new String[amount];
        for (int i = 0; i < amount; i++) {
            var splitter = new String[1];
            splitter = StdIn.readLine().split(" ");
            if(splitter[1].contains("F")) splitter[1] = splitter[1].replace("F","G");
            if(splitter[1].contains("GX")) splitter[1] = splitter[1].replace("GX","F");
            if(splitter[1].length() == 1)splitter[1] = splitter[1]+"B";
            //System.out.println(splitter[1]);
            if(splitter[1].contains("+")){
                String in = "AAAAAAAAAA";
                for (int j = 0; j < splitter[1].length(); j++) {
                    in = in.replaceFirst("A","");
                }
                splitter[1] = splitter[1].replace("+","");
                splitter[1]= splitter[1]+in;
            }
            if(splitter[1].contains("-")) splitter[1] = splitter[1].replace("-","C");
            String out = splitter[1]+" "+splitter[0];
            names[i] = out;
            //System.out.println(names[i]);
        }
        Insertion.sort(names);

        for (int i = 0; i < names.length; i++) {
            var split = new String[1];
            split = names[i].split(" ");
            String out = split[1];
            names[i] = out;
            //System.out.println(names[i]);
        }
        for (String name:names) {
            System.out.println(name);
        }
    }
}
