using UnityEngine;

namespace Shadowfront.Difficulty
{
    public enum DifficultyLevel { Easy, Normal, Hard, Nightmare }
    
    public class DifficultyManager : MonoBehaviour
    {
        public static DifficultyManager Instance { get; private set; }
        
        public DifficultyLevel currentDifficulty = DifficultyLevel.Normal;
        
        // Multipliers per difficulty
        float[] healthMultipliers = { 0.8f, 1.0f, 1.3f, 1.6f };
        float[] damageMultipliers = { 0.7f, 1.0f, 1.3f, 1.7f };
        float[] speedMultipliers = { 0.9f, 1.0f, 1.1f, 1.25f };
        float[] detectionMultipliers = { 0.8f, 1.0f, 1.2f, 1.4f };
        float[] xpMultipliers = { 0.7f, 1.0f, 1.3f, 1.6f };
        int[] enemyCountMultipliers = { 1, 1, 2, 3 };
        
        public System.Action<DifficultyLevel> OnDifficultyChanged;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
        }
        
        public void SetDifficulty(DifficultyLevel level)
        {
            currentDifficulty = level;
            OnDifficultyChanged?.Invoke(level);
        }
        
        public float GetHealthMultiplier() => healthMultipliers[(int)currentDifficulty];
        public float GetDamageMultiplier() => damageMultipliers[(int)currentDifficulty];
        public float GetSpeedMultiplier() => speedMultipliers[(int)currentDifficulty];
        public float GetDetectionMultiplier() => detectionMultipliers[(int)currentDifficulty];
        public float GetXPMultiplier() => xpMultipliers[(int)currentDifficulty];
        public int GetEnemyCountMultiplier() => enemyCountMultipliers[(int)currentDifficulty];
        
        public void ScaleEnemy(Shadowfront.AI.EnemyData data)
        {
            if(data == null) return;
            int idx = (int)currentDifficulty;
            // Apply level-based scaling on top of difficulty
            int playerLevel = Shadowfront.Data.Progression.Instance?.level ?? 1;
            float levelScale = 1f + (playerLevel - 1) * 0.1f;
            
            data.maxHealth = Mathf.RoundToInt(data.maxHealth * healthMultipliers[idx] * levelScale);
            data.attackDamage = Mathf.RoundToInt(data.attackDamage * damageMultipliers[idx] * levelScale);
            data.moveSpeed *= speedMultipliers[idx];
            data.detectionRange *= detectionMultipliers[idx];
            data.xpReward = Mathf.RoundToInt(data.xpReward * xpMultipliers[idx]);
        }
        
        public string GetDifficultyName() => currentDifficulty.ToString();
    }
}
