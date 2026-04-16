// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - FileManager Interface (Domain Logic)

// File logic imports
import java.nio.file.Path;

/* Interface to further abstract file logic away from GUI logic reducing coupling. */

public interface FileManager {

    // Display current working directory path
    public String getCurrentPath();

    // Display current directory's files
    public String[] getFiles(Path directory);

    //================== CRUD Logic ========================
    public OperationResult createFile(Path directory, String fileName);
    public OperationResult createDirectory(Path directory, String folderName);
    public OperationResult readFile(Path selected);
    public OperationResult deleteItem(Path selected);
    public OperationResult updateFile(Path selected, String newContent);
    public OperationResult renameItem(Path selected, Path newPath);
    public OperationResult getMetadata(Path selected);
    public OperationResult copyFile(Path source, Path destination);
}
