// SPDX-License-Identifier: AGPL-3.0-only

using MetaMystia.Extensions.SprintControl;

using Xunit;

namespace MetaMystia.Extensions.SprintControl.Tests;

public sealed class GameVersionGuardTests
{
    [Fact]
    public void SupportedVersionMatchesExactly()
    {
        Assert.True(GameVersionGuard.IsSupported("RELEASE 4.4.0e"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("RELEASE 4.4.0d")]
    [InlineData("release 4.4.0e")]
    [InlineData(" RELEASE 4.4.0e")]
    public void OtherVersionsAreRejected(string? version)
    {
        Assert.False(GameVersionGuard.IsSupported(version));
    }
}
