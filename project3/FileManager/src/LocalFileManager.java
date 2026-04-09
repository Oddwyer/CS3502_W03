// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - FileManager (Domain Logic)

// File logic imports
import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

public class LocalFileManager implements FileManager {

    public LocalFileManager() {
    }

    // Display current working directory
    public String getCurrentPath() {
        return System.getProperty("user.dir");
    }

    //================== CRUD Logic ========================
    public OperationResult createFile(String fileName) {
        boolean success = false;
        String message;
        String content = "";
        // Error handling
        try {
            // Create file path to requested file
            File file = new File(fileName);

            // Create file if it does not already exist; if it does return notice
            if (file.createNewFile()) {
                message = "File created: " + fileName;
                success = true;
            } else {
                message = "File already exists.";
            }
        } catch (IOException ex) {
            message = "Error creating file.";
        }

        return new OperationResult(success, message, content);
    }

    public OperationResult readFile(String selected) {
        boolean success = false;
        String message;
        String content = "";
        // Error handling

        try {
            // Save content by reading entire file available at the path accessed via Paths.get()
            // Note: Paths.getPath() used when path exists.
            // As "selected" is simply a string, Paths.get() is needed to build it.
            // TODO: Update to Paths.get(currentPath, selected) later
            content = Files.readString(Paths.get(selected));
            message = "Opened: " + selected;
            success = true;
        } catch (Exception ex) {
            message = "Error reading file.";
        }
        return new OperationResult(success, message, content);
    }

    public OperationResult deleteFile(String selected) {
        boolean success = false;
        String message;
        String content = "";

        // Error handling
        if (selected != null) {
            File deleteFile = new File(selected);
            if (deleteFile.isFile()) {
                if (deleteFile.delete()) {
                    message = "Deleted: " + selected;
                    success = true;
                } else {
                    message = "Could not delete file.";
                }
            } else {
                message = "Cannot delete a directory.";
            }
        } else {
            message = "No file selected.";
        }
        return new OperationResult(success, message, content);
    }

    //================Helper Methods===================
// Refresh file list method
    public String[] getFiles() {
            return new File(".").list();
    }

}
