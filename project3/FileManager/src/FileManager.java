
// File logic imports
import java.nio.file.Paths;

public interface FileManager {

    // Display current working directory
    public String getCurrentPath();

    //================== CRUD Logic ========================
    public OperationResult createFile(String fileName);
    public OperationResult readFile(String selected);
    public OperationResult deleteFile(String selected);
    public String[] getFiles();

}
