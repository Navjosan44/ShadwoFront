using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Shadowfront.Missions
{
    public enum MissionState { NotStarted, Deploying, Active, Extracting, Success, Failed }
    
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }
        public MissionData currentMission;
        public MissionState State { get; private set; }
        
        [Header(\"Extraction\")]
        public Transform extractionPoint;
        public float extractionRadius = 3f;
        float extractionTimer;
        bool isExtracting;
        
        [Header(\"Objectives\")]
        public List<MissionObjective> objectives = new List<MissionObjective>();
        
        [Header(\"Spawner\")]
        public Shadowfront.AI.EnemySpawner spawner;
        
        int killCount;
        int lootCount;
        
        public System.Action<MissionState> OnStateChanged;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
        }
        
        public void StartMission(MissionData mission)
        {
            currentMission = mission;
            State = MissionState.Deploying;
            objectives.Clear();
            isExtracting = false;
            extractionTimer = 0;
            killCount = 0;
            lootCount = 0;
            
            // Copy objectives from mission data
            if(mission?.objectives != null)
            {
                foreach(var obj in mission.objectives)
                    objectives.Add(new MissionObjective { id=obj.id, description=obj.description, type=obj.type, targetAmount=obj.targetAmount, isComplete=false });
            }
            
            OnStateChanged?.Invoke(State);
            Shadowfront.Events.GameEvents.MissionStarted();
            
            // Load mission scene
            if(mission != null && !string.IsNullOrEmpty(mission.sceneName))
                SceneManager.LoadScene(mission.sceneName);
        }
        
        void Update()
        {
            if(State != MissionState.Active && State != MissionState.Extracting) return;
            
            var player = GameObject.FindGameObjectWithTag(\"Player\");
            if(player == null) return;
            
            // Check extraction
            if(extractionPoint != null)
            {
                float dist = Vector3.Distance(player.transform.position, extractionPoint.position);
                if(dist <= extractionRadius)
                {
                    if(!isExtracting)
                    {
                        isExtracting = true;
                        State = MissionState.Extracting;
                        extractionTimer = currentMission?.extractionTime ?? 10f;
                        OnStateChanged?.Invoke(State);
                    }
                    extractionTimer -= Time.deltaTime;
                    float progress = 1f - (extractionTimer / (currentMission?.extractionTime ?? 10f));
                    Shadowfront.Events.GameEvents.ExtractionProgress(progress);
                    if(extractionTimer <= 0) CompleteMission();
                }
                else if(isExtracting)
                {
                    isExtracting = false;
                    State = MissionState.Active;
                    OnStateChanged?.Invoke(State);
                }
            }
            
            CheckObjectives();
        }
        
        void CheckObjectives()
        {
            bool allComplete = true;
            foreach(var obj in objectives)
            {
                if(!obj.isComplete)
                {
                    // Auto-complete based on type
                    if(obj.type == ObjectiveType.Kill && killCount >= obj.targetAmount) obj.isComplete = true;
                    else if(obj.type == ObjectiveType.Collect && lootCount >= obj.targetAmount) obj.isComplete = true;
                    else if(obj.type == ObjectiveType.Extract && isExtracting) obj.isComplete = true;
                }
                if(!obj.isComplete) allComplete = false;
            }
        }
        
        public void AddObjective(MissionObjective obj) => objectives.Add(obj);
        public void CompleteObjective(string id)
        {
            var obj = objectives.Find(o => o.id == id);
            if(obj != null) obj.isComplete = true;
        }
        
        public void RegisterKill()
        {
            killCount++;
            var prog = Shadowfront.Data.Progression.Instance;
            if(prog != null) prog.RegisterKill();
            var ach = Shadowfront.Achievements.AchievementManager.Instance;
            if(ach != null) ach.TrackKill();
            CheckObjectives();
        }
        
        public void RegisterLoot()
        {
            lootCount++;
            var ach = Shadowfront.Achievements.AchievementManager.Instance;
            if(ach != null) ach.TrackLoot();
            CheckObjectives();
        }
        
        void CompleteMission()
        {
            State = MissionState.Success;
            OnStateChanged?.Invoke(State);
            Shadowfront.Events.GameEvents.MissionCompleted();
            
            var prog = Shadowfront.Data.Progression.Instance;
            if(prog != null) prog.RegisterMissionComplete();
            var ach = Shadowfront.Achievements.AchievementManager.Instance;
            if(ach != null) ach.TrackMissionComplete();
            
            GrantRewards();
        }
        
        void GrantRewards()
        {
            if(currentMission?.rewardTable == null) return;
            foreach(var entry in currentMission.rewardTable.entries)
            {
                if(Random.value < entry.weight)
                {
                    int count = Random.Range(entry.minCount, entry.maxCount + 1);
                    Inventory.Instance?.AddItem(entry.itemPrefab, count);
                }
            }
            int xp = 100 * (currentMission?.recommendedLevel ?? 1);
            var diff = Shadowfront.Difficulty.DifficultyManager.Instance;
            if(diff != null) xp = Mathf.RoundToInt(xp * diff.GetXPMultiplier());
            Progression.Instance?.AddXP(xp);
        }
        
        public void FailMission()
        {
            State = MissionState.Failed;
            OnStateChanged?.Invoke(State);
            Shadowfront.Events.GameEvents.MissionFailed();
        }
        
        public float ExtractionProgress => isExtracting ? 1f - (extractionTimer / (currentMission?.extractionTime ?? 10f)) : 0f;
        public int KillCount => killCount;
        public int LootCount => lootCount;
        public List<MissionObjective> Objectives => objectives;
    }
}
