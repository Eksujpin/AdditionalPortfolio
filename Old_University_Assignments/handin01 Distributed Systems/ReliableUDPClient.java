import java.io.IOException;
import java.net.*;
import java.util.UUID;

public class ReliableUDPClient {
    public static final int CHECK_LENGTH = 36;

    private InetAddress toAddress;
    private int toPort;
    private String message;
    private DatagramSocket socket;

    public static void main(String[] args) throws IOException{
        new ReliableUDPClient(InetAddress.getByName(args[0]), Integer.parseInt(args[1]), args[2]);
        
    }

    public ReliableUDPClient (InetAddress toAddress, int toPort, String message) throws IOException{
        this.toAddress = toAddress;
        this.toPort = toPort;
        this.message = message;

        socket = new DatagramSocket(8008);
        socket.setSoTimeout(250);
        send();
        socket.close();
    }

    private void send()throws IOException{
        String generatedUuid = UUID.randomUUID().toString();
        String receivedUuid = "";

        while(!generatedUuid.equals(receivedUuid)){
            var messageBytes = (generatedUuid + message).getBytes();
            DatagramPacket packet = new DatagramPacket(messageBytes, messageBytes.length, toAddress, toPort);
            socket.send(packet);

            receivedUuid = receiveCheck().trim();
        //    System.out.println(receivedUuid + " =? " + generatedUuid);
          //  System.out.println(receivedUuid.equals(generatedUuid));
        }   
    }

    private String receiveCheck()throws IOException{
        try {
            DatagramPacket check = new DatagramPacket(new byte[CHECK_LENGTH], CHECK_LENGTH);
            socket.receive(check);
            return new String(check.getData());
        } catch (SocketTimeoutException e) {
            // Received no message, return empty checksum
            return "";
        }
    }
}
