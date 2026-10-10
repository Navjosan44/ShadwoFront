using UnityEngine;
using System.Collections.Generic;

namespace Shadowfront.Data
{
    [CreateAssetMenu(menuName=\"Shadowfront/Weapon Unlock\")]
    public class WeaponUnlock : ScriptableObject
    {
        public string weaponId;
        public Weapons.WeaponData weaponData;
        public int requiredLevel = 1;
        public int cost = 0;
    }
    
    public class Progression : MonoBehaviour
    {
        public static Progression Instance { get; private set; }
        
        [Header(\"Player\")]
        public int level = 1;
        public int xp = 0;
        public int xpToNextLevel = 100;
        
        [Header(\"Unlocks\")]
        public List<WeaponUnlock> availableWeapons = new List<WeaponUnlock>();
        public HashSet<string> unlockedWeapons = new HashSet<string>();
        
        [Header(\"Stats\")]
        public int missionsCompleted = 0;
        public int enemiesKilled = 0;
        public int totalXPEarned = 0;
        public int totalLootCollected = 0;
        public int totalExtractions = 0;
        
        public System.Action<int> OnLevelUp;
        public System.Action<int> OnXPChanged;
        public System.Action<string> OnWeaponUnlocked;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
        }
        
        public void AddXP(int amount)
        {
            xp += amount;
            totalXPEarned += amount;
            Events.GameEvents.XPGained(amount);
            OnXPChanged?.Invoke(xp);
            CheckLevelUp();
        }
        
        void CheckLevelUp()
        {
            while(xp >= xpToNextLevel)
            {
                xp -= xpToNextLevel;
                level++;
                xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
                OnLevelUp?.Invoke(level);
                Events.GameEvents.LevelUp(level);
                var ach = Achievements.AchievementManager.Instance;
                if(ach != null) ach.TrackLevel(level);
                CheckUnlocks();
            }
        }
        
        void CheckUnlocks()
        {
            foreach(var w in availableWeapons)
            {
                if(!unlockedWeapons.Contains(w.weaponId) && level >= w.requiredLevel)
                {
                    unlockedWeapons.Add(w.weaponId);
                    OnWeaponUnlocked?.Invoke(w.weaponId);
                    Events.GameEvents.WeaponUnlocked(w.weaponId);
                }
            }
        }
        
        public bool IsWeaponUnlocked(string id) => unlockedWeapons.Contains(id);
        public void UnlockWeapon(string id) { unlockedWeapons.Add(id); OnWeaponUnlocked?.Invoke(id); }
        
        public void RegisterKill() { enemiesKilled++; }
        public void RegisterMissionComplete() { missionsCompleted++; }
        public void RegisterLoot() { totalLootCollected++; }
        public void RegisterExtraction() { totalExtractions++; }
    }
}
