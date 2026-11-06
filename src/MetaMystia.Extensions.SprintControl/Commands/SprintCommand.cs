// SPDX-License-Identifier: AGPL-3.0-only
using System.CommandLine;
using System.CommandLine.Invocation;

using MetaMystia.ConsoleSystem;

namespace MetaMystia.Extensions.SprintControl;

internal static class SprintCommand
{
    internal static void Register(RootCommand root, Plugin plugin)
    {
        if (root.Children.OfType<Command>().Any(
                command => command.Name.Equals("sprint", StringComparison.OrdinalIgnoreCase)))
            return;

        var multiplierArgument = new Argument<string>("multiplier", () => "")
        {
            Arity = ArgumentArity.ZeroOrOne,
            Description = SprintLocalization.Get(SprintText.ArgumentDescription)
        };
        var sprintCommand = new Command("sprint", SprintLocalization.Get(SprintText.CommandDescription))
        {
            multiplierArgument
        };
        sprintCommand.SetHandler(context => Handle(context, multiplierArgument, plugin));

        root.AddCommand(sprintCommand);
        CommandRegistry.RegisterCompletions("sprint", 0, "reset", "1", "1.5", "2", "2.5", "3");
        CommandRegistry.RegisterHint(
            "sprint",
            0,
            SprintLocalization.Get(
                SprintText.ArgumentHint,
                SprintMath.FormatMultiplier(SprintMath.MinMultiplier),
                SprintMath.FormatMultiplier(SprintMath.MaxMultiplier)));
    }

    private static void Handle(
        InvocationContext context,
        Argument<string> multiplierArgument,
        Plugin plugin)
    {
        string raw = (context.ParseResult.GetValueForArgument(multiplierArgument) ?? "").Trim();
        if (raw.Length == 0)
        {
            PrintUsage(context, plugin);
            return;
        }

        if (raw.Equals("reset", StringComparison.OrdinalIgnoreCase))
        {
            if (!plugin.TryResetSprintMultiplier())
            {
                context.Log(ConsoleFormat.Err(
                    SprintLocalization.Get(SprintText.ConfigSaveFailed)));
                return;
            }

            context.Log(ConsoleFormat.Ok(
                SprintLocalization.Get(
                    SprintText.ResetSuccess,
                    SprintMath.FormatMultiplier(plugin.SprintMultiplier))));
            return;
        }

        if (!SprintMath.TryParseMultiplier(raw, out float multiplier))
        {
            context.Log(ConsoleFormat.Err(
                SprintLocalization.Get(
                    SprintText.InvalidMultiplier,
                    raw,
                    SprintMath.FormatMultiplier(SprintMath.MinMultiplier),
                    SprintMath.FormatMultiplier(SprintMath.MaxMultiplier))));
            return;
        }

        if (!plugin.TrySetSprintMultiplier(multiplier))
        {
            context.Log(ConsoleFormat.Err(
                SprintLocalization.Get(SprintText.ConfigSaveFailed)));
            return;
        }

        context.Log(ConsoleFormat.Ok(
            SprintLocalization.Get(
                SprintText.SetSuccess,
                SprintMath.FormatMultiplier(plugin.SprintMultiplier))));
    }

    private static void PrintUsage(InvocationContext context, Plugin plugin)
    {
        context.Log(ConsoleFormat.Header(SprintLocalization.Get(SprintText.UsageHeader)));
        context.Log(ConsoleFormat.SubCmd(
            "/sprint",
            SprintLocalization.Get(SprintText.UsageArguments),
            SprintLocalization.Get(
                SprintText.UsageLine,
                SprintMath.FormatMultiplier(plugin.SprintMultiplier),
                SprintMath.FormatMultiplier(SprintMath.MinMultiplier),
                SprintMath.FormatMultiplier(SprintMath.MaxMultiplier))));
        context.Log(ConsoleFormat.Line);
    }
}
