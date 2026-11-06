// SPDX-License-Identifier: AGPL-3.0-only

namespace MetaMystia.Extensions.SprintControl;

internal static class GameVersionGuard
{
    internal const string SupportedVersion = "RELEASE 4.4.0e";

    internal static bool IsSupported(string? version) =>
        string.Equals(version, SupportedVersion, StringComparison.Ordinal);
}
