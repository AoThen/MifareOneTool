//-----------------------------------------------------------------------------
// Copyright (C) 2016, 2017 by piwi
// GPL version 2 or later
// aczid's Copyright notice embedded below (MIT)
//-----------------------------------------------------------------------------

#include "hardnested_bruteforce.h"

#include <inttypes.h>
#include <stdbool.h>
#include <stdio.h>
#include <pthread.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include "cmdhfmfhard.h"
#include "hardnested_bf_core.h"
#include "crapto1.h"
#include "parity.h"

#define NUM_BRUTE_FORCE_THREADS			(4)
#define DEFAULT_BRUTE_FORCE_RATE		(120000000.0)
#define TEST_BENCH_SIZE					(6000)

static uint32_t nonces_to_bruteforce = 0;
static uint32_t bf_test_nonce[256];
static uint8_t bf_test_nonce_2nd_byte[256];
static uint8_t bf_test_nonce_par[256];
static uint32_t bucket_count = 0;
static statelist_t* buckets[128];
static uint32_t keys_found = 0;
static uint64_t num_keys_tested;

uint8_t trailing_zeros(uint8_t byte)
{
	static const uint8_t trailing_zeros_LUT[256] = {
		8, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		4, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		5, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		4, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		6, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		4, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		5, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		4, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		7, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		4, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		5, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		4, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		6, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		4, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		5, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0,
		4, 0, 1, 0, 2, 0, 1, 0,	3, 0, 1, 0, 2, 0, 1, 0
	};
	return trailing_zeros_LUT[byte];
}

bool verify_key(uint32_t cuid, noncelist_t *nonces, uint8_t *best_first_bytes, uint32_t odd, uint32_t even)
{
	struct Crypto1State pcs;
	for (uint16_t test_first_byte = 1; test_first_byte < 256; test_first_byte++) {
		noncelistentry_t *test_nonce = nonces[best_first_bytes[test_first_byte]].first;
		while (test_nonce != NULL) {
			pcs.odd = odd;
			pcs.even = even;
			lfsr_rollback_byte(&pcs, (cuid >> 24) ^ best_first_bytes[0], true);
			for (int8_t byte_pos = 3; byte_pos >= 0; byte_pos--) {
				uint8_t test_par_enc_bit = (test_nonce->par_enc >> byte_pos) & 0x01;
				uint8_t test_byte_enc = (test_nonce->nonce_enc >> (8*byte_pos)) & 0xff;
				uint8_t test_byte_dec = crypto1_byte(&pcs, test_byte_enc, true) ^ test_byte_enc;
				uint8_t ks_par = filter(pcs.odd);
				uint8_t test_par_enc2 = ks_par ^ evenparity8(test_byte_dec);
				if (test_par_enc_bit != test_par_enc2) {
					return false;
				}
			}
			test_nonce = test_nonce->next;
		}
	}
	return true;
}

static void*
crack_states_thread(void* x){
	struct arg {
		bool silent;
		int thread_ID;
		uint32_t cuid;
		uint32_t num_acquired_nonces;
		uint64_t maximum_states;
		noncelist_t *nonces;
		uint8_t* best_first_bytes;
	} *thread_arg;

	thread_arg = (struct arg *)x;
    const int thread_id = thread_arg->thread_ID;
    uint32_t current_bucket = thread_id;
    while(current_bucket < bucket_count){
        statelist_t *bucket = buckets[current_bucket];
        if(bucket){
            const uint64_t key = crack_states_bitsliced(thread_arg->cuid, thread_arg->best_first_bytes, bucket, &keys_found, &num_keys_tested, nonces_to_bruteforce, bf_test_nonce_2nd_byte, thread_arg->nonces);
            if(key != -1){
                __sync_fetch_and_add(&keys_found, 1);
                printf("Brute force phase completed. Key found: %012" PRIx64 "\n", key);
                break;
            } else if(keys_found){
                break;
            } else {
				if (!thread_arg->silent) {
					printf("Brute force phase: %6.02f%%\n", 100.0*(float)num_keys_tested/(float)(thread_arg->maximum_states));
				}
            }
        }
        current_bucket += NUM_BRUTE_FORCE_THREADS;
    }
    return NULL;
}

void prepare_bf_test_nonces(noncelist_t *nonces, uint8_t best_first_byte)
{
	noncelistentry_t *test_nonce = nonces[best_first_byte].first;
	uint32_t i = 0;
	while (test_nonce != NULL) {
		bf_test_nonce[i] = test_nonce->nonce_enc;
		bf_test_nonce_par[i] = test_nonce->par_enc;
		bf_test_nonce_2nd_byte[i] = (test_nonce->nonce_enc >> 16) & 0xff;
		test_nonce = test_nonce->next;
		i++;
	}
	nonces_to_bruteforce = i;

	uint8_t best_4[4] = {0};
	int sum_best = -1;
	for (uint16_t n1 = 0; n1 < nonces_to_bruteforce; n1++) {
		for (uint16_t n2 = 0; n2 < nonces_to_bruteforce; n2++) {
			if (n2 != n1) {
				for (uint16_t n3 = 0; n3 < nonces_to_bruteforce; n3++) {
					if ((n3 != n2 && n3 != n1) || nonces_to_bruteforce < 3) {
						for (uint16_t n4 = 0; n4 < nonces_to_bruteforce; n4++) {
							if ((n4 != n3 && n4 != n2 && n4 != n1) || nonces_to_bruteforce < 4) {
								int sum = nonces_to_bruteforce > 1 ? trailing_zeros(bf_test_nonce_2nd_byte[n1] ^ bf_test_nonce_2nd_byte[n2]) : 0
								          + nonces_to_bruteforce > 2 ? trailing_zeros(bf_test_nonce_2nd_byte[n2] ^ bf_test_nonce_2nd_byte[n3]) : 0
									  + nonces_to_bruteforce > 3 ? trailing_zeros(bf_test_nonce_2nd_byte[n3] ^ bf_test_nonce_2nd_byte[n4]) : 0;
								if (sum > sum_best) {
									sum_best = sum;
									best_4[0] = n1;
									best_4[1] = n2;
									best_4[2] = n3;
									best_4[3] = n4;
								}
							}
						}
					}
				}
			}
		}
	}

	uint32_t bf_test_nonce_temp[4];
	uint8_t bf_test_nonce_par_temp[4];
	uint8_t bf_test_nonce_2nd_byte_temp[4];
	for (uint8_t i = 0; i < 4 && i < nonces_to_bruteforce; i++) {
		bf_test_nonce_temp[i] = bf_test_nonce[best_4[i]];
		bf_test_nonce_par_temp[i] = bf_test_nonce_par[best_4[i]];
		bf_test_nonce_2nd_byte_temp[i] = bf_test_nonce_2nd_byte[best_4[i]];
	}
	for (uint8_t i = 0; i < 4 && i < nonces_to_bruteforce; i++) {
		bf_test_nonce[i] = bf_test_nonce_temp[i];
		bf_test_nonce_par[i] = bf_test_nonce_par_temp[i];
		bf_test_nonce_2nd_byte[i] = bf_test_nonce_2nd_byte_temp[i];
	}
}

bool brute_force_bs(float *bf_rate, statelist_t *candidates, uint32_t cuid, uint32_t num_acquired_nonces, uint64_t maximum_states, noncelist_t *nonces, uint8_t *best_first_bytes)
{
	bool silent = (bf_rate != NULL);
	keys_found = 0;
	num_keys_tested = 0;

	bitslice_test_nonces(nonces_to_bruteforce, bf_test_nonce, bf_test_nonce_par);

	bucket_count = 0;
	for (statelist_t *p = candidates; p != NULL; p = p->next) {
		if (p->states[ODD_STATE] != NULL && p->states[EVEN_STATE] != NULL) {
			buckets[bucket_count] = p;
			bucket_count++;
		}
	}

	uint64_t start_time = clock();

	pthread_t threads[NUM_BRUTE_FORCE_THREADS];
	struct args {
		bool silent;
		int thread_ID;
		uint32_t cuid;
		uint32_t num_acquired_nonces;
		uint64_t maximum_states;
		noncelist_t *nonces;
		uint8_t *best_first_bytes;
	} thread_args[NUM_BRUTE_FORCE_THREADS];

	for(uint32_t i = 0; i < NUM_BRUTE_FORCE_THREADS; i++){
		thread_args[i].thread_ID = i;
		thread_args[i].silent = silent;
		thread_args[i].cuid = cuid;
		thread_args[i].num_acquired_nonces = num_acquired_nonces;
		thread_args[i].maximum_states = maximum_states;
		thread_args[i].nonces = nonces;
		thread_args[i].best_first_bytes = best_first_bytes;
		pthread_create(&threads[i], NULL, crack_states_thread, (void*)&thread_args[i]);
	}
	for(uint32_t i = 0; i < NUM_BRUTE_FORCE_THREADS; i++){
		pthread_join(threads[i], 0);
	}

	uint64_t elapsed_time = clock() - start_time;

	if (bf_rate != NULL) {
		*bf_rate = (float)num_keys_tested / ((float)elapsed_time / CLOCKS_PER_SEC);
	}

	return (keys_found != 0);
}

float brute_force_benchmark()
{
	statelist_t test_candidates[NUM_BRUTE_FORCE_THREADS];

	test_candidates[0].states[ODD_STATE] = malloc((TEST_BENCH_SIZE+1) * sizeof(uint32_t));
	test_candidates[0].states[EVEN_STATE] = malloc((TEST_BENCH_SIZE+1) * sizeof(uint32_t));
	for (uint8_t i = 0; i < NUM_BRUTE_FORCE_THREADS - 1; i++){
		test_candidates[i].next = test_candidates + i + 1;
		test_candidates[i+1].states[ODD_STATE] = test_candidates[0].states[ODD_STATE];
		test_candidates[i+1].states[EVEN_STATE] = test_candidates[0].states[EVEN_STATE];
	}
	test_candidates[NUM_BRUTE_FORCE_THREADS-1].next = NULL;

	for (uint8_t i = 0; i < NUM_BRUTE_FORCE_THREADS; i++) {
		test_candidates[i].len[ODD_STATE] = TEST_BENCH_SIZE;
		test_candidates[i].len[EVEN_STATE] = TEST_BENCH_SIZE;
		test_candidates[i].states[ODD_STATE][TEST_BENCH_SIZE] = -1;
		test_candidates[i].states[EVEN_STATE][TEST_BENCH_SIZE] = -1;
	}

	uint64_t maximum_states = TEST_BENCH_SIZE*TEST_BENCH_SIZE*(uint64_t)NUM_BRUTE_FORCE_THREADS;

	float bf_rate;
	brute_force_bs(&bf_rate, test_candidates, 0, 0, maximum_states, NULL, 0);

	free(test_candidates[0].states[ODD_STATE]);
	free(test_candidates[0].states[EVEN_STATE]);

	return bf_rate;
}