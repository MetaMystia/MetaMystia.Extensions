// SPDX-License-Identifier: AGPL-3.0-only

namespace MetaMystia.Extensions.AddonKit;

internal static class ExtensionStatusFormatting
{
    internal static string Append(string label, int extensionCount) => extensionCount switch
    {
        <= 0 => label,
        1 => $"{label}, 1 extension",
        _ => $"{label}, {extensionCount} extensions"
    };
}
