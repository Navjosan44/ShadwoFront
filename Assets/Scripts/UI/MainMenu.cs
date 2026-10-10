using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Shadowfront.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header(\"Panels\")]
        public GameObject mainPanel;
        public GameObject missionSelectPanel;
        public GameObject settingsPanel;
        public GameObject creditsPanel;
        public GameObject achievementsPanel;
        
        [Header(\"Mission Select\")]
        public Transform missionListContainer;
        public GameObject missionButtonPrefab;
        
        [Header(\"Difficulty\")]
        public Dropdown difficultyDropdown;
        
        [Header(\"Achievements\")]
        public Transform achievementsContainer;
        public GameObject achievementPrefab;
        
        [Header(\"Settings\")]
        public Slider masterVolume;
        public Slider musicVolume;
        public Slider sfxVolume;
        public Dropdown qualityDropdown;
        public Dropdown fpsDropdown;
        public Toggle vibrationToggle;
        public Slider sensitivitySlider;
        
        Shadowfront.Difficulty.DifficultyManager difficultyManager;
        
        void Start()
        {
            ShowMain();
            LoadSettings();
            PopulateMissions();
            difficultyManager = Shadowfront.Difficulty.DifficultyManager.Instance;
            if(difficultyDropdown != null && difficultyManager != null)
                difficultyDropdown.value = (int)difficultyManager.currentDifficulty;
        }
        
        public void ShowMain() { SetPanel(mainPanel); }
        public void ShowMissionSelect() { SetPanel(missionSelectPanel); PopulateMissions(); }
        public void ShowSettings() { SetPanel(settingsPanel); }
        public void ShowCredits() { SetPanel(creditsPanel); }
        public void ShowAchievements() { SetPanel(achievementsPanel); PopulateAchievements(); }
        
        void SetPanel(GameObject active)
        {
            mainPanel?.SetActive(false);
            missionSelectPanel?.SetActive(false);
            settingsPanel?.SetActive(false);
            creditsPanel?.SetActive(false);
            achievementsPanel?.SetActive(false);
            active?.SetActive(true);
        }
        
        public void StartMission(int index)
        {
            var mm = Shadowfront.Missions.MissionManager.Instance;
            if(mm != null)
            {
                // Load mission data from Resources
                var missions = Resources.LoadAll<Shadowfront.Missions.MissionData>(\"Missions\");
                if(missions != null && missions.Length > index)
                    mm.StartMission(missions[index]);
                else
                    SceneManager.LoadScene(\"Main\");
            }
            else
            {
                SceneManager.LoadScene(\"Main\");
            }
        }
        
        public void SetDifficulty(int index)
        {
            if(difficultyManager != null)
                difficultyManager.SetDifficulty((Shadowfront.Difficulty.DifficultyLevel)index);
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        void PopulateMissions()
        {
            if(missionListContainer == null || missionButtonPrefab == null) return;
            
            foreach(Transform child in missionListContainer) Destroy(child.gameObject);
            
            var missions = Resources.LoadAll<Shadowfront.Missions.MissionData>(\"Missions\");
            if(missions == null || missions.Length == 0)
            {
                // Fallback to hardcoded names
                string[] names = { \"Training Grounds\", \"Sector 7\", \"The Docks\", \"Industrial Zone\" };
                for(int i=0;i<names.Length;i++)
                {
                    var btn = Instantiate(missionButtonPrefab, missionListContainer);
                    var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
                    if(txt != null) txt.text = names[i];
                    int idx = i;
                    btn.GetComponent<Button>().onClick.AddListener(() => StartMission(idx));
                }
            }
            else
            {
                for(int i=0;i<missions.Length;i++)
                {
                    var btn = Instantiate(missionButtonPrefab, missionListContainer);
                    var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
                    if(txt != null) txt.text = missions[i].missionName;
                    int idx = i;
                    btn.GetComponent<Button>().onClick.AddListener(() => StartMission(idx));
                }
            }
        }
        
        void PopulateAchievements()
        {
            if(achievementsContainer == null || achievementPrefab == null) return;
            
            foreach(Transform child in achievementsContainer) Destroy(child.gameObject);
            
            var achManager = Shadowfront.Achievements.AchievementManager.Instance;
            if(achManager == null) return;
            
            foreach(var ach in achManager.achievements)
            {
                var slot = Instantiate(achievementPrefab, achievementsContainer);
                var txt = slot.GetComponentInChildren<TextMeshProUGUI>();
                if(txt != null)
                    txt.text = $\"{ach.title}: {ach.description} {(ach.unlocked ? \"[UNLOCKED]\" : \"\")}\";
            }
        }
        
        public void ApplySettings()
        {
            AudioListener.volume = masterVolume?.value ?? 1f;
            QualitySettings.SetQualityLevel(qualityDropdown?.value ?? 2);
            Application.targetFrameRate = fpsDropdown?.value switch { 0 => 30, 1 => 45, 2 => 60, _ => 60 };
            SaveSettings();
        }
        
        void LoadSettings()
        {
            masterVolume?.SetValueWithoutNotify(PlayerPrefs.GetFloat(\"MasterVol\", 1f));
            musicVolume?.SetValueWithoutNotify(PlayerPrefs.GetFloat(\"MusicVol\", 1f));
            sfxVolume?.SetValueWithoutNotify(PlayerPrefs.GetFloat(\"SFXVol\", 1f));
            qualityDropdown?.SetValueWithoutNotify(PlayerPrefs.GetInt(\"Quality\", 2));
            fpsDropdown?.SetValueWithoutNotify(PlayerPrefs.GetInt(\"TargetFPS\", 2));
            vibrationToggle?.SetIsOnWithoutNotify(PlayerPrefs.GetInt(\"Vibration\", 1) == 1);
            sensitivitySlider?.SetValueWithoutNotify(PlayerPrefs.GetFloat(\"Sensitivity\", 1f));
        }
        
        void SaveSettings()
        {
            PlayerPrefs.SetFloat(\"MasterVol\", masterVolume?.value ?? 1f);
            PlayerPrefs.SetFloat(\"MusicVol\", musicVolume?.value ?? 1f);
            PlayerPrefs.SetFloat(\"SFXVol\", sfxVolume?.value ?? 1f);
            PlayerPrefs.SetInt(\"Quality\", qualityDropdown?.value ?? 2);
            PlayerPrefs.SetInt(\"TargetFPS\", fpsDropdown?.value ?? 2);
            PlayerPrefs.SetInt(\"Vibration\", vibrationToggle?.isOn == true ? 1 : 0);
            PlayerPrefs.SetFloat(\"Sensitivity\", sensitivitySlider?.value ?? 1f);
            PlayerPrefs.Save();
        }
    }
}
