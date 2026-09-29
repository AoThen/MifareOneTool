//-----------------------------------------------------------------------------
// This code is licensed to you under the terms of the GNU GPL, version 2 or,
// at your option, any later version.
//-----------------------------------------------------------------------------

#ifndef HARDNESTED_BITARRAY_CORE_H__
#define HARDNESTED_BITARRAY_CORE_H__

#include <stdint.h>

extern uint32_t *malloc_bitarray(uint32_t x);
extern void free_bitarray(uint32_t *x);
extern uint32_t bitcount(uint32_t a);
extern uint32_t count_states(uint32_t *A);
extern void bitarray_AND(uint32_t *A, uint32_t *B);
extern void bitarray_low20_AND(uint32_t *A, uint32_t *B);
extern uint32_t count_bitarray_AND(uint32_t *A, uint32_t *B);
extern uint32_t count_bitarray_low20_AND(uint32_t *A, uint32_t *B);
extern void bitarray_AND4(uint32_t *A, uint32_t *B, uint32_t *C, uint32_t *D);
extern void bitarray_OR(uint32_t *A, uint32_t *B);
extern uint32_t count_bitarray_AND2(uint32_t *A, uint32_t *B);
extern uint32_t count_bitarray_AND3(uint32_t *A, uint32_t *B, uint32_t *C);
extern uint32_t count_bitarray_AND4(uint32_t *A, uint32_t *B, uint32_t *C, uint32_t *D);

#endif