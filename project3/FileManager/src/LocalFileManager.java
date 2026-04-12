// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - FileManager (Domain Logic)

// File logic imports

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardOpenOption;

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

    /*Creates file at given location with given name and returns whether  file creation was successful
    along with feedback message*/

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
        // Return packaged result details
        return new OperationResult(success, message, content);
    }

    /* Reads selected file and returns whether successful (if so, displays any file content)
    along with feedback message*/
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
        // Return packaged result details
        return new OperationResult(success, message, content);
    }

    /* Updates file and returns whether successful along with feedback message*/
    public OperationResult updateFile(Path selected, String newContent) {
        boolean success = false;
        String message;
        String content = "";

        // Create a file pointer to the selected path
        File updateFile = selected.toFile();
        // If the file exists, try to update
        try {
            // Error handling: check if file exists...
            if (!updateFile.exists()) {
                message = "File not found.";
                // Check if a file...
            } else if (!updateFile.isFile()) {
                message = "Cannot update a directory.";
            } else if (!updateFile.canWrite()) {
                message = "File is read-only or not writable.";
            } else {
                // Attempt to update file: erase old content and replace with revised/new content
                Files.writeString(selected, newContent, StandardOpenOption.TRUNCATE_EXISTING);
                message = "Updated: " + selected.getFileName();
                success = true;
            }
        } catch (IOException ex) {
            message = "Could not update file.";
        }
        // Return packaged result details
        return new OperationResult(success, message, content);
    }

    /* Deletes file and returns whether successful along with feedback message*/
    public OperationResult deleteFile(Path selected) {
        boolean success = false;
        String message = "";
        String content = "";

        // Create a file pointer to the selected path
        File deleteFile = selected.toFile();
        // If the file exists, try to delete
        try {
            // Error handling: check if file exists...
            if (!deleteFile.exists()) {
                message = "File not found.";
                // Check if a file...
            } else if (!deleteFile.isFile()) {
                message = "Cannot delete a directory.";
            } else {
                // Attempt to delete file
                if (deleteFile.delete()) {
                    message = "Deleted: " + selected.getFileName();
                    success = true;
                }
            }
        } catch (Exception ex) {
            message = "Could not delete file.";
        }
        // Return packaged result details
        return new OperationResult(success, message, content);
    }

    /* Updates file and returns whether successful along with feedback message*/
    public OperationResult renameItem(Path oldPath, Path newPath) {
        boolean success = false;
        String message = "";
        String content = "";

        // Error handling
        if (oldPath != null) {
            // Create a file pointer to the selected path
            // If the file exists, try to update
            try {
                // Error handling
                // Check if file exists...
                if (!Files.exists(oldPath)) {
                    message = "File or directory not found.";
                    // Check if a file...
                } else if (Files.exists(newPath)) {
                    message = "A file or directory with that name already exists.";
                } else {
                    // Attempt to rename file by moving path
                    Files.move(oldPath, newPath);
                    message = "Renamed: " + oldPath.getFileName() + " to " + newPath.getFileName();
                    success = true;
                }
            } catch (IOException ex) {
                message = "Could not rename item.";
            }
         }
        // Return packaged result details
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
