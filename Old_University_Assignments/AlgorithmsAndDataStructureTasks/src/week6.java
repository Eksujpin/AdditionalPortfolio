

import java.util.HashMap;
import java.util.Scanner;
import static java.lang.Integer.*;

public class week6 {

    public static void main(String[] args) {
        Scanner read = new Scanner(System.in);
        HashMap flights = new HashMap();
        int nrFlights;
        int actions;

        nrFlights = read.nextInt();
        actions = read.nextInt();
        read.nextLine();

        for (int i = 0; i < nrFlights; i++) {
            String red = read.nextLine();
            String[] fixed = red.split(" ");
            flights.put(fixed[0],fixed[1]);
        }


        for (int i = 0; i < actions ; i++) {
            String red = read.nextLine();
            String[] fixed = red.split(" ");
            switch (fixed[0]){
                case "destination":
                    if(flights.get(fixed[1]) != null) System.out.println(flights.get(fixed[1]));
                    else System.out.println("-");
                    break;
                case "reroute":
                    flights.replace(fixed[1],fixed[2]);
                    break;
                case "cancel":
                    flights.remove(fixed[1]);
                    break;
                case"delay":
                    String temp = flights.get(fixed[1]).toString();
                    flights.remove(fixed[1]).toString();
                    String[] numbers = fixed[1].split(":");
                    int[]values = new int[3];
                    for (int j = 0; j < numbers.length; j++) {
                        values[j] = parseInt(numbers[j]);
                    }
                    //int temp = values[2] + values[1]*60 + values[0]*3600 + valueOf(fixed[2]);
                    for (int j = 0; j < valueOf(fixed[2]); j++) {
                        values[2] = values[2]+1;
                        if (values[2] >59){
                            values[2] = 0;
                            values[1]++;
                        }
                        if (values[1] >59){
                            values[1] = 0;
                            values[0]++;
                        }
                    }
                    for (int j = 0; j < numbers.length; j++) {
                        String formed = String.format("%02d", values[j]);
                        numbers[j] = formed;
                    }
                    flights.put(numbers[0]+":"+numbers[1]+":"+numbers[2],temp);
                    break;
            }


        }


    }



}
