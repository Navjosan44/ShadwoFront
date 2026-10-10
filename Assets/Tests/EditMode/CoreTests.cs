using NUnit.Framework;
using UnityEngine;
using Shadowfront.Core;
using Shadowfront.Data;
using Shadowfront.Weapons;
using Shadowfront.AI;
using Shadowfront.Missions;
using Shadowfront.Difficulty;
using Shadowfront.Achievements;

public class CoreTests
{
    [Test]
    public void Health_TakeDamage_ReducesCurrent()
    {
        var go = new GameObject();
        var h = go.AddComponent<Health>();
        h.max = 100;
        h.TakeDamage(30);
        Assert.AreEqual(70, h.current);
    }
    
    [Test]
    public void Health_TakeDamage_ZeroOrLess_Dies()
    {
        var go = new GameObject();
        var h = go.AddComponent<Health>();
        h.max = 100;
        h.TakeDamage(150);
        Assert.IsFalse(go.activeInHierarchy);
    }
    
    [Test]
    public void Health_Heal_IncreasesCurrent()
    {
        var go = new GameObject();
        var h = go.AddComponent<Health>();
        h.max = 100;
        h.TakeDamage(30);
        h.Heal(20);
        Assert.AreEqual(90, h.current);
    }
    
    [Test]
    public void WeaponData_CanBeCreated()
    {
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.weaponName = \"Test Rifle\";
        data.damage = 25;
        data.magazineSize = 30;
        Assert.AreEqual(\"Test Rifle\", data.weaponName);
        Assert.AreEqual(25, data.damage);
    }
    
    [Test]
    public void EnemyData_CanBeCreated()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.displayName = \"Grunt\";
        data.archetype = EnemyArchetype.Grunt;
        data.maxHealth = 50;
        Assert.AreEqual(EnemyArchetype.Grunt, data.archetype);
        Assert.AreEqual(50, data.maxHealth);
    }
    
    [Test]
    public void MissionData_CanBeCreated()
    {
        var data = ScriptableObject.CreateInstance<MissionData>();
        data.missionName = \"Test Mission\";
        data.recommendedLevel = 1;
        data.extractionTime = 10f;
        Assert.AreEqual(\"Test Mission\", data.missionName);
        Assert.AreEqual(1, data.recommendedLevel);
    }
    
    [Test]
    public void SaveData_VersionSerialization()
    {
        var data = new SaveData { version = 1 };
        string json = JsonUtility.ToJson(data);
        var loaded = JsonUtility.FromJson<SaveData>(json);
        Assert.AreEqual(1, loaded.version);
    }
    
    [Test]
    public void DifficultyManager_Multipliers()
    {
        var go = new GameObject();
        var dm = go.AddComponent<DifficultyManager>();
        dm.SetDifficulty(DifficultyLevel.Hard);
        Assert.Greater(dm.GetHealthMultiplier(), 1f);
        Assert.Greater(dm.GetDamageMultiplier(), 1f);
    }
    
    [Test]
    public void Achievement_TrackProgress_Unlocks()
    {
        var go = new GameObject();
        var am = go.AddComponent<AchievementManager>();
        am.TrackKill();
        // First Blood requires 10 kills
        Assert.IsFalse(am.IsUnlocked(\"kills_10\"));
        for(int i=0;i<9;i++) am.TrackKill();
        Assert.IsTrue(am.IsUnlocked(\"kills_10\"));
    }
}
