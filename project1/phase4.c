// ======================================
// CS3502 W03 | Spring 2026
// Amber O'Dwyer
// Project 1 - Phase 4

// AI Assistance Disclosure:
// ChatGPT (OpenAI, personal communication, February 25, 2026) was used
// for conceptual clarification regarding pthread usage, deadlock vs.
// livelock distinctions, and Makefile structure. All implementation
// decisions and final code reflect my own understanding.
// ======================================

#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <unistd.h>
#include <errno.h>
#include <string.h>

// Configuration
#define NUM_ACCOUNTS 2
#define INITIAL_BALANCE 5000.00
// Unused: #define NUM_THREADS 2
// Unused: #define TRANSACTIONS_PER_THREAD 10

// Updated Account structure with mutex
typedef struct {
  int account_id;
  double balance;
  int transaction_count;
  pthread_mutex_t lock;
} Account;

// Transfer function params struct
typedef struct {
  int thread_id;
  int from_id;
  int to_id;
  double amount;
} transfer_args;

// Global shared array
Account accounts[NUM_ACCOUNTS];

// ========================Functions==========================
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

// Transfer function w/ STRATEGY 1: Lock Ordering (RECOMMENDED)
void safe_transfer_ordered(int from_id, int to_id, double amount, int thread_id) {
  // ALGORITHM: To prevent circular wait, always acquire locks in consistent order
  // Step 1: Identify which account ID is first (lower)
  int first = 0;
  int second = 0;
  if (from_id < to_id) {
    first = from_id;
    second = to_id;
  } else {
    second = from_id;
    first = to_id;
  }

  // WHY THIS WORKS:
  // - Thread 1: transfer (0 ,1) locks 0 then 1
  // - Thread 2: transfer (1 ,0) locks 0 then 1 (SAME ORDER!)
  // - No circular wait (Coffman 4) possible

  // Step 2: Lock first ID account -> Coffman 1: mutual exclusion
  pthread_mutex_lock(&accounts[first].lock);
  printf("Thread %d: Locked account %d\n", thread_id, first);

  // Simulate processing delay
  usleep(100);

  // Step 3: Try to lock destination (second) account
  // Coffman 2: hold-and-wait (first lock held)
  // Coffman 3: no preemption (no force release of first lock)
  printf("Thread %d: Waiting for account %d\n", thread_id, second);
  pthread_mutex_lock(&accounts[second].lock); // DEADLOCK HERE!

  // Step 4: Perform transfer
  // Balance checking + error handling
  if (accounts[from_id].balance <= 0) {
    printf("Insufficient funds for transfer.");
    return;
  } else {
    accounts[from_id].balance -= amount;
    accounts[to_id].balance += amount;
  }

  // Step 5: Try to unlock in reverse order
  pthread_mutex_unlock(&accounts[second].lock);
  pthread_mutex_unlock(&accounts[first].lock);
}

// transfer_thread outer function
void *transfer_thread(void *arg) {
  // Cast from void to transfer_args
  transfer_args *args = (transfer_args *)arg;
  // Pass to deadlock function
  safe_transfer_ordered(args->from_id, args->to_id, args->amount, args->thread_id);
  // Release memory
  free(args);
  return NULL;
}

// timer_thread function
void *timer_thread(void *arg) {
  sleep(5); // sleep 5 seconds
  return NULL;
}

// Unused: teller_thread function
// void *teller_thread(void *arg) {
//   int teller_id = *(int *)arg;
//   unsigned int seed = time(NULL) ^ pthread_self();
//   for (int i = 0; i < TRANSACTIONS_PER_THREAD; i++) {
//     int from_account = rand_r(&seed) % NUM_ACCOUNTS;
//     int to_account = rand_r(&seed) % NUM_ACCOUNTS;
//     double amount = (rand_r(&seed) % 100) + 1;
//     int operation = rand_r(&seed) % 2;
//  }
//  return NULL;
//}

// Mutex cleanup function
void cleanup_mutexes() {
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    pthread_mutex_destroy(&accounts[i].lock);
  }
}


// =====================Main Function=========================
int main() {
  printf("=== Phase 4: Deadlock Resolution ===\n\n");

  // Initialize all accounts
  initialize_accounts();

  // Display initial state
  printf("Initial State:\n");
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    printf("Account %d: $%.2f\n", i, accounts[i].balance);
  }

  // Calculate INITIAL expected final balance
  double expected_total = NUM_ACCOUNTS * INITIAL_BALANCE;
  printf("\nExpected total: $%.2f\n\n", expected_total);

  // Unused: Create thread and thread ID arrays
  // pthread_t threads[NUM_THREADS];
  // int thread_ids[NUM_THREADS];
  
    // Thread 1 creation: transfer(0, 1, amount)
    pthread_t thread1;
    transfer_args *args1 = malloc(sizeof(*args1));
    unsigned int seed = time(NULL) ^ 1;
    double amount = (rand_r(&seed) % 100) + 1;

    args1->from_id = 0;
    args1->to_id = 1;
    args1->amount = amount;
    args1->thread_id = 1;
    pthread_create(&thread1, NULL, transfer_thread, args1);

    // Thread 2 creation: transfer(1, 0, amount)
    pthread_t thread2;
    transfer_args *args2 = malloc(sizeof(*args2));
    seed = time(NULL) ^ 2;
    amount = (rand_r(&seed) % 100) + 1;

    args2->from_id = 1;
    args2->to_id = 0;
    args2->amount = amount;
    args2->thread_id = 2;
    pthread_create(&thread2, NULL, transfer_thread, args2);

  // Implement deadlock detection
  // Add timeout counter in main() -> via timeout thread
  pthread_t timeout;
  pthread_create(&timeout, NULL, timer_thread, NULL);

  // Wait for timeout thread to complete only; other threads will never complete.
  pthread_join(timeout, NULL);
  // After 5 seconds, announce deadlock suspected
  printf("5 seconds has lapsed. Deadlock suspected.\n");
  // exit(1) = abnormal termination due to deadlock
  exit(1);

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
