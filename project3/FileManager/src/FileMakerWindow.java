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
    JButton createButton;
    JButton readButton;
    JButton updateButton;
    JButton deleteButton;
    JButton renameButton;

    // File properties
    private Path currentPath;
    private JList<String> fileList;
    private FileManager fileManager;
    private OperationResult result;

    // Window constructor which includes logic to build window
    public FileMakerWindow(FileManager fileManager) {
        super("File Manager"); // Title bar text
        this.fileManager = fileManager;
        // Invoke fileManager for a path string and convert to a Path.
        currentPath = Paths.get(fileManager.getCurrentPath());
        pathLabel = new JLabel("Path:" + currentPath.toString());
        fileList = new JList<>(fileManager.getFiles(currentPath));

        // Window properties + layout
        textArea = new JTextArea(10, 30);
        textArea.setEditable(true);
        scrollPane = new JScrollPane(fileList,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
        textScroll = new JScrollPane(textArea,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
        label = new JLabel("No action yet.");
        createButton = new JButton("Create File");
        readButton = new JButton("Read File");
        updateButton = new JButton("Update File");
        deleteButton = new JButton("Delete File");
        renameButton = new JButton("Rename Item");
        setLayout(new BorderLayout()); // Simple layout
        setSize(600, 400); // Window size
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
        buttonPanel.add(createButton);
        createFile();

        buttonPanel.add(readButton);
        readFile();

        buttonPanel.add(updateButton);
        updateFile();

        buttonPanel.add(deleteButton);
        deleteFile();

        buttonPanel.add(renameButton);
        renameFile();

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

    //================== CRUD Button Actions ========================

    // createButton action that invokes FileManager's createFile method
    private void createFile() {
        createButton.addActionListener(e -> {
            // Save requested file name from pop-up input box
            String fileName = javax.swing.JOptionPane.showInputDialog("Enter file name:");

            // Error Handling: create file if file name provided
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

    // readButton action that invokes FileManager's readFile method
    private void readFile() {
        readButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // Clear context (old state) first
            textArea.setText("");

            // Error handling: if valid selection, read file
            if (selected != null) {
                // Save selected path
                Path selectedPath = currentPath.resolve(selected);
                // Save operation result from invoking readFile and display accordingly
                result = fileManager.readFile(selectedPath);
                if (result.isSuccess()) {
                    textArea.setText(result.getContent());
                    textArea.requestFocusInWindow(); // Makes text appear editable
                    label.setText(result.getMessage());
                } else {
                    label.setText(result.getMessage());
                }
            } else {
                label.setText("No file selected.");
            }
        });
    }

    // updateButton action that invokes FileManager's updateFile method
    private void updateFile() {
        updateButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // Error handling: if valid selection, update file
            if (selected != null) {
                // Save selected path
                Path selectedPath = currentPath.resolve(selected);
                // Save newly entered content in text box
                String newContent = textArea.getText();
                // Save operation result from invoking updateFile and display accordingly
                result = fileManager.updateFile(selectedPath, newContent);
                if (result.isSuccess()) {
                    label.setText(result.getMessage());
                    refreshDirectory();
                } else {
                    label.setText(result.getMessage());
                }
            } else {
                label.setText("No file selected.");
            }
        });
    }

    // renameButton action that invokes FileManager's renameFile method
    private void renameFile() {
        renameButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // Error handling: if valid selection, request new name for file
            if (selected != null) {
                // Save requested file name from pop-up input box
                String fileName = javax.swing.JOptionPane.showInputDialog("Enter new file name:");
                // If file name provided, identify file path and rename file
                if (fileName != null && !fileName.isEmpty()) {
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
                    label.setText("No file name entered.");
                }
            }else {
                label.setText("No file or directory selected.");
            }
        });
    }


    // deleteButton action that invokes FileManager's deleteFile method
    private void deleteFile() {
        deleteButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // Clear context (old state) first
            textArea.setText("");

            // Read file
            if (selected != null) {
                // Save selected path
                Path selectedPath = currentPath.resolve(selected);
                // Save operation result from invoking deleteFile and display accordingly
                result = fileManager.deleteFile(selectedPath);
                if (result.isSuccess()) {
                    label.setText(result.getMessage());
                    refreshDirectory();
                    textArea.setText(""); // clear content
                } else {
                    label.setText(result.getMessage());
                }
            } else {
                label.setText("No file selected.");
            }
        });
    }

}


