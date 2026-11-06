// SPDX-License-Identifier: AGPL-3.0-only
namespace MetaMystia.Extensions.AddonKit;

public static class GameLanguage
{
    public static AddonLanguage Current =>
        Common.UI.EscapeUtility.EscConfigPannel.CurrentLanguage switch
        {
            GameData.MultiLanguageTextMesh.LoadLanguageType.Chinese => AddonLanguage.ChineseSimplified,
            GameData.MultiLanguageTextMesh.LoadLanguageType.CNT => AddonLanguage.ChineseTraditional,
            GameData.MultiLanguageTextMesh.LoadLanguageType.Japanese => AddonLanguage.Japanese,
            GameData.MultiLanguageTextMesh.LoadLanguageType.Korean => AddonLanguage.Korean,
            _ => AddonLanguage.English
        };
}
