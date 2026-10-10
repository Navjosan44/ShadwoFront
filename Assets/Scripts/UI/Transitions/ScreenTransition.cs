using UnityEngine;
using UnityEngine.UI;
using System;

namespace Shadowfront.UI.Transitions
{
    public class ScreenTransition : MonoBehaviour
    {
        public static ScreenTransition Instance { get; private set; }

        public Image overlay;
        public float fadeDuration = 0.5f;
        public Color fadeColor = Color.black;

        float currentAlpha;
        bool isTransitioning;

        public System.Action OnTransitionComplete;

        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(transform.root); return; }
            Instance=this; DontDestroyOnLoad(transform.root);
            if(overlay != null) overlay.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        }

        public void FadeIn()
        {
            if(overlay == null) return;
            isTransitioning = true;
            currentAlpha = 1f;
            overlay.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha);
        }

        public void FadeOut()
        {
            if(overlay == null) return;
            isTransitioning = true;
            currentAlpha = 0f;
            overlay.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha);
        }

        public void FadeToBlack()
        {
            if(overlay == null) return;
            isTransitioning = true;
            currentAlpha = 0f;
            overlay.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha);
        }

        void Update()
        {
            if(!isTransitioning || overlay == null) return;

            float target = overlay.color.a > 0 ? 0f : 1f;
            currentAlpha = Mathf.MoveTowards(currentAlpha, target, Time.deltaTime / fadeDuration);
            overlay.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha);

            if(Mathf.Abs(currentAlpha - target) < 0.01f)
            {
                isTransitioning = false;
                OnTransitionComplete?.Invoke();
            }
        }

        public void SetFadeColor(Color color) => fadeColor = color;
        public bool IsTransitioning => isTransitioning;
    }
}