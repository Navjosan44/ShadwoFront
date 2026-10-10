using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TextPro = UnityEngine.TextMesh;

namespace Shadowfront.Accessibility
{
    public class SubtitleManager : MonoBehaviour
    {
        public static SubtitleManager Instance { get; private set; }

        public Canvas subtitleCanvas;
        public Text subtitleText;
        public float defaultDuration = 4f;
        public int fontSize = 24;

        Queue<SubtitleEntry> subtitleQueue = new Queue<SubtitleEntry>();
        float displayTimer;
        bool isDisplaying;

        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(transform.root); return; }
            Instance=this; DontDestroyOnLoad(transform.root);
        }

        public void ShowSubtitle(string text, float duration = -1)
        {
            if(subtitleText == null) return;
            if(duration < 0) duration = defaultDuration;

            subtitleText.text = text;
            subtitleText.fontSize = fontSize;
            subtitleCanvas?.enabled = true;
            displayTimer = duration;
            isDisplaying = true;
        }

        void Update()
        {
            if(!isDisplaying) return;
            displayTimer -= Time.deltaTime;
            if(displayTimer <= 0)
            {
                subtitleText.text = "";
                subtitleCanvas?.enabled = false;
                isDisplaying = false;
            }
        }

        public void SetFontSize(int size)
        {
            fontSize = size;
            if(subtitleText != null) subtitleText.fontSize = size;
        }

        public void SetEnabled(bool enabled)
        {
            if(subtitleCanvas != null) subtitleCanvas.enabled = enabled;
        }
    }

    public class SubtitleEntry
    {
        public string text;
        public float duration;
    }
}