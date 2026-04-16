// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - Main (Entry Point)

// GUI imports
import javax.swing.*;

/*Entry point of the application. Initializes and displays the GUI*/

public class Main {
    public static void main(String[] args) {

        SwingUtilities.invokeLater(() -> {
            FileManager fileManager = new LocalFileManager();
            FileMakerWindow window = new FileMakerWindow(fileManager);
            window.setVisible(true);
        });
    }
}
