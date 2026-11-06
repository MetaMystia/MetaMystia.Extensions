# MetaMystia.Extensions.AddonKit

AddonKit 是扩展模块的共享基础库。它不会被玩家单独安装，而是随功能模块打包。

## 能力

- MetaMystia 软依赖检查；
- BepInEx 与游戏内提示；
- 五种语言加载；
- MetaMystia 命令树注册桥；
- 可嵌入模块的独立程序集边界。

## 依赖检查

```csharp
if (!MetaMystiaIntegration.EnsureAvailable(
        this,
        MyPluginInfo.PLUGIN_NAME,
        new SemanticVersioning.Version(0, 29, 0)))
{
    return;
}
```

不传版本时使用 AddonKit 的最低要求 `0.19.1`。

检查失败时：

- 写入 BepInEx 错误日志；
- 显示游戏内提示层；
- 返回 `false`，调用方必须停止注册 Hook。

## 命令桥

```csharp
var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
CommandRegistrationBridge.Install(
    harmony,
    root => MyCommands.Register(root, this));
```

桥接层只向 MetaMystia 的 `GeneralCommands.Register` 安装一次 postfix。后续模块注册的回调共享同一个命令树。

命令注册函数必须自行保证幂等，避免命令树被重建时出现重复命令。

## 项目链接

```csharp
CommandRegistrationBridge.InstallProjectLink(harmony);
```

桥接层会在 MetaMystia 现有 `/link` 命令下注册 `MetaMystia.Extensions` 子命令，多个模块共享同一次注册。玩家可通过以下命令打开项目仓库：

```text
/link MetaMystia.Extensions
```

## 扩展状态

```csharp
MetaMystiaExtensionStatus.Register(harmony, "MyModule");
```

加载成功的模块注册名称后会复用同一个 MetaMystia 状态栏补丁。左下角状态会在资源包数量后追加扩展数量，例如：

```text
MetaMystia v0.29.5 loaded with 1 rex pack, 1 extension
```

该能力要求 MetaMystia `0.26.7` 或更高版本；使用它的模块必须据此声明最低版本。

## 本地化

```csharp
private static readonly LocalizationCatalog<MyText> Catalog =
    new(() => GameLanguage.Current);

LocalizationLoader.LoadEmbedded(
    Catalog,
    Assembly.GetExecutingAssembly(),
    "MyPlugin.Locales.");
```

`enum` 成员名必须与 JSON 键一致。文本使用 `{0}`、`{1}` 占位符。

## UI 字体

缺少依赖提示会从系统中选择可显示中日韩文本的字体；找不到候选字体时使用 Unity 当前 GUI 字体。

多个模块同时缺少依赖时，会复用同一个提示面板并按模块列出最低版本。

## 版本要求

AddonKit 当前最低要求 MetaMystia `0.19.1`。它依赖：

- `MetaMystia.ConsoleSystem.Commands.GeneralCommands`
- System.CommandLine `2.0.0-beta4.22272.1`
- BepInEx 6 IL2CPP

功能模块如使用更新的 MetaMystia API，应单独声明更高最低版本。
