// Amber O'Dwyer
// CS3502 - W07 | Operating Systems
// Project 3: File Manager - Main

// Window + Button GUI Imports
import javax.swing.*;
import java.awt.FlowLayout;

// Entry point of the application. Initializes and displays the GUI.

public class Main {
    public static void main(String[] args) {

        // Window Properties
        JFrame window = new JFrame("File Manager");  // Title bar text
        window.setSize(500, 400); // Window size
        window.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE); // Properly close app

        // Window button + label
        JButton button = new JButton("Click Me"); // Create button
        JLabel label = new JLabel("No action yet");

        window.setLayout(new FlowLayout()); // simple layout

        window.add(button); // Add button to window
        window.add(label); // Add label to window

        // Button action upon click (updates label)
        button.addActionListener(e -> {
            label.setText("Button was clicked!");
        });

        // Display window
        window.setVisible(true);


    }
}