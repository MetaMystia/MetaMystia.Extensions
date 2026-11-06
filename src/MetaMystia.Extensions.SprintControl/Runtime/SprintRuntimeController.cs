// SPDX-License-Identifier: AGPL-3.0-only

using Common.CharacterUtility;
using Common.UI;

namespace MetaMystia.Extensions.SprintControl;

internal static class SprintRuntimeController
{
    private static CharacterControllerUnit? _activeCharacter;

    internal static void Activate(CharacterControllerUnit character)
    {
        _activeCharacter = character;
        character.sprintMultiplier = Plugin.Instance.SprintMultiplier;
    }

    internal static void Deactivate(CharacterControllerUnit character)
    {
        if (_activeCharacter == null || character == null)
            return;

        if (_activeCharacter.GetInstanceID() != character.GetInstanceID())
            return;

        character.sprintMultiplier = 1f;
        _activeCharacter = null;
    }

    internal static void DeactivateCurrent()
    {
        CharacterControllerUnit? character = _activeCharacter;
        _activeCharacter = null;

        if (character != null)
            character.sprintMultiplier = 1f;
    }

    internal static void RefreshActiveMultiplier()
    {
        if (!UniversalGameManager.IsInputEnabled)
        {
            DeactivateCurrent();
            return;
        }

        CharacterControllerUnit? character = _activeCharacter;
        if (character == null)
        {
            _activeCharacter = null;
            return;
        }

        character.sprintMultiplier = Plugin.Instance.SprintMultiplier;
    }
}
