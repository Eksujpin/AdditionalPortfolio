import java.util.Random;
import java.net.DatagramSocket;
import java.net.DatagramPacket;
import java.net.SocketException;
import java.io.IOException;

public class QuestionableDatagramSocket extends DatagramSocket{
    private DatagramPacket reordered;

    public QuestionableDatagramSocket() throws SocketException{
        super();
    }

    public QuestionableDatagramSocket(int i) throws SocketException{
        super(i);
    }

    public void send(DatagramPacket p) throws IOException{ 
        Random rand = new Random();
        int action = rand.nextInt(4);
        System.out.println(action);
        
        switch(action){
            case 0: //Discard
                break;
            case 1: //Duplicate
                super.send(p);
                super.send(p);
                sendReorder();
                break;
            case 2: //Reorder
                sendReorder();
                reordered = p;
                break;
            case 3://Send
                super.send(p);
                sendReorder();
                break;
        }
    }

    private void sendReorder() throws IOException{
        if(reordered != null){
            super.send(reordered);
            reordered = null;
        }
    }

    public void close() {
        try {
            sendReorder();
        } catch (IOException e) {
            System.out.println(e.getStackTrace());     
        }
        
        super.close();
    }
}
