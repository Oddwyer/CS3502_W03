// ======================================
// CS3502 W03 | Spring 2026
// Amber O'Dwyer
// Project 1 - Phase 4

// AI Assistance Disclosure:
// ChatGPT (OpenAI, personal communication, February 25, 2026) was used for high-level
// conceptual clarification regarding:
// - pthread behavior
// - deadlock prevention strategies (e.g., lock ordering)
// - proper resource-handling practices (e.g., malloc and thread error checks).
// All implementation logic, design, and final code reflect my own understanding.
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
  pthread_mutex_lock(&accounts[second].lock);

  // Step 4: Perform transfer
  // Balance checking + error handling
  if (accounts[from_id].balance < amount) {
    printf("Insufficient funds for transfer.");
  } else {
    accounts[from_id].balance -= amount;
    accounts[from_id].transaction_count++;
    accounts[to_id].balance += amount;
    accounts[to_id].transaction_count++;
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

  // Thread 1 creation: transfer(0, 1, amount)
  pthread_t thread1;
  transfer_args *args1 = malloc(sizeof(*args1));
  // Error handling: confirming memory space exists on heap -> did not return null
  if (args1 == NULL) {
    perror("malloc failed");
    exit(1); //abnormal exit
  }

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

  // Error handling: confirming memory space exists on heap -> did not return null
  if (args2 == NULL) {
    perror("malloc failed");
    exit(1); //abnormal exit
  }

  seed = time(NULL) ^ 2;
  amount = (rand_r(&seed) % 100) + 1;

  args2->from_id = 1;
  args2->to_id = 0;
  args2->amount = amount;
  args2->thread_id = 2;
  pthread_create(&thread2, NULL, transfer_thread, args2);

  // Wait for transfer threads to complete.
  pthread_join(thread1, NULL);
  pthread_join(thread2, NULL);

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

  // Mutex cleanup in main()
  cleanup_mutexes();
  return 0;
}
