// SPDX-License-Identifier: AGPL-3.0-only
using System.CommandLine;
using System.CommandLine.IO;

using MetaMystia.Extensions.AddonKit;

using Xunit;

namespace MetaMystia.Extensions.AddonKit.Tests;

public sealed class ProjectLinkCommandTests
{
    [Fact]
    public void AddToLink_RegistersCommandOnce()
    {
        var root = new RootCommand();
        var link = new Command("link");
        var extensionLink = new Command("MetaMystia.Extensions");
        root.AddCommand(link);

        bool first = ProjectLinkCommand.AddToLink(root, extensionLink);
        bool second = ProjectLinkCommand.AddToLink(root, new Command("MetaMystia.Extensions"));

        Assert.True(first);
        Assert.False(second);
        Assert.Single(link.Children, command => command.Name == "MetaMystia.Extensions");
    }

    [Fact]
    public void Execute_OpensUrlAndWritesMessage()
    {
        const string url = "https://github.com/MetaMystia/MetaMystia.Extensions";
        string? openedUrl = null;
        string? loggedMessage = null;
        Command command = ProjectLinkCommand.Create(
            "MetaMystia.Extensions",
            "MetaMystia.Extensions GitHub repository",
            url,
            "Opened MetaMystia.Extensions",
            value => openedUrl = value,
            (_, message) => loggedMessage = message);
        var root = new RootCommand();
        root.AddCommand(command);

        int exitCode = root.Invoke(command.Name, new TestConsole());

        Assert.Equal(0, exitCode);
        Assert.Equal(url, openedUrl);
        Assert.Equal("Opened MetaMystia.Extensions", loggedMessage);
    }
}
