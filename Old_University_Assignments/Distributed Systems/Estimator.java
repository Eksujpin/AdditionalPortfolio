import java.io.IOException;
import java.net.*;
import java.util.*;
import java.util.concurrent.TimeUnit;

public class Estimator {
    private int size;
    private int number;
    private int interval;

    public static void main(String[] args)throws Exception{
        var est = new Estimator(10, 10000, 10);
        var from = new DatagramSocket(8008);
        //var toAddress = InetAddress.getByName("localhost");
        //var toAddress = InetAddress.getByName("192.168.0.106");
        var toAddress = InetAddress.getByName("192.168.0.107");
        int toPort = 7007;
        est.estimate(from, toAddress, toPort);

       
        /*List<String> sent = new ArrayList<>(Arrays.asList("00", "01", "02", "03", "04", "05"));
        List<String> received1 = new ArrayList<>(Arrays.asList("00", "01", "01", "03", "02", "05"));
        List<String> received2 = new ArrayList<>(Arrays.asList("00", "01", "04", "05"));
        List<String> received3 = new ArrayList<>(Arrays.asList("05", "04", "03", "02", "01", "00"));

        System.out.println(Arrays.toString(Estimator.LostCounter(sent, received1)));
        System.out.println(Arrays.toString(Estimator.LostCounter(sent, received2)));
        System.out.println(Arrays.toString(Estimator.LostCounter(sent, received3)));*/
    }

    public Estimator(int size, int number, int interval){
        this.size = size;
        this.number = number;
        this.interval = interval;
    }

    public void estimate(DatagramSocket from, InetAddress toAddress, int toPort) throws Exception{
        List<String> sentMessages = new ArrayList<>();
        List<String> receivedMessages = new ArrayList<>();
        for(int i = 0; i < number; i++){
            
            var message = createZeroPaddedMessage(size, "" + i);
            var msgByte = message.getBytes();
            sentMessages.add(message);
            DatagramPacket packet = new DatagramPacket(msgByte, msgByte.length, toAddress, toPort);
            
            from.send(packet);
            TimeUnit.MILLISECONDS.sleep(interval);
            if(i % 1000 == 0){
                receivedMessages.addAll(emptySocket(from));
            }
        }

        receivedMessages.addAll(emptySocket(from));

        var stats = LostCounter(sentMessages, receivedMessages); 

        System.out.println(String.format("Lost messages: %d/%d, %.2f %%", stats[0], number, 100.0 * stats[0]/number));
        System.out.println(String.format("Dublicated messages: %d/%d, %.2f %%", stats[1], number, 100.0 * stats[1]/number));
        System.out.println(String.format("Reordered messages: %d/%d", stats[2], number, 100.0 * stats[2]/number));        

    }

    private String createZeroPaddedMessage(int size, String originalMessage){
        StringBuilder sb = new StringBuilder();

        for(int i = originalMessage.length(); i < size; i++){
            sb.append("0");
        }
        sb.append(originalMessage);
        return sb.toString(); 
    }

    private List<String> emptySocket(DatagramSocket toEmpty) throws IOException{
        /* Empty all UDP messages from Socket.
         * A datagramsocket can hold a lot of messages,
         * by experiment we put 2000 messages in queue and none where deleted
         */
        List<String> messages = new ArrayList<>();

        toEmpty.setSoTimeout(250);
        while(true){
            try{
                DatagramPacket response = new DatagramPacket(new byte[size], size);
                toEmpty.receive(response);
                messages.add(new String(response.getData()));
            } catch(SocketTimeoutException e) {
               //System.out.println("Timed out!"); 
                break;
            }
        }

        toEmpty.setSoTimeout(0);  // Set timeout to infinite

        return messages;
    }

    public static int[] LostCounter(List<String> sent, List<String> received){
        int lose = 0;
        int dup = 0;
        int reorder = 0;
        //check if everything is good
        if(!received.equals(sent)){
            Set<String> noDup = new HashSet<>();
            //check for duplicates
            for (String msg : received){
                if (noDup.add(msg)==false) 
                dup++;
            }
            //check for lost packets
            for(String message : sent){
                if(!noDup.contains(message)){
                    System.out.println(message);
                    lose++;
                }
            }

            //check for reoreder
            // A reordered packet is defined as on which arrives out of sequence in relation to its neighbours.
            // Since our messages are numbered, that means that if a message is higher than the left neighbour and lower than the right
            // it us ordered. Otherwise, it is reordered. 

            for(int i = 0; i < received.size(); i++){
                int left, cur, right;
                try{
                    left = Integer.parseInt(received.get(i-1));
                } catch(IndexOutOfBoundsException e){
                    left = Integer.MIN_VALUE;
                }

                cur = Integer.parseInt(received.get(i));
                
                try{
                    right = Integer.parseInt(received.get(i+1));
                } catch(IndexOutOfBoundsException e){
                    right = Integer.MAX_VALUE;
                }
                
                if(!(left <= cur && cur <= right)) {
                    reorder++;
                }
            }
            
        }
        int[] tmp = new int[]{lose, dup, reorder};
        return tmp;
    }
}
