using System.Collections.Generic;
using UnityEngine;

namespace Shadowfront.Localization
{
    [System.Serializable]
    public class LocalizedString
    {
        public string key;
        public string value;
    }

    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        public string currentLanguage = "en";
        public LocalizedString[] strings;
        public string[] supportedLanguages = { "en", "es", "fr", "de", "zh", "hi" };

        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public System.Action<string> OnLanguageChanged;

        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(transform.root); return; }
            Instance=this; DontDestroyOnLoad(transform.root);
            currentLanguage =PlayerPrefs.GetString("Language", "en");
            BuildDictionary();
        }

        void BuildDictionary()
        {
            dictionary.Clear();
            foreach(var s in strings)
            {
                if(!dictionary.ContainsKey(s.key))
                    dictionary[s.key] = s.value;
            }
        }

        public string Get(string key)
        {
            if(dictionary.TryGetValue(key, out var value))
                return value;
            return $"[{key}]";
        }

        public void SetLanguage(string lang)
        {
            if(System.Array.IndexOf(supportedLanguages, lang) < 0) return;
            currentLanguage = lang;
           PlayerPrefs.SetString("Language", lang);
            OnLanguageChanged?.Invoke(lang);
        }

        public string GetLanguageName(int index) => index >= 0 && index < supportedLanguages.Length ? supportedLanguages[index] : "Unknown";
    }
}