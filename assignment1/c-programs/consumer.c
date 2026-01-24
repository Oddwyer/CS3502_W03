#include <getopt.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h> //getopt, read, write, close

int main(int argc, char **argv) {

  // needed variables
  FILE *cfile = stdin; // read from...
  char readfile[100];  // note 100 is how many bytes read at a time
  // when reading in from a file vs stdin, requires getopt() to parse CDL flags.

  // needed getopt() variables
  int opt = 0;
  int maxlines = 0;
  int linecount = 0;
  int charcount = 0;
  bool maxpassed = false;
  bool verbose = false;

  while ((opt = getopt(argc, argv, "vn:")) !=
         -1) { // note: ":" removed after v if no argument passed, only flag
    switch (opt) {
    case 'v':
      verbose = true;
      break;
    case 'n':
      maxlines = atoi(optarg); // atio = ascii to int
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
    // prints to stdout by default; if -v flag,  print to stdout
    if (verbose) {
      printf("%s", readfile);
    }
    // if new line, count line
    if (readfile[strlen(readfile) - 1] == '\n') {
      linecount++;
    }
    // count characters
    charcount += strlen(readfile);
  }

  // accounts for one line of text only, thus no \n
  if (charcount > 0 && linecount == 0) {
    linecount++;
  }

  fprintf(stderr, "Line count: %d\nCharacter count: %d\n", linecount,
          charcount);

  // close file
  fclose(cfile);
  return 0;
}
