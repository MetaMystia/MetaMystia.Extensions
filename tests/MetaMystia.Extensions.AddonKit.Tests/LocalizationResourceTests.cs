// SPDX-License-Identifier: AGPL-3.0-only
using System.Text.Json;
using System.Text.RegularExpressions;

using Xunit;

namespace MetaMystia.Extensions.AddonKit.Tests;

public sealed partial class LocalizationResourceTests
{
    private static readonly HashSet<string> ExpectedKeys =
    [
        "Dismiss",
        "MissingDependencyMessage",
        "MissingDependencyTitle",
        "ProjectLinkDescription"
    ];

    private static readonly string[] LocaleFiles =
    [
        "zh-CN.json",
        "zh-TW.json",
        "en.json",
        "ja.json",
        "ko.json"
    ];

    [Fact]
    public void AllLocalesContainTheSameKeys()
    {
        Dictionary<string, string>[] dictionaries = LoadLocales();

        HashSet<string> expected = dictionaries[0].Keys.ToHashSet();
        foreach (Dictionary<string, string> dictionary in dictionaries.Skip(1))
            Assert.Equal(expected, dictionary.Keys.ToHashSet());
    }

    [Fact]
    public void AllLocalesUseTheSamePlaceholderIndexes()
    {
        Dictionary<string, string>[] dictionaries = LoadLocales();

        foreach (string key in dictionaries[0].Keys)
        {
            int[] expected = PlaceholderIndexes(dictionaries[0][key]);
            foreach (Dictionary<string, string> dictionary in dictionaries.Skip(1))
                Assert.Equal(expected, PlaceholderIndexes(dictionary[key]));
        }
    }

    [Fact]
    public void AllLocalesProvideTheAddonKitMessageKeys()
    {
        foreach (Dictionary<string, string> dictionary in LoadLocales())
            Assert.Equal(ExpectedKeys, dictionary.Keys.ToHashSet());
    }

    private static Dictionary<string, string>[] LoadLocales()
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "Locales");
        return LocaleFiles
            .Select(file => JsonSerializer.Deserialize<Dictionary<string, string>>(
                File.ReadAllText(Path.Combine(directory, file)))!)
            .ToArray();
    }

    [GeneratedRegex(@"\{(\d+)(?:[:][^}]*)?\}")]
    private static partial Regex PlaceholderPattern();

    private static int[] PlaceholderIndexes(string text) =>
        PlaceholderPattern()
            .Matches(text)
            .Select(match => int.Parse(match.Groups[1].Value))
            .Order()
            .ToArray();
}
