import java.net.*;
import java.util.LinkedList;

public class ReliableUDPServer {
    private static LinkedList<String> codes = new LinkedList<String>();
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
                    byte[] recieved = new byte[291];// Allocate a buffer into which the code is written 36b code and 255b msg
                    
                    DatagramPacket formated = new DatagramPacket(recieved, recieved.length);
                    aSocket.receive(formated);
                    DatagramPacket code = new DatagramPacket(formated.getData(), ReliableUDPClient.CHECK_LENGTH, formated.getAddress(), formated.getPort());
                    String msg = new String(formated.getData()).substring(ReliableUDPClient.CHECK_LENGTH);
                    
                    
                    if(!codes.contains(new String(code.getData()))){
                        System.out.println(msg);
                        if(codes.size() > 64) codes.removeFirst();
                        codes.add(new String(code.getData()));
                    }
                    
                    aSocket.send(code);
                    
        
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
