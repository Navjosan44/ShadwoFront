using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Shadowfront.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pausePanel;
        public Button resumeButton;
        public Button settingsButton;
        public Button mainMenuButton;
        public Button quitButton;
        public TextMeshProUGUI missionNameText;
        
        Shadowfront.UI.MainMenu mainMenu;
        
        void Awake()
        {
            if(pausePanel != null) pausePanel.SetActive(false);
            mainMenu = FindObjectOfType<Shadowfront.UI.MainMenu>();
            
            resumeButton?.onClick.AddListener(Resume);
            settingsButton?.onClick.AddListener(() => { if(mainMenu) mainMenu.ShowSettings(); });
            mainMenuButton?.onClick.AddListener(ReturnToMainMenu);
            quitButton?.onClick.AddListener(QuitGame);
        }
        
        void Update()
        {
            if(Shadowfront.Input.InputManager.Instance?.PausePressed == true)
            {
                TogglePause();
            }
        }
        
        public void TogglePause()
        {
            bool paused = !GameManager.Instance.IsPaused;
            if(paused) Pause(); else Resume();
        }
        
        public void Pause()
        {
            GameManager.Instance.Pause();
            pausePanel?.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if(missionNameText != null)
            {
                var mm = Shadowfront.Missions.MissionManager.Instance;
                missionNameText.text = mm?.currentMission?.missionName ?? "Mission";
            }
        }
        
        public void Resume()
        {
            GameManager.Instance.Resume();
            pausePanel?.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        void ReturnToMainMenu()
        {
            GameManager.Instance.Resume();
            Shadowfront.Missions.MissionManager.Instance?.FailMission();
            SceneManager.LoadScene("MainMenu");
        }
        
        void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
