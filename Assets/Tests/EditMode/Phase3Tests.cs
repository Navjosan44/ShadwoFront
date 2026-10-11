using NUnit.Framework;
using UnityEngine;
using Shadowfront.Graphics;
using Shadowfront.Accessibility;
using Shadowfront.Optimization;
using Shadowfront.Release;
using Shadowfront.Loading;
using Shadowfront.UI.Transitions;

public class Phase3Tests
{
    [Test]
    public void GraphicsManager_InitializesPresets()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GraphicsManager>();
        Assert.IsTrue(gm.presets.Length > 0);
        Assert.AreEqual(3, gm.presets.Length);
    }

    [Test]
    public void GraphicsManager_SetQuality_ChangesPreset()
    {
        var go = new GameObject();
        var gm = go.AddComponent<GraphicsManager>();
        gm.SetQuality(0);
        Assert.AreEqual(0, gm.currentPresetIndex);
    }

    [Test]
    public void AccessibilityManager_ColorblindModes()
    {
        var go = new GameObject();
        var am = go.AddComponent<AccessibilityManager>();
        am.SetColorblindMode(ColorblindMode.Protanopia);
        Assert.AreEqual(ColorblindMode.Protanopia, am.settings.colorblindMode);
    }

    [Test]
    public void AccessibilityManager_UIScaleClamped()
    {
        var go = new GameObject();
        var am = go.AddComponent<AccessibilityManager>();
        am.SetUIScale(5f);
        Assert.LessOrEqual(am.settings.uiScale, 2f);
        am.SetUIScale(0.1f);
        Assert.GreaterOrEqual(am.settings.uiScale, 0.8f);
    }

    [Test]
    public void ObjectPool_SpawnDespawn()
    {
        var go = new GameObject();
        var pool = go.AddComponent<ObjectPool>();
        pool.pools = new ObjectPool.Pool[]
        {
            new ObjectPool.Pool { tag = "Test", prefab = new GameObject("Test"), size = 5 }
        };
        pool.CreatePools();

        var obj = pool.Spawn("Test", Vector3.zero, Quaternion.identity);
        Assert.IsNotNull(obj);
        pool.Despawn("Test", obj);
    }

    [Test]
    public void PerformanceMonitor_TracksFPS()
    {
        var go = new GameObject();
        var pm = go.AddComponent<PerformanceMonitor>();
        pm.SetTargetFPS(60);
        Assert.AreEqual(60, Application.targetFrameRate);
    }

    [Test]
    public void ReleaseManager_VersionFormat()
    {
        var go = new GameObject();
        var rm = go.AddComponent<ReleaseManager>();
        Assert.IsTrue(rm.GetFullVersion().Contains("v"));
        Assert.IsTrue(rm.GetFullVersion().Contains("Build"));
    }

    [Test]
    public void ReleaseManager_VerifySaveIntegrity()
    {
        var go = new GameObject();
        var rm = go.AddComponent<ReleaseManager>();
        Assert.IsTrue(rm.VerifySaveIntegrity("{\"version\":1}"));
        Assert.IsFalse(rm.VerifySaveIntegrity(""));
    }

    [Test]
    public void LoadingManager_SetProgress()
    {
        var go = new GameObject();
        var lm = go.AddComponent<LoadingManager>();
        lm.SetProgress(0.5f);
        Assert.AreEqual(0.5f, lm.GetProgress());
    }

    [Test]
    public void LocalizationManager_GetUnknownKey()
    {
        var go = new GameObject();
        var lm = go.AddComponent<LocalizationManager>();
        string result = lm.Get("nonexistent_key");
        Assert.IsTrue(result.Contains("["));
    }

    [Test]
    public void ScreenTransition_Fade()
    {
        var go = new GameObject();
        var st = go.AddComponent<ScreenTransition>();
        st.FadeToBlack();
        Assert.IsTrue(st.IsTransitioning);
    }
}
