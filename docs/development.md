# 开发说明

## 目标

本仓库维护多个可独立发布的 MetaMystia 扩展。插件之间共享基础设施，但每个玩家向模块保持单一 DLL 分发。

## 分层

共享命令桥的实现细节见 [AddonKit 文档](addon-kit.md)。

### MetaMystia.Extensions.AddonKit

共享基础设施：

- MetaMystia 存在性与最低版本检查；
- BepInEx 日志和游戏内缺少依赖提示；
- 五种语言加载与回退；
- MetaMystia System.CommandLine 命令注册桥。

AddonKit 不是 BepInEx 插件。它通过 Costura 嵌入每个功能模块。

AddonKit 是内嵌私有依赖。其程序集身份固定为 `1.0.0.0`，让所有模块在默认加载上下文中复用同一份 AddonKit 静态状态。`Version` 用于记录发布版本，不作为源码接口兼容性的判断依据。

AddonKit 发生变化后，先使用现有模块执行构建、测试和行为审查，再逐个模块判断是否需要修改和发版：

- 纯新增接口且模块未使用：现有模块源码和已发布二进制都可继续使用，不要求跟版。
- 模块使用新增或修改后的接口：重建并测试该模块，按需要发布新版本。
- 共享行为发生变化：检查使用该行为的模块。需要采用新行为时，重建并发布对应模块。
- 接口发生破坏性变化：更新所有直接受影响的模块，并在同一发布批次中一起验证。

每次模块发版时，该模块必须与当时选定的 AddonKit 从同一源码快照构建。多个模块经过共同验证时，仍应分别发布，并在兼容性说明中注明建议搭配的模块版本。未纳入本次发布的模块可以保留原版本，无需为了存在 AddonKit 改动而强制重发。

用户混装不同模块版本时，可能同时存在旧、新内嵌副本；支持范围限定在已验证可共同工作的版本组合。文档和发布说明应明确推荐搭配的模块版本。

### 功能模块

每个功能模块拥有独立：

- `[BepInPlugin]` 元数据；
- MetaMystia 最低版本；
- Harmony 补丁；
- 命令；
- 配置；
- 本地化文件；
- 测试。

模块之间不得直接引用或依赖彼此。

## 分发约定

玩家只安装功能模块 DLL。以下程序集不会单独分发：

- `MetaMystia.Extensions.AddonKit`
- MetaMystia 已有依赖
- BepInEx 已有依赖

模块使用 `FodyWeavers.xml` 中的 Costura 配置嵌入 AddonKit。

## 最低版本

`MetaMystiaIntegration.MinimumMetaMystiaVersion` 表示 AddonKit 基础能力的最低要求：

```text
MetaMystia v0.19.1
```

依据：提交 `86f358a` 引入当前 `GeneralCommands.Register(RootCommand)` 命令体系，首个包含该提交的正式标签为 `v0.19.1`。

功能模块可以声明更高要求。SprintControl 使用：

```text
MetaMystia v0.29.0
```

依据：提交 `d022c17` 重构网络层，引入当前 `Client.SendMotion(Motion)`、`Motion.Speed` 和 `Motion.Sprinting` 语义，首个包含该提交的正式标签为 `v0.29.0`。

新增模块必须根据实际依赖的 MetaMystia API 声明独立最低版本，不能复制 SprintControl 的版本要求。

AddonKit 的基础门槛为 `v0.19.1`，但可选能力有各自门槛。例如扩展状态栏使用 `PluginManager.Label` getter，要求 MetaMystia `v0.26.7` 或更高。

## 新增模块

1. 在 `src/` 下新建 `MetaMystia.Extensions.<Module>` 项目。
2. 引用 `MetaMystia.Extensions.AddonKit`。
3. 添加独立 `[BepInPlugin]` GUID。
4. 将 MetaMystia 声明为软依赖，并在 `Load()` 中调用版本检查。
5. 通过 `CommandRegistrationBridge.Install()` 注册命令。
6. 使用 `LocalizationCatalog<T>` 加载五份内嵌语言文件。
7. 为纯逻辑添加测试。
8. 配置 Costura 嵌入 AddonKit。
9. 设置 `ExtensionModule`、`ExtensionModuleId` 和 `EmbeddedAssemblyName`，复用根目录的版本化单 DLL 构建流程。
10. 在根 README 的模块表中登记。

## 依赖检查

缺少 MetaMystia 时，BepInEx 不会跳过插件本身，因此可以输出提示。模块必须：

1. 调用 `MetaMystiaIntegration.EnsureAvailable()`。
2. 返回 `false` 时立即退出，不注册任何 Harmony 补丁。

不要使用强制 `BepInDependency`，否则 BepInEx 会在加载模块前直接跳过它，无法显示自定义提示。

## 本地化

每个模块维护自己的 `Locales/`：

```text
zh-CN.json
zh-TW.json
en.json
ja.json
ko.json
```

键名与模块的 `enum` 成员一致。未知键和空文本会被忽略。运行时回退顺序：

1. 当前游戏语言
2. 简体中文
3. 英文

测试会检查所有语言键和占位符集合是否一致。

## 构建环境

`MetaMystia.Extensions.local.props` 不提交到 Git。路径示例见根目录的 `.example` 文件。

```powershell
dotnet restore MetaMystia.Extensions.sln
dotnet build MetaMystia.Extensions.sln --no-restore
dotnet test MetaMystia.Extensions.sln --no-build
./build.ps1 -Configuration Release
```

`build.ps1` 使用 `DeployToGame=false` 构建，并将每个模块的最终单 DLL 发布到 `artifacts/<Configuration>/<Module>/`。产物文件名包含版本，例如 `MetaMystia.Extensions.SprintControl-v1.0.0.dll`。

`dotnet build MetaMystia.Extensions.sln -c Release` 会把版本化模块 DLL 直接部署到本地 `BepInEx/plugins`。Debug 构建保留在项目的 `bin/Debug` 目录。多个模块可以共享插件目录。

## 发布

发布单位是功能模块。AddonKit 不作为独立 Release 发布。

- 标签格式：`<Module>-v<Version>`，例如 `SprintControl-v1.0.0`。
- Release 标题：`<Module> v<Version>`。
- Release 目标：实际用于构建该模块 DLL 的提交。
- Release 说明：取自模块自己的 `CHANGELOG.md`，补充游戏、BepInEx 和 MetaMystia 要求。
- Release 附件：该模块的版本化 DLL；需要时附带校验和。
- 多个模块即使来自同一提交，也分别创建标签、标题和 Release。

模块版本独立维护。一个模块发版不要求所有模块使用相同的版本号或同时发布。

## 错误处理边界

- 输入事件和 Unity 对象边界保留初始化状态、空对象和对象身份检查，避免在场景加载期间访问未完成初始化的对象。
- 内嵌语言资源属于构建产物；资源缺失或 JSON 无效时直接失败，由资源测试和构建阶段暴露问题。
- 配置保存属于 IO 边界；保存失败时回滚内存值，并由命令返回明确错误。
- 不过度捕获原生 Hook 中的异常。无法安全处理的错误应保留堆栈并尽快暴露。

## 验证边界

当前自动化测试覆盖：

- 加速倍率的解析与范围校验；
- 游戏版本字符串校验；
- 联机运动编码，包括小于 1 的倍率；
- 多语言选择、回退和资源一致性。

Harmony 方法签名、UI 和游戏内实际效果需要在对应游戏和 MetaMystia 版本中进行运行验证。
