using UnityEngine;

using UnityEngine.InputSystem;

using fpsRed.Utilities;
namespace fpsRed.UI.Pause
{
    public class PauseGame : MonoBehaviour
    {
        public class OnPauseEventArgs : System.EventArgs 
        {
            public bool Paused { get; private set; }

            public OnPauseEventArgs(bool paused)
            {
                Paused = paused;
            }
        }

        public event System.EventHandler<OnPauseEventArgs> OnPauseEvent;
        public static PauseGame Instance { get; private set; } = null;

        private PlayerInput playerInput;
        public bool Paused { get; private set; } = false;

        [Space, SerializeField] private bool startPaused;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            playerInput = GeneralUtils.GetPlayerInput();

            playerInput.actions["Player/Pause"].performed += PauseInput;
            playerInput.actions["UI/Pause"].performed += PauseInput;
        }

        private void Start()
        {
            if (startPaused)
            {
                OpenPauseMenu();
                return;
            }
            ClosePauseMenu();
        }

        private void PauseInput(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
            {
                return;
            }

            Pause();
        }

        private void OpenPauseMenu()
        {
            playerInput.SwitchCurrentActionMap("UI");
            Time.timeScale = 0f;
            Cursor.visible = true;

            Paused = true;
            OnPauseEvent?.Invoke(this, new OnPauseEventArgs(true));
        }

        private void ClosePauseMenu()
        {
            playerInput.SwitchCurrentActionMap("Player");
            Time.timeScale = 1f;
            Cursor.visible = false;

            Paused = false;
            OnPauseEvent?.Invoke(this, new OnPauseEventArgs(false));
        }

        public void Pause()
        {
            if (Paused)
            {
                ClosePauseMenu();
                return;
            }

            OpenPauseMenu();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}