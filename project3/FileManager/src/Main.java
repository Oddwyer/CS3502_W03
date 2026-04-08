// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - Main

// Window and button GUI imports

import javax.swing.*;
import java.awt.*;

// File logic imports
import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

// Entry point of the application. Initializes and displays the GUI

public class Main {
    public static void main(String[] args) {
        // ================= File Directory + Display ============================
        // Begin with current folder: "." + Swing components for display
        // TODO: Upgrade to JList<File> to use Paths.getPath()
        JList<String> fileList = new JList<>(getFiles());
        JScrollPane scrollPane = new JScrollPane(fileList,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);

        // ================= Window Properties + Layout ====================
        JFrame window = new JFrame("File Manager");  // Title bar text
        window.setLayout(new BorderLayout()); // Simple layout
        window.setSize(500, 400); // Window size
        window.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE); // Properly close app
        // Scrollbars
        
        //================== CRUD Logic + Buttons ========================
        // 1. "Create file" button + label
        JButton createButton = new JButton("Create File");
        JLabel label = new JLabel("No action yet");

        // 2. "Read file" button + label
        JButton readButton = new JButton("Read File");
        // Display file content
        JTextArea textArea = new JTextArea(10, 30);
        textArea.setEditable(false); // Do not permit typing until add update feature.
        JScrollPane textScroll = new JScrollPane(textArea,
                JScrollPane.VERTICAL_SCROLLBAR_ALWAYS,
                JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);

        // 3. "Delete file" button + label
        JButton deleteButton = new JButton("Delete File");

        // ======================== Window Panels ===========================
        JPanel topPanel = new JPanel();
        JPanel centerPanel = new JPanel();
        centerPanel.setLayout(new java.awt.GridLayout(1, 2));
        JPanel bottomPanel = new JPanel();
        bottomPanel.setLayout(new FlowLayout(FlowLayout.CENTER, 10, 10));

        // TOP PANEL: Current directory path
        JLabel pathLabel = new JLabel("Path:" + getCurrentPath());
        topPanel.add(pathLabel);

        // CENTER PANEL: File directory details
        centerPanel.add(scrollPane); // file list
        centerPanel.add(textScroll); // file content

        // BOTTOM PANEL: Buttons
        bottomPanel.add(createButton);
        bottomPanel.add(readButton);
        bottomPanel.add(deleteButton);
        bottomPanel.add(label);

        // Add created elements to window
        window.add(topPanel, BorderLayout.NORTH);
        window.add(centerPanel, BorderLayout.CENTER);
        window.add(bottomPanel, BorderLayout.SOUTH);

        // Create button action upon click (updates label)
        createButton.addActionListener(e -> {
            // Save requested file name from pop-up input box
            String fileName = javax.swing.JOptionPane.showInputDialog("Enter file name:");

            // Error handling
            if (fileName != null && !fileName.isEmpty()) {
                try {
                    // Create file path to requested file
                    File file = new File(fileName);

                    // Create file if it does not already exist; if it does return notice
                    if (file.createNewFile()) {
                        label.setText("File created: " + fileName);
                        fileList.setListData(getFiles()); // Refresh list
                    } else {
                        label.setText("File already exists.");
                    }
                } catch (IOException ex) {
                    label.setText("Error creating file.");
                }
            } else {
                label.setText("No file name entered.");
            }
        });

        // Read button action upon click (updates label)
        readButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();
            // Clear context (old state) first
            textArea.setText("");

            // Error handling
            if (selected != null) {
                try {
                    // Save content by reading entire file available at the path accessed via Paths.get()
                    // Note: Paths.getPath() used when path exists. As "selected" is simply a string, Paths.get()
                    // is needed to build it.
                    // TODO: Update to Paths.get(currentPath, selected) later
                    String content = Files.readString(Paths.get(selected));
                    textArea.setText(content);
                    label.setText("Opened: " + selected);
                } catch (Exception ex) {
                    label.setText("Error reading file.");
                }
            } else {
                label.setText("No file selected.");
            }
        });

        // Delete button action upon click (updates label)
        deleteButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // Error handling
            if (selected != null) {
                File deleteFile = new File(selected);
                if (deleteFile.isFile()) {
                    if (deleteFile.delete()) {
                        label.setText("Deleted: " + selected);
                        fileList.setListData(getFiles()); // refresh
                        textArea.setText(""); // clear content
                    } else {
                        label.setText("Could not delete file.");
                    }
                } else {
                    label.setText("Cannot delete a directory.");
                }
            } else {
                label.setText("No file selected.");
            }
        });

        // Display window
        window.setVisible(true);
    }

    //================Helper Methods===================

    // Refresh file list method
    public static String[] getFiles() {
        return new File(".").list();
    }

    // Display current working directory
    public static String getCurrentPath() {
        return System.getProperty("user.dir");
    }
}