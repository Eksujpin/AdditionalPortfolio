package controller;

import javafx.scene.control.*;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.layout.GridPane;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import model.*;
import model.exceptions.MediumAlreadyAtMyListException;
import model.exceptions.MediumNotFoundException;

public class Controller {
    //VARIABLS
    private StreamingService model;
    //FXML IMPORTS
    @FXML private GridPane primaryGrid;
    @FXML private ScrollPane scroller;
    @FXML private Label nameSpace;
    @FXML private ImageView infoImage;
    @FXML private Label Title;
    @FXML private Label Genre;
    @FXML private Label Year;
    @FXML private Label Seasons;
    @FXML private Label Score;
    @FXML private Button add_btn;
    @FXML private Button remove_btn;
    @FXML private Button play_btn;
    @FXML private Button stop_btn;
    @FXML private ButtonBar controlButtonBar;
    @FXML private TextField searchField;
    @FXML private Label textField;
    @FXML private Label userLabel;

    public Controller() {
        model = StreamingService.getStreamingService();
        premadeUsers();
        model.switchUser("Joachim");
    }

    // skaber tre forsklige premade users med en tilfædig mængde mediums på deres mylist
    private void premadeUsers(){
        model.createUser("Anne-Katrine");
        model.createUser("Jonas");
        model.createUser("Joachim");
        List<Integer> oldValues = new ArrayList<>();
        int nextValue = new Random().nextInt(200);
        int limit;
        try {
            for (User user : model.getUsers()) {
                limit = new Random().nextInt(20+1)+5;
                for(int i = 0; i < limit; i++){
                    while (oldValues.contains(nextValue)){
                        nextValue = new Random().nextInt(200);
                    }
                    user.addMedium(model.getMediums().get(nextValue));
                    oldValues.add(nextValue);
                }
            }
        }catch (MediumAlreadyAtMyListException e){
            e.printStackTrace();
        }
    }

    // Skifter til en anden bruger ud fra den knap der er trykket på
    public void updateUser(ActionEvent e){
        Object pressed = e.getSource();
        MenuItem info = (MenuItem) pressed;
        model.switchUser(info.getText());
        userLabel.setText(model.getCurrentUser().getUserName());
        showMyList();
    }

    // viser en users mylist
    public void showMyList(){
        userLabel.setText(model.getCurrentUser().getUserName());
        nameSpace.setText("My List");
        if( model.getCurrentUser().getMyList() != null){
            loadPictures(model.getCurrentUser().getMyList());
        }
    }

    // udskriver listen af enten alle film eller serier
    public void allOfType(ActionEvent e){
        Object pressed = e.getSource();
        MenuItem info = (MenuItem) pressed;
        nameSpace.setText(info.getId());
        List<Medium> temp = (model.speceficList(info.getId()));
        loadPictures(temp);
    }

    // udskriver listen af alle film eller serier inden for en genre
    public void allOfGenre(ActionEvent e){
        Object pressed = e.getSource();
        MenuItem info = (MenuItem) pressed;
        String type = info.getParentMenu().getText();
        List<Medium> temp = (model.speceficList(type));
        nameSpace.setText(info.getText()+" "+type);
        try {
            loadPictures(model.search(info.getText(),temp));
        } catch (MediumNotFoundException ex) {
            ex.printStackTrace();
        }
    }

    // søger de input i søgefeltet i forhold til alle film og serier og retunerer alt der matcher titlen, hvis ingen titel matcher søger den efter en genre
    public void handleSearch()  {
        try {
            if ((searchField.getText() != null && !searchField.getText().isEmpty())) {
                {
                    loadPictures(model.search(searchField.getText(),model.getMediums()));
                    textField.setText("Last Search: " + searchField.getText());
                    searchField.setText("");
                    nameSpace.setText("Search results");
                }
            }
        } catch(MediumNotFoundException exception){
            primaryGrid.getChildren().clear();
            textField.setText("Last Search: " + searchField.getText());
            searchField.setText("");
            nameSpace.setText("No results found");
        }
    }

    // Viser alt inormation og billed om det medium der blev klikket på
    private void selectMedium(String title){
        Image show;
        try {
            stop_btn.setDisable(true);
            play_btn.setDisable(false);
            if (model.getCurrentUser().getMyList().contains(model.getMedium(title))){
                remove_btn.setDisable(false);
                add_btn.setDisable(true);
            }else{
                remove_btn.setDisable(true);
                add_btn.setDisable(false);
            }
            show = new Image(model.getPoster(title));
            Title.setText(title);
            infoImage.setImage(show);
            Year.setText("Year: "+model.getYear(title));
            Score.setText("Score: "+model.getScore(title).toString());
            Genre.setText("Genre(s): "+model.getGenre(title));
            controlButtonBar.setVisible(true);
            Seasons.setText("");
            if (model.getMedium(title) instanceof Series){
                Seasons.setText("Seasons: "+model.getSeasonsAndEpisodes(title));
            }
        } catch (MediumNotFoundException e) {
            controlButtonBar.setVisible(false);
            Title.setText("404 not found");
            infoImage.setImage(null);
            Year.setText("");
            Score.setText("");
            Genre.setText("");
        }
    }

    //håndterer de knapper der giver brugeren mulighed for at add/remove fra deres list og play/stop en film
    public void actionButtons(ActionEvent e){
        Object temp = e.getSource();
        Button pressedButton = (Button) temp;
        //PLAY and STOP
        if (pressedButton.getText().toLowerCase().equals("play") && stop_btn.isDisabled()){
            pressedButton.setDisable(true);
            stop_btn.setDisable(false);
        }
        if (pressedButton.getText().toLowerCase().equals("stop") && play_btn.isDisabled()){
            pressedButton.setDisable(true);
            play_btn.setDisable(false);
        }
        //ADD and REMOVE
        try{
            if (pressedButton.getText().toLowerCase().equals("add") && remove_btn.isDisabled()){
                model.getCurrentUser().addMedium(model.getMedium(Title.getText()));
                pressedButton.setDisable(true);
                remove_btn.setDisable(false);
            }
            if (pressedButton.getText().toLowerCase().equals("remove") && add_btn.isDisabled()){
                model.getCurrentUser().removeMedium(model.getMedium(Title.getText()));
                pressedButton.setDisable(true);
                add_btn.setDisable(false);
                if(nameSpace.getText().equals("My List")) showMyList();
            }
        }catch (Exception ex){
            ex.printStackTrace();
        }

    }

    // Tager en liste af mediums og opretter den mængde Imageviewers som der er elementer i listen hvorefter den viser de tilhørende posters
    private void loadPictures(List<Medium> mediums){
        int count = 0;
        int col = 0;
        int row = 0;
        String name;
        Image loadImage;
        List<ImageView> frames = new ArrayList<>();
        primaryGrid.getChildren().clear();
        scroller.setVvalue(0.0);

        for (int i = 0; mediums.size() > frames.size(); i++) {
            name = mediums.get(i).getTitle();
            ImageView filler = new ImageView();
            filler.setFitWidth(122);
            filler.setFitHeight(164);
            filler.setId(name);
            frames.add(filler);
        }

        for (Medium med:mediums) {
            name = mediums.get(count).getTitle();
            ImageView newView = frames.get(count);
            Button interactable = new Button();
            interactable.setPrefSize(122.0,164.0);
            interactable.setOpacity(0.0);
            interactable.setId(name);
            interactable.setOnAction(event -> selectMedium(interactable.getId()));
            loadImage = new Image(med.getPosterURL(),122.0,164.0,false,true);
            newView.setImage(loadImage);
            primaryGrid.add(newView,col,row);
            primaryGrid.add(interactable,col,row);
            count++;
            col++;
            if (col > 4){
                col = 0;
                row++;
            }
        }
    }

}



