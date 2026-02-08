// ============================================
//  CS3502-W03 | Amber O'Dwyer
//  producer.c - Producer process starter
// ============================================

#include "buffer.h"
#include <errno.h>

// Global variables for cleanup
shared_buffer_t *buffer = NULL;
sem_t *mutex = NULL;
sem_t *empty = NULL;
sem_t *full = NULL;
int shm_id = -1;

//  Put method with pointer, item parameters.
static void put(shared_buffer_t *ptr, item_t item) {
  ptr->buffer[ptr->head] = item;
  ptr->head = (ptr->head + 1) % BUFFER_SIZE;
  ptr->count++;
}

void cleanup() {
  // Detach shared memory
  if (buffer != NULL) {
    shmdt(buffer);
  }

  // Close semaphores (don't unlink - other processes may be using)
  if (mutex != SEM_FAILED)
    sem_close(mutex);
  if (empty != SEM_FAILED)
    sem_close(empty);
  if (full != SEM_FAILED)
    sem_close(full);
}

void signal_handler(int sig) {
  printf("\nProducer: Caught signal %d, cleaning up...\n", sig);
  cleanup();
  exit(0);
}

int main(int argc, char *argv[]) {
  if (argc != 3) {
    fprintf(stderr, "Usage: %s <producer_id> <num_items>\n", argv[0]);
    exit(1);
  }

  int producer_id = atoi(argv[1]);
  int num_items = atoi(argv[2]);

  // Set up signal handlers
  signal(SIGINT, signal_handler);
  signal(SIGTERM, signal_handler);

  // Seed random number generator
  srand(time(NULL) + producer_id);

  // TODO: Attach to shared memory
  // Create boolean value to check if created already or not. Make an int w/ 0 =
  // false, non-zero = true
  int created = 0;
  // Create/get existing shared memory
  shm_id =
      shmget(SHM_KEY, sizeof(shared_buffer_t), IPC_CREAT | IPC_EXCL | 0666);
  // If shared was created, change boolean value
  if (shm_id >= 0) {
    created = 1;
  } else {
    // If not created and shared memory doesn't exist
    if (errno != EEXIST) {
      perror("shmget"); // print error reason
      exit(1);
    }
    // If not created but exists, return shm_id for shared memory
    shm_id = shmget(SHM_KEY, sizeof(shared_buffer_t), 0666);
  }

  // Attach to shared memory location
  buffer = shmat(shm_id, NULL, 0);

  // Once shared memory created and pointer set, intialize head, tail, count.
  // (Cannot be done in buffer- must be done at run time!)
  if (created) {
    buffer->head = 0;
    buffer->tail = 0;
    buffer->count = 0;
  }
  // TODO: Open semaphores
  mutex = sem_open(SEM_MUTEX, O_CREAT, 0644, 1);
  empty = sem_open(SEM_EMPTY, O_CREAT, 0644, BUFFER_SIZE);
  full = sem_open(SEM_FULL, O_CREAT, 0644, 0);

  printf("Producer %d: Starting to produce %d items\n", producer_id, num_items);

  // TODO: Main production loop
  for (int i = 0; i < num_items; i++) {

    // Create item
    item_t item;
    int item_number = i;
    item.value = producer_id * 1000 + item_number;
    item.producer_id = producer_id;

    // TODO: Wait for empty slot
    sem_wait(empty);

    // TODO: Enter critical section

    sem_wait(mutex);
    // TODO: Add item to buffer
    put(buffer, item);
    printf("Producer %d: Produced value %d\n", producer_id, item.value);

    // TODO: Exit critical section
    sem_post(mutex);

    // TODO: Signal item available
    sem_post(full);

    // Simulate production time
    usleep(rand() % 100000);
  }

  printf("Producer %d: Finished producing %d items\n", producer_id, num_items);

  // Detaches shared memory, closes semaphores
  cleanup();

  return 0;
}
