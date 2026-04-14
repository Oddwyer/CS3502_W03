// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - FileMakerWindow (GUI Logic)

// Window and button GUI imports

import javax.swing.*;
import java.awt.*;
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
    private JList<String> fileList;
    private FileManager fileManager;
    private OperationResult result;

    // Window constructor which includes logic to build window
    public FileMakerWindow(FileManager fileManager) {
        super("File Manager"); // Title bar text
        this.fileManager = fileManager;
        // Invoke fileManager for a path string and convert to a Path.
        currentPath = Paths.get(fileManager.getCurrentPath());
        pathLabel = new JLabel("Path: " + currentPath.toString());
        fileList = new JList<>(fileManager.getFiles(currentPath));
        displayMetadata();

        // Window properties + layout
        textArea = new JTextArea(10, 30);
        textArea.setEditable(false);
        scrollPane = new JScrollPane(fileList,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
        textScroll = new JScrollPane(textArea,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
        label = new JLabel("No action yet.");
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

    // ======================== Window Helper Methods ===========================

    // TOP PANEL: Displays current directory path
    private JPanel createTopPanel() {
        JPanel panel = new JPanel();
        panel.add(pathLabel);
        return panel;
    }

    // CENTER PANEL: Displays file directory details
    private JPanel createCenterPanel() {
        JPanel panel = new JPanel();
        panel.setLayout(new java.awt.GridLayout(1, 2));
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

                // If selected path or current path is null, return error
                if (selected == null || currentPath == null) {
                    label.setText("No item selected.");
                } // Do not replace file contents while editing an opened file
                else if (currentFile != null) {
                    label.setText("Cannot display metadata while editing a file. Save changes first.");
                } // If not editing an opened file, display metadata
                else {
                    Path selectedPath = currentPath.resolve(selected);
                    result = fileManager.getMetadata(selectedPath);

                    if (result.isSuccess()) {
                        textArea.setText(result.getContent());
                        label.setText(result.getMessage());
                    } else {
                        label.setText(result.getMessage());
                    }
                }
            }
        });
    }

    //================== CRUD Button Actions ========================

    // createButton action: invokes FileManager's createFile method
    private void createFile() {
        createFileButton.addActionListener(e -> {
            // Save requested file name from pop-up input box
            String fileName = javax.swing.JOptionPane.showInputDialog("Enter file name:");

            // If file name is provided, create file
            if (fileName != null && !fileName.isEmpty()) {
                // Save operation result from invoking createFile and display accordingly
                result = fileManager.createFile(currentPath, fileName);
                if (result.isSuccess()) {
                    label.setText(result.getMessage());
                    refreshDirectory();
                } else {
                    label.setText(result.getMessage());
                }
            } else {
                label.setText("No file name entered.");
            }
        });
    }

    // createButton action: invokes FileManager's createFile method
    private void createDirectory() {
        createDirectoryButton.addActionListener(e -> {
            // Save requested file name from pop-up input box
            String folderName = javax.swing.JOptionPane.showInputDialog("Enter directory name:");

            // If directory name provided, create directory
            if (folderName != null && !folderName.isEmpty()) {
                // Save operation result from invoking createFile and display accordingly
                result = fileManager.createDirectory(currentPath, folderName);
                if (result.isSuccess()) {
                    label.setText(result.getMessage());
                    refreshDirectory();
                } else {
                    label.setText(result.getMessage());
                }
            } else {
                label.setText("No directory name entered.");
            }
        });
    }

    // openButton action: navigates into directories or opens a file for editing
    private void openItem() {
        openButton.addActionListener(e -> {
            // Get selected item from list (file or directory)
            String selected = fileList.getSelectedValue();
            // Clear context (old state) first
            textArea.setText("");

            // If not valid selection, return error
            if (selected == null) {
                label.setText("No item selected.");
            } // Do not open new file while editing an opened file
            else if (currentFile != null) {
                label.setText("Finish editing the current file before performing this action.");
            } // If valid selection, open item
            else {
                // Save selected path
                Path selectedPath = currentPath.resolve(selected);
                // If a directory, enter it
                if (java.nio.file.Files.isDirectory(selectedPath)) {
                    currentPath = selectedPath;
                    currentFile = null; // Reset current file
                    pathLabel.setText("Path: " + currentPath);
                    refreshDirectory();
                    textArea.setText(""); // Clear content
                    textArea.setEditable(false); // Disable text editing
                    updateButton.setText("Update File"); // Reset button text
                    label.setText("Entered: " + selected);
                    return;
                }
                // If a file, save operation result by invoking readFile and display accordingly
                result = fileManager.readFile(selectedPath);
                if (result.isSuccess()) {
                    currentFile = selectedPath; // Set current file
                    textArea.setText(result.getContent());
                    textArea.setEditable(true); // Enable text editing
                    textArea.requestFocusInWindow(); // Makes text appear editable
                    updateButton.setText("Save File"); // Change button text
                    label.setText(result.getMessage());
                } else {
                    currentFile = null;
                    textArea.setText(""); // Clear content
                    textArea.setEditable(false); // Disable text editing
                    updateButton.setText("Update File"); // Reset button text
                    label.setText(result.getMessage());
                }
            }
        });
    }

    // updateButton action: invokes FileManager's updateFile method to save changes to the currently open file
    private void updateFile() {
        updateButton.addActionListener(e -> {

            // If no file is open, return error
            if (currentFile == null) {
                label.setText("No file open.");
            } // If file is open, permit file updates
            else {
                String newContent = textArea.getText();
                // Save operation result from invoking updateFile and display accordingly
                result = fileManager.updateFile(currentFile, newContent);
                if (result.isSuccess()) {
                    refreshDirectory();
                    updateButton.setText("Update File"); // Reset button text
                    label.setText(result.getMessage());
                    currentFile = null; // Reset current file
                    textArea.setEditable(false); // Disable text editing
                } else {
                    label.setText(result.getMessage());
                }
            }
        });
    }

    // renameButton action: invokes FileManager's renameFile method
    private void renameItem() {
        renameButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // If not valid selection, return error
            if (selected == null) {
                label.setText("No item selected.");
            } // Do not rename file while editing an opened file
            else if (currentFile != null) {
                label.setText("Finish editing the current file before performing this action.");
            } // If valid selection, rename item
            else {
                // Save requested file name from pop-up input box
                String fileName = javax.swing.JOptionPane.showInputDialog("Enter new file or directory name:");
                // If renaming to same name, return error
                if (selected.equals(fileName)) {
                    label.setText("New name is the same as the current name.");
                    return;
                }
                // If file name provided, identify file path and rename file
                else if (fileName != null && !fileName.isEmpty()) {
                    // Save selected path
                    Path oldPath = currentPath.resolve(selected);
                    Path newPath = currentPath.resolve(fileName);
                    result = fileManager.renameItem(oldPath, newPath);
                    if (result.isSuccess()) {
                        label.setText(result.getMessage());
                        refreshDirectory();
                    } else {
                        label.setText(result.getMessage());
                    }
                } else {
                    label.setText("No name entered.");
                }
            }
        });
    }

    // deleteButton action: invokes FileManager's deleteITem method
    private void deleteItem() {
        deleteButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();
            // Clear context (old state) first
            textArea.setText("");

            // If not valid selection, return error
            if (selected == null) {
                label.setText("No item selected.");
            } // Do not delete file while editing an opened file
            else if (currentFile != null) {
                label.setText("Finish editing the current file before performing this action.");
            } // If valid selection, delete item
            else {
                // Save selected path
                Path selectedPath = currentPath.resolve(selected);
                // Deletion confirmation dialog
                int confirm = JOptionPane.showConfirmDialog(
                        null,
                        "Are you sure you want to delete: " + selected + "?",
                        "Confirm Delete",
                        JOptionPane.YES_NO_OPTION
                );
                if (confirm != JOptionPane.YES_OPTION) {
                    label.setText("Delete cancelled.");
                    return;
                }

                // If proceeding, save operation result from invoking deleteFile and display accordingly
                result = fileManager.deleteItem(selectedPath);
                if (result.isSuccess()) {
                    label.setText(result.getMessage());
                    refreshDirectory();
                    textArea.setText("");
                    // Reset current file and text area if deleted file was currently open
                    if (selectedPath.equals(currentFile)) {
                        currentFile = null;
                        textArea.setEditable(false);
                        updateButton.setText("Update File");
                    }
                } else {
                    label.setText(result.getMessage());
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
                label.setText("Already at directory root.");
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
                label.setText("Moved up a directory.");
            }
        });
    }

}


