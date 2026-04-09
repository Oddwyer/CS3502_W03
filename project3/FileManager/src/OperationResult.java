public class OperationResult {
    private boolean result = false;
    private String resultMessage = "";
    private String content = "";

    public OperationResult(boolean success, String message, String content){
        result = success;
        resultMessage = message;
        this.content = content;
    }

    public boolean isSuccess(){
        return result;
    }

    public String getMessage(){
        return resultMessage;
    }

    public String getContent(){
        return content;
    }
}
