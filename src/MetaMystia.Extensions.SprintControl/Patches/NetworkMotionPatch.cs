// SPDX-License-Identifier: AGPL-3.0-only
using HarmonyLib;

using MetaMystia.Network;

namespace MetaMystia.Extensions.SprintControl.Patches;

[HarmonyPatch(typeof(Client), nameof(Client.SendMotion))]
internal static class NetworkMotionPatch
{
    [HarmonyPrefix]
    private static void SendMotion_Prefix(ref Motion motion)
    {
        if (!Plugin.Instance.EnsureGameVersionSupported())
            return;

        MotionEncoding encoded = SprintMath.EncodeNetworkMotion(
            motion.Speed,
            motion.Sprinting,
            Plugin.Instance.SprintMultiplier);

        // Stock MetaMystia restores Sprinting as a fixed 1.5x multiplier. Send custom sprint as base speed instead.
        if (encoded.Speed != motion.Speed || encoded.Sprinting != motion.Sprinting)
            motion = motion with { Speed = encoded.Speed, Sprinting = encoded.Sprinting };
    }
}
