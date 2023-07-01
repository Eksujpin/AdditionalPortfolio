package model;

import model.exceptions.MediumAlreadyAtMyListException;

import java.util.ArrayList;
import java.util.List;

public class Demo {

    public static void main(String[] args) {
        //testLoad();
        userTest();
    }

    public static void testLoad(){

        List<Medium> mediums;
        int movies = 0;
        int series = 0;


        mediums = new ArrayList<>();

        MediumReader test = new MediumReader("movies.txt");
        mediums.addAll(test.readMediums());
        MediumReader test1 = new MediumReader("series.txt");
        mediums.addAll(test1.readMediums());

        for (Medium medium: mediums) {
            if (medium instanceof Movie){
                movies++;
                System.out.println(medium.getTitle()+" "+medium.getYear()+" "+medium.getGenre()+" "+medium.getScore());
            }
            if (medium instanceof Series){
                series++;
                System.out.println(medium.getTitle()+" "+medium.getYear()+" "+medium.getGenre()+" "+medium.getScore()+ " || "+((Series) medium).getSeaAndEpi());
            }

            System.out.println("test of picture.:  "+medium.getPosterURL());
        }
        System.out.println("Size of array: " +mediums.size());
        System.out.println("Counted movies: " +movies);
        System.out.println("Counted series: " +series);

        String input = "god";
        List<Medium>searchResult = new ArrayList<>();
        for (Medium medium : mediums) {
            if (medium.getTitle().toLowerCase().contains(input.toLowerCase())) {
                System.out.println(medium.getTitle());
                searchResult.add(medium);
            }
        }
    }

    public  static void userTest() {
        List<User> users;
        User currentUser;
        Medium test1 = new Movie("12 Strong", "2017", "Action, war, Drama", 8.7);
        Medium test2 = new Movie("13 Strong", "2017", "Action, war, Drama", 8.7);

        users = new ArrayList<>();
        User user1 = new User("user1");
        User user2 = new User("user2");
        users.add(user1);
        users.add(user2);
        currentUser = user1;
        try {
            System.out.println("#1 " + currentUser.getUserName());
            currentUser.addMedium(test1);
            for (Medium med : currentUser.getMyList()) {
                System.out.println(med.getTitle());
            }

            for (User user : users) {
                if (user.getUserName().equals("user2")) {
                    currentUser = user;
                }
            }

            System.out.println("#2 " + currentUser.getUserName());
            currentUser.addMedium(test2);

            for (Medium med : currentUser.getMyList()) {
                System.out.println(med.getTitle());
            }

            for (User user : users) {
                if (user.getUserName().equals("user1")) {
                    currentUser = user;
                }
            }
            currentUser.addMedium(test2);
            System.out.println("#3 " + currentUser.getUserName());
            for (Medium med : currentUser.getMyList()) {
                System.out.println(med.getTitle());
            }
            currentUser.removeMedium(test2);
            System.out.println("removal of 13 strong");
            for (Medium med : currentUser.getMyList()) {
                System.out.println(med.getTitle());
            }
            currentUser.addMedium(test1);
        } catch (MediumAlreadyAtMyListException e){
            e.printStackTrace();
        }

    }
}