using UnityEngine;
using UnityEngine.UI;
using TMPro;
using fpsRed.UI.Pause;

namespace fpsRed.UI.Options
{
    public abstract class UIOptionSlider : MonoBehaviour
    {
        [SerializeField] protected Slider slider;
        [SerializeField] protected TMP_InputField inputField;

        protected virtual void Start()
        {
            PauseGame.Instance.OnPauseEvent += OnPause;

            slider.onValueChanged.AddListener((value) =>
            {
                inputField.text = value.ToString();
                OnSliderChanged(value);
            });

            inputField.onEndEdit.AddListener((value) =>
            {
                slider.value = float.Parse(value);
                OnInputChanged(value);
            });

            SetUIValues(GetStartingValue());
        }

        protected abstract void OnPause(object sender, PauseGame.OnPauseEventArgs args);

        protected virtual void SetUIValues(float value)
        {
            slider.value = value;
            inputField.text = value.ToString();
        }

        protected abstract void OnSliderChanged(float value);

        protected abstract void OnInputChanged(string value);

        protected abstract float GetStartingValue();
    }
}