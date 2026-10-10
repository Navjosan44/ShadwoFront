using UnityEngine;

namespace Shadowfront.Accessibility
{
    [System.Serializable]
    public class AccessibilitySettings
    {
        public ColorblindMode colorblindMode = ColorblindMode.None;
        public float subtitleSize = 1f;
        public bool subtitlesEnabled = true;
        public bool screenShakeEnabled = true;
        public bool screenFlashEnabled = true;
        public float uiScale = 1f;
        public bool highContrast = false;
        public bool slowMotionOnLowHealth = true;
        public bool autoAimAssist = false;
        public float autoAimStrength = 0.3f;
        public bool holdToFire = false;
        public bool holdToAim = false;
    }
    
    public enum ColorblindMode { None, Protanopia, Deuteranopia, Tritanopia }
    
    public class AccessibilityManager : MonoBehaviour
    {
        public static AccessibilityManager Instance { get; private set; }
        
        public AccessibilitySettings settings = new AccessibilitySettings();
        
        public System.Action<AccessibilitySettings> OnSettingsChanged;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(transform.root); return; }
            Instance=this; DontDestroyOnLoad(transform.root);
            LoadSettings();
        }
        
        public void SetColorblindMode(ColorblindMode mode)
        {
            settings.colorblindMode = mode;
            ApplyColorblindMode(mode);
            OnSettingsChanged?.Invoke(settings);
        }
        
        void ApplyColorblindMode(ColorblindMode mode)
        {
            // In production, swap HUD colors for colorblind modes
            // Low: Protanopia/Deuteranopia -> red/green shift
            // Tritanopia -> blue/yellow shift
            switch(mode)
            {
                case ColorblindMode.Protanopia:
                    Debug.Log(\"[Accessibility] Protanopia mode: reds shifted to orange\");
                    break;
                case ColorblindMode.Deuteranopia:
                    Debug.Log(\"[Accessibility] Deuteranopia mode: greens shifted to yellow\");
                    break;
                case ColorblindMode.Tritanopia:
                    Debug.Log(\"[Accessibility] Tritanopia mode: blues shifted to magenta\");
                    break;
            }
        }
        
        public void SetSubtitles(bool enabled, float size)
        {
            settings.subtitlesEnabled = enabled;
            settings.subtitleSize = size;
            OnSettingsChanged?.Invoke(settings);
        }
        
        public void SetScreenShake(bool enabled)
        {
            settings.screenShakeEnabled = enabled;
            OnSettingsChanged?.Invoke(settings);
        }
        
        public void SetUIScale(float scale)
        {
            settings.uiScale = Mathf.Clamp(scale, 0.8f, 2f);
            OnSettingsChanged?.Invoke(settings);
        }
        
        public void SetHighContrast(bool enabled)
        {
            settings.highContrast = enabled;
            OnSettingsChanged?.Invoke(settings);
        }
        
        public float GetAutoAimStrength() => settings.autoAimAssist ? settings.autoAimStrength : 0f;
        public bool HoldToFire => settings.holdToFire;
        public bool HoldToAim => settings.holdToAim;
        
        void LoadSettings()
        {
            settings.colorblindMode = (ColorblindMode)PlayerPrefs.GetInt(\"ColorblindMode\", 0);
            settings.subtitlesEnabled =PlayerPrefs.GetInt(\"Subtitles\", 1) == 1;
            settings.subtitleSize =PlayerPrefs.GetFloat(\"SubtitleSize\", 1f);
            settings.screenShakeEnabled =PlayerPrefs.GetInt(\"ScreenShake\", 1) == 1;
            settings.uiScale =PlayerPrefs.GetFloat(\"UIScale\", 1f);
            settings.highContrast =PlayerPrefs.GetInt(\"HighContrast\", 0) == 1;
            settings.autoAimAssist =PlayerPrefs.GetInt(\"AutoAim\", 0) == 1;
            settings.autoAimStrength =PlayerPrefs.GetFloat(\"AutoAimStrength\", 0.3f);
            settings.holdToFire =PlayerPrefs.GetInt(\"HoldToFire\", 0) == 1;
            settings.holdToAim =PlayerPrefs.GetInt(\"HoldToAim\", 0) == 1;
        }
        
        public void SaveSettings()
        {
           Prefs.SetInt(\"ColorblindMode\", (int)settings.colorblindMode);
           Prefs.SetInt(\"Subtitles\", settings.subtitlesEnabled ? 1 : 0);
           Prefs.SetFloat(\"SubtitleSize\", settings.subtitleSize);
           Prefs.SetInt(\"ScreenShake\", settings.screenShakeEnabled ? 1 : 0);
           Prefs.SetFloat(\"UIScale\", settings.uiScale);
           Prefs.SetInt(\"HighContrast\", settings.highContrast ? 1 : 0);
           Prefs.SetInt(\"AutoAim\", settings.autoAimAssist ? 1 : 0);
           Prefs.SetFloat(\"AutoAimStrength\", settings.autoAimStrength);
           Prefs.SetInt(\"HoldToFire\", settings.holdToFire ? 1 : 0);
           Prefs.SetInt(\"HoldToAim\", settings.holdToAim ? 1 : 0);
           Prefs.Save();
        }
    }
}
