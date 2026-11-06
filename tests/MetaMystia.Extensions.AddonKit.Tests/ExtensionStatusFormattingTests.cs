// SPDX-License-Identifier: AGPL-3.0-only

using MetaMystia.Extensions.AddonKit;

using Xunit;

namespace MetaMystia.Extensions.AddonKit.Tests;

public sealed class ExtensionStatusFormattingTests
{
    private const string BaseLabel = "MetaMystia v0.29.5 loaded with 1 rex pack";

    [Fact]
    public void NoExtensionsLeavesLabelUnchanged()
    {
        Assert.Equal(BaseLabel, ExtensionStatusFormatting.Append(BaseLabel, 0));
    }

    [Fact]
    public void OneExtensionUsesSingular()
    {
        Assert.Equal($"{BaseLabel}, 1 extension", ExtensionStatusFormatting.Append(BaseLabel, 1));
    }

    [Fact]
    public void MultipleExtensionsUsePlural()
    {
        Assert.Equal($"{BaseLabel}, 2 extensions", ExtensionStatusFormatting.Append(BaseLabel, 2));
    }
}
