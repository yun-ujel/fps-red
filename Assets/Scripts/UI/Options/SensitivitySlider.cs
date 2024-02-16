using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using fpsRed.UI.Pause;
using fpsRed.Utilities;

namespace fpsRed.UI.Options
{
    public class SensitivitySlider : UIOptionSlider
    {
        private PlayerInput playerInput;
        private InputBinding binding;

        private float queuedSensChange = 1f;

        [Space, SerializeField] private Toggle invertYToggle;

        protected override float GetStartingValue()
        {
            playerInput = GeneralUtils.GetPlayerInput();
            binding = new InputBinding();

            return 1f;
        }

        protected override void OnInputChanged(string value)
        {
            queuedSensChange = float.Parse(value);
        }

        protected override void OnPause(object sender, PauseGame.OnPauseEventArgs args)
        {
            if (!args.Paused)
            {
                ChangeSensitivity(queuedSensChange);
            }
        }

        protected override void OnSliderChanged(float value)
        {
            queuedSensChange = value;
        }

        private void ChangeSensitivity(float sens)
        {
            binding.overrideProcessors = $"ScaleVector2(x={sens},y={sens});InvertVector2(invertX=false,invertY={invertYToggle.isOn})";
            playerInput.actions["Player/Look"].ApplyBindingOverride(1, binding);
        }
    }
}