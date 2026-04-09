// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - MainWindow (GUI Logic)

// Window and button GUI imports

import javax.swing.*;
import java.awt.*;

public class WindowMaker{
    // Framing properties
    private JFrame window;
    private JScrollPane scrollPane;
    private JScrollPane textScroll;
    private JTextArea textArea;
    private JLabel label = new JLabel("No action yet");
    private JLabel pathLabel;

    // Window buttons
    JButton createButton = new JButton("Create File");
    JButton readButton = new JButton("Read File");
    JButton deleteButton = new JButton("Delete File");

    // File properties
    private String currentPath;
    private JList<String> fileList;
    private FileManager fileManager;
    private OperationResult result;

    public WindowMaker(FileManager fileManager) {
        this.fileManager = fileManager;
        currentPath = fileManager.getCurrentPath();

        window = new JFrame("File Manager"); // Title bar text
        pathLabel =  new JLabel("Path:" + fileManager.getCurrentPath());
        fileList = new JList<>(fileManager.getFiles());
    }

    // Create main window with properties, panels, layout
    public JFrame createWindow() {
        // Window properties + layout
        window.setLayout(new BorderLayout()); // Simple layout
        window.setSize(500, 400); // Window size
        window.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE); // Properly close app

        JPanel topPanel = createTopPanel();
        JPanel centerPanel = createCenterPanel();
        JPanel bottomPanel = createBottomPanel();

        // Add created elements to window
        window.add(topPanel, BorderLayout.NORTH);
        window.add(centerPanel, BorderLayout.CENTER);
        window.add(bottomPanel, BorderLayout.SOUTH);

        return window;
    }

    // ======================== Helper Methods ===========================


    // TOP PANEL: Display current directory path
    private JPanel createTopPanel() {
        JPanel topPanel = new JPanel();
        topPanel.add(pathLabel);
        return topPanel;
    }

    // CENTER PANEL: Display file directory details
    private JPanel createCenterPanel() {
        JPanel centerPanel = new JPanel();
        centerPanel.setLayout(new java.awt.GridLayout(1, 2));
        scrollPane = new JScrollPane(fileList,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
        textArea = new JTextArea(10, 30);
        textArea.setEditable(false); // Do not permit typing until add update feature.
        textScroll = new JScrollPane(textArea,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
        centerPanel.add(scrollPane); // file list
        centerPanel.add(textScroll); // file content
        return centerPanel;
    }

    // BOTTOM PANEL: File CRUD buttons + actions
    private JPanel createBottomPanel() {
        JPanel bottomPanel = new JPanel();
        bottomPanel.setLayout(new FlowLayout(FlowLayout.CENTER, 10, 10));
        // BOTTOM PANEL: Buttons
        bottomPanel.add(createButton);
        create();
        bottomPanel.add(readButton);
        read();
        bottomPanel.add(deleteButton);
        delete();
        bottomPanel.add(label);
        return bottomPanel;
    }

    // Refresh list
    private void refresh(){
        fileList.setListData(fileManager.getFiles());
    }

    //================== CRUD Button Actions ========================
    private void create() {
        createButton.addActionListener(e -> {
            // Save requested file name from pop-up input box
            String fileName = javax.swing.JOptionPane.showInputDialog("Enter file name:");

            // Create file
            if (fileName != null && !fileName.isEmpty()) {
                result = fileManager.createFile(fileName);
                if (result.isSuccess()) {
                    label.setText(result.getMessage());
                    refresh();
                } else {
                    label.setText(result.getMessage());

                }
            } else {
                label.setText("No file name entered.");
            }
        });
    }

    private void read() {
        readButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // Clear context (old state) first
            textArea.setText("");

            // Read file
            if (selected != null) {
                result = fileManager.readFile(selected);
                if (result.isSuccess()) {
                    textArea.setText(result.getContent());
                    label.setText(result.getMessage());
                } else {
                    label.setText(result.getMessage());
                }
            } else {
                label.setText("No file selected.");
            }
        });
    }

    private void delete() {
        deleteButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // Clear context (old state) first
            textArea.setText("");

            // Read file
            if (selected != null) {
                result = fileManager.deleteFile(selected);
                if (result.isSuccess()) {
                    label.setText(result.getMessage());
                    refresh();
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


