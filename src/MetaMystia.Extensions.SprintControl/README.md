# MetaMystia.Extensions.SprintControl

[English](README.en.md)

调整白天加速走时的加速倍率。

## 运行要求

- 游戏：`RELEASE 4.4.0e`
- BepInEx：6 IL2CPP
- MetaMystia：`0.29.0` 或更高

## 安装

将 `MetaMystia.Extensions.SprintControl-v1.0.0.dll` 直接放入：

```text
BepInEx/plugins/
```

MetaMystia 是《东方夜雀食堂》的联机 Mod，本插件只在其基础上增加加速倍率功能。

## 命令

```text
/sprint              查看当前加速倍率
/sprint 2.5          设置加速倍率
/sprint 0.5          加速走速度低于普通移动
/sprint 1            加速走速度等于普通移动
/sprint reset        恢复原版 1.5
```

允许范围为 `0.1～10`，结果实时生效并保存。

## 行为

- 调整白天加速走时的加速倍率。
- 玩家等级、地图效果和其他移动速度修正仍然保留。
- 动画速度会随最终移动速度变化。
- 联机时，使用自定义倍率的玩家安装本插件，插件会通过 MetaMystia 现有运动字段传递速度。其他玩家通常可使用标准 MetaMystia；所有玩家应使用相同的 MetaMystia 版本。
- 游戏版本不匹配时，插件会显示提示，并保持本地速度与联机运动数据不变。
- 打开菜单或控制台后，输入状态切换期间不会把当前倍率写入非加速状态。

## 配置

该文件由插件首次加载时自动创建，用于保存当前加速倍率，使 `/sprint 2.5` 等设置在重启游戏后继续生效。也可以退出游戏后手动修改 `Multiplier`，范围仍为 `0.1～10`。删除该文件后，下次启动会重新生成默认配置。

```text
BepInEx/config/MetaMystia.Extensions.SprintControl.cfg
```

```ini
[SpeedUp]
Multiplier = 1.5
```

配置值出现异常时，插件会记录警告并在运行时恢复为 `1.5`。

## 验证建议

当前版本已通过编译和自动化测试；请在目标游戏与 MetaMystia 版本下确认实际效果。

## 版本记录

见 [SprintControl 变更记录](CHANGELOG.md)。

[返回项目首页](../../README.md)
