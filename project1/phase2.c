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

// Configuration - experiment with different values!
#define NUM_ACCOUNTS 4
#define NUM_THREADS 6
#define TRANSACTIONS_PER_THREAD 10
#define INITIAL_BALANCE 5000.00

// GIVEN: Updated Account structure with mutex
typedef struct {
  int account_id;
  double balance;
  int transaction_count;
  pthread_mutex_t lock; // NEW: Mutex for this account
} Account;

// GIVEN: Example of mutex initialization
void initialize_accounts() {
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    accounts[i].account_id = i;
    accounts[i].balance = INITIAL_BALANCE;
    accounts[i].transaction_count = 0;

    // Initialize the mutex
    pthread_mutex_init(&accounts[i].lock, NULL);
  }
}

// GIVEN: Example deposit function WITH proper protection
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

// TODO 1: Implement withdrawal_safe() with mutex protection
// Reference: Follow the pattern of deposit_safe() above
// Remember: lock BEFORE accessing data, unlock AFTER
void withdrawal_safe(int account_id, double amount) {
  // YOUR CODE HERE
  // Hint: pthread_mutex_lock
  // Hint: Modify balance
  // Hint: pthread_mutex_unlock
}

// TODO 2: Update teller_thread to use safe functions
// Change: deposit_unsafe -> deposit_safe
// Change: withdrawal_unsafe -> withdrawal_safe

// TODO 3: Add performance timing
// Reference: Section 7.2 "Performance Measurement"
// Hint: Use clock_gettime(CLOCK_MONOTONIC, &start);

// TODO 4: Add mutex cleanup in main()
// Reference: man pthread_mutex_destroy
// Important: Destroy mutexes AFTER all threads complete!
void cleanup_mutexes() {
  for (int i = 0; i < NUM_ACCOUNTS; i++) {
    pthread_mutex_destroy(&accounts[i].lock);
  }
}

// TODO 5: Compare Phase 1 vs Phase 2 performance
// Measure execution time for both versions
// Document the overhead of synchronization
