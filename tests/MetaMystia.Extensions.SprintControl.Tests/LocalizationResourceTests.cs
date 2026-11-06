// SPDX-License-Identifier: AGPL-3.0-only
using System.Text.Json;
using System.Text.RegularExpressions;

using MetaMystia.Extensions.SprintControl;

using Xunit;

namespace MetaMystia.Extensions.SprintControl.Tests;

public sealed partial class LocalizationResourceTests
{
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
        HashSet<string> keys = dictionaries[0].Keys.ToHashSet();

        foreach (string key in keys)
        {
            int[] expected = PlaceholderIndexes(dictionaries[0][key]);
            foreach (Dictionary<string, string> dictionary in dictionaries.Skip(1))
            {
                int[] actual = PlaceholderIndexes(dictionary[key]);
                Assert.Equal(expected, actual);
            }
        }
    }

    [Fact]
    public void AllLocalesDefineEverySprintTextKey()
    {
        HashSet<string> expected = Enum.GetNames<SprintText>().ToHashSet();

        foreach (Dictionary<string, string> dictionary in LoadLocales())
            Assert.Equal(expected, dictionary.Keys.ToHashSet());
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
