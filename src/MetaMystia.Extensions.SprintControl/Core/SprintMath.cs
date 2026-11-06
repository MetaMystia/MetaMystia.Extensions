// SPDX-License-Identifier: AGPL-3.0-only
using System.Globalization;

namespace MetaMystia.Extensions.SprintControl;

internal readonly record struct MotionEncoding(float Speed, bool Sprinting);

internal static class SprintMath
{
    internal const float DefaultMultiplier = 1.5f;
    internal const float MinMultiplier = 0.1f;
    internal const float MaxMultiplier = 10f;

    internal static bool TryParseMultiplier(string? text, out float multiplier)
    {
        multiplier = 0f;
        return !string.IsNullOrWhiteSpace(text)
            && float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out multiplier)
            && IsValidMultiplier(multiplier);
    }

    internal static string FormatMultiplier(float multiplier) =>
        multiplier.ToString("0.###", CultureInfo.InvariantCulture);

    internal static MotionEncoding EncodeNetworkMotion(float speed, bool sprinting, float multiplier)
    {
        if (!sprinting
            || !float.IsFinite(speed)
            || !IsValidMultiplier(multiplier)
            || MathF.Abs(multiplier - DefaultMultiplier) <= 0.0001f)
        {
            return new(speed, sprinting);
        }

        float encodedSpeed = speed * multiplier;
        return float.IsFinite(encodedSpeed)
            ? new(encodedSpeed, false)
            : new(speed, sprinting);
    }

    internal static bool IsValidMultiplier(float multiplier) =>
        float.IsFinite(multiplier)
        && multiplier is >= MinMultiplier and <= MaxMultiplier;
}
