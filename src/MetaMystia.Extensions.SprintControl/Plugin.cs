// SPDX-License-Identifier: AGPL-3.0-only
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

using MetaMystia.Extensions.AddonKit;

namespace MetaMystia.Extensions.SprintControl;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("MetaMystia", BepInDependency.DependencyFlags.SoftDependency)]
public sealed class Plugin : BasePlugin
{
    private static readonly SemanticVersioning.Version MinimumMetaMystiaVersion = new(0, 29, 0);

    internal static Plugin Instance { get; private set; } = null!;

    private ConfigEntry<float> _sprintMultiplier = null!;

    internal float SprintMultiplier => SprintMath.IsValidMultiplier(_sprintMultiplier.Value)
        ? _sprintMultiplier.Value
        : SprintMath.DefaultMultiplier;

    private bool? _gameVersionSupported;

    public override void Load()
    {
        Instance = this;
        SprintLocalization.Initialize();
        _sprintMultiplier = Config.Bind(
            "SpeedUp",
            "Multiplier",
            SprintMath.DefaultMultiplier,
            new ConfigDescription(
                SprintLocalization.Get(
                    AddonLanguage.English,
                    SprintText.ConfigMultiplierDescription),
                new AcceptableValueRange<float>(SprintMath.MinMultiplier, SprintMath.MaxMultiplier)));

        if (!SprintMath.IsValidMultiplier(_sprintMultiplier.Value))
        {
            Log.LogWarning(
                $"Invalid sprint multiplier {_sprintMultiplier.Value}; restoring {SprintMath.DefaultMultiplier}.");
            _sprintMultiplier.Value = SprintMath.DefaultMultiplier;
        }

        if (!MetaMystiaIntegration.EnsureAvailable(
                this,
                MyPluginInfo.PLUGIN_NAME,
                MinimumMetaMystiaVersion))
        {
            return;
        }

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        CommandRegistrationBridge.Install(
            harmony,
            root => SprintCommand.Register(root, this));
        CommandRegistrationBridge.InstallProjectLink(harmony);
        MetaMystiaExtensionStatus.Register(harmony, "SprintControl");
        harmony.PatchAll(typeof(Plugin).Assembly);
        Log.LogInfo($"Sprint multiplier: {SprintMath.FormatMultiplier(SprintMultiplier)}");
    }

    internal bool EnsureGameVersionSupported()
    {
        if (_gameVersionSupported.HasValue)
            return _gameVersionSupported.Value;

        string? actualVersion = Common.LoadingSceneManager.VersionData;
        if (string.IsNullOrEmpty(actualVersion))
            return false;

        bool supported = GameVersionGuard.IsSupported(actualVersion);
        _gameVersionSupported = supported;
        if (supported)
            return true;

        string message = SprintLocalization.Get(
            SprintText.UnsupportedGameVersion,
            actualVersion,
            GameVersionGuard.SupportedVersion);
        Log.LogError(message);
        MetaMystia.UI.InGameConsole.LogError(message);

        return false;
    }

    internal bool TrySetSprintMultiplier(float multiplier)
    {
        if (!SprintMath.IsValidMultiplier(multiplier))
            throw new ArgumentOutOfRangeException(nameof(multiplier));

        float previous = _sprintMultiplier.Value;
        _sprintMultiplier.Value = multiplier;
        try
        {
            Config.Save();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            _sprintMultiplier.Value = previous;
            Log.LogError($"Failed to save sprint configuration: {exception.Message}");
            return false;
        }

        SprintRuntimeController.RefreshActiveMultiplier();
        return true;
    }

    internal bool TryResetSprintMultiplier() =>
        TrySetSprintMultiplier(SprintMath.DefaultMultiplier);
}
