// SPDX-License-Identifier: AGPL-3.0-only
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace MetaMystia.Extensions.AddonKit;

public enum AddonLanguage
{
    ChineseSimplified,
    ChineseTraditional,
    English,
    Japanese,
    Korean
}

public sealed class LocalizationCatalog<TText> where TText : struct, Enum
{
    private readonly Dictionary<TText, Dictionary<AddonLanguage, string>> _table = [];
    private readonly Func<AddonLanguage> _currentLanguage;

    public LocalizationCatalog(Func<AddonLanguage>? currentLanguage = null)
    {
        _currentLanguage = currentLanguage ?? (() => AddonLanguage.English);
    }

    public void Merge(AddonLanguage language, IReadOnlyDictionary<string, string> entries)
    {
        foreach ((string key, string text) in entries)
        {
            if (!Enum.TryParse(key, out TText textId)
                || !Enum.IsDefined(textId)
                || string.IsNullOrEmpty(text))
            {
                continue;
            }

            if (!_table.TryGetValue(textId, out var translations))
            {
                translations = [];
                _table[textId] = translations;
            }

            translations[language] = text;
        }
    }

    public string Get(TText textId, params object[] args) =>
        Get(_currentLanguage(), textId, args);

    public string Get(AddonLanguage language, TText textId, params object[] args)
    {
        if (!_table.TryGetValue(textId, out var translations))
            return Missing(textId);

        if (!translations.TryGetValue(language, out string? text) || string.IsNullOrEmpty(text))
            translations.TryGetValue(AddonLanguage.ChineseSimplified, out text);
        if (string.IsNullOrEmpty(text))
            translations.TryGetValue(AddonLanguage.English, out text);
        if (string.IsNullOrEmpty(text))
            return Missing(textId);

        return args.Length == 0 ? text : string.Format(text, args);
    }

    private static string Missing(TText textId) => $"[L10N_MISSING:{textId}]";
}

public static class LocalizationLoader
{
    private static readonly (AddonLanguage Language, string Code)[] Locales =
    [
        (AddonLanguage.ChineseSimplified, "zh-CN"),
        (AddonLanguage.ChineseTraditional, "zh-TW"),
        (AddonLanguage.English, "en"),
        (AddonLanguage.Japanese, "ja"),
        (AddonLanguage.Korean, "ko")
    ];

    public static void LoadEmbedded<TText>(
        LocalizationCatalog<TText> catalog,
        Assembly assembly,
        string resourcePrefix)
        where TText : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(assembly);
        if (string.IsNullOrWhiteSpace(resourcePrefix))
            throw new ArgumentException("Resource prefix is required.", nameof(resourcePrefix));

        foreach ((AddonLanguage language, string code) in Locales)
        {
            string resourceName = $"{resourcePrefix}{code}.json";
            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Localization resource not found: {resourceName}");
            using var reader = new StreamReader(stream, Encoding.UTF8);
            Dictionary<string, string> entries =
                JsonSerializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd())
                ?? throw new InvalidDataException($"Localization resource is empty: {resourceName}");
            catalog.Merge(language, entries);
        }
    }
}
