// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - Operation Result (Domain Logic)

/* Result of an operation performed on a file. */
public class OperationResult {
    private boolean result = false;
    private String resultMessage = "";
    private String content = "";

    public OperationResult(boolean success, String message, String content){
        result = success;
        resultMessage = message;
        this.content = content;
    }

    // Returns whether operation was successful
    public boolean isSuccess(){
        return result;
    }

    // Returns feedback message
    public String getMessage(){
        return resultMessage;
    }

    // Returns file content
    public String getContent(){
        return content;
    }
}
