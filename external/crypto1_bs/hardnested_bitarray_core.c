//-----------------------------------------------------------------------------
// Copyright (C) 2016, 2017 by piwi
// GPL version 2 or later
// Simplified: NOSIMD only, no SIMD dispatch (CI runs on generic x86_64)
//-----------------------------------------------------------------------------

#include "hardnested_bitarray_core.h"

#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#ifndef __APPLE__
#include <malloc.h>
#endif

#define MALLOC_BITARRAY malloc_bitarray_NOSIMD
#define FREE_BITARRAY free_bitarray_NOSIMD
#define BITCOUNT bitcount_NOSIMD
#define COUNT_STATES count_states_NOSIMD
#define BITARRAY_AND bitarray_AND_NOSIMD
#define BITARRAY_LOW20_AND bitarray_low20_AND_NOSIMD
#define COUNT_BITARRAY_AND count_bitarray_AND_NOSIMD
#define COUNT_BITARRAY_LOW20_AND count_bitarray_low20_AND_NOSIMD
#define BITARRAY_AND4 bitarray_AND4_NOSIMD
#define BITARRAY_OR bitarray_OR_NOSIMD
#define COUNT_BITARRAY_AND2 count_bitarray_AND2_NOSIMD
#define COUNT_BITARRAY_AND3 count_bitarray_AND3_NOSIMD
#define COUNT_BITARRAY_AND4 count_bitarray_AND4_NOSIMD

inline uint32_t *MALLOC_BITARRAY(uint32_t x)
{
#ifdef _WIN32
	return (uint32_t *)_aligned_malloc(x, 32);
#else
	return memalign(32, (x));
#endif
}

inline void FREE_BITARRAY(uint32_t *x)
{
#ifdef _WIN32
	_aligned_free((void *)x);
#else
	free(x);
#endif
}

inline uint32_t BITCOUNT(uint32_t a)
{
	return __builtin_popcountl(a);
}

inline uint32_t COUNT_STATES(uint32_t *A)
{
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		count += BITCOUNT(A[i]);
	}
	return count;
}

inline void BITARRAY_AND(uint32_t *restrict A, uint32_t *restrict B)
{
	for (uint32_t i = 0; i < (1<<19); i++) {
		A[i] &= B[i];
	}
}

inline void BITARRAY_LOW20_AND(uint32_t *restrict A, uint32_t *restrict B)
{
	uint16_t *a = (uint16_t *)A;
	uint16_t *b = (uint16_t *)B;
	for (uint32_t i = 0; i < (1<<20); i++) {
		if (!b[i]) {
			a[i] = 0;
		}
	}
}

inline uint32_t COUNT_BITARRAY_AND(uint32_t *restrict A, uint32_t *restrict B)
{
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		A[i] &= B[i];
		count += BITCOUNT(A[i]);
	}
	return count;
}

inline uint32_t COUNT_BITARRAY_LOW20_AND(uint32_t *restrict A, uint32_t *restrict B)
{
	uint16_t *a = (uint16_t *)A;
	uint16_t *b = (uint16_t *)B;
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<20); i++) {
		if (!b[i]) {
			a[i] = 0;
		}
		count += BITCOUNT(a[i]);
	}
	return count;
}

inline void BITARRAY_AND4(uint32_t *restrict A, uint32_t *restrict B, uint32_t *restrict C, uint32_t *restrict D)
{
	for (uint32_t i = 0; i < (1<<19); i++) {
		A[i] = B[i] & C[i] & D[i];
	}
}

inline void BITARRAY_OR(uint32_t *restrict A, uint32_t *restrict B)
{
	for (uint32_t i = 0; i < (1<<19); i++) {
		A[i] |= B[i];
	}
}

inline uint32_t COUNT_BITARRAY_AND2(uint32_t *restrict A, uint32_t *restrict B)
{
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		count += BITCOUNT(A[i] & B[i]);
	}
	return count;
}

inline uint32_t COUNT_BITARRAY_AND3(uint32_t *restrict A, uint32_t *restrict B, uint32_t *restrict C)
{
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		count += BITCOUNT(A[i] & B[i] & C[i]);
	}
	return count;
}

inline uint32_t COUNT_BITARRAY_AND4(uint32_t *restrict A, uint32_t *restrict B, uint32_t *restrict C, uint32_t *restrict D)
{
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		count += BITCOUNT(A[i] & B[i] & C[i] & D[i]);
	}
	return count;
}

uint32_t *malloc_bitarray(uint32_t x) {
#ifdef _WIN32
	return (uint32_t *)_aligned_malloc(x, 32);
#else
	return memalign(32, (x));
#endif
}

void free_bitarray(uint32_t *x) {
#ifdef _WIN32
	_aligned_free((void *)x);
#else
	free(x);
#endif
}

uint32_t bitcount(uint32_t a) {
	return __builtin_popcountl(a);
}

uint32_t count_states(uint32_t *bitarray) {
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		count += bitcount(bitarray[i]);
	}
	return count;
}

void bitarray_AND(uint32_t *A, uint32_t *B) {
	for (uint32_t i = 0; i < (1<<19); i++) {
		A[i] &= B[i];
	}
}

void bitarray_low20_AND(uint32_t *A, uint32_t *B) {
	uint16_t *a = (uint16_t *)A;
	uint16_t *b = (uint16_t *)B;
	for (uint32_t i = 0; i < (1<<20); i++) {
		if (!b[i]) {
			a[i] = 0;
		}
	}
}

uint32_t count_bitarray_AND(uint32_t *A, uint32_t *B) {
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		A[i] &= B[i];
		count += bitcount(A[i]);
	}
	return count;
}

uint32_t count_bitarray_low20_AND(uint32_t *A, uint32_t *B) {
	uint16_t *a = (uint16_t *)A;
	uint16_t *b = (uint16_t *)B;
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<20); i++) {
		if (!b[i]) {
			a[i] = 0;
		}
		count += bitcount(a[i]);
	}
	return count;
}

void bitarray_AND4(uint32_t *A, uint32_t *B, uint32_t *C, uint32_t *D) {
	for (uint32_t i = 0; i < (1<<19); i++) {
		A[i] = B[i] & C[i] & D[i];
	}
}

void bitarray_OR(uint32_t *A, uint32_t *B) {
	for (uint32_t i = 0; i < (1<<19); i++) {
		A[i] |= B[i];
	}
}

uint32_t count_bitarray_AND2(uint32_t *A, uint32_t *B) {
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		count += bitcount(A[i] & B[i]);
	}
	return count;
}

uint32_t count_bitarray_AND3(uint32_t *A, uint32_t *B, uint32_t *C) {
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		count += bitcount(A[i] & B[i] & C[i]);
	}
	return count;
}

uint32_t count_bitarray_AND4(uint32_t *A, uint32_t *B, uint32_t *C, uint32_t *D) {
	uint32_t count = 0;
	for (uint32_t i = 0; i < (1<<19); i++) {
		count += bitcount(A[i] & B[i] & C[i] & D[i]);
	}
	return count;
}