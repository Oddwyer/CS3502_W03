// ======================================
// CS3502 W03 | Spring 2026
// Amber O'Dwyer
// Project 1 - Phase 1
// ======================================

#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <unistd.h>

// Configuration
#define NUM_ACCOUNTS 4
#define NUM_THREADS 6
#define TRANSACTIONS_PER_THREAD 10
#define INITIAL_BALANCE 5000.00

// Account data structure
typedef struct {
  int account_id;
  double balance;
  int transaction_count;
} Account;

// Global shared array - THIS CAUSES RACE CONDITIONS!
Account accounts[NUM_ACCOUNTS];

// Deposit function WITH race condition
void deposit_unsafe(int account_id, double amount) {
  // READ
  double current_balance = accounts[account_id].balance;
  // MODIFY (simulate processing time)
  usleep(1); // This increases likelihood of race condition!
  double new_balance = current_balance + amount;

  // WRITE (another thread might have changed balance between READ and WRITE!)
  accounts[account_id].balance = new_balance;
  accounts[account_id].transaction_count++;
}

// Implement withdrawal_unsafe() following the same pattern
void withdrawal_unsafe(int account_id, double amount) {

  // READ current balance
  double current_balance = accounts[account_id].balance;

  // MODIFY (simulate processing time)
  usleep(1); // This increases likelihood of race condition!
  double new_balance = current_balance - amount;

  // WRITE (another thread might have changed balance between READ and WRITE !)
  accounts[account_id].balance = new_balance;
  accounts[account_id].transaction_count++;
}

// Implement the thread function
void *teller_thread(void *arg) {
  // Extract thread ID (cast *arg to int via (int *)
  int teller_id = *(int *)arg;
  // Initialize thread - safe random seed
  // The seed ensures each thread experiences a different random history instead
  // of replaying the same script.
  unsigned int seed = time(NULL) ^ pthread_self();
  for (int i = 0; i < TRANSACTIONS_PER_THREAD; i++) {
    // Randomly select an account (0 to NUM_ACCOUNTS -1)
    int account_idx = rand_r(&seed) % NUM_ACCOUNTS;
    // Generate random amount (1-100)
    double amount = (rand_r(&seed) % 100) + 1;
    // Randomly choose deposit (1) or withdrawal (0)
    int operation = rand_r(&seed) % 2;

    // Call appropriate function
    if (operation == 1) {
      // Call deposit_unsafe
      deposit_unsafe(account_idx, amount);
      printf("Teller %d: Deposited $%.2f to Account %d\n", teller_id, amount,
             account_idx);
    } else {
      // Call withdrawal_unsafe
      withdrawal_unsafe(account_idx, amount);
      printf("Teller %d: Withdrew $%.2f from Account %d\n", teller_id, amount,
             account_idx);
    }
  }
  return NULL;
}

// Implement main function
int main() {
  printf("=== Phase 1: Race Conditions Demo ===\n\n");
  // Initialize all accounts
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    accounts[i].account_id = i;
    accounts[i].balance = INITIAL_BALANCE;
    accounts[i].transaction_count = 0;
  }
  // Display initial state
  printf("Initial State:\n");
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    printf("Account %d: $%.2f\n", i, accounts[i].balance);
  }

  // Calculate expected final balance
  double expected_total = NUM_ACCOUNTS * INITIAL_BALANCE;
  printf("\nExpected total: $%.2f\n\n", expected_total);

  // Create thread and thread ID arrays
  pthread_t threads[NUM_THREADS];
  int thread_ids[NUM_THREADS];

  // Added performance measurements per phase 2 instruction
  // Start timer
  struct timespec start, end;
  clock_gettime(CLOCK_MONOTONIC, &start);

  // Create all threads
  for (int i = 0; i < NUM_THREADS; i++) {
    // Store ID persistently
    thread_ids[i] = i;
    pthread_create(&threads[i], NULL, teller_thread, &thread_ids[i]);
  }

  // Wait for all threads to complete
  for (int i = 0; i < NUM_THREADS; i++) {
    // Note: The second parameter is a pointer to the value returned by the
    // function the thread runs. If returns NULL, the value is NULL.
    pthread_join(threads[i], NULL);
  }

  // End performance measurement timer
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
  return 0;
}

