// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - FileManager (Domain Logic)

// File logic imports

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.nio.file.Path;

// Implements FileManager and handles all file logic. Keeps file (domain) logic separate from GUI logic for
// clear separation of concerns.

public class LocalFileManager implements FileManager {

    public LocalFileManager() {
    }

    // Display current working directory
    public String getCurrentPath() {
        return System.getProperty("user.dir");
    }

    //================== CRUD Logic ========================

    // Creates file at given location with given name and returns whether
    // file creation was successful along with feedback message
    public OperationResult createFile(Path directory, String fileName) {
        boolean success = false;
        String message;
        String content = "";

        // Error handling
        try {
            // Create a file pointer to: current path / fileName
            File file = directory.resolve(fileName).toFile();

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

    // Reads selected file and returns whether successful (if so, displays any file content)
    // along with feedback message
    public OperationResult readFile(Path selected) {
        boolean success = false;
        String message;
        String content = "";

        // Error handling
        try {
            // Save content by reading entire file available at the selected path
            content = Files.readString(selected);
            message = "Opened: " + selected.getFileName();
            success = true;
        } catch (Exception ex) {
            message = "Error reading file.";
        }
        return new OperationResult(success, message, content);
    }

    // Deletes file and returns whether successful along with feedback message
    public OperationResult deleteFile(Path selected) {
        boolean success = false;
        String message;
        String content = "";

        // Error handling
        if (selected != null) {
            // Create a file pointer to the selected path
            File deleteFile = selected.toFile();
            // If the file exists, delete the file
            if (deleteFile.isFile()) {
                if (deleteFile.delete()) {
                    message = "Deleted: " + selected.getFileName();
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

    // Returns current file listing at designated path
    public String[] getFiles(Path directory) {
        // Create a pointer to the directory
        File folder = directory.toFile();
        // List the files at the directory
        String[] files = folder.list();
        // If files is not null, return, return empty array
        return files != null ? files : new String[0];
    }

}
