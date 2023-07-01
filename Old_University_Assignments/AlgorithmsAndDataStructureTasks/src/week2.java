
import edu.princeton.cs.algs4.Stack;
import edu.princeton.cs.algs4.StdIn;
import edu.princeton.cs.algs4.StdOut;

class Week2 {
    public static void main(String[] args) {
        int amount = StdIn.readInt();
        StdIn.readLine();
        boolean fail = false;
        Stack<Character> stack0 = new Stack<Character>();
        Stack<Character> stack1 = new Stack<Character>();
        Stack<Character> stack2 = new Stack<Character>();
        for(int i =0; i < amount; i++){
            stack0.push(StdIn.readChar());
        }
        for (int i = 0; i < amount ; i++) {
            if(stack0.peek().equals('[') || stack0.peek().equals(']')) stack1.push(stack0.pop());
            else stack2.push(stack0.pop());
        }
        if (stack1.size() % 2 != 0 ||stack2.size() % 2 != 0) fail = true;
        if (!fail) {
            int h = 0;
            int v = 0;
            int size = stack1.size();
            for(int i = 0; i < size; i++) {
                if(stack1.peek().equals(']')) v++;
                else h++;
                if (h < v) {fail = true; break;}
                stack1.pop();
            }
            if (h != v)fail = true;
            h = 0;
            v = 0;
            size = stack2.size();
            for (int i = 0; i < size; i++) {
                if(stack2.peek().equals(')')) v++;
                else h++;
                if (h < v) {fail = true; break;}
                stack2.pop();
            }
            if (h != v)fail = true;
        }
        if(!fail) StdOut.println("1");
        else StdOut.println("0");
    }
}
