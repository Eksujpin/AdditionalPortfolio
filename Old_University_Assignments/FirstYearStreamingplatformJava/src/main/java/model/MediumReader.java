package model;

import model.exceptions.FileIsEmptyException;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

public class MediumReader {

    private InputStream stream;
    private BufferedReader br;
    private boolean series = false;

    public MediumReader(String fileName) {
        if(fileName.isEmpty() || fileName == null){
            throw new FileIsEmptyException();
        }
        //stream gør at filer kan læses uanset hvor i projektet det er placeret.
        //Taget fra LiveCoding10 - class: PictureReader - line 17-18
        stream = Thread.currentThread().getContextClassLoader().getResourceAsStream(fileName);
        br = new BufferedReader(new InputStreamReader(stream));
        //henvisning slut

        if (fileName.equals("series.txt")) {
            series = true;
        }
    }

    // Så længe næste linje i filen der læses fra ikke et tom. Bliver den opdelt og indlæst på en string array.
    // Derefter bliver et opjekt oprettet med de givne værdier og tilføjet til en følles ArrayListe for alle objekterne.
    // Metoden tjekker om Mediet er en film eller en Serie og indlæser det på en liste, hvilken retuneres til sidst.
    public List<Medium> readMediums(){
        List<Medium> mediums = new ArrayList<>();
        String line;
        try{
            while ((line = br.readLine())!= null){
                String[] info = line.split(";");

                String title = info[0].trim();
                String year = info[1].trim();
                String genre = info[2].trim();
                double score = Double.parseDouble(info[3].replace(";","").replace(",",".").trim());
                if(series){
                    String seasonsAndEpisodes = info[4].trim();
                    Medium m = new Series(title, year, genre, score,seasonsAndEpisodes);
                    m.setPosterURL((this.getClass().getResource("/seriesPosters/"+m.getTitle()+".jpg")).toString());
                    mediums.add(m);
                }else{
                    Medium m = new Movie(title, year, genre, score);
                    m.setPosterURL((this.getClass().getResource("/moviePosters/"+m.getTitle()+".jpg")).toString());
                    mediums.add(m);
                }
            }
        } catch (IOException e){
            e.printStackTrace();
        }
        return mediums;
    }
}

