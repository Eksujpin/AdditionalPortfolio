package model;

public class Series extends Medium{

    private String seasonsAndEpisodes;

    public Series(String title, String year, String genres, double score, String seasonsAndEpisodes) {
        super(title, year, genres, score);
        this.seasonsAndEpisodes = seasonsAndEpisodes;
    }

    public String getSeaAndEpi() {
        return seasonsAndEpisodes;
    }

}
