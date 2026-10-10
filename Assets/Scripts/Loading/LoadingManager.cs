using UnityEngine;
using UnityEngine.UI;
using System;

namespace Shadowfront.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager Instance { get; private set; }

        public GameObject loadingPanel;
        public Slider progressSlider;
        public Text loadingText;
        public Text percentText;
        public Text tipText;

        string[] tips = {
            "Did you know? Headshots deal 2x damage.",
            "Use cover to survive.",
            "Ammo is precious - reload only when needed.",
            "Extract quickly to secure your rewards.",
            "Higher difficulty = better rewards."
        };

        float targetProgress;
        float currentProgress;
        bool isLoading;

        public System.Action OnLoadingComplete;

        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(transform.root); return; }
            Instance=this; DontDestroyOnLoad(transform.root);
            loadingPanel?.SetActive(false);
        }

        public void LoadScene(string sceneName, float loadTime = 3f)
        {
            if(isLoading) return;
            isLoading = true;
            loadingPanel?.SetActive(true);
            targetProgress = 0;
            currentProgress = 0;
            progressSlider?.value = 0;

            if(tipText != null)
                tipText.text = tips[Random.Range(0, tips.Length)];

            // In production, use Scenes.loadableSceneList + AsyncOperation
            Debug.Log($"[Loading] Loading scene: {sceneName}");
        }

        void Update()
        {
            if(!isLoading) return;

            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, Time.deltaTime * 0.5f);
            progressSlider?.value = currentProgress;
            if(percentText != null) percentText.text = $"{(int)(currentProgress * 100)}%\";
            if(loadingText != null) loadingText.text = currentProgress < 1f ? "Loading..." : "Ready";

            if(currentProgress >= 1f)
            {
                isLoading = false;
                loadingPanel?.SetActive(false);
                OnLoadingComplete?.Invoke();
            }
        }

        public void SetProgress(float progress)
        {
            targetProgress = Mathf.Clamp01(progress);
        }

        public void SetLoadingText(string text) => loadingText.text = text;
        public bool IsLoading => isLoading;
    }
}