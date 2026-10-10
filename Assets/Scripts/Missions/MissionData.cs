using UnityEngine;

namespace Shadowfront.Missions
{
    [CreateAssetMenu(menuName=\"Shadowfront/Mission Data\")]
    public class MissionData : ScriptableObject
    {
        public string missionName = \"Mission\";
        public string description;
        public string sceneName;
        public int recommendedLevel = 1;
        public int minEnemyLevel = 1;
        public int maxEnemyLevel = 3;
        public LootTable rewardTable;
        public float extractionTime = 10f;
        public int maxEnemies = 10;
        public int targetKills = 0;
        public int targetLoot = 0;
        public MissionObjective[] objectives;
    }
    
    [System.Serializable]
    public class MissionObjective
    {
        public string id;
        public string description;
        public ObjectiveType type;
        public int targetAmount;
        public bool isComplete;
    }
    
    public enum ObjectiveType { Kill, Collect, Reach, Survive, Interact, Extract }
}
