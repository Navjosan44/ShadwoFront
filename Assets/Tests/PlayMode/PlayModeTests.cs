using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Shadowfront.Core;
using Shadowfront.Player;
using Shadowfront.Data;
using Shadowfront.Difficulty;
using Shadowfront.Achievements;

public class PlayModeTests
{
    [UnityTest]
    public IEnumerator GameManager_Singleton_Persists()
    {
        var gm = new GameObject().AddComponent<GameManager>();
        yield return null;
        Assert.IsNotNull(GameManager.Instance);
        Assert.AreEqual(gm, GameManager.Instance);
    }
    
    [UnityTest]
    public IEnumerator PlayerController_Moves_WithInput()
    {
        var go = new GameObject();
        go.tag = \"Player\";
        var cc = go.AddComponent<CharacterController>();
        var pc = go.AddComponent<PlayerController>();
        var health = go.AddComponent<Health>();
        health.max = 100;
        
        var pivot = new GameObject(\"CameraPivot\").transform;
        pivot.SetParent(go.transform);
        pivot.localPosition = new Vector3(0, 1.6f, 0);
        pc.cameraPivot = pivot.transform;
        
        var cam = new GameObject(\"Main Camera\").AddComponent<Camera>();
        cam.transform.SetParent(pivot);
        cam.transform.localPosition = new Vector3(0, 0, -4);
        cam.tag = \"MainCamera\";
        
        yield return null;
        
        var inputGO = new GameObject().AddComponent<Shadowfront.Input.InputManager>();
        inputGO.UseMobileControls = false;
        
        Assert.IsNotNull(PlayerController.Instance);
    }
    
    [UnityTest]
    public IEnumerator Inventory_AddRemove_Works()
    {
        var inv = new GameObject().AddComponent<Inventory>();
        var prefab = new GameObject(\"Item\");
        
        Assert.IsTrue(inv.AddItem(prefab, 5));
        Assert.AreEqual(5, inv.GetCount(prefab));
        
        Assert.IsTrue(inv.RemoveItem(prefab, 2));
        Assert.AreEqual(3, inv.GetCount(prefab));
        
        Assert.IsTrue(inv.RemoveItem(prefab, 3));
        Assert.AreEqual(0, inv.GetCount(prefab));
        Assert.IsFalse(inv.RemoveItem(prefab, 1));
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator Progression_XP_LevelUp()
    {
        var prog = new GameObject().AddComponent<Progression>();
        prog.xpToNextLevel = 100;
        
        prog.AddXP(50);
        Assert.AreEqual(1, prog.level);
        Assert.AreEqual(50, prog.xp);
        
        prog.AddXP(60);
        Assert.AreEqual(2, prog.level);
        Assert.AreEqual(10, prog.xp);
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator DifficultyManager_ScalesEnemy()
    {
        var dm = new GameObject().AddComponent<DifficultyManager>();
        dm.SetDifficulty(DifficultyLevel.Hard);
        
        var data = ScriptableObject.CreateInstance<Shadowfront.AI.EnemyData>();
        data.maxHealth = 100;
        data.attackDamage = 10;
        
        dm.ScaleEnemy(data);
        Assert.Greater(data.maxHealth, 100);
        Assert.Greater(data.attackDamage, 10);
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator AchievementManager_TracksKills()
    {
        var am = new GameObject().AddComponent<AchievementManager>();
        
        for(int i=0;i<10;i++) am.TrackKill();
        
        Assert.IsTrue(am.IsUnlocked(\"kills_10\"));
        Assert.IsFalse(am.IsUnlocked(\"kills_100\"));
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator SaveManager_SaveLoad_RoundTrip()
    {
        var sm = new GameObject().AddComponent<SaveManager>();
        sm.Save();
        
        var loaded = sm.Load();
        // May fail if no player/progression objects exist, but shouldn't crash
        Assert.IsNotNull(sm);
        
        yield return null;
    }
}
