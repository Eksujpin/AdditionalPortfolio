
import edu.princeton.cs.algs4.Bipartite;
import edu.princeton.cs.algs4.Graph;

import java.util.Scanner;

public class week7 {
    public static void main(String[] args) {
        Scanner read = new Scanner(System.in);
        int verts;
        int edges;
        verts = read.nextInt();
        edges = read.nextInt();
        Graph graph = new Graph(verts);
        read.nextLine();

        for (int i = 0; i < edges; i++) {

            graph.addEdge(read.nextInt(),read.nextInt());
            read.nextLine();
        }

        Bipartite test = new Bipartite(graph);
        if (test.isBipartite())System.out.println("1");
        else System.out.println("0");


    }

}
