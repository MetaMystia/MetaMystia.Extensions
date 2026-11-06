// SPDX-License-Identifier: AGPL-3.0-only

using HarmonyLib;

using Common.CharacterUtility;
using Common.UI;
using DayScene.Input;

namespace MetaMystia.Extensions.SprintControl.Patches;

[HarmonyPatch(typeof(DayScenePlayerInputGenerator))]
internal static class DaySceneSprintInputPatch
{
    [HarmonyPatch(nameof(DayScenePlayerInputGenerator.OnSprintPerformed))]
    [HarmonyPostfix]
    private static void OnSprintPerformed_Postfix(DayScenePlayerInputGenerator __instance)
    {
        if (!__instance.initialized)
            return;

        if (!Plugin.Instance.EnsureGameVersionSupported())
        {
            SprintRuntimeController.DeactivateCurrent();
            return;
        }

        CharacterControllerUnit? character = __instance.Character;
        if (character != null && UniversalGameManager.IsInputEnabled && __instance.internalAvailability)
            SprintRuntimeController.Activate(character);
        else if (character != null)
            SprintRuntimeController.Deactivate(character);
    }

    [HarmonyPatch(nameof(DayScenePlayerInputGenerator.OnSprintCanceled))]
    [HarmonyPostfix]
    private static void OnSprintCanceled_Postfix(DayScenePlayerInputGenerator __instance)
    {
        if (!__instance.initialized)
            return;

        if (!Plugin.Instance.EnsureGameVersionSupported())
            return;

        CharacterControllerUnit? character = __instance.Character;
        if (character != null)
            SprintRuntimeController.Deactivate(character);
    }
}
