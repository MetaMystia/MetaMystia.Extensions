// SPDX-License-Identifier: AGPL-3.0-only
using System.Globalization;

using MetaMystia.Extensions.SprintControl;

using Xunit;

namespace MetaMystia.Extensions.SprintControl.Tests;

public sealed class SprintMathTests
{
    [Theory]
    [InlineData("0.1", 0.1f)]
    [InlineData("1", 1f)]
    [InlineData("1.5", 1.5f)]
    [InlineData("2.25", 2.25f)]
    [InlineData("10", 10f)]
    public void TryParseMultiplier_AcceptsValuesInsideSupportedRange(string input, float expected)
    {
        bool parsed = SprintMath.TryParseMultiplier(input, out float actual);

        Assert.True(parsed);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("0")]
    [InlineData("0.099")]
    [InlineData("10.001")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    [InlineData("reset")]
    public void TryParseMultiplier_RejectsInvalidValues(string input)
    {
        bool parsed = SprintMath.TryParseMultiplier(input, out _);

        Assert.False(parsed);
    }

    [Fact]
    public void EncodeNetworkMotion_UsesTheBaselineSprintEncoding()
    {
        MotionEncoding actual = SprintMath.EncodeNetworkMotion(
            speed: 2f,
            sprinting: true,
            multiplier: SprintMath.DefaultMultiplier);

        Assert.Equal(new MotionEncoding(2f, true), actual);
    }

    [Fact]
    public void EncodeNetworkMotion_EncodesCustomSprintAsBaseSpeed()
    {
        MotionEncoding actual = SprintMath.EncodeNetworkMotion(
            speed: 2f,
            sprinting: true,
            multiplier: 2.5f);

        Assert.Equal(new MotionEncoding(5f, false), actual);
    }

    [Fact]
    public void EncodeNetworkMotion_PreservesNonSprintMotion()
    {
        MotionEncoding actual = SprintMath.EncodeNetworkMotion(
            speed: 2f,
            sprinting: false,
            multiplier: 2.5f);

        Assert.Equal(new MotionEncoding(2f, false), actual);
    }

    [Fact]
    public void EncodeNetworkMotion_EncodesSlowerSprintForRemotePlayers()
    {
        MotionEncoding actual = SprintMath.EncodeNetworkMotion(
            speed: 2f,
            sprinting: true,
            multiplier: 0.5f);

        Assert.Equal(new MotionEncoding(1f, false), actual);
    }

    [Fact]
    public void EncodeNetworkMotion_RejectsInvalidMultiplier()
    {
        MotionEncoding actual = SprintMath.EncodeNetworkMotion(
            speed: 2f,
            sprinting: true,
            multiplier: float.NaN);

        Assert.Equal(new MotionEncoding(2f, true), actual);
    }

    [Fact]
    public void FormatMultiplier_UsesInvariantDecimalSeparator()
    {
        string actual = SprintMath.FormatMultiplier(2.5f);

        Assert.Equal("2.5", actual);
        Assert.Equal("2.5", float.Parse(actual, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture));
    }
}
