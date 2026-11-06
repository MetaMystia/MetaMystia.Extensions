// SPDX-License-Identifier: AGPL-3.0-only
using BepInEx;
using BepInEx.Unity.IL2CPP;

namespace MetaMystia.Extensions.AddonKit;

public static class MetaMystiaIntegration
{
    public const string PluginGuid = "MetaMystia";
    public static readonly SemanticVersioning.Version MinimumMetaMystiaVersion = new(0, 19, 1);

    public static bool EnsureAvailable(BasePlugin plugin, string addonName) =>
        EnsureAvailable(plugin, addonName, MinimumMetaMystiaVersion);

    public static bool EnsureAvailable(
        BasePlugin plugin,
        string addonName,
        SemanticVersioning.Version minimumVersion)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        if (string.IsNullOrWhiteSpace(addonName))
            throw new ArgumentException("Addon name is required.", nameof(addonName));
        ArgumentNullException.ThrowIfNull(minimumVersion);

        if (TryGetSupported(minimumVersion, out _))
            return true;

        string requiredVersion = minimumVersion.ToString();
        plugin.Log.LogError(
            $"{addonName} requires {PluginGuid} {requiredVersion} or newer. "
            + $"Install or update {PluginGuid}, then restart the game.");
        AddonLocalization.Initialize();
        MissingDependencyNotice.Show(plugin, addonName, PluginGuid, requiredVersion);
        return false;
    }

    public static bool TryGetSupported(
        SemanticVersioning.Version minimumVersion,
        out PluginInfo? plugin)
    {
        ArgumentNullException.ThrowIfNull(minimumVersion);
        return IL2CPPChainloader.Instance.Plugins.TryGetValue(PluginGuid, out plugin)
            && plugin.Metadata.Version >= minimumVersion;
    }
}
