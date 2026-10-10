using UnityEngine;

namespace Shadowfront.AI
{
    public enum EnemyArchetype { Grunt, Heavy, Scout, Sniper }
    
    [CreateAssetMenu(menuName=\"Shadowfront/Enemy Data\")]
    public class EnemyData : ScriptableObject
    {
        public EnemyArchetype archetype = EnemyArchetype.Grunt;
        public string displayName = \"Grunt\";
        public GameObject prefab;
        
        [Header(\"Stats\")]
        public int maxHealth = 50;
        public float moveSpeed = 3.5f;
        public float attackDamage = 10;
        public float attackRange = 3f;
        public float detectionRange = 20f;
        public float attackCooldown = 1f;
        public float accuracy = 0.6f;
        public int xpReward = 25;
        public int creditReward = 10;
        
        [Header(\"Loot\")]
        public LootTable lootTable;
        public float lootChance = 0.5f;
        
        [Header(\"Behavior\")]
        public bool usesCover = true;
        public bool canPursue = true;
        public float patrolRadius = 15f;
        public float alertDuration = 5f;
    }
}
