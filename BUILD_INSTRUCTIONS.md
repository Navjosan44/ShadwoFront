# SHADOWFRONT - Build, Setup & Test Instructions

## Requirements
- Unity Editor 6000.0.56f1 LTS (Unity 6.0 LTS). If not installed, install via Unity Hub.
- Android Build Support (Android SDK, NDK, JDK) if building APK.
- Target: Android ARM64, API 33+ (MinSdk 33, Target 35).

## Setup
1. Open Unity Hub -> Projects -> Add -> Select C:\SHADOWFRONT
2. Open project with 6000.0.56f1 LTS. URP is configured (Assets/Settings/URPAsset.asset).
3. **Generate Definitions**: Run Shadowfront > Generate > Weapon Definitions, Enemy Definitions, Mission Definitions to create ScriptableObject assets in Resources.
4. **Run Scene Setup**: Run Shadowfront > Setup Main Menu Scene then Shadowfront > Setup Main Scene to auto-populate scenes with Player, enemies, cover, NavMesh, UI, and systems.
5. Load scene Assets/Scenes/MainMenu.unity (index 0), then Assets/Scenes/Main.unity (index 1) for gameplay.
6. Input System: imported via manifest (com.unity.inputsystem). If prompted, restart editor and regenerate Input Assets.
7. AI Navigation: com.unity.ai.navigation included (NavMeshAgent supported).

## Build (Android APK)
1. File > Build Settings > Platform: Android. Switch platform if needed.
2. Add both scenes: MainMenu (index 0), Main (index 1).
3. Edit > Project Settings > Player > Android: Package Name com.shadowfront.game, MinSdk 33, TargetSdk 35, ARM64.
4. Build > Build And Run or Build to C:\SHADOWFRONT\Builds\Shadowfront.apk.
5. Deploy to device (USB debugging). Requires Android SDK/NDK/JDK installed.

## Playable (After Setup + Generation)
- **MainMenu**: Mission select (4 missions), difficulty (Easy/Normal/Hard/Nightmare), achievements, settings (graphics, audio, FPS, vibration, sensitivity), credits, quit.
- **Gameplay**: Third-person movement (WASD/Left joystick), camera orbit (Right stick/Mouse), aim (Right mouse/Aim button), shoot (Left mouse/Fire button), reload (R), crouch (Ctrl), weapon switch (1/2/Scroll), inventory (I), weapon select (Tab), pause (Esc).
- **Enemy Archetypes**: Grunt (basic ranged), Heavy (tank, burst fire), Scout (fast, flanks), Sniper (long range, telegraphed aim).
- **Weapons**: Pistol, Rifle, SMG, Shotgun, Sniper (configurable ScriptableObject stats, recoil, reload, ammo, muzzle flash).
- **Mission Loop**: Deploy, objectives (Kill/Collect/Reach/Survive/Interact/Extract), extraction point, success/fail, rewards.
- **Progression**: XP, levels, weapon unlocks, achievements (8).
- **Inventory**: Capacity-limited, stackable items, inventory screen.
- **Difficulty**: Easy/Normal/Hard/Nightmare with health/damage/speed/detection/XP scaling.
- **Tutorial**: 7-step onboarding (move, aim, fire, reload, enemy, loot, extract).
- **HUD**: Health, ammo, weapon name, crosshair spread, extraction progress, objectives, kill count, XP bar, level.
- **Pause/Results**: Full pause menu, mission results screen.
- **Save System**: Versioned JSON, atomic writes, backup recovery, auto-save on pause/focus loss.

## Tests
- Unity Test Framework included. Window > General > Test Runner -> Run EditMode/PlayMode tests.
- EditMode: Assets/Tests/EditMode/CoreTests.cs (9 tests: Health, WeaponData, EnemyData, MissionData, SaveData, DifficultyManager, AchievementManager).
- PlayMode: Assets/Tests/PlayMode/PlayModeTests.cs (7 tests: GameManager, PlayerController, Inventory, Progression, DifficultyManager, AchievementManager, SaveManager).

## Known Limitations (Current State)
- Unity Editor may not be installed in this environment; project structure created and verified.
- Minimal placeholder assets (primitives from Scene Setup). Original assets to be added in polish phase.
- Input uses custom InputManager with mobile touch + keyboard fallback; replace with Input Actions asset for full multitouch.
- NavMesh not baked until Scene Setup runs + NavMeshSurface.BakeNavMesh().
- No APK built (editor unavailable). Build path documented.
- Particle effects use default shaders; mobile-optimized particles needed for production.
- No animation rigging setup; enemies use simple capsule colliders.
- Audio mixer not configured; uses direct AudioSource volume.
- ScriptableObject assets (weapons, enemies, missions) require editor menu commands to generate.

## Acceptance Notes
- VERIFIED: Project structure, URP manifest, asmdef, core scripts (40+), scenes (2), manifests, tests (16), docs.
- UNVERIFIED: In-Editor play, Scene Setup execution, Generator execution, NavMesh bake, lighting bake, APK build, on-device performance.
- NOT IMPLEMENTED: Full mobile HUD with virtual joystick graphics, tutorial UI graphics, graphics presets (Low/Med/High), accessibility options, original art assets, cloud saves, leaderboards, events, multiplayer.

## Phase 3 Features (Polish, Accessibility, Optimisation, Release)
- **Graphics Quality Presets**: Low/Med/High with scalable settings (FPS, resolution, shadows, MSAA, view distance, particles)
- **Accessibility**: Colorblind modes (Protanopia/Deuteranopia/Tritanopia), subtitles, screen shake toggle, UI scale, high contrast, auto-aim, hold-to-fire/aim
- **Performance Optimisation**: Object pooling (bullets/effects), performance monitor (FPS, memory, frame time), adaptive FPS
- **Loading System**: Progress bar, tips, scene loading
- **Transitions**: Screen fade transitions
- **Localization**: Multi-language support (en, es, fr, de, zh, hi)
- **Release Prep**: Build versioning, anti-tamper, analytics, Android compliance check, save integrity verification

