//-----------------------------------------------------------------------------
// This code is licensed to you under the terms of the GNU GPL, version 2 or,
// at your option, any later version.
//-----------------------------------------------------------------------------

#ifndef HARDNESTED_BRUTEFORCE_H__
#define HARDNESTED_BRUTEFORCE_H__

#include <stdint.h>
#include <stdbool.h>
#include "cmdhfmfhard.h"

typedef enum {
	EVEN_STATE = 0,
	ODD_STATE = 1
} odd_even_t;

typedef struct {
	uint32_t *states[2];
	uint32_t len[2];
	void* next;
} statelist_t;

extern void prepare_bf_test_nonces(noncelist_t *nonces, uint8_t best_first_byte);
extern bool brute_force_bs(float *bf_rate, statelist_t *candidates, uint32_t cuid, uint32_t num_acquired_nonces, uint64_t maximum_states, noncelist_t *nonces, uint8_t *best_first_bytes);
extern float brute_force_benchmark(void);
extern uint8_t trailing_zeros(uint8_t byte);
extern bool verify_key(uint32_t cuid, noncelist_t *nonces, uint8_t *best_first_bytes, uint32_t odd, uint32_t even);

#endif