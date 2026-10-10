#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Shadowfront.AI;

public static class EnemyDefinitions
{
    [MenuItem(\"Shadowfront/Generate/Enemy Definitions\")]
    public static void Generate()
    {
        CreateEnemy(EnemyArchetype.Grunt, \"Grunt\", 50, 3.5f, 10, 3f, 20f, 1f, 0.6f, 25, 10, true, true, 15f);
        CreateEnemy(EnemyArchetype.Heavy, \"Heavy\", 150, 2f, 20, 5f, 15f, 1.5f, 0.5f, 60, 25, true, true, 10f);
        CreateEnemy(EnemyArchetype.Scout, \"Scout\", 30, 5f, 6, 2.5f, 30f, 0.6f, 0.7f, 20, 15, false, true, 20f);
        CreateEnemy(EnemyArchetype.Sniper, \"Sniper\", 40, 1f, 40, 40f, 80f, 3f, 0.8f, 50, 30, false, false, 8f);
        
        AssetDatabase.Refresh();
        Debug.Log(\"[Shadowfront] Enemy definitions generated\");
    }
    
    static void CreateEnemy(EnemyArchetype arch, string name, int hp, float speed, int dmg, float atkRange, float detRange, float atkCD, float acc, int xp, int credits, bool cover, bool pursue, float patrol)
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.archetype = arch;
        data.displayName = name;
        data.maxHealth = hp;
        data.moveSpeed = speed;
        data.attackDamage = dmg;
        data.attackRange = atkRange;
        data.detectionRange = detRange;
        data.attackCooldown = atkCD;
        data.accuracy = acc;
        data.xpReward = xp;
        data.creditReward = credits;
        data.usesCover = cover;
        data.canPursue = pursue;
        data.patrolRadius = patrol;
        data.lootChance = 0.5f;
        
        string path = $\"Assets/Resources/Enemies/{name}.asset\";
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        AssetDatabase.CreateAsset(data, path);
    }
}
#endif
