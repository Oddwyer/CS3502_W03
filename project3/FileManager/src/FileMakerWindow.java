// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - FileMakerWindow (GUI Logic)

// Window and button GUI imports

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

// Extends JFrame and instantiates GUI frame and layouts for file directory window. Keeps GUI logic

// separate from file (domain) logic for clear separation of concerns.

public class FileMakerWindow extends JFrame {

    // Window components
    private JScrollPane scrollPane;
    private JScrollPane textScroll;
    private JTextArea textArea;
    private JLabel label;
    private JLabel pathLabel;

    // Window CRUD buttons
    JButton createFileButton;
    JButton createDirectoryButton;
    JButton openButton;
    JButton updateButton;
    JButton deleteButton;
    JButton renameButton;
    JButton upButton;

    // File properties
    private Path currentPath; // Location in file system
    private Path currentFile; // Currently opened (active) file
    private boolean isEditing = false;
    private JList<String> fileList;
    private FileManager fileManager;
    private OperationResult result;

    // Window constructor that includes logic to build window
    public FileMakerWindow(FileManager fileManager) {
        super("File Manager"); // Title bar text
        this.fileManager = fileManager;
        // Invoke fileManager for a path string and convert to a Path.
        currentPath = Paths.get(fileManager.getCurrentPath());
        pathLabel = new JLabel("Path: " + currentPath);
        fileList = new JList<>(fileManager.getFiles(currentPath));
        enableDoubleClick();
        displayMetadata();

        // Window properties + layout
        textArea = new JTextArea(10, 30);
        textArea.setEditable(false);
        textArea.setLineWrap(true);
        textArea.setWrapStyleWord(true);
        scrollPane = new JScrollPane(fileList,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
        textScroll = new JScrollPane(textArea,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
        label = new JLabel("No action yet.");
        label.setVerticalAlignment(SwingConstants.TOP);
        createFileButton = new JButton("Create File");
        createDirectoryButton = new JButton("Create Directory");
        openButton = new JButton("Open");
        updateButton = new JButton("Update File");
        deleteButton = new JButton("Delete");
        renameButton = new JButton("Rename");
        upButton = new JButton("Up");
        setLayout(new BorderLayout()); // Simple layout
        setSize(700, 400); // Window size
        label.setHorizontalAlignment(SwingConstants.CENTER);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE); // Properly close app

        // Window panels
        JPanel topPanel = createTopPanel();
        JPanel centerPanel = createCenterPanel();
        JPanel bottomPanel = createBottomPanel();

        // Add and place panels in window
        add(topPanel, BorderLayout.NORTH);
        add(centerPanel, BorderLayout.CENTER);
        add(bottomPanel, BorderLayout.SOUTH);
    }

    //============================================== Helpers ============================================

    // TOP PANEL: Displays current directory path
    private JPanel createTopPanel() {
        JPanel panel = new JPanel();
        panel.add(pathLabel);
        return panel;
    }

    // CENTER PANEL: Displays file directory details
    private JPanel createCenterPanel() {
        JPanel panel = new JPanel();
        panel.setLayout(new GridLayout(1, 2));
        panel.add(scrollPane); // file list
        panel.add(textScroll); // file content
        return panel;
    }

    // BOTTOM PANEL: Displays file CRUD buttons and related actions
    private JPanel createBottomPanel() {
        JPanel panel = new JPanel();
        panel.setLayout(new BorderLayout());

        // Row 1: Buttons
        JPanel buttonPanel = new JPanel(new FlowLayout(FlowLayout.CENTER, 10, 10));

        buttonPanel.add(openButton);
        openItem();

        buttonPanel.add(upButton);
        navigateUp();

        buttonPanel.add(createFileButton);
        createFile();

        buttonPanel.add(createDirectoryButton);
        createDirectory();

        buttonPanel.add(updateButton);
        updateFile();

        buttonPanel.add(renameButton);
        renameItem();

        buttonPanel.add(deleteButton);
        deleteItem();

        // Row 2: Status label
        JPanel statusPanel = new JPanel(new FlowLayout(FlowLayout.CENTER));
        statusPanel.add(label);

        // Add both rows to bottom panel
        panel.add(buttonPanel, BorderLayout.NORTH);
        panel.add(statusPanel, BorderLayout.SOUTH);

        return panel;
    }

    // Refreshes directory to show new files or removal of deleted files
    private void refreshDirectory() {
        fileList.setListData(fileManager.getFiles(currentPath));
    }

    // Displays metadata when file / directory is selected in list
    private void displayMetadata() {
        fileList.addListSelectionListener(e -> {
            if (!e.getValueIsAdjusting()) {
                String selected = fileList.getSelectedValue();

                // If selected or current path is null, return error
                if (selected == null || currentPath == null) {
                    setStatus("No item selected.");
                } // Do not replace file contents while editing an opened file
                else if (currentFile != null && isEditing) {
                    setStatus("Cannot display metadata while editing a file. Save changes first.");
                } // If not editing an opened file, reset state and display metadata
                else {
                    Path selectedPath = currentPath.resolve(selected);

                    // Reset state
                    currentFile = null;
                    textArea.setEditable(false);
                    isEditing = false;
                    updateButton.setText("Update File");
                    textArea.setText("");

                    // Invoke FileManager's getMetadata method
                    result = fileManager.getMetadata(selectedPath);

                    // If metadata retrieval successful, display metadata and update state
                    if (result.isSuccess()) {
                        textArea.setText(result.getContent());
                        setStatus(result.getMessage());
                    } else {
                        setStatus(result.getMessage());
                    }
                }
            }
        });
    }

    // Wraps status label text
    private void setStatus(String message) {
        label.setText("<html><div style='width: 300px; text-align: center;'>"
                + message +
                "</div></html>");
    }

    //============================================== CRUD ============================================

    // createFileButton action: creates a new file
    private void createFile() {
        createFileButton.addActionListener(e -> {
            // Save requested file name from pop-up input box
            String fileName = JOptionPane.showInputDialog("Enter file name:");

            // If file name is provided, create file
            if (fileName != null && !fileName.isEmpty()) {
                // Invoke FileManager's createFile method and displays results
                result = fileManager.createFile(currentPath, fileName);
                if (result.isSuccess()) {
                    setStatus(result.getMessage());
                    refreshDirectory();
                } else {
                    setStatus(result.getMessage());
                }
            } else {
                setStatus("No file name entered.");
            }
        });
    }

    // createDirectoryButton action: creates a new directory
    private void createDirectory() {
        createDirectoryButton.addActionListener(e -> {
            // Save requested file name from pop-up input box
            String folderName = JOptionPane.showInputDialog("Enter directory name:");

            // If directory name provided, create directory
            if (folderName != null && !folderName.isEmpty()) {
                // Invoke FileManager's createDirectory method and displays results
                result = fileManager.createDirectory(currentPath, folderName);
                if (result.isSuccess()) {
                    setStatus(result.getMessage());
                    refreshDirectory();
                } else {
                    setStatus(result.getMessage());
                }
            } else {
                setStatus("No directory name entered.");
            }
        });
    }

    // openButton action: navigates into directories or opens a file for editing
    private void openItem() {
        openButton.addActionListener(e -> openAction());
    }

    // Opens a file for editing: used for openButton and enableDoubleClick
    private void openAction() {
        // Save selected file name from list
        String selected = fileList.getSelectedValue();

        // If not valid selection, return error
        if (selected == null) {
            setStatus("No item selected.");
        } // Do not open another file while in the process of editing another
        else if (currentFile != null && isEditing) {
            setStatus("Save changes before performing this action.");
        }
        // Open item
        else {
            // Create and save selected path
            Path selectedPath = currentPath.resolve(selected);

            // Reset to default state before switching to new directory or file
            currentFile = null;
            textArea.setEditable(false);
            isEditing = false;
            updateButton.setText("Update File");

            // If a directory, enter it
            if (Files.isDirectory(selectedPath)) {
                currentPath = selectedPath; // Set current path
                pathLabel.setText("Path: " + currentPath);
                refreshDirectory();
                textArea.setText("");
                setStatus("Entered: " + selected);
                return;
            }
            // If a file, invoke FileManager's readFile method and display results
            result = fileManager.readFile(selectedPath);
            if (result.isSuccess()) {
                currentFile = selectedPath; // Set current file
                textArea.setText(result.getContent());
                setStatus(result.getMessage());
            } else {
                textArea.setText("");
                setStatus(result.getMessage());
            }
        }
    }

    // updateButton action: Save changes to the currently open file
    private void updateFile() {
        updateButton.addActionListener(e -> {
            // If no file is open, return error
            if (currentFile == null) {
                setStatus("No file open.");
            } // If file is open, but not being edited, switch to editing state
            else if (!isEditing) {
                textArea.setEditable(true);
                isEditing = true;
                updateButton.setText("Save File");
                setStatus("Editing: " + currentFile.getFileName());
            } // Save new content, invoke FileManager's updateFile method, display results
            else {
                String newContent = textArea.getText();
                result = fileManager.updateFile(currentFile, newContent);
                if (result.isSuccess()) {
                    refreshDirectory();
                    setStatus(result.getMessage());
                    // Reset to default state
                    updateButton.setText("Update File");
                    textArea.setEditable(false);
                    isEditing = false;
                } else {
                    setStatus(result.getMessage());
                }
            }
        });
    }

    // renameButton action: invokes FileManager's renameFile method and displays result
    private void renameItem() {
        renameButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // If not valid selection, return error
            if (selected == null) {
                setStatus("No item selected.");
            } // Do not rename file while editing another file
            else if (currentFile != null && isEditing) {
                setStatus("Save changes before performing this action.");
            } // If valid selection, rename item
            else {
                // Save requested file name from pop-up input box
                String fileName = javax.swing.JOptionPane.showInputDialog("Enter new file or directory name:");
                // If renaming to same name, return error
                if (selected.equals(fileName)) {
                    setStatus("New name is the same as the current name.");
                }
                // If new file name provided, create and save file path and rename file
                else if (fileName != null && !fileName.isEmpty()) {
                    Path oldPath = currentPath.resolve(selected); // Save selected path
                    Path newPath = currentPath.resolve(fileName); // Save new path
                    result = fileManager.renameItem(oldPath, newPath);
                    if (result.isSuccess()) {
                        setStatus(result.getMessage());
                        refreshDirectory();
                    } else {
                        setStatus(result.getMessage());
                    }
                } else {
                    setStatus("No name entered.");
                }
            }
        });
    }

    // deleteButton action: invokes FileManager's deleteITem method and displays result
    private void deleteItem() {
        deleteButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // If not valid selection, return error
            if (selected == null) {
                setStatus("No item selected.");
            } // Do not delete file while editing another file
            else if (currentFile != null && isEditing) {
                setStatus("Save changes before performing this action.");
            } // If valid selection, delete item
            else {
                // Save selected path
                Path selectedPath = currentPath.resolve(selected);

                // Display deletion confirmation dialog
                int confirm = JOptionPane.showConfirmDialog(
                        null,
                        "Are you sure you want to delete: " + selected + "?",
                        "Confirm Delete",
                        JOptionPane.YES_NO_OPTION
                );
                if (confirm != JOptionPane.YES_OPTION) {
                    setStatus("Delete cancelled.");
                    return;
                }

                // If proceeding, invoke FileManager's deleteFile method and display results
                result = fileManager.deleteItem(selectedPath);
                if (result.isSuccess()) {
                    setStatus(result.getMessage());
                    refreshDirectory();
                    textArea.setText("");
                    // Reset to default state if currently open file was deleted file
                    if (selectedPath.equals(currentFile)) {
                        currentFile = null;
                        textArea.setEditable(false);
                        isEditing = false;
                        updateButton.setText("Update File");
                    }
                } else {
                    setStatus(result.getMessage());
                }
            }
        });
    }

    // upButton action: navigates up the current path to the immediate parent in the path
    private void navigateUp() {
        upButton.addActionListener(e -> {

            // Save parent path
            Path parent = currentPath.getParent();
            // Error handling: If no parent, already at root
            if (parent == null) {
                setStatus("Already at directory root.");
                return;
                // If parent exists, set current path to parent and refresh to display parent directory
            } else {
                currentPath = parent;
                pathLabel.setText("Path: " + currentPath);
                // Reset current file and text area
                currentFile = null;
                textArea.setEditable(false);
                updateButton.setText("Update File");
                textArea.setText("");
                refreshDirectory();
                setStatus("Moved up a directory.");
            }
        });
    }

//======================================= Additional Features ====================================

    // Double-click action: navigates into directories or opens a file for editing
    private void enableDoubleClick() {
        fileList.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (e.getClickCount() == 2 && SwingUtilities.isLeftMouseButton(e)) {
                    openAction();
                }
            }
        });
    }
}


