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
        String content = "";

        // If directory or file name is missing, return error
        if (directory == null || fileName == null || fileName.isBlank()) {
            return new OperationResult(false, "File name is missing.", content);
        }
        if (!Files.isDirectory(directory)) {
            return new OperationResult(false, "Directory path is not valid.", content);
        }
        try {
            // Create a path to: current path / fileName
            Path newFile = directory.resolve(fileName);
            Files.createFile(newFile);
            return new OperationResult(true, "Created: " + fileName, content);
        }  // If EEXIST
        catch (FileAlreadyExistsException ex) {
            return new OperationResult(false, "File already exists.", content);
        } // If EACCES
        catch (AccessDeniedException ex) {
            return new OperationResult(false, "Permission denied.", content);
        } // If EBUSY or invalid (too long, illegal characters)
        catch (FileSystemException ex) {
            String details = ex.getMessage();

            // Checks error message for invalid path or too long path and updates message accordingly
            if (details != null) {
                String lower = details.toLowerCase();
                if (lower.contains("too long")) {
                    return new OperationResult(false, "File path is too long.", content);
                }
                if (lower.contains("invalid")) {
                    return new OperationResult(false, "Invalid file name or path.", content);
                }
            }
            return new OperationResult(false, "File system error: " + details, content);

        } // If EIO, ENOSPC, or other error
        catch (IOException ex) {
            return new OperationResult(false, "Error creating file: " + ex.getMessage(), content);
        }
    }

    /* Creates directory folder at given location with given name and returns whether directory creation was successful
    along with feedback message*/
    public OperationResult createDirectory(Path directory, String folderName) {
        String content = "";

        // If directory or folder name is missing, return error
        if (directory == null || folderName == null || folderName.isBlank()) {
            return new OperationResult(false, "Directory name is missing.", content);
        }
        if (!Files.isDirectory(directory)) {
            return new OperationResult(false, "Directory path is not valid.", content);
        }
        try {
            // Create a path pointer to: current path / directory
            Path newDirectory = directory.resolve(folderName);
            Files.createDirectory(newDirectory);
            return new OperationResult(true, "Created: " + folderName, content);
        } // If EEXIST
        catch (FileAlreadyExistsException ex) {
            return new OperationResult(false, "Directory already exists.", content);
        } // If EACCES
        catch (AccessDeniedException ex) {
            return new OperationResult(false, "Permission denied.", content);
        } // If EBUSY or invalid (too long, illegal characters)
        catch (FileSystemException ex) {
            String details = ex.getMessage();

            // Checks error message for invalid path or too long path and updates message accordingly
            if (details != null) {
                String lower = details.toLowerCase();
                if (lower.contains("too long")) {
                    return new OperationResult(false, "File path is too long.", content);
                }
                if (lower.contains("invalid")) {
                    return new OperationResult(false, "Invalid file name or path.", content);
                }
            }
            return new OperationResult(false, "File system error: " + details, content);

        }// If EIO, ENOSPC, or other error
        catch (IOException ex) {
            return new OperationResult(false, "Error creating directory: " + ex.getMessage(), content);
        }
    }

    /* Reads selected file and returns whether successful (if so, displays any file content)
    along with feedback message*/
    public OperationResult readFile(Path selected) {
        String content = "";

        // If selected path is null, return error
        if (selected == null) {
            return new OperationResult(false, "No file selected.", content);
        } // If selected path is a directory, return error
        else if (Files.isDirectory(selected)) {
            return new OperationResult(false, "Cannot read a directory.", content);
        } // If selected path is a file, read file content
        else {
            try {
                // Save content by reading entire file available at the selected path
                content = Files.readString(selected);
                return new OperationResult(true, "Opened file: " + selected.getFileName(), content);
            } // If ENOENT
            catch (NoSuchFileException ex) {
                return new OperationResult(false, "File not found.", content);
            } // If EACCES
            catch (AccessDeniedException ex) {
                return new OperationResult(false, "Permission denied.", content);
            }// If EBUSY or invalid (too long, illegal characters)
            catch (FileSystemException ex) {
                return new OperationResult(false, "File system error: " + ex.getMessage(), content);
            } // If other error
            catch (IOException ex) {
                return new OperationResult(false, "Error reading file: " + ex.getMessage(), content);
            }
        }
    }

    /* Updates file contents and returns whether successful along with feedback message*/
    public OperationResult updateFile(Path selected, String newContent) {
        String content = "";

        // If selected path is null, return error
        if (selected == null) {
            return new OperationResult(false, "No file selected.", content);
        } else {
            try {
                // Check existence first so missing files are not misreported as non-writable.
                if (!Files.exists(selected)) {
                    return new OperationResult(false, "File not found.", content);
                } // Check if file is a directory
                else if (Files.isDirectory(selected)) {
                    return new OperationResult(false, "Cannot update a directory.", content);
                } // Check if file is writable
                else if (!Files.isWritable(selected)) {
                    return new OperationResult(false, "File is read-only or not writable.", content);
                } // Attempt to update file: erase old content and replace with revised/new content
                else {
                    Files.writeString(selected, newContent, StandardOpenOption.TRUNCATE_EXISTING);
                    return new OperationResult(true, "Updated: " + selected.getFileName(), content);
                }
            }  // If EACCES
            catch (AccessDeniedException ex) {
                return new OperationResult(false, "Permission denied.", content);
            } // If EBUSY or invalid (too long, illegal characters)
            catch (FileSystemException ex) {
                return new OperationResult(false, "File system error: " + ex.getMessage(), content);
            } // If other error
            catch (IOException ex) {
                return new OperationResult(false, "Could not update file: " + ex.getMessage(), content);
            }
        }
    }

    /* Deletes file or directory (if not empty) and returns whether successful along with feedback message*/
    public OperationResult deleteItem(Path selected) {
        String content = "";

        if (selected == null) {
            return new OperationResult(false, "No item selected.", content);
        } else {
            try {
                // If file or directory exists, delete it
                Files.delete(selected);
                return new OperationResult(true, "Deleted: " + selected.getFileName(), content);
            } // If ENOENT
            catch (NoSuchFileException ex) {
                return new OperationResult(false, "File or directory not found.", content);
            } // If ENOTEMPTY
            catch (DirectoryNotEmptyException ex) {
                return new OperationResult(false, "Cannot delete a non-empty directory.", content);
            } // If EACCES
            catch (AccessDeniedException ex) {
                return new OperationResult(false, "Permission denied.", content);
            } // If EBUSY or invalid (too long, illegal characters)
            catch (FileSystemException ex) {
                return new OperationResult(false, "File system error: " + ex.getMessage(), content);
            }
            // If other error
            catch (IOException ex) {
                return new OperationResult(false, "Could not delete item: " + ex.getMessage(), content);
            }
        }
    }

    /* Renames file or directory and returns whether successful along with feedback message*/
    public OperationResult renameItem(Path oldPath, Path newPath) {
        String content = "";

        // If old or new path is null, return error
        if (oldPath == null || newPath == null) {
            return new OperationResult(false, "No item selected.", content);
        } else {
            try {
                // Attempt to rename file by moving path; must be done atomically
                Files.move(oldPath, newPath);
                return new OperationResult(true, "Renamed: " + oldPath.getFileName() + " to " + newPath.getFileName(), content);
            } // If ENOENT
            catch (NoSuchFileException ex) {
                return new OperationResult(false, "File or directory not found.", content);
            } // If EEXIST
            catch (FileAlreadyExistsException ex) {
                return new OperationResult(false, "File or directory already exists.", content);
            } // If EACCES
            catch (AccessDeniedException ex) {
                return new OperationResult(false, "Permission denied.", content);
            } // If EBUSY or invalid (too long, illegal characters)
            catch (FileSystemException ex) {
                String details = ex.getMessage();

                // Checks error message for invalid path or too long path and updates message accordingly
                if (details != null) {
                    String lower = details.toLowerCase();
                    if (lower.contains("too long")) {
                        return new OperationResult(false, "File path is too long.", content);
                    }
                    if (lower.contains("invalid")) {
                        return new OperationResult(false, "Invalid file name or path.", content);
                    }
                }
                return new OperationResult(false, "File system error: " + details, content);

            } // If other error
            catch (IOException ex) {
                return new OperationResult(false, "Could not rename item: " + ex.getMessage(), content);
            }
        }
    }

    /* If file or directory exists, displays metadata*/
    public OperationResult getMetadata(Path selected) {
        String content = "";

        // If selected path is null, return error
        if (selected == null) {
            return new OperationResult(false, "No item selected.", "");
        } else {
            // Error handling
            try {
                // Check existence first to avoid multiple exceptions from metadata calls
                if (!Files.exists(selected)) {
                    return new OperationResult(false, "File or directory not found.", content);
                } // Check if a file or directory and provide metadata accordingly
                else {
                    if (!Files.isDirectory(selected)) {
                        content += "Size: " + Files.size(selected) + " bytes\n";
                    }
                    content += "Owner: " + Files.getOwner(selected) + "\n";
                    content += "Last Modified: " + Files.getLastModifiedTime(selected) + "\n";
                    content += "Readable: " + Files.isReadable(selected) + "\n";
                    content += "Writable: " + Files.isWritable(selected) + "\n";
                    content += "Type: " + (Files.isDirectory(selected) ? "Directory" : "File");

                    return new OperationResult(true, "Metadata loaded.", content);
                }
            } catch (IOException ex) {
                return new OperationResult(false, "Could not retrieve metadata: " + ex.getMessage(), content);
            }
        }
    }

    //============================================== Helpers ============================================

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
