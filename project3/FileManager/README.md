# File Manager – CS 3502 Project 3

## Overview
This project implements a graphical file manager that demonstrates core operating system file system concepts. The application provides a user-friendly interface for performing CRUD operations on files and directories while interacting with underlying OS system calls.

## Objectives
- Implement file system CRUD operations using OS APIs
- Understand file descriptors, inodes, and system calls
- Build a GUI that abstracts OS-level operations
- Handle errors and edge cases gracefully

## OS Concepts Demonstrated
- File descriptors (managed internally by Java I/O)
- Inodes and file metadata (retrieved via file attributes)
- System calls (open, read, write, close, rename, delete)
- Directory traversal and path resolution

---

## Technologies Used
- Language: Java
- GUI Framework: Swing
- File APIs: java.io and java.nio.file

---

## Architecture

This separation ensures that GUI concerns remain independent from OS-level file operations, aligning with separation of concerns principles.

The project follows a layered architecture:

- GUI Layer: Handles user interaction and display
- File Operations Layer: Handles CRUD logic
- OS Layer: Executes system-level file operations

---

## Features Implemented
- Uses OS-level file operations such as open, read, write, rename, and delete through Java file I/O APIs

### Core Operations (Required)
- Create files and directories
- Read file contents
- Update file contents
- Delete files/directories
- Rename files/directories
- Navigate directory structure

### Additional Features
- File metadata display (size, timestamps)
- Context menu (right-click actions)
- Double-click to open files

---

## Error Handling
The application handles:
- File not found (ENOENT)
- Permission denied (EACCES)
- File already exists (EEXIST)
- Invalid file names
- Read-only files

All errors are displayed with clear user feedback.

---

## Testing
Tested scenarios include:
- Full CRUD workflow (basic operations)
- Directory operations (navigate, delete empty/non-empty directories)
- Edge cases:
    - long file names
    - special characters
    - empty files
    - large files
    - permission-restricted files
    - paths with spaces
    - concurrent access

---

## How to Run

#### Dependencies & Prerequisites:
1. Java JDK 8+
2. IntelliJ IDEA (recommended) or any Java-compatible IDE
3. Verify that Java is properly installed on your system:
    ```bash
    java -version
   javac -version
    ```
4. Clone the repository:
    ```bash
    git clone https://github.com/Oddwyer/CS3502_W03.git
   cd CS3502_W03/project3/FileManager
    ```
#### Option 1: Run Using IDE  
1. Open IDE and import the project. 
2. Locate the main class (Main.java) and run it. 
3. The application window will launch.

#### Option 2: Run Using Command Line
1. Navigate to the project directory.
2. Build (compile) the project:
    ```bash
    javac -d out src/*.java
    ```
3. Run the application:
    ```bash
    java -cp out Main
    ```
4. The application window will launch.