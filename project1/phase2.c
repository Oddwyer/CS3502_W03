// ======================================
// CS3502 W03 | Spring 2026
// Amber O'Dwyer
// Project 1 - Phase 2
// ======================================

#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <unistd.h>

// Configuration
#define NUM_ACCOUNTS 4
#define NUM_THREADS 6
#define TRANSACTIONS_PER_THREAD 10000
#define INITIAL_BALANCE 5000.00

// Updated Account structure with mutex
typedef struct {
  int account_id;
  double balance;
  int transaction_count;
  pthread_mutex_t lock; // NEW: Mutex for this account
} Account;

// Global shared array - THIS CAUSES RACE CONDITIONS!
Account accounts[NUM_ACCOUNTS];

// Create thread arrays for deposit/withdrawl totals + initialize all to 0
// Must keep each thread cummulative deposits/withdraws separate for proper
// tracking Since we have no tranfer method, deposits/withdrawals could be
// anything!
double deposits[NUM_THREADS] = {0};
double withdrawals[NUM_THREADS] = {0};

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

// Deposit function WITH proper protection
void deposit_safe(int account_id, double amount) {
  // Acquire lock BEFORE accessing shared data
  pthread_mutex_lock(&accounts[account_id].lock);

  // ===== CRITICAL SECTION =====
  // Only ONE thread can execute this at a time for this account
  accounts[account_id].balance += amount;
  accounts[account_id].transaction_count++;
  // ============================

  // Release lock AFTER modifying shared data
  pthread_mutex_unlock(&accounts[account_id].lock);
}

// Withdrawal_safe() with mutex protection
void withdrawal_safe(int account_id, double amount) {
  // Acquire lock BEFORE accessing shared data
  pthread_mutex_lock(&accounts[account_id].lock);

  // ===== CRITICAL SECTION =====
  accounts[account_id].balance -= amount;
  accounts[account_id].transaction_count++;
  // ============================

  // Release lock AFTER modifying shared data
  pthread_mutex_unlock(&accounts[account_id].lock);
}

// Updated teller_thread (uses safe functions)
void *teller_thread(void *arg) {
  int teller_id = *(int *)arg;
  unsigned int seed = time(NULL) ^ pthread_self();
  for (int i = 0; i < TRANSACTIONS_PER_THREAD; i++) {
    int account_idx = rand_r(&seed) % NUM_ACCOUNTS;
    double amount = (rand_r(&seed) % 100) + 1;
    int operation = rand_r(&seed) % 2;

    // Call appropriate function
    if (operation == 1) {
      // Call deposit_safe
      deposit_safe(account_idx, amount);
      printf("Teller %d: Deposited $%.2f to Account %d\n", teller_id, amount,
             account_idx);
      deposits[teller_id] += amount;
    } else {
      // Call withdrawal_safe
      withdrawal_safe(account_idx, amount);
      printf("Teller %d: Withdrew $%.2f from Account %d\n", teller_id, amount,
             account_idx);
      withdrawals[teller_id] += amount;
    }
  }
  return NULL;
}

// Mutex cleanup function
void cleanup_mutexes() {
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    pthread_mutex_destroy(&accounts[i].lock);
  }
}

// Calculate expected total function
double compute_expected(double deposits[], double withdrawals[]) {
  double total_deposits = 0.0;
  double total_withdrawals = 0.0;
  double initial_total = NUM_ACCOUNTS * INITIAL_BALANCE;

  for (int i = 0; i < NUM_THREADS; i++) {
    // Total all deposit/withdrawl amounts
    total_deposits += deposits[i];
    total_withdrawals += withdrawals[i];
  }
  double expected = initial_total + total_deposits - total_withdrawals;
  return expected;
}

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

  // Create all threads
  for (int i = 0; i < NUM_THREADS; i++) {
    // Store ID persistently
    thread_ids[i] = i;
    pthread_create(&threads[i], NULL, teller_thread, &thread_ids[i]);
  }

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

  printf("\nInitial balance: $%.2f\n", expected_total);

  // New expected total based on random withdrawals/deposits without tranfers.
  expected_total = compute_expected(deposits, withdrawals);
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

// Compare Phase 1 vs Phase 2 performance
// Measure execution time for both versions
