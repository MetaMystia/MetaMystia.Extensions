// SPDX-License-Identifier: AGPL-3.0-only
using System.Reflection;

using MetaMystia.Extensions.AddonKit;

namespace MetaMystia.Extensions.SprintControl;

internal static class SprintLocalization
{
    private static readonly LocalizationCatalog<SprintText> Catalog = new(() => GameLanguage.Current);

    internal static void Initialize()
    {
        LocalizationLoader.LoadEmbedded(
            Catalog,
            Assembly.GetExecutingAssembly(),
            "MetaMystia.Extensions.SprintControl.Locales.");
    }

    internal static string Get(AddonLanguage language, SprintText textId, params object[] args) =>
        Catalog.Get(language, textId, args);

    internal static string Get(SprintText textId, params object[] args) =>
        Catalog.Get(textId, args);
}
