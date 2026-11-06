// SPDX-License-Identifier: AGPL-3.0-only
using MetaMystia.Extensions.AddonKit;

using Xunit;

namespace MetaMystia.Extensions.AddonKit.Tests;

public sealed class LocalizationCatalogTests
{
    [Fact]
    public void Get_UsesSelectedLanguage()
    {
        var catalog = new LocalizationCatalog<TestText>();
        catalog.Merge(AddonLanguage.English, new Dictionary<string, string>
        {
            [nameof(TestText.CommandDescription)] = "Configure sprint speed"
        });

        string actual = catalog.Get(AddonLanguage.English, TestText.CommandDescription);

        Assert.Equal("Configure sprint speed", actual);
    }

    [Fact]
    public void Get_FallsBackToSimplifiedChinese()
    {
        var catalog = new LocalizationCatalog<TestText>();
        catalog.Merge(AddonLanguage.ChineseSimplified, new Dictionary<string, string>
        {
            [nameof(TestText.CommandDescription)] = "设置冲刺速度"
        });

        string actual = catalog.Get(AddonLanguage.Japanese, TestText.CommandDescription);

        Assert.Equal("设置冲刺速度", actual);
    }

    [Fact]
    public void Get_FallsBackToEnglishWhenSimplifiedChineseIsMissing()
    {
        var catalog = new LocalizationCatalog<TestText>();
        catalog.Merge(AddonLanguage.English, new Dictionary<string, string>
        {
            [nameof(TestText.CommandDescription)] = "Configure sprint speed"
        });

        string actual = catalog.Get(AddonLanguage.Korean, TestText.CommandDescription);

        Assert.Equal("Configure sprint speed", actual);
    }

    [Fact]
    public void Get_ReportsMissingKey()
    {
        var catalog = new LocalizationCatalog<TestText>();

        string actual = catalog.Get(AddonLanguage.English, TestText.SetSuccess, 2.5f);

        Assert.Equal("[L10N_MISSING:SetSuccess]", actual);
    }

    private enum TestText
    {
        CommandDescription,
        SetSuccess
    }
}
