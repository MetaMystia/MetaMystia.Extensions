# Contributing

1. 新建模块时保持一个玩家向插件对应一个 DLL。
2. 模块之间不得互相引用；共享逻辑放入 AddonKit。
3. 游戏逻辑必须依据实际 Interop 签名和可验证的调用链，不得只按方法名猜测。
4. 纯逻辑和本地化资源必须有自动化测试。
5. 面向用户的文本必须支持五种语言。
6. 缺少 MetaMystia 时不得注册 Hook，并应显示清晰提示。
7. 提交前运行：

   ```powershell
   dotnet build MetaMystia.Extensions.sln
   dotnet test MetaMystia.Extensions.sln --no-build
   ```

8. 为每个模块维护独立的 `CHANGELOG.md`，并在根 README 的模块表中登记模块。
9. 发布时使用 `<Module>-v<Version>` 标签和 `<Module> v<Version>` 标题，每个模块单独创建 Release。
