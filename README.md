# CS 3502 Operating Systems
Operating Systems Coursework:

### Assignments:
- Assignment 1: Creating Linux Environment & C Programming Fundamentals
- Assignment 2: Memory Management & Synchronization Primitives

### Projects:
#### Project 1: Multi-threaded Banking System Simulator

- Build Instructions:
  - This project uses a Makefile to compile each phase of the assignment. 
  - From the command line:
      ```bash
      - git clone https://github.com/Oddwyer/CS3502_W03.git
      - cd CS3502_W03/project1
      - make
      ```
  - This compiles all phases using:
      ```bash
      gcc -Wall -Wextra -pthread phaseX.c -o phaseX
      ```
      - Where X corresponds to the phase number (1–4).
    
  - To clean compiled binaries:
     ```bash	
      make clean
     ```
- Run Instructions: 
  - After building, execute the desired phase:
    ```bash
    ./phase1
    ./phase2
    ./phase3
    ./phase4
    ./phase4E
    ```
    - It is recommended to run multiple times to observe thread scheduling differences and contention behavior.



#### Project 2: CPU Scheduling Algorithm Comparison
#### Project 3: File System Implementation
