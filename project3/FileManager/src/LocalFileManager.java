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

    //============================================== CRUD Logic ============================================

    /*Creates file at given location with given name and returns whether  file creation was successful
    along with feedback message*/
    public OperationResult createFile(Path directory, String fileName) {
        boolean success = false;
        String message;
        String content = "";

        // Error handling
        try {
            // Create a path to: current path / fileName
            Path newFile = directory.resolve(fileName);

            // Create file if it does not already exist; if it does return notice
            if (Files.exists(newFile)) {
                message = "File already exists.";
            } else {
                Files.createFile(newFile);
                message = "Created: " + fileName;
                success = true;
            }
        } catch (IOException ex) {
            message = "Error creating file.";
        }
        // Return packaged result details
        return new OperationResult(success, message, content);
    }

    /*Creates directory folder at given location with given name and returns whether directory creation was successful
    along with feedback message*/
    public OperationResult createDirectory(Path directory, String folderName) {
        boolean success = false;
        String message;
        String content = "";

        // Error handling
        try {
            // Create a path pointer to: current path / directory
            Path newDirectory = directory.resolve(folderName);

            // Create directory if it does not already exist; if it does return notice

            if (Files.exists(newDirectory)) {
                message = "Directory folder already exists.";
            } else {
                Files.createDirectory(newDirectory);
                message = "Created: " + folderName;
                success = true;
            }
        } catch (IOException ex) {
            message = "Error creating directory.";
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

        // If the file exists, try to update
        try {
            // Error handling: check if file exists
            if (!Files.exists(selected)) {
                message = "File not found.";
                // Check if a file and not a directory
            } else if (Files.isDirectory(selected)) {
                message = "Cannot update a directory.";
                // Check if file is writable
            } else if (!Files.isWritable(selected)) {
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

    /* Deletes file or directory (if not empty) and returns whether successful along with feedback message*/
    public OperationResult deleteItem(Path selected) {
        boolean success = false;
        String message;
        String content = "";

        // If the file exists, try to delete
        try {
            // Error handling: check if file/directory exists
            if (!Files.exists(selected)) {
                message = "File or directory not found.";
            }
            // Check if a directory and whether directory contains files
            else if (Files.isDirectory(selected)) {
                try (var files = Files.list(selected)) {
                    if (files.findAny().isPresent()) {
                        message = "Cannot delete a non-empty directory.";
                    } else {
                        Files.delete(selected);
                        message = "Deleted: " + selected.getFileName();
                        success = true;
                    }
                }
            } // If not a directory, delete the file
            else {
                Files.delete(selected);
                message = "Deleted: " + selected.getFileName();
                success = true;
            }

            // Handles both tries above: directory existence, directory containment
        } catch (IOException ex) {
            message = "Could not delete item.";
        }
        // Return packaged result details
        return new OperationResult(success, message, content);
    }

    /* Renames file or directory and returns whether successful along with feedback message*/
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

    //===================================== Helper Methods ============================================

    // Returns current files list at designated path
    public String[] getFiles(Path directory) {
        // Convert all paths at the directory to strings and save to array for display
        try (var stream = Files.list(directory)) {
            return stream.map(path -> path.getFileName().toString()).toArray(String[]::new);
        } catch (IOException ex) {
            return new String[0];
        }
    }
}
