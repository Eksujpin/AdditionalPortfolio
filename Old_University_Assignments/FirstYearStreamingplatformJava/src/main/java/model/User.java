package model;

import model.exceptions.MediumAlreadyAtMyListException;
import java.util.ArrayList;
import java.util.List;

public class User {

    private String userName;
    private List<Medium> myList;

    public User(String userName) {
        this.userName = userName;
        myList = new ArrayList<>();
    }

    public String getUserName() {
        return userName;
    }
    public List<Medium> getMyList() {
        return myList;
    }

    // Metode tilføjer et medium til en brugers myList. Kaster exception, hvis medium allerede er på listen.
    public void addMedium(Medium medium)throws MediumAlreadyAtMyListException{
        for (Medium med:myList) {
            if (med.equals(medium)){
                throw new MediumAlreadyAtMyListException();
            }
        }
        myList.add(medium);
    }

    // Metode løber en brugers myList igennem og fjerner et givent medium.
    public void removeMedium(Medium medium){
        for (Medium m: myList) {
            if (m.equals(medium)){
               myList.remove(m);
               break;
            }
        }
    }

}


