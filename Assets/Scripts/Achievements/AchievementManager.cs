using UnityEngine;
using System.Collections.Generic;

namespace Shadowfront.Achievements
{
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public bool unlocked;
        public int progress;
        public int target;
        public AchievementType type;
    }
    
    public enum AchievementType { Kills, Missions, Level, Loot, Accuracy, Extraction }
    
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }
        
        public List<Achievement> achievements = new List<Achievement>();
        
        public System.Action<Achievement> OnAchievementUnlocked;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
            InitializeAchievements();
        }
        
        void InitializeAchievements()
        {
            achievements.Add(new Achievement { id=\"kills_10\", title=\"First Blood\", description=\"Kill 10 enemies\", type=AchievementType.Kills, target=10 });
            achievements.Add(new Achievement { id=\"kills_100\", title=\"Veteran\", description=\"Kill 100 enemies\", type=AchievementType.Kills, target=100 });
            achievements.Add(new Achievement { id=\"missions_1\", title=\"First Mission\", description=\"Complete 1 mission\", type=AchievementType.Missions, target=1 });
            achievements.Add(new Achievement { id=\"missions_10\", title=\"Mission Specialist\", description=\"Complete 10 missions\", type=AchievementType.Missions, target=10 });
            achievements.Add(new Achievement { id=\"level_5\", title=\"Rising Star\", description=\"Reach level 5\", type=AchievementType.Level, target=5 });
            achievements.Add(new Achievement { id=\"level_10\", title=\"Elite Operative\", description=\"Reach level 10\", type=AchievementType.Level, target=10 });
            achievements.Add(new Achievement { id=\"loot_50\", title=\"Collector\", description=\"Collect 50 loot items\", type=AchievementType.Loot, target=50 });
            achievements.Add(new Achievement { id=\"extraction_5\", title=\"Ghost\", description=\"Extract 5 times\", type=AchievementType.Extraction, target=5 });
        }
        
        public void TrackKill() => TrackProgress(AchievementType.Kills, 1);
        public void TrackMissionComplete() => TrackProgress(AchievementType.Missions, 1);
        public void TrackLoot() => TrackProgress(AchievementType.Loot, 1);
        public void TrackExtraction() => TrackProgress(AchievementType.Extraction, 1);
        public void TrackLevel(int level) => TrackProgress(AchievementType.Level, level);
        
        void TrackProgress(AchievementType type, int amount)
        {
            foreach(var ach in achievements)
            {
                if(ach.unlocked || ach.type != type) continue;
                ach.progress += amount;
                if(ach.progress >= ach.target)
                {
                    ach.unlocked = true;
                    OnAchievementUnlocked?.Invoke(ach);
                    Debug.Log($\"[Achievement] Unlocked: {ach.title}\");
                }
            }
        }
        
        public bool IsUnlocked(string id)
        {
            var ach = achievements.Find(a => a.id == id);
            return ach?.unlocked ?? false;
        }
        
        public List<Achievement> GetUnlocked() => achievements.FindAll(a => a.unlocked);
        public List<Achievement> GetLocked() => achievements.FindAll(a => !a.unlocked);
    }
}
