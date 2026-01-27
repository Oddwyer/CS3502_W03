#include <getopt.h>
#include <signal.h> // signal handling
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h> // getopt, read, write, close

volatile sig_atomic_t shutdown_flag = 0;

void handle_sigint(int sig) {
 shutdown_flag = 1;
 }

int main(int argc, char **argv) {

  // needed variables:
  FILE *pfile = stdin; // create pointer to read from stdin by default

  // signal() method via sigaction()
  struct sigaction sa;
  sa.sa_handler = handle_sigint;
  sigemptyset(&sa.sa_mask);
  sa.sa_flags = 0;
  sigaction(SIGINT, &sa, NULL);

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

  // run while not trigger by event signal
  while (!shutdown_flag) {

    // NOTE: reading file from stdn -> does not require f(open)!
    // fgets(storage location, size of storage, input file)

    while (fgets(readfile, sizeof(readfile), pfile) != NULL) {

      // printf prints to stdout by default
      printf("%s", readfile);
    }
  // solution for improper line count ensures all data passed to consumer.
  fflush(stdout);

  }


  // close file
  fclose(pfile);

  return 0;
}
