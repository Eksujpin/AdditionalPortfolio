package model;

import model.exceptions.MediumNotFoundException;
import model.exceptions.UsernameAlreadyExistException;
import java.util.ArrayList;
import java.util.List;

public class StreamingService {
    private List<Medium> mediums;
    private List<User> users;
    private User currentUser;

    public static StreamingService getStreamingService(){
        StreamingService streamingService;
        streamingService = new StreamingService();
        return streamingService;
    }

    // Læser filerne "movies.txt" og "series.txt"  og tilføjer til samlet liste når objekt oprettes.
    private StreamingService(){
        users = new ArrayList<>();
        mediums = new ArrayList<>();
        MediumReader movieReader = new MediumReader("movies.txt");
        mediums.addAll(movieReader.readMediums());
        MediumReader seriesReader = new MediumReader("series.txt");
        mediums.addAll(seriesReader.readMediums());
    }

    // Metoden kan oprette en ny bruger. Før dette tjekker den om der allerede er en anden bruger med det username der bruges.
    // Hvis brugernavn allerede eksitere, kaster den en exception.
    public void createUser(String username) {
        for (User u: users) {
            if(u.getUserName().equals(username)){
              throw new UsernameAlreadyExistException();
            }
        }
        User user = new User(username);
        users.add(user);
    }

    // Metoden kan skifte mellem brugerne.
    public void switchUser(String switchTo){
        for (User user:users){
            if (user.getUserName().equals(switchTo)){
                currentUser = user;
            }
        }
    }

    public List<User> getUsers() {
        return users;
    }

    public User getCurrentUser() {
        return currentUser;
    }

    // Metoden retunerer listen med alle mediums
    public List<Medium> getMediums(){
        return mediums;
    }

    //Metoden retunerer et specefikt medium ud fra en title
    public Medium getMedium (String title)throws MediumNotFoundException{
        for (Medium med:mediums) {
            if(med.getTitle().equals(title)) return med;
        }throw new MediumNotFoundException();
    }

    // Metoden retunere Årstaller for det specifikke medium. Hvis titel ikke findes på listen, kastes exception.
    public String getYear(String title) throws MediumNotFoundException {
        for (Medium medium : mediums){
            if(medium.getTitle().equals(title)){
                return medium.getYear();
            }
        } throw new MediumNotFoundException();
    }

    // Metoden retunere listen med genre for det specifikke medium. Hvis titel ikke findes på listen, kastes exception.
    public String getGenre(String title) throws MediumNotFoundException {
        String output;
        for (Medium medium : mediums){
            if(medium.getTitle().equals(title)){
                output = medium.getGenre();
                return output;
            }
        } throw new MediumNotFoundException();
    }

    // Metoden retunere scoren for det specifikke medium. Hvis titel ikke findes på listen, kastes exception.
    public Double getScore(String title) throws MediumNotFoundException {
        for (Medium medium : mediums){
            if(medium.getTitle().equals(title)){
                return medium.getScore();
            }
        } throw new MediumNotFoundException();
    }

    // Metoden retunere strengen med sæsoner og episoder for det specifikke medium. Hvis titel ikke findes på listen, kastes exception.
    public  String getSeasonsAndEpisodes(String title) throws MediumNotFoundException {
        String seasonsAndEpisodes;
        for (Medium medium : mediums)
        {
            if (medium.getTitle().equals(title)){
                if (medium instanceof Series){
                    Series s = (Series) medium;
                    seasonsAndEpisodes = s.getSeaAndEpi();
                    return seasonsAndEpisodes;
                }
            }
        } throw new MediumNotFoundException();
    }

    // Metoden retunerer posterens url i form af en string. Hvis titel ikke findes på listen, kastes exception.
    public String getPoster(String title) throws MediumNotFoundException {
        for (Medium medium : mediums){
            if(medium.getTitle().equals(title)){
                return medium.getPosterURL();
            }
        } throw new MediumNotFoundException();
    }

    // Metoden henter en list af mediums ud fra en type, i dette tilfælde om det er film eller serier
    public List<Medium> speceficList(String type) {
        List<Medium> output = new ArrayList<>();
        if (type.toLowerCase().equals("movies")) {
            for (Medium med : mediums) {
                if (med instanceof Movie) output.add(med);
            }
        } else if (type.toLowerCase().equals("series")) {
            for (Medium med : mediums) {
                if (med instanceof Series) output.add(med);
            }
        }
        return output;
    }

    // Denne metode har to funktioner, den søger efter et medium med titlen eller søger efter mediums der indeholder en genre
    public List<Medium> search(String input, List<Medium> searchThisList) throws MediumNotFoundException{
        List<Medium>searchResult = new ArrayList<>();
        for (Medium medium : searchThisList) {
            if (medium.getTitle().toLowerCase().contains(input.toLowerCase())) {
                searchResult.add(medium);
            }else if(medium.getGenre().toLowerCase().contains(input.toLowerCase())){
                searchResult.add(medium);
            }
        }
        if (searchResult.size() <= 0){
            throw new MediumNotFoundException();
        }
        return searchResult;
    }

}
