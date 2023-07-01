package modelTest;

import model.Medium;
import model.Movie;
import model.Series;
import model.StreamingService;
import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

class StreamingServiceTest {

    StreamingService model = StreamingService.getStreamingService();

    @Test
    public void testList() {
        int expectedMovies = 100;
        int expectedSeries = 100;
        String expctedFirstMovie = "The Godfather";
        String expctedLastMovie = "Yankee Doodle Dandy";
        String expctedFirstSeries = "Twin Peaks";
        String expctedLastSeries = "Dexter";
        int actualMovies = 0;
        int actualSeries = 0;
        List<Medium> mediums = model.getMediums();
        for (Medium medium : mediums) {
            if (medium instanceof Movie) {
                List<String> movieString = new ArrayList<>();
                movieString.add(medium.getTitle());movieString.add(medium.getYear());movieString.add(medium.getGenre());
                movieString.add(String.valueOf(medium.getScore())); movieString.add(medium.getPosterURL());
                actualMovies++;
                assertEquals(5,movieString.size(),"Series does not contain all parameters");
            }
            if (medium instanceof Series) {
                List<String> seriesString = new ArrayList<>();
                seriesString.add(medium.getTitle());seriesString.add(medium.getYear());seriesString.add(medium.getGenre());
                seriesString.add(String.valueOf(medium.getScore()));seriesString.add(medium.getPosterURL());
                seriesString.add(((Series) medium).getSeaAndEpi());
                actualSeries++;
                assertEquals(6,seriesString.size(),"Series does not contain all parameters");
            }
        }
        String actualFirstMovie = mediums.get(0).getTitle();
        String actualLastMovie = mediums.get(99).getTitle();
        String actualFirstSeries = mediums.get(100).getTitle();
        String actualLastSeries = mediums.get(199).getTitle();

        assertEquals(expectedMovies,actualMovies,"movie count fail");
        assertEquals(expectedSeries,actualSeries,"series count fail");
        assertEquals(expctedFirstMovie,actualFirstMovie,"wrong first movie");
        assertEquals(expctedLastMovie,actualLastMovie,"wrong last movie");
        assertEquals(expctedFirstSeries,actualFirstSeries,"wrong first series");
        assertEquals(expctedLastSeries,actualLastSeries,"wrong last movie");

    }

    @Test
    public void createUserTest(){
        String expectedUser = "Jonas";
        int expectedAmount = 2;
        model.createUser("Jonas");
        model.createUser("Joachim");
        model.switchUser("Jonas");
        int actualAmount = model.getUsers().size();
        String actualUser =model.getCurrentUser().getUserName();

        assertEquals(expectedUser, actualUser,"Wrong user");
        assertEquals(expectedAmount, actualAmount,"Wrong amount");
    }

    @Test
    public void myListTest(){
        List<Medium> actualMylist = null;
        List<Medium> actualMylist2 = null;
        model.createUser("Jonas");
        model.createUser("Joachim");
        model.switchUser("Jonas");
        int expectedSize = 2;
        try{
            model.getCurrentUser().addMedium(model.getMedium("The Godfather"));
            model.getCurrentUser().addMedium(model.getMedium("GLOW"));
            actualMylist = model.getCurrentUser().getMyList();
            model.switchUser("Joachim");
            model.getCurrentUser().addMedium(model.getMedium("Rain Man"));
            model.getCurrentUser().addMedium(model.getMedium("House"));
            model.getCurrentUser().addMedium(model.getMedium("Dexter"));
            model.getCurrentUser().removeMedium(model.getMedium("Dexter"));
            actualMylist2 = model.getCurrentUser().getMyList();
        }catch (Exception e){
            e.printStackTrace();
        }
        assertEquals("The Godfather",actualMylist.get(0).getTitle(),"Wrong #1 medium on Jonas list");
        assertEquals("GLOW",actualMylist.get(1).getTitle(),"Wrong #2 medium on Jonas list");
        assertEquals("Rain Man",actualMylist2.get(0).getTitle(),"Wrong #1 medium on Joachim list");
        assertEquals("House",actualMylist2.get(1).getTitle(),"Wrong #2 medium on Joachim list");
        assertEquals(expectedSize,actualMylist2.size(),"Remove didnt work");
    }

    @Test
    public void testSearch(){
        try {
            List<Medium> return1 = model.search("god", model.getMediums());
            List<Medium> return2 = model.search("Animation", model.speceficList("series"));
            assertEquals("The Godfather",return1.get(0).getTitle(),"#1 result failed");
            assertEquals("The Godfather part II",return1.get(1).getTitle(),"#2 result failed");
            assertEquals("The Simpsons",return2.get(0).getTitle(),"#1 genre result failed");
            assertEquals("South Park",return2.get(1).getTitle(),"#2 genre result failed");
        }catch (Exception e){e.printStackTrace();}
    }

}