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

    // Framing properties
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
    private Path currentPath;
    private JList<String> fileList;
    private FileManager fileManager;
    private OperationResult result;

    // Window constructor which includes logic to build window
    public FileMakerWindow(FileManager fileManager) {
        this.fileManager = fileManager;
        super("File Manager"); // Title bar text

        // Invoke fileManager for a path string and convert to a Path.
        currentPath = Paths.get(fileManager.getCurrentPath());
        fileList = new JList<>(fileManager.getFiles(currentPath));

        // Window properties + layout
        setLayout(new BorderLayout()); // Simple layout
        setSize(500, 400); // Window size
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE); // Properly close app

        // Create panels
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
        JPanel topPanel = new JPanel();
        pathLabel = new JLabel("Path:" + currentPath.toString());
        topPanel.add(pathLabel);
        return topPanel;
    }

    // CENTER PANEL: Displays file directory details
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

    // BOTTOM PANEL: Displays file CRUD buttons and related actions
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

    // Refreshes list to show new files or removal of deleted files
    private void refreshList(){
        fileList.setListData(fileManager.getFiles(currentPath));
    }

    //================== CRUD Button Actions ========================

    // createButton action that invokes createFile method in FileManager
    private void create() {
        createButton.addActionListener(e -> {
            // Save requested file name from pop-up input box
            String fileName = javax.swing.JOptionPane.showInputDialog("Enter file name:");

            // Create file
            if (fileName != null && !fileName.isEmpty()) {
                result = fileManager.createFile(currentPath,  fileName);
                if (result.isSuccess()) {
                    label.setText(result.getMessage());
                    refreshList();
                } else {
                    label.setText(result.getMessage());

                }
            } else {
                label.setText("No file name entered.");
            }
        });
    }

    // readButton action that invokes readFile method in FileManager
    private void read() {
        readButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // Clear context (old state) first
            textArea.setText("");

            // Read file
            if (selected != null) {
                Path selectedPath = currentPath.resolve(selected);
                result = fileManager.readFile(selectedPath);
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

    // deleteButton action that invokes deleteFile method in FileManager
    private void delete() {
        deleteButton.addActionListener(e -> {
            // Save selected file name from list
            String selected = fileList.getSelectedValue();

            // Clear context (old state) first
            textArea.setText("");

            // Read file
            if (selected != null) {
                Path selectedPath = currentPath.resolve(selected);
                result = fileManager.deleteFile(selectedPath);
                if (result.isSuccess()) {
                    label.setText(result.getMessage());
                    refreshList();
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


