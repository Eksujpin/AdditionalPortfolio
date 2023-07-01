package model;

public abstract class Medium {

    private String title;
    private String year;
    private String genres;
    private double score;
    private String posterURL;

    public Medium(String title, String year, String lineOfGenres, double score) {
        this.title = title;
        this.year = year;
        this.score = score;
        this.genres = lineOfGenres;
    }

    public String getTitle() {
        return title;
    }

    public String getYear() {
        return year;
    }

    public double getScore() {
        return score;
    }

    public String getGenre() {
        return genres;
    }

    public String getPosterURL() {
        return posterURL;
    }

    public void setPosterURL(String posterURL) {
        this.posterURL = posterURL;
    }
}
