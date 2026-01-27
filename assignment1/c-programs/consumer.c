#include <signal.h> // signal library
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h> // getopt, read, write, close

// variable triggered during signal event intialized to 0.
volatile sig_atomic_t shutdown_flag = 0;
volatile sig_atomic_t stats_flag = 0;

// signal handler 1 (Ctrl + C)
void handle_sigint(int sig) { shutdown_flag = 1; }

// signal handler 2 (SIGUSR1)
void handle_sigusr1(int sig) { stats_flag = 1; }

int main(int argc, char **argv) {

  // needed variables
  FILE *cfile = stdin; // read from...
  char readfile[100];  // note 100 is how many bytes read at a time

  // signal() function via sigaction()
  struct sigaction sa;
  sa.sa_handler = handle_sigint;
  sigemptyset(&sa.sa_mask);
  sa.sa_flags = 0;
  sigaction(SIGINT, &sa, NULL);

  struct sigaction sa1;
  sa1.sa_handler = handle_sigusr1;
  sigemptyset(&sa1.sa_mask);
  sa1.sa_flags = 0;
  sigaction(SIGUSR1, &sa1, NULL);

  // GETOPT() Method: required to parse CLI flags when reading in from a file
  // vs stdin.
  // 1. needed getopt() variables
  int opt = 0;
  int maxlines = 0;
  int linecount = 0;
  int charcount = 0;
  bool maxpassed = false;
  bool verbose = false;

  // 2.getopt() method
  while ((opt = getopt(argc, argv, "vn:")) !=
         -1) { // note: ":" removed after v if only flag and no argument passed.
    switch (opt) {
    case 'v':
      verbose = true;
      break;
    case 'n':
      maxlines = atoi(optarg); // atio = "ASCII to int"
      maxpassed = true;
      break;
    default:
      fprintf(stderr, "Usage: %s [-v]  [-n max lines]\n", argv[0]);
      exit(1);
    }
  }

  // reading file from stdin -> does not require f(open)!
  // fgets(storage location, size of storage, input file)
  while (fgets(readfile, sizeof(readfile), cfile) != NULL) {
    // max lines condition
    if (maxpassed && linecount == maxlines) {
      break;
    }
    // if -v flag,  print to stdout
    if (verbose) {
      printf("%s", readfile);
    }
    // if new line, count line
    if (readfile[strlen(readfile) - 1] == '\n') {
      linecount++;
    }
    // count characters
    charcount += strlen(readfile);

    // SIGUSR1 flag condition
    if (stats_flag) {
      printf("Current line count: %d\nCurrent character count: %d\n", linecount,
             charcount);
      stats_flag = 0;
    }
  }

  // if one line of text only (thus no \n), count line
  if (charcount > 0 && linecount == 0) {
    linecount++;
  }

  // print stats to standard error
  fprintf(stderr, "Line count: %d\nCharacter count: %d\n", linecount,
          charcount);

  if (shutdown_flag) {
    fprintf(stderr, "Consumer Graceful Shutdown...\n");
  } else {
    printf("Shutting down...");
  }
  // close file
  fclose(cfile);

  return 0;
}
