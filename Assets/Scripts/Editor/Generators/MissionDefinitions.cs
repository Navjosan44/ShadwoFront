#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Shadowfront.Missions;

public static class MissionDefinitions
{
    [MenuItem(\"Shadowfront/Generate/Mission Definitions\")]
    public static void Generate()
    {
        CreateMission(\"Training Grounds\", \"Complete basic training. Eliminate all hostiles.\", \"Main\", 1, 1, 1, 10f, 8, 0, 0);
        CreateMission(\"Sector 7\", \"Infiltrate Sector 7. Collect intel and extract.\", \"Main\", 2, 2, 3, 15f, 12, 0, 3);
        CreateMission(\"The Docks\", \"Secure the docks. Heavy resistance expected.\", \"Main\", 3, 3, 5, 20f, 15, 0, 5);
        CreateMission(\"Industrial Zone\", \"Destroy the facility. Survive and extract.\", \"Main\", 4, 4, 6, 25f, 20, 0, 8);
        
        AssetDatabase.Refresh();
        Debug.Log(\"[Shadowfront] Mission definitions generated\");
    }
    
    static void CreateMission(string name, string desc, string scene, int level, int minLvl, int maxLvl, float extractTime, int maxEnemies, int targetKills, int targetLoot)
    {
        var data = ScriptableObject.CreateInstance<MissionData>();
        data.missionName = name;
        data.description = desc;
        data.sceneName = scene;
        data.recommendedLevel = level;
        data.minEnemyLevel = minLvl;
        data.maxEnemyLevel = maxLvl;
        data.extractionTime = extractTime;
        data.maxEnemies = maxEnemies;
        data.targetKills = targetKills;
        data.targetLoot = targetLoot;
        
        string path = $\"Assets/Resources/Missions/{name.Replace(\" \", \"_\")}.asset\";
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        AssetDatabase.CreateAsset(data, path);
    }
}
#endif
