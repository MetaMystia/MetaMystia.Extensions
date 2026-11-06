# MetaMystia.Extensions.SprintControl

[简体中文](README.md)

Adjust the daytime speed-up multiplier while moving.

## Requirements

- Game: `RELEASE 4.4.0e`
- BepInEx: 6 IL2CPP
- MetaMystia: `0.29.0` or newer

## Installation

Place `MetaMystia.Extensions.SprintControl-v1.0.0.dll` directly in:

```text
BepInEx/plugins/
```

MetaMystia is the multiplayer mod for *Touhou Mystia's Izakaya*. This plugin adds an optional speed-up multiplier on top of it.

## Commands

```text
/sprint              Show the current speed-up multiplier
/sprint 2.5          Set the speed-up multiplier
/sprint 0.5          Make speed-up slower than normal movement
/sprint 1            Make speed-up equal to normal movement
/sprint reset        Restore the original value, 1.5
```

The accepted range is `0.1` to `10`. Changes apply immediately and persist across restarts.

## Behavior

- Adjusts the speed-up multiplier used during daytime movement.
- Preserves player level, map effects, and other movement modifiers.
- Animation speed follows the resulting movement speed.
- In multiplayer, the player using a custom multiplier installs the plugin. Other players should generally be able to use standard MetaMystia. All players should use the same MetaMystia version.
- If the game version does not match, the plugin shows a notice and leaves local movement and network motion data unchanged.
- While menus or the console are open, the plugin avoids writing the multiplier into a non-sprinting state.

## Configuration

The plugin creates this file on first load and stores the current speed-up multiplier so values such as `/sprint 2.5` persist across restarts. You can also edit `Multiplier` while the game is closed. Deleting the file recreates the default configuration on the next launch.

```text
BepInEx/config/MetaMystia.Extensions.SprintControl.cfg
```

```ini
[SpeedUp]
Multiplier = 1.5
```

If the stored value is invalid, the plugin logs a warning and restores `1.5` at runtime.

## Verification

The current version passes compilation and automated tests. Verify the behavior with the target game and MetaMystia versions.

## Release history

See the [SprintControl changelog](CHANGELOG.md).

[Back to project home](../../README.en.md)
