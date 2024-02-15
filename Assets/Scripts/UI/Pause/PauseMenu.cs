using UnityEngine;

namespace fpsRed.UI.Pause
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private int startingSubmenuIndex = 0;

        [Space]

        [SerializeField] private GameObject menuOverlay;
        [SerializeField] private GameObject[] submenus;

        private void Start()
        {
            PauseGame.Instance.OnPauseEvent += OnPaused;
        }

        private void OnPaused(object sender, PauseGame.OnPauseEventArgs args)
        {
            if (args.Paused)
            {
                menuOverlay.SetActive(true);
                OpenMenu(startingSubmenuIndex);
                return;
            }
            menuOverlay.SetActive(false);
            OpenMenu(-1);
        }

        public void OpenMenu(int menu)
        {
            for (int i = 0; i < submenus.Length; i++)
            {
                if (i == menu)
                {
                    submenus[i].SetActive(true);
                    continue;
                }
                submenus[i].SetActive(false);
            }
        }
    }
}