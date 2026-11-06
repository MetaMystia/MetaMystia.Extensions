// SPDX-License-Identifier: AGPL-3.0-only

using System.Reflection;

using HarmonyLib;

using MetaMystiaPluginManager = MetaMystia.PluginManager;

namespace MetaMystia.Extensions.AddonKit;

public static class MetaMystiaExtensionStatus
{
    private static readonly object Gate = new();
    private static readonly HashSet<string> ExtensionNames = new(StringComparer.Ordinal);
    private static bool _patched;

    public static void Register(Harmony harmony, string extensionName)
    {
        ArgumentNullException.ThrowIfNull(harmony);
        if (string.IsNullOrWhiteSpace(extensionName))
            throw new ArgumentException("Extension name is required.", nameof(extensionName));

        lock (Gate)
        {
            if (!_patched)
            {
                MethodInfo getter = AccessTools.PropertyGetter(
                    typeof(MetaMystiaPluginManager),
                    nameof(MetaMystiaPluginManager.Label))
                    ?? throw new MissingMethodException(
                        typeof(MetaMystiaPluginManager).FullName,
                        $"get_{nameof(MetaMystiaPluginManager.Label)}");
                MethodInfo postfix = AccessTools.Method(
                    typeof(MetaMystiaExtensionStatus),
                    nameof(Label_Postfix))
                    ?? throw new MissingMethodException(
                        typeof(MetaMystiaExtensionStatus).FullName,
                        nameof(Label_Postfix));

                harmony.Patch(getter, postfix: new HarmonyMethod(postfix));
                _patched = true;
            }

            ExtensionNames.Add(extensionName);
        }
    }

    private static void Label_Postfix(ref string __result)
    {
        int extensionCount;
        lock (Gate)
            extensionCount = ExtensionNames.Count;

        __result = ExtensionStatusFormatting.Append(__result, extensionCount);
    }
}
