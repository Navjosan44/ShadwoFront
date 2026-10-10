#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class SceneSetup
{
    [MenuItem(\"Shadowfront/Setup Main Scene\")]
    public static void SetupMainScene()
    {
        var scene = SceneManager.OpenScene(\"Assets/Scenes/Main.unity\");
        SetupScene(scene);
    }
    
    [MenuItem(\"Shadowfront/Setup Main Menu Scene\")]
    public static void SetupMainMenuScene()
    {
        var scene = SceneManager.OpenScene(\"Assets/Scenes/MainMenu.unity\");
        SetupMenuScene(scene);
    }
    
    static void SetupScene(Scene scene)
    {
        var urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(\"Assets/Settings/URPAsset.asset\");
        if(urpAsset != null)
        {
            GraphicsSettings.defaultRenderPipeline = urpAsset;
            QualitySettings.renderPipeline = urpAsset;
        }
        
        // Player
        var player = new GameObject(\"Player\");
        player.tag = \"Player\";
        var cc = player.AddComponent<CharacterController>();
        cc.height = 2f; cc.radius = 0.3f; cc.center = new Vector3(0, 1, 0);
        var pc = player.AddComponent<Shadowfront.Player.PlayerController>();
        var health = player.AddComponent<Shadowfront.Core.Health>();
        health.max = 100;
        
        var pivot = new GameObject(\"CameraPivot\");
        pivot.transform.SetParent(player.transform);
        pivot.transform.localPosition = new Vector3(0, 1.6f, 0);
        pc.cameraPivot = pivot.transform;
        
        var camObj = GameObject.Find(\"Main Camera\");
        if(camObj == null) camObj = new GameObject(\"Main Camera\");
        camObj.tag = \"MainCamera\";
        var cam = camObj.GetComponent<Camera>();
        if(cam == null) cam = camObj.AddComponent<Camera>();
        camObj.transform.SetParent(pivot.transform);
        camObj.transform.localPosition = new Vector3(0, 0, -4);
        cam.fieldOfView = 60;
        if(camObj.GetComponent<AudioListener>() == null) camObj.AddComponent<AudioListener>();
        
        var weaponHolder = new GameObject(\"WeaponHolder\");
        weaponHolder.transform.SetParent(camObj.transform);
        weaponHolder.transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f);
        
        var wm = player.AddComponent<Shadowfront.Weapons.WeaponManager>();
        
        var input = new GameObject(\"InputManager\");
        input.AddComponent<Shadowfront.Input.InputManager>();
        
        var gm = new GameObject(\"Managers\");
        gm.AddComponent<Shadowfront.Core.GameManager>();
        gm.AddComponent<Shadowfront.Core.Bootstrap>();
        gm.AddComponent<Shadowfront.Missions.MissionManager>();
        gm.AddComponent<Shadowfront.Data.Inventory>();
        gm.AddComponent<Shadowfront.Data.Progression>();
        gm.AddComponent<Shadowfront.Data.SaveManager>();
        gm.AddComponent<Shadowfront.Audio.AudioManager>();
        gm.AddComponent<Shadowfront.Difficulty.DifficultyManager>();
        gm.AddComponent<Shadowfront.Achievements.AchievementManager>();
        gm.AddComponent<Shadowfront.Tutorial.TutorialManager>();
        gm.AddComponent<Shadowfront.Graphics.GraphicsManager>();
        gm.AddComponent<Shadowfront.Accessibility.AccessibilityManager>();
        gm.AddComponent<Shadowfront.Accessibility.SubtitleManager>();
        gm.AddComponent<Shadowfront.Optimization.PerformanceMonitor>();
        gm.AddComponent<Shadowfront.Optimization.ObjectPool>();
        gm.AddComponent<Shadowfront.Release.ReleaseManager>();
        gm.AddComponent<Shadowfront.Loading.LoadingManager>();
        gm.AddComponent<Shadowfront.Localization.LocalizationManager>();
        gm.AddComponent<Shadowfront.UI.Transitions.ScreenTransition>();
        gm.AddComponent<Shadowfront.Cloud.CloudSaveManager>();
        gm.AddComponent<Shadowfront.Auth.AuthManager>();
        gm.AddComponent<Shadowfront.Leaderboards.LeaderboardManager>();
        gm.AddComponent<Shadowfront.Multiplayer.MultiplayerManager>();
        
        // Canvas
        var canvas = new GameObject(\"Canvas\");
        canvas.layer = LayerMask.NameToLayer(\"UI\");
        var cv = canvas.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 100;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        var hudObj = new GameObject(\"HUD\");
        hudObj.transform.SetParent(canvas.transform);
        hudObj.AddComponent<Shadowfront.UI.HUD>();
        
        var pauseObj = new GameObject(\"PauseMenu\");
        pauseObj.transform.SetParent(canvas.transform);
        pauseObj.AddComponent<Shadowfront.UI.PauseMenu>();
        
        var resultsObj = new GameObject(\"ResultsScreen\");
        resultsObj.transform.SetParent(canvas.transform);
        resultsObj.AddComponent<Shadowfront.UI.ResultsScreen>();
        
        var menuObj = new GameObject(\"MainMenu\");
        menuObj.transform.SetParent(canvas.transform);
        menuObj.AddComponent<Shadowfront.UI.MainMenu>();
        
        var invObj = new GameObject(\"InventoryScreen\");
        invObj.transform.SetParent(canvas.transform);
        invObj.AddComponent<Shadowfront.UI.Screens.InventoryScreen>();
        
        var wsObj = new GameObject(\"WeaponSelectScreen\");
        wsObj.transform.SetParent(canvas.transform);
        wsObj.AddComponent<Shadowfront.UI.Screens.WeaponSelectScreen>();
        
        var tutObj = new GameObject(\"TutorialPanel\");
        tutObj.transform.SetParent(canvas.transform);
        
        // NavMesh
        var nm = new GameObject(\"NavMesh\");
        nm.AddComponent<UnityEngine.AI.NavMeshSurface>();
        
        // Ground
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = \"Ground\";
        ground.transform.localScale = new Vector3(10, 1, 10);
        var groundMat = new Material(Shader.Find(\"Universal Render Pipeline/Lit\"));
        groundMat.color = new Color(0.3f, 0.3f, 0.3f);
        ground.GetComponent<Renderer>().material = groundMat;
        ground.isStatic = true;
        
        // Cover objects
        for(int i=0;i<8;i++)
        {
            var cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cover.name = $\"Cover_{i}\";
            cover.transform.position = new Vector3(Random.Range(-30,30), 1, Random.Range(-30,30));
            cover.transform.localScale = new Vector3(2, 2, 2);
            cover.isStatic = true;
            var cmat = new Material(Shader.Find(\"Universal Render Pipeline/Lit\"));
            cmat.color = new Color(0.4f, 0.35f, 0.3f);
            cover.GetComponent<Renderer>().material = cmat;
        }
        
        // Enemy spawner
        var spawnerObj = new GameObject(\"EnemySpawner\");
        var spawner = spawnerObj.AddComponent<Shadowfront.AI.EnemySpawner>();
        spawner.maxEnemies = 10;
        spawner.spawnRadius = 30f;
        
        // Spawn enemies
        for(int i=0;i<5;i++)
        {
            var enemy = new GameObject($\"Enemy_{i}\");
            enemy.transform.position = new Vector3(Random.Range(-20,20), 1, Random.Range(-20,20));
            enemy.tag = \"Enemy\";
            var eagent = enemy.AddComponent<UnityEngine.AI.NavMeshAgent>();
            eagent.radius = 0.5f; eagent.height = 1.8f;
            var ehealth = enemy.AddComponent<Shadowfront.Core.Health>();
            ehealth.max = 50;
            var eai = enemy.AddComponent<Shadowfront.AI.EnemyAI>();
            enemy.AddComponent<Animator>();
            var cap = enemy.AddComponent<CapsuleCollider>();
            cap.height = 1.8f; cap.radius = 0.5f;
        }
        
        // Extraction point
        var extract = new GameObject(\"ExtractionPoint\");
        extract.transform.position = new Vector3(0, 1, 40);
        var extractTrigger = extract.AddComponent<SphereCollider>();
        extractTrigger.radius = 3f; extractTrigger.isTrigger = true;
        
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log(\"[Shadowfront] Main scene setup complete\");
    }
    
    static void SetupMenuScene(Scene scene)
    {
        var urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(\"Assets/Settings/URPAsset.asset\");
        if(urpAsset != null)
        {
            GraphicsSettings.defaultRenderPipeline = urpAsset;
            QualitySettings.renderPipeline = urpAsset;
        }
        
        var gm = new GameObject(\"Managers\");
        gm.AddComponent<Shadowfront.Core.GameManager>();
        gm.AddComponent<Shadowfront.Core.Bootstrap>();
        gm.AddComponent<Shadowfront.Data.Inventory>();
        gm.AddComponent<Shadowfront.Data.Progression>();
        gm.AddComponent<Shadowfront.Data.SaveManager>();
        gm.AddComponent<Shadowfront.Audio.AudioManager>();
        gm.AddComponent<Shadowfront.Difficulty.DifficultyManager>();
        gm.AddComponent<Shadowfront.Achievements.AchievementManager>();
        
        var input = new GameObject(\"InputManager\");
        input.AddComponent<Shadowfront.Input.InputManager>();
        
        var canvas = new GameObject(\"Canvas\");
        canvas.layer = LayerMask.NameToLayer(\"UI\");
        var cv = canvas.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 100;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        var menuObj = new GameObject(\"MainMenu\");
        menuObj.transform.SetParent(canvas.transform);
        menuObj.AddComponent<Shadowfront.UI.MainMenu>();
        
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log(\"[Shadowfront] MainMenu scene setup complete\");
    }
}
#endif


