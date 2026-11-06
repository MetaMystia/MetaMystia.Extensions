# MetaMystia.Extensions

《东方夜雀食堂》[MetaMystia](https://github.com/MetaMystia/MetaMystia) 的独立扩展插件集合。

[English](README.en.md)

## MetaMystia 是什么

MetaMystia 是《东方夜雀食堂》的联机 Mod，使原本单人的经营流程支持多名玩家协作完成白天探索、备菜和夜间营业。

MetaMystia.Extensions 以独立 BepInEx 插件的形式在 MetaMystia 基础上增加可选功能。使用前请先安装正确版本的 MetaMystia。

## 可用模块

| 模块 | 功能 | 最低 MetaMystia 版本 | 安装与使用 |
| --- | --- | --- | --- |
| SprintControl | 调整白天加速走时的加速倍率 | 0.29.0 | [查看模块说明](src/MetaMystia.Extensions.SprintControl/README.md) |

每个模块都可以独立安装和运行。

## 安装流程

1. 确认游戏版本与所用 MetaMystia 版本匹配。当前构建目标为游戏 `4.4.0e`。
2. 安装 BepInEx 6 IL2CPP 和最新版 MetaMystia。
3. 从上方模块表选择一个模块，进入对应说明页。
4. 按模块说明安装其 DLL。
5. 启动游戏，在 BepInEx 日志或 MetaMystia 控制台中确认加载结果。

每个模块只分发自己的一个版本化 DLL，直接放入 `BepInEx/plugins` 即可使用。

## 通用行为

- MetaMystia 缺失或版本过低时，插件会显示安装提示；满足版本要求并重启后，模块功能自动启用。
- 用户界面跟随游戏语言，支持简体中文、繁体中文、英文、日文和韩文。
- 各模块按自己的功能声明最低 MetaMystia 版本。
- 同时使用多个模块时，请按各模块说明选择互相兼容的版本。
- MetaMystia 左下角状态行会显示本次已加载的扩展数量。
- 在 MetaMystia 控制台输入 `/link MetaMystia.Extensions` 可打开本项目仓库。

## 版本说明

| 模块 | 最低 MetaMystia 版本 |
| --- | --- |
| SprintControl | 0.29.0 |

当前源码使用 MetaMystia `0.29.5` 编译。MetaMystia 接口发生变化时，相关模块会同步更新适配。

## 开发

本仓库采用 monorepo：共享基础库、多个模块和测试位于同一个解决方案。新增模块、构建要求和架构说明见：

- [开发说明](docs/development.md)
- [贡献指南](CONTRIBUTING.md)

## 许可

本项目使用 GNU Affero General Public License v3.0，与 MetaMystia 相同。详见 [LICENSE](LICENSE)。

本项目是社区项目，由 MetaMystia.Extensions 贡献者维护。
