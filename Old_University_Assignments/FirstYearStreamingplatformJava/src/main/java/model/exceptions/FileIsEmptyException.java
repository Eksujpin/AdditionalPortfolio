package model.exceptions;

public class FileIsEmptyException extends RuntimeException{

    public FileIsEmptyException(){
        super("File is empty");
    }

}
