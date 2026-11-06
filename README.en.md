# MetaMystia.Extensions

Independent add-ons for [MetaMystia](https://github.com/MetaMystia/MetaMystia), the multiplayer mod for *Touhou Mystia's Izakaya*.

[简体中文](README.md)

## What is MetaMystia?

MetaMystia turns the game's single-player exploration, preparation, and restaurant business loop into a cooperative multiplayer experience.

MetaMystia.Extensions adds optional features as independent BepInEx plugins. Install the required MetaMystia version first.

## Available modules

| Module | Feature | Minimum MetaMystia | Documentation |
| --- | --- | --- | --- |
| SprintControl | Adjust the daytime speed-up multiplier while moving | 0.29.0 | [Module guide](src/MetaMystia.Extensions.SprintControl/README.en.md) |

Every player-facing module is an independent plugin and runs on its own.

## Installation

1. Verify that the game and MetaMystia versions match. The current build targets game release `4.4.0e`.
2. Install BepInEx 6 IL2CPP and MetaMystia.
3. Choose a module from the table above and follow its documentation.
4. Install the module DLL.
5. Check the BepInEx log or the MetaMystia console after starting the game.

Each module ships as one versioned DLL and works when placed directly in `BepInEx/plugins`.

## Common behavior

- If MetaMystia is missing or too old, the module shows setup guidance. Once the required version is installed and the game restarts, its features activate automatically.
- User-facing text supports Simplified Chinese, Traditional Chinese, English, Japanese, and Korean.
- Each module declares the minimum MetaMystia version required by its features.
- When using multiple modules, choose compatible versions according to each module's guide.
- MetaMystia's bottom-left status line shows the number of loaded extensions.
- Run `/link MetaMystia.Extensions` in the MetaMystia console to open this repository.

## Version boundaries

| Module | Minimum MetaMystia |
| --- | --- |
| SprintControl | 0.29.0 |

The current source builds against MetaMystia `0.29.5`. Modules will be updated alongside relevant MetaMystia interface changes.

## Development

- [Development guide](docs/development.md)
- [Contributing](CONTRIBUTING.md)

## License

GNU Affero General Public License v3.0. See [LICENSE](LICENSE).

This community project is maintained by the MetaMystia.Extensions contributors.
