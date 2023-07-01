
import edu.princeton.cs.algs4.MaxPQ;
import edu.princeton.cs.algs4.StdIn;
import edu.princeton.cs.algs4.StdOut;


public class week5 {
    public static void main(String[] args) {
        int parties = StdIn.readInt();
        int seats = StdIn.readInt();
        float[] votes = new float[parties];
        float[] partyList = new float[parties];
        float[] orgPartyList = new float[parties];
        var testList = new MaxPQ();
        StdIn.readLine();

        for (int i = 0; i < parties; i++) {
            int temp = StdIn.readInt();
            votes[i] = 0;
            partyList[i] = temp;
            orgPartyList[i] = temp;
            testList.insert(temp);
        }


        for (int i = 0; i < seats; i++) {
            int biggest = 0;
            for (int j = 0; j <parties; j++) {
                //StdOut.println(partyList[biggest]+" < "+partyList[j] +" j = "+j);
                if (partyList[biggest] < partyList[j]) {biggest = j;
                    //StdOut.println("change");
                }
            }
            //StdOut.println("old: "+partyList[biggest]);
            //StdOut.println("point for " +biggest);
            votes[biggest] = votes[biggest]+1;
            //StdOut.println("var = "+orgPartyList[biggest]/(votes[biggest]+1));

            partyList[biggest] = orgPartyList[biggest]/(votes[biggest]+1);
            //StdOut.println("new: "+partyList[biggest]);
        }
        for (var vote:votes) {
            StdOut.println((int)vote);
        }



        //print whole list
        /*for (int i = 0; i < parties; i++) {
            StdOut.println(testList.delMax());
        }*/

        //WIP doesent work
        /*
        int votesCast = 0;
        for (int i = 0; i < seats; i++) {
            for (int j = 0; j < parties; j++) {
                if (partyList[j] == (Integer)testList.max() && votesCast < seats){
                    votes[j] = votes[j]+1;
                    int newvote = orgPartyList[j]/(votes[j]+1);
                    partyList[j] = newvote;
                    StdOut.println("change "+ newvote);
                    testList.insert(newvote);
                    testList.delMax();
                    votesCast++;
                }
            }
        }

        for (int i = 0; i <votes.length ; i++) {
            StdOut.println(votes[i]);
        }*/

    }
}
