//-----------------------------------------------------------------------------
// This code is licensed to you under the terms of the GNU GPL, version 2 or,
// at your option, any later version.
//-----------------------------------------------------------------------------

#ifndef HARDNESTED_BF_CORE_H__
#define HARDNESTED_BF_CORE_H__

#include <stdint.h>
#include "hardnested_bruteforce.h"

typedef enum {
	SIMD_AUTO,
	SIMD_AVX512,
	SIMD_AVX2,
	SIMD_AVX,
	SIMD_SSE2,
	SIMD_MMX,
	SIMD_NONE,
} SIMDExecInstr;
extern void SetSIMDInstr(SIMDExecInstr instr);
extern SIMDExecInstr GetSIMDInstrAuto();

extern const uint64_t crack_states_bitsliced(uint32_t cuid, uint8_t *best_first_bytes, statelist_t *p, uint32_t *keys_found, uint64_t *num_keys_tested, uint32_t nonces_to_bruteforce, uint8_t *bf_test_nonces_2nd_byte, noncelist_t *nonces);
extern void bitslice_test_nonces(uint32_t nonces_to_bruteforce, uint32_t *bf_test_nonces, uint8_t *bf_test_nonce_par);

#endif