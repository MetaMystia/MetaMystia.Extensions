// SPDX-License-Identifier: AGPL-3.0-only
using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine;

namespace MetaMystia.Extensions.AddonKit;

public sealed class MissingDependencyNotice : MonoBehaviour
{
    private GUIStyle? _titleStyle;
    private GUIStyle? _messageStyle;
    private GUIStyle? _buttonStyle;

    internal static void Show(
        BasePlugin plugin,
        string addonName,
        string dependencyName,
        string requiredVersion)
    {
        MissingDependencyNoticeState.Instance ??= plugin.AddComponent<MissingDependencyNotice>();
        MissingDependencyNoticeState.Requirements[addonName] = (dependencyName, requiredVersion);
    }

    private void OnGUI()
    {
        InitializeStyles();

        float width = Mathf.Min(680f, Screen.width - 32f);
        float height = 126f + MissingDependencyNoticeState.Requirements.Count * 28f;
        var panel = new Rect((Screen.width - width) / 2f, 24f, width, height);
        GUI.Box(panel, GUIContent.none);
        GUI.Label(
            new Rect(panel.x + 18f, panel.y + 14f, panel.width - 36f, 28f),
            AddonLocalization.Get(AddonText.MissingDependencyTitle),
            _titleStyle);

        float lineY = panel.y + 48f;
        foreach ((string addonName, (string dependencyName, string requiredVersion))
                 in MissingDependencyNoticeState.Requirements)
        {
            GUI.Label(
                new Rect(panel.x + 18f, lineY, panel.width - 36f, 26f),
                AddonLocalization.Get(
                    AddonText.MissingDependencyMessage,
                    addonName,
                    dependencyName,
                    requiredVersion),
                _messageStyle);
            lineY += 28f;
        }

        if (GUI.Button(
                new Rect(panel.x + panel.width - 124f, panel.y + height - 42f, 106f, 28f),
                AddonLocalization.Get(AddonText.Dismiss),
                _buttonStyle))
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (ReferenceEquals(MissingDependencyNoticeState.Instance, this))
        {
            MissingDependencyNoticeState.Instance = null;
            MissingDependencyNoticeState.Requirements.Clear();
        }
    }

    private void InitializeStyles()
    {
        if (_titleStyle is not null)
            return;

        Font font = FindFont();
        _titleStyle = new GUIStyle(GUI.skin.label)
        {
            font = font,
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(1f, 0.72f, 0.32f) }
        };
        _messageStyle = new GUIStyle(GUI.skin.label)
        {
            font = font,
            fontSize = 15,
            wordWrap = true,
            normal = { textColor = Color.white }
        };
        _buttonStyle = new GUIStyle(GUI.skin.button)
        {
            font = font,
            fontSize = 14
        };
    }

    private static Font FindFont()
    {
        string[] candidates =
        [
            "Microsoft YaHei",
            "Microsoft JhengHei",
            "Yu Gothic UI",
            "Malgun Gothic",
            "Arial Unicode MS",
            "Segoe UI"
        ];
        string[] installed = Font.GetOSInstalledFontNames();
        string? name = candidates.FirstOrDefault(candidate => installed.Contains(candidate));
        return name is null ? GUI.skin.font : Font.CreateDynamicFontFromOSFont(name, 16);
    }
}

internal static class MissingDependencyNoticeState
{
    internal static MissingDependencyNotice? Instance;
    internal static readonly SortedDictionary<string, (string Dependency, string Version)> Requirements = [];
}
