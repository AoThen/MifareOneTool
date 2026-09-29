//-----------------------------------------------------------------------------
// This code is licensed to you under the terms of the GNU GPL, version 2 or,
// at your option, any later version.
//-----------------------------------------------------------------------------

#ifndef CMDHFMFHARD_API_H__
#define CMDHFMFHARD_API_H__

#include <stdint.h>
#include <stdbool.h>
#include "cmdhfmfhard.h"

void init_nonce_memory(void);
int add_nonce(uint32_t nonce_enc, uint8_t par_enc);
void free_nonces_memory(void);

void init_bitflip_bitarrays(void);
void free_bitflip_bitarrays(void);

void init_part_sum_bitarrays(void);
void free_part_sum_bitarrays(void);

void init_sum_bitarrays(void);
void free_sum_bitarrays(void);

void check_for_BitFlipProperties(bool time_budget);
void update_allbitflips_array(void);
void update_sum_bitarrays(uint32_t odd_even);
void apply_sum_a0(void);
void estimate_sum_a8(void);
void update_p_K(void);
float sort_best_first_bytes(void);
void get_best_first_bytes(uint8_t **bytes, uint32_t *len);
void get_maximum_states(uint64_t *states);
uint64_t get_found_key(void);
uint64_t get_num_keys_tested(void);
bool shrink_key_space(float *brute_forces);
void update_nonce_data(bool time_budget);

void prepare_bf_test_nonces(noncelist_t *nonces, uint8_t best_first_byte);
void generate_candidates(uint8_t sum_a0_idx, uint8_t sum_a8_idx);
bool brute_force(void);

uint32_t get_cuid(void);
uint32_t get_num_acquired_nonces(void);
uint16_t get_first_byte_Sum(void);
void get_best_sum_a8_idx(uint8_t *sum_a8_idx);
void get_sums_array(uint16_t **sums, uint32_t *num_sums);

#endif