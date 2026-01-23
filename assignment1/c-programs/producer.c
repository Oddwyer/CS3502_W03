#include <getopt.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h> //getopt, read, write, close

int main(int argc, char **argv) {

  // needed variables
  FILE *pfile = stdin; // read from...

  // when reading in from a file vs stdin, requires getopt() to parse CDL flags.

  // needed getopt() variables
  int opt;
  int buffer_size = 4096; // 4096 by default
  char *filename = NULL;  // no file by default

  while ((opt = getopt(argc, argv, "f:b:")) != -1) {
    switch (opt) {
    case 'f':
      filename = optarg; // optarg contains the argument value
      // save opened file
      pfile = fopen(filename, "r");
      break;
    case 'b':
      buffer_size = atoi(optarg);
      break;
    default:
      fprintf(stderr, "Usage: %s [-f file] [-b size]\n", argv[0]);
      exit(1);
    }
  }

  char readfile[buffer_size]; // update store read size if file exists

  // reading file from stdin -> does not require f(open)!
  // fgets(storage location, size of storage, input file)
  while (fgets(readfile, sizeof(readfile), pfile) != NULL) {
    // prints to stdout by default
    printf("%s", readfile);
  }

  // close file
  fclose(pfile);
  return 0;
}
