using UnityEngine;
using UnityEngine.InputSystem;

namespace fpsRed.Utilities
{
    public static class GeneralUtils
    {
        public static string PlayerInputTag { get; } = "PlayerInput";

        private static PlayerInput cachedPlayerInput;

        public static PlayerInput GetPlayerInput()
        {
            if (cachedPlayerInput == null)
            {
                cachedPlayerInput = GameObject.FindWithTag(PlayerInputTag).GetComponent<PlayerInput>();
            }

            return cachedPlayerInput;
        }

        public static Vector3 Horizontal(this Vector3 vector)
        {
            vector.y = 0f;
            return vector;
        }
    }
}