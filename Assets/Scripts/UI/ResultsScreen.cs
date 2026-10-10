using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Shadowfront.UI
{
    public class ResultsScreen : MonoBehaviour
    {
        public GameObject panel;
        public TextMeshProUGUI resultTitle;
        public TextMeshProUGUI statsText;
        public TextMeshProUGUI rewardsText;
        public Button continueButton;
        public Button retryButton;
        public Button mainMenuButton;
        
        void Awake()
        {
            panel?.SetActive(false);
            continueButton?.onClick.AddListener(Continue);
            retryButton?.onClick.AddListener(Retry);
            mainMenuButton?.onClick.AddListener(MainMenu);
        }
        
        public void Show(bool success, string missionName, int xpGained, int creditsGained, string[] loot)
        {
            panel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            
            resultTitle.text = success ? "MISSION SUCCESS" : "MISSION FAILED";
            resultTitle.color = success ? Color.green : Color.red;
            
            statsText.text = $"XP Gained: {xpGained}\nCredits: {creditsGained}";
            rewardsText.text = loot.Length > 0 ? "Loot:\n" + string.Join("\n", loot) : "No rewards";
        }
        
        void Continue()
        {
            Time.timeScale = 1f;
            panel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Load next mission or hub
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        
        void Retry()
        {
            Time.timeScale = 1f;
            panel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        
        void MainMenu()
        {
            Time.timeScale = 1f;
            panel.SetActive(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
