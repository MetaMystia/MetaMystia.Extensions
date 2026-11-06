// SPDX-License-Identifier: AGPL-3.0-only
using System.CommandLine;
using System.Reflection;

using HarmonyLib;

using MetaMystia.ConsoleSystem;
using MetaMystia.ConsoleSystem.Commands;

using UnityEngine;

namespace MetaMystia.Extensions.AddonKit;

public static class CommandRegistrationBridge
{
    private const string ProjectLinkName = "MetaMystia.Extensions";
    private const string ProjectUrl = "https://github.com/MetaMystia/MetaMystia.Extensions";

    private static readonly object Gate = new();
    private static readonly List<Action<RootCommand>> Registrations = [];
    private static RootCommand? _root;
    private static bool _patched;
    private static bool _linkPatched;

    public static void Install(Harmony harmony, Action<RootCommand> registration)
    {
        ArgumentNullException.ThrowIfNull(harmony);
        ArgumentNullException.ThrowIfNull(registration);

        RootCommand? existingRoot;
        lock (Gate)
        {
            if (!_patched)
            {
                MethodInfo target = AccessTools.Method(
                    typeof(GeneralCommands),
                    nameof(GeneralCommands.Register))
                    ?? throw new MissingMethodException(
                        typeof(GeneralCommands).FullName,
                        nameof(GeneralCommands.Register));
                MethodInfo postfix = AccessTools.Method(
                    typeof(CommandRegistrationBridge),
                    nameof(ApplyRegisteredCommands))
                    ?? throw new MissingMethodException(
                        typeof(CommandRegistrationBridge).FullName,
                        nameof(ApplyRegisteredCommands));
                harmony.Patch(
                    target,
                    postfix: new HarmonyMethod(postfix));
                _patched = true;
            }

            if (!Registrations.Contains(registration))
                Registrations.Add(registration);
            existingRoot = _root;
        }

        if (existingRoot is not null)
            registration(existingRoot);
    }

    public static void InstallProjectLink(Harmony harmony)
    {
        ArgumentNullException.ThrowIfNull(harmony);

        lock (Gate)
        {
            if (_linkPatched)
                return;

            MethodInfo target = AccessTools.Method(
                typeof(LinkCommands),
                nameof(LinkCommands.Register))
                ?? throw new MissingMethodException(
                    typeof(LinkCommands).FullName,
                    nameof(LinkCommands.Register));
            MethodInfo postfix = AccessTools.Method(
                typeof(CommandRegistrationBridge),
                nameof(AddProjectLink))
                ?? throw new MissingMethodException(
                    typeof(CommandRegistrationBridge).FullName,
                    nameof(AddProjectLink));
            harmony.Patch(
                target,
                postfix: new HarmonyMethod(postfix));
            _linkPatched = true;
        }
    }

    private static void ApplyRegisteredCommands(RootCommand root)
    {
        Action<RootCommand>[] callbacks;
        lock (Gate)
        {
            _root = root;
            callbacks = Registrations.ToArray();
        }

        foreach (Action<RootCommand> callback in callbacks)
            callback(root);
    }

    private static void AddProjectLink(RootCommand root)
    {
        AddonLocalization.Initialize();
        string description = AddonLocalization.Get(AddonText.ProjectLinkDescription);
        string message = $"{ConsoleFormat.Ok(description)} {ConsoleFormat.Dim(ProjectUrl)}";
        Command command = ProjectLinkCommand.Create(
            ProjectLinkName,
            description,
            ProjectUrl,
            message,
            Application.OpenURL,
            (context, text) => context.Log(text));
        ProjectLinkCommand.AddToLink(root, command);
    }
}
