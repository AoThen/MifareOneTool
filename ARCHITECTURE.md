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

以下工具因源代码不可公开获取或构建依赖已失效，从原版 release 下载：

#### mff08.exe

| 属性 | 说明 |
|------|------|
| **功能** | MFF08 中国克隆卡（UID 可更改卡）操作工具 |
| **命令格式** | `mff08.exe c <type> u "<dump_file>"` |
| **使用场景** | 恢复因写卡导致 0 块损坏的卡片数据 |
| **源代码状态** | ❌ 未找到公开源代码 |
| **获取方式** | 从原版 [MifareOneTool v1.7.0](https://github.com/xcicode/MifareOneTool/releases/download/v1.7.0/M1T-Release.zip) 下载 |

#### collect.exe (libnfc_crypto1_crack)

| 属性 | 说明 |
|------|------|
| **功能** | HardNested Nonce 收集器（使用 libnfc） |
| **命令格式** | `collect.exe <known_key> <known_block> <A\|B> <target_block> <A\|B>` |
| **源代码位置** | [aczid/crypto1_bs](https://github.com/aczid/crypto1_bs) - `libnfc_crypto1_crack.c` |
| **许可证** | GPLv2 |
| **构建依赖** | CraptEV1 + Crapto1（原站 crapto1.netgarage.org 已下线） |
| **依赖替代** | [li0ard/crapto1](https://github.com/li0ard/crapto1) / [nfc-tools/mfcuk](https://github.com/nfc-tools/mfcuk) |
| **当前获取方式** | 从原版 release 下载（因构建依赖缺失） |

**collect.exe 构建说明**：

```bash
# 克隆源码
git clone https://github.com/aczid/crypto1_bs.git
cd crypto1_bs

# 尝试获取依赖（原站已下线，需要寻找替代源）
make get_craptev1  # 失败：crapto1.netgarage.org 无法访问
make get_crapto1   # 失败

# 构建
make
```

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
