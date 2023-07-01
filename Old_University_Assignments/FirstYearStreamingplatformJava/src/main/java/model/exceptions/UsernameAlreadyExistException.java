package model.exceptions;

public class UsernameAlreadyExistException extends RuntimeException {

    public UsernameAlreadyExistException(){
    super(" User already exists");
    }
}
