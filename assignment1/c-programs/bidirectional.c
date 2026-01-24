#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/wait.h>
#include <unistd.h>

int main() {

  int pipe1[2];
  int pipe2[2];
  pid_t pid;
  char buffer[100];
  char *parentmessage = "Hello child!";
  char *childmessage = "Hi parent!";

  // create pipe using pipe(pipe1)
  // Check for errors (pipe returns -1 on failure)
  if (pipe(pipe1) < 0 || pipe(pipe2) < 0) {
    printf("pipe1 or pipe2 failed");
    return 1;
  } // fork the process
  else {
    pid = fork();
  }

  if (pid < 0) {
    printf("fork failed");
    return 1;
  }

  if (pid == 0) {
    // Child process
    // close the write end (child only reads)
    close(pipe1[1]);
    close(pipe2[0]);
    // child recieves the messge
    while (read(pipe1[0], buffer, strlen(parentmessage) + 1) > 0) {
      printf("%s\n", buffer);
    }
    // close read end
    close(pipe1[0]);
    // child responds
    write(pipe2[1], childmessage, strlen(childmessage) + 1);
    close(pipe2[1]);
  } else {
    // Parent process
    // close the read end (parent only writes)
    close(pipe1[0]);
    close(pipe2[1]);
    // parent sents a message
    write(pipe1[1], parentmessage, strlen(parentmessage) + 1);
    // close write end
    close(pipe1[1]);
    // wait for child to finish
    wait(NULL);
    // parent receives the response
    while (read(pipe2[0], buffer, strlen(childmessage) + 1) > 0) {
      printf("%s\n", buffer);
    }
  }

  return 0;
}
