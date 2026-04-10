
// File logic imports
import java.nio.file.Path;

public interface FileManager {

    // Display current working directory path
    public String getCurrentPath();

    // Display current directory's files
    public String[] getFiles(Path directory);

    //================== CRUD Logic ========================
    public OperationResult createFile(Path directory, String fileName);
    public OperationResult readFile(Path selected);
    public OperationResult deleteFile(Path selected);
    public OperationResult updateFile(Path selected, String newContent);
    public OperationResult renameFile(Path selected, Path newPath);
}
