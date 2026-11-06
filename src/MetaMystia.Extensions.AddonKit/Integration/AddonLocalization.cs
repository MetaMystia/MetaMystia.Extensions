// SPDX-License-Identifier: AGPL-3.0-only
using System.Reflection;

namespace MetaMystia.Extensions.AddonKit;

internal enum AddonText
{
    MissingDependencyTitle,
    MissingDependencyMessage,
    Dismiss,
    ProjectLinkDescription
}

internal static class AddonLocalization
{
    private static readonly LocalizationCatalog<AddonText> Catalog = new(() => GameLanguage.Current);
    private static bool _initialized;

    internal static void Initialize()
    {
        if (_initialized)
            return;

        LocalizationLoader.LoadEmbedded(
            Catalog,
            Assembly.GetExecutingAssembly(),
            "MetaMystia.Extensions.AddonKit.Locales.");
        _initialized = true;
    }

    internal static string Get(AddonText textId, params object[] args) =>
        Catalog.Get(textId, args);
}
