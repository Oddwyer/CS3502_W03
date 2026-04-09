// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - Main

// Window and button GUI imports

import javax.swing.*;

// Entry point of the application. Initializes and displays the GUI

public class Main {
    public static void main(String[] args) {

        SwingUtilities.invokeLater(() -> {
            FileManager fileManager = new LocalFileManager();
            WindowMaker windowMaker = new WindowMaker(fileManager);
            JFrame window = windowMaker.createWindow();
            window.setVisible(true);
        });
    }
}
