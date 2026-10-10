using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Shadowfront.Data
{
    [System.Serializable]
    public class SaveData
    {
        public int version = 1;
        public string saveDate;
        public PlayerData player;
        public InventoryData inventory;
        public ProgressionData progression;
        public SettingsData settings;
    }
    
    [System.Serializable]
    public class PlayerData
    {
        public Vector3Serializable position;
        public int health;
        public int maxHealth;
    }
    
    [System.Serializable]
    public class Vector3Serializable
    {
        public float x, y, z;
        public static implicit operator Vector3(Vector3Serializable v) => new Vector3(v.x, v.y, v.z);
        public static implicit operator Vector3Serializable(Vector3 v) => new Vector3Serializable { x=v.x, y=v.y, z=v.z };
    }
    
    [System.Serializable]
    public class InventoryData
    {
        public List<ItemEntry> items = new List<ItemEntry>();
        [System.Serializable]
        public class ItemEntry { public string prefabPath; public int count; }
    }
    
    [System.Serializable]
    public class ProgressionData
    {
        public int level;
        public int xp;
        public int xpToNextLevel;
        public int missionsCompleted;
        public int enemiesKilled;
        public int totalXPEarned;
        public List<string> unlockedWeapons = new List<string>();
    }
    
    [System.Serializable]
    public class SettingsData
    {
        public float masterVolume = 1f;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public int graphicsQuality = 2;
        public int targetFPS = 60;
        public bool vibration = true;
        public float sensitivity = 1f;
    }
    
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        const string SAVE_FILE = "save.json";
        const string BACKUP_FILE = "save_backup.json";
        const int CURRENT_VERSION = 1;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
        }
        
        public void Save()
        {
            var data = new SaveData
            {
                version = CURRENT_VERSION,
                saveDate = DateTime.UtcNow.ToString("O"),
                player = CollectPlayerData(),
                inventory = CollectInventoryData(),
                progression = CollectProgressionData(),
                settings = CollectSettingsData()
            };
            
            string json = JsonUtility.ToJson(data, true);
            string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
            string backupPath = Path.Combine(Application.persistentDataPath, BACKUP_FILE);
            
            // Atomic write: write to temp, then replace
            string tempPath = path + ".tmp";
            try
            {
                File.WriteAllText(tempPath, json, Encoding.UTF8);
                if(File.Exists(path)) File.Copy(path, backupPath, true);
                File.Move(tempPath, path, true);
                Debug.Log($"[SaveManager] Saved to {path}");
            }
            catch(Exception e)
            {
                Debug.LogError($"[SaveManager] Save failed: {e.Message}");
                if(File.Exists(tempPath)) File.Delete(tempPath);
            }
        }
        
        public bool Load()
        {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
            if(!File.Exists(path))
            {
                Debug.Log("[SaveManager] No save file found");
                return false;
            }
            
            try
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                var data = JsonUtility.FromJson<SaveData>(json);
                
                if(data == null || data.version > CURRENT_VERSION)
                {
                    Debug.LogWarning("[SaveManager] Save version mismatch or corrupt");
                    return TryLoadBackup();
                }
                
                ApplyPlayerData(data.player);
                ApplyInventoryData(data.inventory);
                ApplyProgressionData(data.progression);
                ApplySettingsData(data.settings);
                
                Debug.Log($"[SaveManager] Loaded save from {data.saveDate} (v{data.version})");
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError($"[SaveManager] Load failed: {e.Message}");
                return TryLoadBackup();
            }
        }
        
        bool TryLoadBackup()
        {
            string backupPath = Path.Combine(Application.persistentDataPath, BACKUP_FILE);
            if(!File.Exists(backupPath)) return false;
            try
            {
                string json = File.ReadAllText(backupPath, Encoding.UTF8);
                var data = JsonUtility.FromJson<SaveData>(json);
                if(data != null && data.version <= CURRENT_VERSION)
                {
                    ApplyPlayerData(data.player);
                    ApplyInventoryData(data.inventory);
                    ApplyProgressionData(data.progression);
                    ApplySettingsData(data.settings);
                    Debug.Log("[SaveManager] Loaded from backup");
                    return true;
                }
            }
            catch { }
            return false;
        }
        
        PlayerData CollectPlayerData()
        {
            var pc = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Shadowfront.Player.PlayerController>();
            var h = pc?.GetComponent<Health>();
            return new PlayerData
            {
                position = pc?.transform.position ?? Vector3.zero,
                health = h?.current ?? 100,
                maxHealth = h?.max ?? 100
            };
        }
        
        InventoryData CollectInventoryData()
        {
            var inv = Inventory.Instance;
            var data = new InventoryData();
            if(inv != null)
                foreach(var item in inv.items)
                    data.items.Add(new InventoryData.ItemEntry 
                    { 
                        prefabPath = GetPrefabPath(item.itemPrefab), 
                        count = item.count 
                    });
            return data;
        }
        
        ProgressionData CollectProgressionData()
        {
            var p = Progression.Instance;
            return new ProgressionData
            {
                level = p?.level ?? 1,
                xp = p?.xp ?? 0,
                xpToNextLevel = p?.xpToNextLevel ?? 100,
                missionsCompleted = p?.missionsCompleted ?? 0,
                enemiesKilled = p?.enemiesKilled ?? 0,
                totalXPEarned = p?.totalXPEarned ?? 0,
                unlockedWeapons = p?.unlockedWeapons != null ? new List<string>(p.unlockedWeapons) : new List<string>()
            };
        }
        
        SettingsData CollectSettingsData()
        {
            return new SettingsData
            {
                masterVolume = AudioListener.volume,
                graphicsQuality = QualitySettings.GetQualityLevel(),
                targetFPS = Application.targetFrameRate,
                sensitivity = Shadowfront.Input.InputManager.Instance != null ? 1f : 1f
            };
        }
        
        void ApplyPlayerData(PlayerData data)
        {
            var pc = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Shadowfront.Player.PlayerController>();
            var h = pc?.GetComponent<Health>();
            if(pc != null && data.position != null) pc.transform.position = data.position;
            if(h != null) { h.max = data.maxHealth; h.current = data.health; }
        }
        
        void ApplyInventoryData(InventoryData data)
        {
            var inv = Inventory.Instance;
            if(inv == null) return;
            inv.items.Clear();
            foreach(var entry in data.items)
            {
                var prefab = Resources.Load<GameObject>(entry.prefabPath);
                if(prefab != null) inv.items.Add(new InventoryItem { itemPrefab = prefab, count = entry.count });
            }
            inv.OnInventoryChanged?.Invoke();
        }
        
        void ApplyProgressionData(ProgressionData data)
        {
            var p = Progression.Instance;
            if(p == null) return;
            p.level = data.level;
            p.xp = data.xp;
            p.xpToNextLevel = data.xpToNextLevel;
            p.missionsCompleted = data.missionsCompleted;
            p.enemiesKilled = data.enemiesKilled;
            p.totalXPEarned = data.totalXPEarned;
            p.unlockedWeapons = new HashSet<string>(data.unlockedWeapons);
        }
        
        void ApplySettingsData(SettingsData data)
        {
            AudioListener.volume = data.masterVolume;
            QualitySettings.SetQualityLevel(data.graphicsQuality);
            Application.targetFrameRate = data.targetFPS;
        }
        
        string GetPrefabPath(GameObject prefab)
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(prefab).Replace("Assets/Resources/", "").Replace(".prefab", "");
            #else
            return prefab.name;
            #endif
        }
        
        void OnApplicationPause(bool pause)
        {
            if(pause) Save();
        }
        
        void OnApplicationFocus(bool focus)
        {
            if(!focus) Save();
        }
    }
}
