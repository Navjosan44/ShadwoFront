using UnityEngine;
using System;

namespace Shadowfront.Events
{
    public static class GameEvents
    {
        // Player
        public static event Action<int> OnPlayerDamaged;
        public static event Action OnPlayerDied;
        public static event Action OnPlayerHealed;
        
        // Combat
        public static event Action<Vector3> OnEnemyKilled;
        public static event Action<int> OnEnemyDamaged;
        public static event Action OnShotFired;
        public static event Action OnReload;
        
        // Mission
        public static event Action OnMissionStarted;
        public static event Action OnMissionCompleted;
        public static event Action OnMissionFailed;
        public static event Action<float> OnExtractionProgress;
        
        // Progression
        public static event Action<int> OnXPGained;
        public static event Action<int> OnLevelUp;
        public static event Action<string> OnWeaponUnlocked;
        
        // Loot
        public static event Action<GameObject> OnLootCollected;
        public static event Action OnInventoryChanged;
        
        // Achievement
        public static event Action<string> OnAchievementUnlocked;
        
        // Player
        public static void PlayerDamaged(int dmg) => OnPlayerDamaged?.Invoke(dmg);
        public static void PlayerDied() => OnPlayerDied?.Invoke();
        public static void PlayerHealed() => OnPlayerHealed?.Invoke();
        
        // Combat
        public static void EnemyKilled(Vector3 pos) => OnEnemyKilled?.Invoke(pos);
        public static void EnemyDamaged(int dmg) => OnEnemyDamaged?.Invoke(dmg);
        public static void ShotFired() => OnShotFired?.Invoke();
        public static void Reload() => OnReload?.Invoke();
        
        // Mission
        public static void MissionStarted() => OnMissionStarted?.Invoke();
        public static void MissionCompleted() => OnMissionCompleted?.Invoke();
        public static void MissionFailed() => OnMissionFailed?.Invoke();
        public static void ExtractionProgress(float p) => OnExtractionProgress?.Invoke(p);
        
        // Progression
        public static void XPGained(int xp) => OnXPGained?.Invoke(xp);
        public static void LevelUp(int lvl) => OnLevelUp?.Invoke(lvl);
        public static void WeaponUnlocked(string id) => OnWeaponUnlocked?.Invoke(id);
        
        // Loot
        public static void LootCollected(GameObject item) => OnLootCollected?.Invoke(item);
        public static void InventoryChanged() => OnInventoryChanged?.Invoke();
        
        // Achievement
        public static void AchievementUnlocked(string id) => OnAchievementUnlocked?.Invoke(id);
    }
}
