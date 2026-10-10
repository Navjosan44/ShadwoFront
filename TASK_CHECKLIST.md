SHADOWFRONT - Task Checklist

PHASE 1 (Complete):
[+] Environment inspected (no existing Unity project, no Unity.exe in PATH)
[+] Project structure created (C:\SHADOWFRONT)
[+] Packages configured (URP, InputSystem, AI Nav, UGUI, Test Framework, Animation Rigging)
[+] ProjectSettings initialized (version, PlayerSettings stub)
[+] URP Asset created
[+] Core scripts (GameManager, Health, Bootstrap)
[+] Input System (InputManager: mobile virtual joystick + camera/aim, keyboard fallback)
[+] Player Controller (3rd person movement, camera orbit, aim FOV, crouch)
[+] Enemy AI (Patrol/Alert/Chase/Attack/Cover/Dead, cover search, LOS)
[+] Weapon System (WeaponData SO, Gun with recoil/reload/ammo, WeaponManager switching, MuzzleFlash)
[+] Mission Loop (MissionManager: Deploy/Active/Extracting/Success/Failed, objectives, extraction, rewards)
[+] Inventory (capacity, stacks, events)
[+] Progression (XP, levels, weapon unlocks, stats)
[+] Save System (versioned JSON, atomic write, backup recovery, auto-save on pause/focus loss)
[+] Loot System (LootTable SO, LootItem pickup)
[+] UI (HUD, MainMenu, PauseMenu, ResultsScreen)
[+] Audio Manager (spatial, mixer groups)
[+] Editor SceneSetup (auto-populates gameplay scene via menu)
[+] Tests (EditMode CoreTests, PlayMode PlayModeTests)
[+] Two Scenes (MainMenu, Main)
[+] Assembly Definition
[+] Documentation (README, BUILD_INSTRUCTIONS, PROJECT_MANIFEST, TASK_CHECKLIST, COMPLETION_REPORT)

PHASE 2 (Complete):
[+] Enemy Archetypes: GruntAI (basic ranged), HeavyAI (tank, burst fire), ScoutAI (fast, flank), SniperAI (long range, telegraphed aim)
[+] EnemyData ScriptableObject (archetype, stats, behavior, loot)
[+] EnemySpawner (wave spawning, max enemies, spawn points, spawn interval)
[+] DifficultyManager (Easy/Normal/Hard/Nightmare, health/damage/speed/detection/XP multipliers, enemy scaling)
[+] Multiple Weapons: Pistol, Rifle, SMG, Shotgun, Sniper (via WeaponDefinitions generator)
[+] Multiple Missions: Training Grounds, Sector 7, The Docks, Industrial Zone (via MissionDefinitions generator)
[+] MissionData with objectives array (Kill/Collect/Reach/Survive/Interact/Extract)
[+] MissionManager updated with objectives, kill/loot tracking, event integration
[+] Achievements System (8 achievements: kills, missions, level, loot, extraction)
[+] Tutorial System (7-step onboarding: move, aim, fire, reload, enemy, loot, extract)
[+] GameEvents decoupled event bus (player, combat, mission, progression, loot, achievement)
[+] InventoryScreen UI (item list, capacity, close)
[+] WeaponSelectScreen UI (weapon list, equip, close)
[+] HUD updated (objectives, kill count, mission name, XP bar, level)
[+] MainMenu updated (difficulty dropdown, achievements panel, mission loading from Resources)
[+] Health/Loot/Gun/EnemyAI/Progression integrated with GameEvents
[+] Editor Generators (WeaponDefinitions, EnemyDefinitions, MissionDefinitions)
[+] SceneSetup updated (DifficultyManager, AchievementManager, TutorialManager, EnemySpawner, InventoryScreen, WeaponSelectScreen)
[+] Tests updated (9 EditMode, 7 PlayMode covering new systems)

PENDING:
[-] Open in Unity (editor not available)
[-] Run SceneSetup menu command
[-] Run Generator menu commands (weapons, enemies, missions)
[-] NavMesh bake, lighting bake
[-] Play test in editor
[-] Android APK build
[-] On-device test

PHASE 3 (Next):
[ ] Polish, accessibility, optimisation, testing, release preparation
[ ] Graphics quality presets (Low/Med/High)
[ ] Accessibility options (colorblind, subtitles, remapping)
[ ] Original art assets (materials, models, animations, VFX, SFX)
[ ] Animation rigging setup
[ ] Audio mixer configuration
[ ] Performance profiling and optimisation
[ ] Release preparation (Google Play requirements)

PHASE 4 (Future):
[ ] Optional cloud saves, authentication, leaderboards, events, multiplayer
