// ======================================
// CS3502 W03 | Spring 2026
// Amber O'Dwyer
// Project 1 - Phase 3
// ======================================

#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <unistd.h>

// Configuration
#define NUM_ACCOUNTS 2
#define NUM_THREADS 2
#define TRANSACTIONS_PER_THREAD 10
#define INITIAL_BALANCE 5000.00

// Updated Account structure with mutex
typedef struct {
  int account_id;
  double balance;
  int transaction_count;
  pthread_mutex_t lock; // NEW: Mutex for this account
} Account;

// Transfer params structure
typedef struct {
  int from_id;
  int to_id;
  double amount;
} transfer_args;

// Global shared array - THIS CAUSES RACE CONDITIONS!
Account accounts[NUM_ACCOUNTS];

// Mutex initialization
void initialize_accounts() {
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    accounts[i].account_id = i;
    accounts[i].balance = INITIAL_BALANCE;
    accounts[i].transaction_count = 0;

    // Initialize the mutex
    pthread_mutex_init(&accounts[i].lock, NULL);
  }
}

// Conceptual example showing HOW deadlock occurs
// This code WILL cause deadlock!
void *transfer_thread(void *args) {
  transfer_deadlock_example(args->from_id, args->to_id, args->amount);
}

void transfer_deadlock_example ( int from_id , int to_id , double amount ) {
  // Lock source (from) account
  pthread_mutex_lock(&accounts[from_id].lock);
  printf("Thread %ld: Locked account %d\n", pthread_self(), from_id);

  // Simulate processing delay
  usleep(100);

  // Try to lock destination (to) account
  printf("Thread %ld: Waiting for account %d\n", pthread_self(), to_id);
  pthread_mutex_lock(&accounts[to_id].lock); // DEADLOCK HERE!

  // Transfer (never reached if deadlocked)
  // Balance checking + error handling
  if (accounts[from_id].balance <= 0) {
    printf("Insufficient funds for transfer.");
    return 1;
  } else {
    accounts[from_id].balance -= amount;
    accounts[to_id].balance += amount;
  }

  // Try to release locks
  pthread_mutex_unlock(&accounts[to_id].lock);
  pthread_mutex_unlock(&accounts[from_id].lock);
}

// teller_thread function
void *teller_thread(void *arg) {
  int teller_id = *(int *)arg;
  unsigned int seed = time(NULL) ^ pthread_self();
  for (int i = 0; i < TRANSACTIONS_PER_THREAD; i++) {
    int from_account = rand_r(&seed) % NUM_ACCOUNTS;
    int to_account = rand_r(&seed) % NUM_ACCOUNTS;
    double amount = (rand_r(&seed) % 100) + 1;
    int operation = rand_r(&seed) % 2;
  }
  return NULL;
}

// Mutex cleanup function
void cleanup_mutexes() {
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    pthread_mutex_destroy(&accounts[i].lock);
  }
}

// TODO 3: Implement deadlock detection
// Add timeout counter in main()
// If no progress for 5 seconds, report suspected deadlock
// Reference: time (NULL) for simple timing

// Main function
int main() {
  printf("=== Phase 2: Mutex Protection Demo ===\n\n");

  // Initialize all accounts
  initialize_accounts();

  // Display initial state
  printf("Initial State:\n");
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    printf("Account %d: $%.2f\n", i, accounts[i].balance);
  }

  // Calculate INITIAL  expected final balance
  double expected_total = NUM_ACCOUNTS * INITIAL_BALANCE;
  printf("\nExpected total: $%.2f\n\n", expected_total);

  // Create thread and thread ID arrays
  pthread_t threads[NUM_THREADS];
  int thread_ids[NUM_THREADS];

  // Add performance timing START
  struct timespec start, end;
  clock_gettime(CLOCK_MONOTONIC, &start);

  // Create threads that will deadlock. Result : Circular wait!
  // Thread 1: transfer(0, 1, amount) // Locks 0, wants 1

    thread_ids[0] = 0;
    transfer_args *args1 = malloc(sizeof(*args));
    // unsigned int seed = time(NULL) ^ pthread_self();
    // double amount = (rand_r(&seed) % 100) + 1;
    args1->from_id = 0;
    args1->to_id = 1;
    args1->amount = 15.06;
    pthread_create(&threads[0], NULL, transfer_thread, &args1);

  // Thread 2: transfer(1, 0, amount) // Locks 1, wants 0
    thread_ids[1] = 1;
    transfer_args *args2 = malloc(sizeof(*args));
    // unsigned int seed = time(NULL) ^ pthread_self();
    // double amount = (rand_r(&seed) % 100) + 1;
    args2->from_id = 1;
    args2->to_id = 0;
    args2->amount = 15.06;
    pthread_create(&threads[1], NULL, transfer_thread, &args2);

  // TODO: Thread 3: Time tracking thread that will end program if 5 second wait time is met.
  // exit(status) to kill all threads not just Thread 3.

  // Wait for all threads to complete
  for (int i = 0; i < NUM_THREADS; i++) {
    pthread_join(threads[i], NULL);
  }

  // Add performance timing END
  clock_gettime(CLOCK_MONOTONIC, &end);
  double elapsed =
      (end.tv_sec - start.tv_sec) + (end.tv_nsec - start.tv_nsec) / 1e9;
  printf("Time: %.4f seconds\n", elapsed);

  // Calculate and display results
  printf("\n=== Final Results ===\n");
  double actual_total = 0.0;
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    printf("Account %d: $%.2f (%d transactions)\n", i, accounts[i].balance,
           accounts[i].transaction_count);
    actual_total += accounts[i].balance;
  }

  printf("\nExpected total: $%.2f\n", expected_total);
  printf("Actual total: $%.2f\n", actual_total);
  printf("Difference: $%.2f\n", actual_total - expected_total);

  // Race condition detection message
  if (expected_total != actual_total) {
    printf("\nRACE CONDITION DETECTED!\n");
  }

  // Mutex cleanup in main()
  cleanup_mutexes();
  return 0;
}

// TODO 4: Document the Coffman conditions
// In your report , identify WHERE each condition occurs
// Create resource allocation graph showing circular wait
