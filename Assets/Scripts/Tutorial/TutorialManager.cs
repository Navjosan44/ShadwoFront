using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Shadowfront.Tutorial
{
    [System.Serializable]
    public class TutorialStep
    {
        public string id;
        public string title;
        public string description;
        public TutorialTrigger trigger;
        public string targetTag;
        public float displayDuration = 4f;
        public bool requiresAction;
    }
    
    public enum TutorialTrigger { OnStart, OnMove, OnAim, OnFire, OnReload, OnEnemy, OnLoot, OnExtraction, OnCrouch }
    
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }
        
        public List<TutorialStep> steps = new List<TutorialStep>();
        public GameObject tutorialPanel;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        
        int currentStep = 0;
        bool isActive;
        float displayTimer;
        
        public System.Action<TutorialStep> OnStepShown;
        public System.Action OnTutorialComplete;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
            InitializeSteps();
        }
        
        void InitializeSteps()
        {
            steps.Add(new TutorialStep { id=\"move\", title=\"Move\", description=\"Use left joystick or WASD to move\", trigger=TutorialTrigger.OnStart, displayDuration=5f });
            steps.Add(new TutorialStep { id=\"aim\", title=\"Aim\", description=\"Use right joystick or mouse to aim\", trigger=TutorialTrigger.OnMove, displayDuration=5f });
            steps.Add(new TutorialStep { id=\"fire\", title=\"Fire\", description=\"Tap fire button or left mouse to shoot\", trigger=TutorialTrigger.OnAim, displayDuration=5f });
            steps.Add(new TutorialStep { id=\"reload\", title=\"Reload\", description=\"Press reload button or R to reload\", trigger=TutorialTrigger.OnFire, displayDuration=5f });
            steps.Add(new TutorialStep { id=\"enemy\", title=\"Enemies\", description=\"Watch for enemies. Use cover and aim for headshots\", trigger=TutorialTrigger.OnReload, displayDuration=6f });
            steps.Add(new TutorialStep { id=\"loot\", title=\"Loot\", description=\"Collect loot from defeated enemies\", trigger=TutorialTrigger.OnEnemy, displayDuration=5f });
            steps.Add(new TutorialStep { id=\"extract\", title=\"Extract\", description=\"Reach the extraction point to complete the mission\", trigger=TutorialTrigger.OnLoot, displayDuration=6f });
        }
        
        public void StartTutorial()
        {
            isActive = true;
            currentStep = 0;
            ShowStep(steps[0]);
        }
        
        public void StopTutorial()
        {
            isActive = false;
            tutorialPanel?.SetActive(false);
        }
        
        public void NotifyTrigger(TutorialTrigger trigger)
        {
            if(!isActive || currentStep >= steps.Count) return;
            
            var step = steps[currentStep];
            if(step.trigger == trigger)
            {
                currentStep++;
                if(currentStep < steps.Count)
                {
                    ShowStep(steps[currentStep]);
                }
                else
                {
                    CompleteTutorial();
                }
            }
        }
        
        void ShowStep(TutorialStep step)
        {
            tutorialPanel?.SetActive(true);
            if(titleText != null) titleText.text = step.title;
            if(descriptionText != null) descriptionText.text = step.description;
            displayTimer = step.displayDuration;
            OnStepShown?.Invoke(step);
        }
        
        void Update()
        {
            if(!isActive || currentStep >= steps.Count) return;
            
            displayTimer -= Time.deltaTime;
            if(displayTimer <= 0)
            {
                currentStep++;
                if(currentStep < steps.Count)
                    ShowStep(steps[currentStep]);
                else
                    CompleteTutorial();
            }
        }
        
        void CompleteTutorial()
        {
            isActive = false;
            tutorialPanel?.SetActive(false);
            OnTutorialComplete?.Invoke();
        }
        
        public bool IsActive => isActive;
        public int CurrentStep => currentStep;
    }
}
