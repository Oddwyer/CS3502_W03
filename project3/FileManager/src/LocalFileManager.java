// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - FileManager (Domain Logic)

// File logic imports

import java.io.IOException;
import java.nio.file.*;

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

    /* Creates file at given location with given name and returns whether  file creation was successful
    along with feedback message*/
    public OperationResult createFile(Path directory, String fileName) {
        boolean success = false;
        String message;
        String content = "";

        try {
            // Create a path to: current path / fileName
            Path newFile = directory.resolve(fileName);
            Files.createFile(newFile);
            message = "Created: " + fileName;
            success = true;
        } // If EEXIST
        catch (FileAlreadyExistsException ex) {
            message = "File already exists.";
        } // If EACCES
        catch (AccessDeniedException ex) {
            message = "Permission denied.";
        } // If path too long or illegal characters
        catch (InvalidPathException ex) {
            message = "File path is too long or contains illegal characters.";
        } // If EBUSY
        catch (FileSystemException ex) {
            message = "File(s) are in use by another user or process.";
        } // If EIO, ENOSPC, or other error
        catch (IOException ex) {
            message = "Error creating file: " + ex.getMessage();
        }

        // Return packaged result details
        return new OperationResult(success, message, content);
    }

    /* Creates directory folder at given location with given name and returns whether directory creation was successful
    along with feedback message*/
    public OperationResult createDirectory(Path directory, String folderName) {
        boolean success = false;
        String message;
        String content = "";

        try {
            // Create a path pointer to: current path / directory
            Path newDirectory = directory.resolve(folderName);
            Files.createDirectory(newDirectory);
            message = "Created: " + folderName;
            success = true;
        } // If EEXIST
        catch (FileAlreadyExistsException ex) {
            message = "Directory already exists.";
            // If EACCES
        } catch (AccessDeniedException ex) {
            message = "Permission denied.";
        } // If path too long or illegal characters
        catch (InvalidPathException ex) {
            message = "Directory path is too long or contains illegal characters.";
        } // If EIO, ENOSPC, or other error
        catch (IOException ex) {
            message = "Error creating directory: " + ex.getMessage();
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

        // If selected path is null, return error
        if (selected == null) {
            return new OperationResult(false, "No file selected.", "");
        } else {
            try {
                // Save content by reading entire file available at the selected path
                content = Files.readString(selected);
                message = "Opened: " + selected.getFileName();
                success = true;
            } // If ENOENT
            catch (NoSuchFileException ex) {
                message = "File not found.";
            } // If EACCES
            catch (AccessDeniedException ex) {
                message = "Permission denied.";
            } // If other error
            catch (IOException ex) {
                message = "Error reading file: " + ex.getMessage();
            }

            // Return packaged result details
            return new OperationResult(success, message, content);
        }
    }

    /* Updates file contents and returns whether successful along with feedback message*/
    public OperationResult updateFile(Path selected, String newContent) {
        boolean success = false;
        String message;
        String content = "";

        // If selected path is null, return error
        if (selected == null) {
            return new OperationResult(false, "No file selected.", "");
        } else {
            try {
                // Check existence first so missing files are not misreported as non-writable.
                if (!Files.exists(selected)) {
                    message = "File not found.";
                } // Check if file is a directory
                else if (Files.isDirectory(selected)) {
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
            }  // If EACCES
            catch (AccessDeniedException ex) {
                message = "Permission denied.";
            } // If EBUSY
            catch (FileSystemException ex) {
                message = "File may be in use by another process.";
            }
            // If other error
            catch (IOException ex) {
                message = "Could not update file: " + ex.getMessage();
            }

            // Return packaged result details
            return new OperationResult(success, message, content);
        }
    }

    /* Deletes file or directory (if not empty) and returns whether successful along with feedback message*/
    public OperationResult deleteItem(Path selected) {
        boolean success = false;
        String message;
        String content = "";

        if (selected == null) {
            return new OperationResult(false, "No item selected.", "");
        } else {
            try {
                // If file or directory exists, delete it
                Files.delete(selected);
                message = "Deleted: " + selected.getFileName();
                success = true;
            } // If ENOENT
            catch (NoSuchFileException ex) {
                message = "File or directory not found.";
            } // If ENOTEMPTY
            catch (DirectoryNotEmptyException ex) {
                message = "Cannot delete a non-empty directory.";
            } // If EACCES
            catch (AccessDeniedException ex) {
                message = "Permission denied.";
            } // If EBUSY
            catch (FileSystemException ex) {
                message = "Item may be in use by another process.";
            }
            // If other error
            catch (IOException ex) {
                message = "Could not delete item: " + ex.getMessage();
            }

            // Return packaged result details
            return new OperationResult(success, message, content);
        }
    }

    /* Renames file or directory and returns whether successful along with feedback message*/
    public OperationResult renameItem(Path oldPath, Path newPath) {
        boolean success = false;
        String message = "";
        String content = "";

        // If old or new path is null, return error
        if (oldPath == null || newPath == null) {
            return new OperationResult(false, "No item selected.", "");
        } else {
            try {
                // Attempt to rename file by moving path; must be done atomically
                Files.move(oldPath, newPath, StandardCopyOption.ATOMIC_MOVE);
                message = "Renamed: " + oldPath.getFileName() + " to " + newPath.getFileName();
                success = true;
            } // If ENOENT
            catch (NoSuchFileException ex) {
                message = "File or directory not found.";
            } // If EEXIST
            catch (FileAlreadyExistsException ex) {
                message = "File or directory already exists.";
            } // If EACCES
            catch (AccessDeniedException ex) {
                message = "Permission denied.";
            } // If EBUSY
            catch (FileSystemException ex) {
                message = "Item may be in use by another process.";
            } // If other error
            catch (IOException ex) {
                message = "Could not rename item: " + ex.getMessage();
            }
        }

        // Return packaged result details
        return new OperationResult(success, message, content);
    }

    /* If file or directory exists, displays metadata*/
    public OperationResult getMetadata(Path selected) {
        boolean success = false;
        String message = "";
        String content = "";

        // If selected path is null, return error
        if (selected == null) {
            return new OperationResult(false, "No item selected.", "");
        } else {
            // Error handling
            try {
                // Check existence first to avoid multiple exceptions from metadata calls
                if (!Files.exists(selected)) {
                    message = "File or directory not found.";
                    // Check if a file or directory and provide metadata accordingly
                } else {
                    if (!Files.isDirectory(selected)) {
                        content += "Size: " + Files.size(selected) + " bytes\n";
                    }
                    content += "Owner: " + Files.getOwner(selected) + "\n";
                    content += "Last Modified: " + Files.getLastModifiedTime(selected) + "\n";
                    content += "Readable: " + Files.isReadable(selected) + "\n";
                    content += "Writable: " + Files.isWritable(selected) + "\n";
                    content += "Type: " + (Files.isDirectory(selected) ? "Directory" : "File");

                    message = "Metadata loaded.";
                    success = true;
                }
            } catch (IOException ex) {
                message = "Could not retrieve metadata: " + ex.getMessage();
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
