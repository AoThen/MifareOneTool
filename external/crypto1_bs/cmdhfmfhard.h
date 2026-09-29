//-----------------------------------------------------------------------------
// This code is licensed to you under the terms of the GNU GPL, version 2 or,
// at your option, any later version.
//-----------------------------------------------------------------------------

#ifndef CMDHFMFHARD_H__
#define CMDHFMFHARD_H__

#include <stdint.h>
#include <stdbool.h>

#define NUM_SUMS 						19

typedef struct guess_sum_a8 {
	float prob;
	uint64_t num_states;
	uint8_t sum_a8_idx;
} guess_sum_a8_t;

typedef struct noncelistentry {
	uint32_t nonce_enc;
	uint8_t par_enc;
	void *next;
} noncelistentry_t;

typedef struct noncelist {
	uint16_t num;
	uint16_t Sum;
	guess_sum_a8_t sum_a8_guess[NUM_SUMS];
	bool sum_a8_guess_dirty;
	float expected_num_brute_force;
	uint8_t BitFlips[0x400];
	uint32_t *states_bitarray[2];
	uint32_t num_states_bitarray[2];
	bool all_bitflips_dirty[2];
	noncelistentry_t *first;
} noncelist_t;

#endif