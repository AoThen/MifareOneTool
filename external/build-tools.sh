#!/bin/bash
# MifareOneTool 外部工具统一构建脚本
# 用法: ./build-tools.sh [output_dir]
# 输出目录默认为 ../MifareOneTool/nfc-bin/

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
OUTPUT_DIR="${1:-$SCRIPT_DIR/../MifareOneTool/nfc-bin}"
BUILD_DIR="$SCRIPT_DIR/build"

# 交叉编译工具链
export CC=x86_64-w64-mingw32-gcc
export AR=x86_64-w64-mingw32-ar
export RANLIB=x86_64-w64-mingw32-ranlib

echo "=== MifareOneTool 外部工具构建 ==="
echo "构建目录: $BUILD_DIR"
echo "输出目录: $OUTPUT_DIR"
echo ""

# 创建目录
mkdir -p "$BUILD_DIR" "$OUTPUT_DIR"

# ============================================
# 1. 构建 libnfc (核心库)
# ============================================
build_libnfc() {
    echo "[1/5] 构建 libnfc..."
    cd "$SCRIPT_DIR/libnfc"
    
    if [ ! -f "$BUILD_DIR/libnfc/lib/libnfc.a" ]; then
        mkdir -p "$BUILD_DIR/libnfc"
        cd "$BUILD_DIR/libnfc"
        
        # 配置交叉编译
        cmake "$SCRIPT_DIR/libnfc" \
            -DCMAKE_SYSTEM_NAME=Windows \
            -DCMAKE_C_COMPILER="$CC" \
            -DBUILD_SHARED_LIBS=OFF \
            -DCMAKE_INSTALL_PREFIX="$BUILD_DIR/libnfc/install" \
            -DNFC_DRIVER_PN53X_USB=ON \
            -DNFC_DRIVER_ACR122_PCSC=ON \
            -DNFC_DRIVER_ACR122_USB=ON
        
        make -j$(nproc)
        make install
    fi
    
    echo "libnfc 构建完成"
}

# ============================================
# 2. 构建 libnfc 命令行工具
# ============================================
build_libnfc_tools() {
    echo "[2/5] 构建 libnfc 工具..."
    
    # nfc-scan-device
    $CC -O2 -static \
        "$SCRIPT_DIR/libnfc/utils/nfc-scan-device.c" \
        -I"$BUILD_DIR/libnfc/install/include" \
        -L"$BUILD_DIR/libnfc/install/lib" \
        -lnfc -lusb-1.0 \
        -o "$OUTPUT_DIR/nfc-scan-device.exe"
    
    # nfc-list
    $CC -O2 -static \
        "$SCRIPT_DIR/libnfc/utils/nfc-list.c" \
        -I"$BUILD_DIR/libnfc/install/include" \
        -L"$BUILD_DIR/libnfc/install/lib" \
        -lnfc -lusb-1.0 \
        -o "$OUTPUT_DIR/nfc-list.exe"
    
    # nfc-mfclassic
    $CC -O2 -static \
        "$SCRIPT_DIR/libnfc/utils/nfc-mfclassic.c" \
        -I"$BUILD_DIR/libnfc/install/include" \
        -L"$BUILD_DIR/libnfc/install/lib" \
        -lnfc -lusb-1.0 \
        -o "$OUTPUT_DIR/nfc-mfclassic.exe"
    
    # nfc-mfsetuid
    $CC -O2 -static \
        "$SCRIPT_DIR/libnfc/utils/nfc-mfsetuid.c" \
        -I"$BUILD_DIR/libnfc/install/include" \
        -L"$BUILD_DIR/libnfc/install/lib" \
        -lnfc -lusb-1.0 \
        -o "$OUTPUT_DIR/nfc-mfsetuid.exe"
    
    echo "libnfc 工具构建完成"
}

# ============================================
# 3. 构建 mfoc
# ============================================
build_mfoc() {
    echo "[3/5] 构建 mfoc..."
    cd "$SCRIPT_DIR/mfoc"
    
    mkdir -p "$BUILD_DIR/mfoc"
    cd "$BUILD_DIR/mfoc"
    
    cmake "$SCRIPT_DIR/mfoc" \
        -DCMAKE_SYSTEM_NAME=Windows \
        -DCMAKE_C_COMPILER="$CC" \
        -DCMAKE_FIND_ROOT_PATH="$BUILD_DIR/libnfc/install" \
        -DLIBNFC_INCLUDE_DIR="$BUILD_DIR/libnfc/install/include" \
        -DLIBNFC_LIBRARY="$BUILD_DIR/libnfc/install/lib/libnfc.a"
    
    make -j$(nproc)
    cp mfoc.exe "$OUTPUT_DIR/"
    
    echo "mfoc 构建完成"
}

# ============================================
# 4. 构建 mfcuk
# ============================================
build_mfcuk() {
    echo "[4/5] 构建 mfcuk..."
    cd "$SCRIPT_DIR/mfcuk"
    
    mkdir -p "$BUILD_DIR/mfcuk"
    cd "$BUILD_DIR/mfcuk"
    
    cmake "$SCRIPT_DIR/mfcuk" \
        -DCMAKE_SYSTEM_NAME=Windows \
        -DCMAKE_C_COMPILER="$CC" \
        -DCMAKE_FIND_ROOT_PATH="$BUILD_DIR/libnfc/install" \
        -DLIBNFC_INCLUDE_DIR="$BUILD_DIR/libnfc/install/include" \
        -DLIBNFC_LIBRARY="$BUILD_DIR/libnfc/install/lib/libnfc.a"
    
    make -j$(nproc)
    cp mfcuk.exe "$OUTPUT_DIR/"
    
    echo "mfcuk 构建完成"
}

# ============================================
# 5. 构建 collect.exe (crypto1_bs)
# ============================================
build_collect() {
    echo "[5/5] 构建 collect.exe..."
    cd "$SCRIPT_DIR/crypto1_bs"
    
    # 下载依赖 (crapto1, craptev1)
    if [ ! -d "crapto1-v3.3" ]; then
        curl -sL -o crapto1-v3.3.tar.xz \
            "https://github.com/SnoopyTools/acr122uNFC/raw/master/crapto1-v3.3.tar.xz"
        mkdir -p crapto1-v3.3
        tar Jxvf crapto1-v3.3.tar.xz -C crapto1-v3.3
    fi
    
    if [ ! -d "craptev1-v1.1" ]; then
        curl -sL -o craptev1-v1.1.tar.xz \
            "https://github.com/SnoopyTools/acr122uNFC/raw/master/craptev1-v1.1.tar.xz"
        tar Jxvf craptev1-v1.1.tar.xz
    fi
    
    $CC -std=gnu99 -O3 -march=x86-64 -static \
        libnfc_crypto1_crack.c \
        crypto1_bs.c \
        crypto1_bs_crack.c \
        crapto1-v3.3/crapto1.c \
        crapto1-v3.3/crypto1.c \
        craptev1-v1.1/craptev1.c \
        -I crapto1-v3.3/ \
        -I craptev1-v1.1/ \
        -I"$BUILD_DIR/libnfc/install/include" \
        -L"$BUILD_DIR/libnfc/install/lib" \
        -lnfc -lusb-1.0 -lpthread -lm \
        -Wl,--allow-multiple-definition \
        -o "$OUTPUT_DIR/collect.exe"
    
    echo "collect.exe 构建完成"
}

# ============================================
# 主流程
# ============================================
main() {
    build_libnfc
    build_libnfc_tools
    build_mfoc
    build_mfcuk
    build_collect
    
    echo ""
    echo "=== 构建完成 ==="
    echo "输出文件:"
    ls -la "$OUTPUT_DIR"/*.exe
}

main "$@"
