# MifareOneTool

A GUI Mifare Classic tool on Windows

## 功能特性

- 设备扫描与配置
- Mifare Classic S50 卡片读写
- MFOC/MFCUK/HardNested 密钥破解
- UID/CUID/FUID/UFUID 卡片支持
- MFD 文件十六进制编辑器
- 卡片数据对比工具
- 多语言支持（中文/英文/俄文）

## 截图

<!-- 截图占位，请替换为实际截图 -->
| 主界面 | 高级模式 |
|:------:|:--------:|
| ![主界面](docs/screenshot_main.png) | ![高级模式](docs/screenshot_adv.png) |

| Hex编辑器 | 数据对比 |
|:---------:|:--------:|
| ![Hex编辑器](docs/screenshot_hex.png) | ![数据对比](docs/screenshot_diff.png) |

## 支持的硬件

### NFC 读写器

| 设备 | 状态 | 说明 |
|------|:----:|------|
| PN532 (UART) | ✅ 已支持 | 推荐，稳定可靠 |
| ACR122U | ✅ 已支持 | 需手动切换 DLL（高级模式中启用） |
| ProxMark | ❌ 不支持 | 请使用专用软件 |

### 卡片类型

| 卡片类型 | 读 | 写 | 说明 |
|----------|:--:|:--:|------|
| Mifare Classic S50 (1K) | ✅ | ✅ | 标准卡片 |
| Mifare Classic S70 (4K) | ❌ | ❌ | 不支持 |
| UID 卡 | ✅ | ✅ | 可修改卡号 |
| CUID 卡 | ✅ | ✅ | 后门指令写入 |
| FUID 卡 | ✅ | ✅ | 一次性写入 UID |
| UFUID 卡 | ✅ | ✅ | 可锁定 UID |

## 系统要求

- Windows 7 SP1 或更高版本
- .NET Framework 4.8
- USB 接口（用于 NFC 读写器）

## 快速开始

1. 连接 NFC 读写器到电脑
2. 启动软件，点击「扫描设备」
3. 放置卡片，点击「扫描卡片」
4. 选择需要的操作（读取/写入/破解等）

## 常见问题 (FAQ)

### Q: 扫描设备时没有发现任何设备？

检查以下几点：
- USB 线缆是否正确连接
- 设备电源是否已打开
- 驱动程序是否正确安装（PN532 需要 USB-UART 驱动）
- 尝试以管理员权限运行软件

### Q: 读取卡片时提示失败？

可能原因：
- 卡片已加密，需要先破解密钥
- 卡片与读写器距离太远
- 卡片已损坏

### Q: MFOC 破解失败？

- 确保卡片有已知密钥的扇区（如扇区 0 的默认密钥）
- 尝试使用「自定义密钥」添加已知密钥
- 如果卡片全加密，考虑使用 HardNested 攻击

### Q: 如何写入 CUID 卡？

1. 选择「CUID/FUID 写入」按钮
2. 在设置中勾选「CUID 空卡写入补丁」
3. 选择要写入的 MFD 文件

### Q: 支持 ACR122U 吗？

支持，但需要手动切换：
1. 进入「高级模式」
2. 点击「打开 ACR122U 支持」
3. 切换后可能影响操作速度

### Q: 如何切换语言？

首次启动会自动弹出语言选择，也可以在设置中修改。

## 项目架构

详细架构说明请参阅 [ARCHITECTURE.md](ARCHITECTURE.md)。

## 免责声明

**⚠️ 重要声明**

本工具仅供学习和研究目的使用。请遵守以下规定：

1. **合法使用**：仅对自己拥有的卡片进行操作，或获得明确授权后使用
2. **禁止滥用**：不得用于非法复制门禁卡、公交卡或其他受保护的卡片
3. **法律责任**：使用本工具产生的任何法律责任由使用者自行承担
4. **数据安全**：操作前请备份原始数据，数据丢失概不负责

本项目作者不对因使用本软件而导致的任何直接或间接损失承担责任。

## 许可证

本项目基于 MIT 许可证开源，详见 [LICENSE](LICENSE)。

## 致谢

- [libnfc](https://github.com/nfc-tools/libnfc) - NFC 开发库
- [mfoc](https://github.com/nfc-tools/mfoc) - Mifare Classic 离线攻击工具
- [mfcuk](https://github.com/nfc-tools/mfcuk) - Mifare Classic 密钥恢复工具

---

因作者事务繁忙，软件不再更新。