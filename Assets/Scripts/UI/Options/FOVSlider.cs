using UnityEngine;
using Cinemachine;

using fpsRed.UI.Pause;

namespace fpsRed.UI.Options
{
    public class FOVSlider : UIOptionSlider
    {
        [Space, SerializeField] private CinemachineVirtualCamera virtualCamera;
        protected override float GetStartingValue()
        {
            return virtualCamera.m_Lens.FieldOfView;
        }

        protected override void OnInputChanged(string value)
        {
            float fov = float.Parse(value);

            fov = Mathf.Clamp(fov, slider.minValue, slider.maxValue);
            inputField.text = fov.ToString();

            virtualCamera.m_Lens.FieldOfView = fov;
        }

        protected override void OnSliderChanged(float value)
        {
            virtualCamera.m_Lens.FieldOfView = value;
        }

        protected override void OnPause(object sender, PauseGame.OnPauseEventArgs args)
        {

        }
    }
}