#include <getopt.h>
#include <signal.h> // signal handling
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h> // getopt, read, write, close

volatile sig_atomic_t shutdown_flag = 0;

void handle_sigint(int sig) { shutdown_flag = 1; }

// signal handler 2 (SIGUSR1)
void handle_sigusr1(int sig) {}

int main(int argc, char **argv) {

  // needed variables:
  FILE *pfile = stdin; // create pointer to read from stdin by default

  // signal() method via sigaction()
  struct sigaction sa;
  sa.sa_handler = handle_sigint;
  sigemptyset(&sa.sa_mask);
  sa.sa_flags = 0;
  sigaction(SIGINT, &sa, NULL);

  // Must create override to avoid default
  // sigusr1 event which is to terminate.
  struct sigaction sa1;
  sa1.sa_handler = handle_sigusr1;
  sigemptyset(&sa1.sa_mask);
  sa1.sa_flags = 0;
  sigaction(SIGUSR1, &sa1, NULL);

  // GETOPT() Method: required to parse CLI flags  when reading in from a file
  // vs stdin.
  // 1. getopt() variables
  int opt = 0;
  int buffer_size = 4096; // 4096 by default
  char *filename = NULL;  // no file by default

  // 2. getopt() method
  while ((opt = getopt(argc, argv, "f:b:")) != -1) {
    switch (opt) {
    case 'f':
      filename = optarg; // stores user input file name from CLI
      // point to new opened file stream
      pfile = fopen(filename, "r");
      break;
    case 'b':
      buffer_size = atoi(optarg); // stores user input buffer size from CLI
      break;
    default:
      fprintf(stderr, "Usage: %s [-f file] [-b size]\n", argv[0]);
      exit(1);
    }
  }

  char readfile[buffer_size]; // create storage array for streamed characters
                              // IF file exists

  // while (!shutdown_flag) loops indefinitely

  // NOTE: reading file from stdn -> does not require f(open)!
  // fgets(storage location, size of storage, input file)

  while (fgets(readfile, sizeof(readfile), pfile) != NULL) {

    // printf prints to stdout by default
    printf("%s", readfile);

    // solution for improper line count ensures all data passed to consumer.
    fflush(stdout);
    sleep(1);
    if (shutdown_flag) {
      break;
    }
  }

  // close file
  fclose(pfile);
  if (shutdown_flag) {
    fprintf(stderr, "Producer Graceful Shutdown...\n");
  }
  return 0;
}
