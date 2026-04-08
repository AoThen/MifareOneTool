# 项目架构说明

## 项目概述

MifareOneTool 是一款 Windows 平台下的 Mifare Classic 卡片操作工具，提供图形化界面，基于 libnfc 库实现对 NFC 设备的控制。

## 技术栈

- **框架**: .NET Framework 4.8 / Windows Forms
- **语言**: C#
- **外部依赖**: libnfc、mfoc、mfcuk 等命令行工具
- **JSON 解析**: Newtonsoft.Json

## 目录结构

```
MifareOneTool/
├── MifareOneTool.sln          # Visual Studio 解决方案
├── external/                  # 外部工具源码 (Git Submodules)
│   ├── libnfc/                # libnfc 核心 + 工具
│   ├── crypto1_bs/            # collect.exe (nonce 收集器)
│   ├── mfoc/                  # MFOC 暗文攻击
│   ├── mfcuk/                 # MFCUK 暴力破解
│   └── build-tools.sh         # 统一构建脚本
└── MifareOneTool/             # 主项目目录
    ├── Program.cs             # 应用程序入口
    ├── Form1.cs               # 主窗体（核心业务逻辑）
    ├── Form1.Designer.cs      # 主窗体设计器
    ├── ClassMifareS50.cs      # M1卡数据模型与工具类
    ├── SelectLanguage.cs      # 语言选择窗体
    ├── FormDiff.cs            # 卡片数据对比工具
    ├── FormHardNes.cs         # HardNested 攻击配置窗体
    ├── FormHTool.cs           # 十六进制编辑器窗体
    ├── FormMFF08.cs           # MFF08 卡片操作窗体
    ├── Properties/            # 项目属性与资源
    │   ├── Resources.*.resx   # 多语言资源文件（中/英/俄）
    │   └── Settings.settings  # 用户设置
    └── Resources/             # 图标与图片资源
```

## 核心模块

### 1. 数据模型层 (`ClassMifareS50.cs`)

负责 Mifare Classic S50 卡片的数据表示与操作：

```
┌─────────────────────────────────────────────────────┐
│                    S50 (整卡)                        │
│  ┌─────────────────────────────────────────────┐   │
│  │ Sector 0 (厂商扇区)                          │   │
│  │   Block 0: UID + BCC + 厂商数据              │   │
│  │   Block 1-2: 数据块                          │   │
│  │   Block 3: KeyA + AC + KeyB (扇区尾)         │   │
│  └─────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────┐   │
│  │ Sector 1-15 (数据扇区)                       │   │
│  │   Block 0-2: 数据块                          │   │
│  │   Block 3: KeyA + AC + KeyB (扇区尾)         │   │
│  └─────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

**关键类**：

| 类名 | 职责 |
|------|------|
| `Utils` | 十六进制转换、访问控制位编解码 |
| `Sector` | 单个扇区（4块×16字节）的数据封装 |
| `S50` | 整张卡片（16扇区）的数据封装与文件操作 |

**主要功能**：
- 卡片数据的内存表示
- MFD/MCT 格式的导入导出
- 访问控制位（AC Bits）的解析与生成
- 卡片数据校验（BCC、AC 有效性）

### 2. 业务逻辑层 (`Form1.cs`)

主窗体，包含所有 NFC 操作的业务逻辑：

```
┌──────────────────────────────────────────────────────────────┐
│                         Form1                                 │
├──────────────────────────────────────────────────────────────┤
│  设备管理                                                     │
│    ├── 设备扫描 (nfc-scan-device)                            │
│    ├── 设备配置 (libnfc.conf)                                │
│    └── 卡片检测 (nfc-list)                                   │
├──────────────────────────────────────────────────────────────┤
│  卡片读取                                                     │
│    ├── 普通读取 (nfc-mfclassic r)                            │
│    ├── UID卡读取 (nfc-mfclassic R)                           │
│    └── 密钥文件加载                                          │
├──────────────────────────────────────────────────────────────┤
│  卡片写入                                                     │
│    ├── 普通写入 (nfc-mfclassic w)                            │
│    ├── UID卡写入 (nfc-mfclassic W)                           │
│    └── CUID/FUID写入 (nfc-mfclassic c)                       │
├──────────────────────────────────────────────────────────────┤
│  密钥破解                                                     │
│    ├── MFOC 暗文攻击 (mfoc)                                  │
│    ├── MFCUK 暴力破解 (mfcuk)                                │
│    ├── HardNested 攻击 (libnfc_hardnested)                   │
│    └── 字典攻击                                              │
├──────────────────────────────────────────────────────────────┤
│  UID卡操作                                                    │
│    ├── UID 重置 (nfc-mfsetuid)                               │
│    ├── UID 写入                                              │
│    ├── 卡片格式化                                            │
│    └── UFUID 锁定                                            │
└──────────────────────────────────────────────────────────────┘
```

### 3. 工具窗体

| 窗体 | 功能 |
|------|------|
| `FormDiff` | 两张卡片数据的差异对比 |
| `FormHTool` | MFD 文件的十六进制编辑器 |
| `FormHardNes` | HardNested 攻击参数配置 |
| `FormMFF08` | MFF08 特殊卡片操作 |
| `SelectLanguage` | 语言选择界面 |

### 4. 外部工具调用

程序通过 `System.Diagnostics.Process` 调用 `nfc-bin/` 目录下的命令行工具：

| 工具 | 用途 |
|------|------|
| `nfc-scan-device.exe` | 扫描 NFC 设备 |
| `nfc-list.exe` | 检测卡片信息 |
| `nfc-mfclassic.exe` | M1卡读写操作 |
| `nfc-mfsetuid.exe` | UID卡号设置 |
| `mfoc.exe` | MFOC 密钥破解 |
| `mfcuk.exe` | MFCUK 暴力破解 |
| `libnfc_hardnested.exe` | HardNested 攻击 |
| `mfdetect.exe` | 卡片加密检测 |

### 5. 专有工具来源说明

#### collect.exe (libnfc_crypto1_crack)

| 属性 | 说明 |
|------|------|
| **功能** | HardNested Nonce 收集器（使用 libnfc） |
| **命令格式** | `collect.exe <known_key> <known_block> <A\|B> <target_block> <A\|B>` |
| **源代码位置** | [aczid/crypto1_bs](https://github.com/aczid/crypto1_bs) - `libnfc_crypto1_crack.c` |
| **许可证** | GPLv2 |
| **构建依赖** | CraptEV1 + Crapto1（从 [SnoopyTools/acr122uNFC](https://github.com/SnoopyTools/acr122uNFC) 获取） |
| **CI 构建状态** | ✅ 从源码编译 |

**collect.exe 构建步骤**：

```bash
# 1. 克隆源码
git clone https://github.com/aczid/crypto1_bs.git
cd crypto1_bs

# 2. 下载依赖（从 GitHub mirror）
curl -sL -o craptev1-v1.1.tar.xz "https://github.com/SnoopyTools/acr122uNFC/raw/master/craptev1-v1.1.tar.xz"
curl -sL -o crapto1-v3.3.tar.xz "https://github.com/SnoopyTools/acr122uNFC/raw/master/crapto1-v3.3.tar.xz"
tar Jxvf craptev1-v1.1.tar.xz
mkdir -p crapto1-v3.3
tar Jxvf crapto1-v3.3.tar.xz -C crapto1-v3.3

# 3. 编译（Windows x64）
x86_64-w64-mingw32-gcc -std=gnu99 -O3 libnfc_crypto1_crack.c \
  crypto1_bs.c crypto1_bs_crack.c \
  crapto1-v3.3/crapto1.c crapto1-v3.3/crypto1.c \
  craptev1-v1.1/craptev1.c \
  -I crapto1-v3.3/ -I craptev1-v1.1/ \
  -static -o collect.exe -lpthread -lnfc -lm \
  -Wl,--allow-multiple-definition
```

#### mff08.exe → nfc-mfclassic.exe

mff08.exe 已被 `nfc-mfclassic.exe` 完全替代，两者功能完全相同：

| 功能 | mff08.exe | nfc-mfclassic.exe |
|------|:---------:|:-----------------:|
| 格式化 | `f` | `f` |
| 普通读取 | `r` | `r` |
| 解锁读取 | `R` | `R` |
| 普通写入 | `w` | `w` |
| 解锁写入 | `W` | `W` |
| A/B 密钥 | `a\|b\|A\|B` | `a\|b\|A\|B` |
| UID 选择 | `u\|U<uid>` | `u\|U<uid>` |

**解锁功能说明**：
- `R` - 解锁读取：绕过认证，直接读取 Key A 和 Key B
- `W` - 解锁写入：允许覆盖 block 0（包括 UID）
- 仅适用于中国克隆卡（Chinese clones / Magic Cards）

**为何不能使用 mfoc-hardnested -F 替代 collect.exe**：

| 对比项 | collect.exe | mfoc-hardnested -F |
|--------|:-----------:|:------------------:|
| 参数格式 | `<key> <blk> <A\|B> <blk> <A\|B>` | `-F -f keys.txt -O out.mfd` |
| 指定目标扇区 | ✅ | ❌ 自动选择 |
| 只收集 nonces | ✅ | ❌ 完整攻击流程 |
| 输出 nonces 文件 | ✅ `.txt` | ❌ |
| 稳定性 | ✅ | ❌ [Issue #26](https://github.com/nfc-tools/mfoc-hardnested/issues/26)：只收集 1 个 nonce 就卡住 |

## 数据流

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   NFC 设备   │ ←→  │  libnfc工具  │ ←→  │   Form1     │
│  (PN532等)  │     │ (命令行程序) │     │  (业务逻辑)  │
└─────────────┘     └─────────────┘     └─────────────┘
                                               ↓
                        ┌─────────────────────────────────┐
                        │           数据模型层             │
                        │  ┌───────┐  ┌───────┐  ┌─────┐  │
                        │  │  S50  │  │Sector │  │Utils│  │
                        │  └───────┘  └───────┘  └─────┘  │
                        └─────────────────────────────────┘
                                               ↓
                        ┌─────────────────────────────────┐
                        │           文件系统               │
                        │   MFD / MCT / Key Dictionary    │
                        └─────────────────────────────────┘
```

## 国际化

项目支持三种语言：
- 中文 (`*.zh.resx`)
- 英文 (`*.resx` 默认)
- 俄文 (`*.ru.resx`)

语言设置保存在 `Properties.Settings.Default.Language`，启动时通过 `CultureInfo` 应用。

## 外部工具源码集成

项目使用 Git Submodules 直接集成上游源码，实现完全可控的构建流程：

```
external/
├── libnfc/           # libnfc 核心 + 工具
├── crypto1_bs/       # collect.exe (nonce 收集器)
├── mfoc/             # MFOC 暗文攻击
├── mfcuk/            # MFCUK 暴力破解
└── build-tools.sh    # 统一构建脚本
```

| Submodule | 上游仓库 | 构建产物 |
|-----------|----------|----------|
| `libnfc` | [nfc-tools/libnfc](https://github.com/nfc-tools/libnfc) | nfc-*.exe, libnfc.dll |
| `crypto1_bs` | [aczid/crypto1_bs](https://github.com/aczid/crypto1_bs) | collect.exe |
| `mfoc` | [nfc-tools/mfoc](https://github.com/nfc-tools/mfoc) | mfoc.exe |
| `mfcuk` | [nfc-tools/mfcuk](https://github.com/nfc-tools/mfcuk) | mfcuk.exe |

**mfoc-hardnested** 因仓库较大未作为 submodule，CI 中直接克隆。

**使用 Submodule 的优势**：
1. **版本可控** - 锁定特定 commit，避免上游变更导致构建失败
2. **可定制** - 可直接修改源码适配项目需求
3. **离线构建** - 无需每次从网络下载
4. **透明可审计** - 源码集成在仓库中，可审查安全

**初始化 Submodules**：
```bash
git clone --recursive https://github.com/AoThen/MifareOneTool.git
# 或在已克隆仓库中：
git submodule update --init --recursive
```

**本地构建工具**：
```bash
cd external
./build-tools.sh ../MifareOneTool/nfc-bin/
```

## 配置文件

| 文件 | 用途 |
|------|------|
| `libnfc.conf` | libnfc 设备连接配置 |
| `auto_keys/` | 自动保存的密钥文件目录 |
| `app.config` | .NET 应用程序配置 |

## 扩展建议

如需扩展功能，建议：

1. **抽取公共模块**：将 `Form1.cs` 中的进程调用逻辑抽取为独立的 `NfcToolRunner` 类
2. **添加插件机制**：支持第三方工具的动态加载
3. **增加日志系统**：使用 log4net 或 NLog 替代 `richTextBox` 日志
4. **单元测试**：为 `Utils`、`Sector`、`S50` 添加测试覆盖
