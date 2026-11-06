// SPDX-License-Identifier: AGPL-3.0-only
using System.CommandLine;
using System.CommandLine.Invocation;

namespace MetaMystia.Extensions.AddonKit;

internal static class ProjectLinkCommand
{
    internal static bool AddToLink(RootCommand root, Command command)
    {
        Command? linkCommand = root.Children
            .OfType<Command>()
            .FirstOrDefault(existing => existing.Name.Equals("link", StringComparison.OrdinalIgnoreCase));
        if (linkCommand is null
            || linkCommand.Children.OfType<Command>().Any(
                existing => existing.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        linkCommand.AddCommand(command);
        return true;
    }

    internal static Command Create(
        string name,
        string description,
        string url,
        string message,
        Action<string> openUrl,
        Action<InvocationContext, string> log)
    {
        var command = new Command(name, description);
        command.SetHandler(context =>
        {
            openUrl(url);
            log(context, message);
        });
        return command;
    }
}
