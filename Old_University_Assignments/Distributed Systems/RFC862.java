
import java.net.*;

public class RFC862{
    public static void main(String[] args){
        DatagramSocket aSocket = null;
        try{
            aSocket = new DatagramSocket(7007);
        } catch (Exception e){
            e.printStackTrace();
            aSocket.close();
            return;
        }
        

        try{

            while(true){
                //Receive reply
                try{
                    byte[] buffer = new byte[1000]; // Allocate a buffer into which the reply message is written
                    DatagramPacket reply = new DatagramPacket(buffer, buffer.length);
                    aSocket.receive(reply);
                    
                    //InetAddress aHost = InetAddress.getByName("localhost");
                    DatagramPacket request = new DatagramPacket(reply.getData(), reply.getLength(), reply.getAddress(), reply.getPort());
                    aSocket.send(request);

        
                } catch(Exception e){
                    e.printStackTrace();
                    aSocket.close();
                    return;
                }
            }
        }
        finally{
            aSocket.close();
        }
    }
}