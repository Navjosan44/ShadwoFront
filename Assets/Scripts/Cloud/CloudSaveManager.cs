using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shadowfront.Cloud
{
    [Serializable]
    public class CloudSaveData
    {
        public string userId;
        public string saveData;
        public string timestamp;
        public int version;
    }

    public class CloudSaveManager : MonoBehaviour
    {
        public static CloudSaveManager Instance { get; private set; }

        [Header("Cloud Settings")]
        public bool cloudSavesEnabled = false;
        public string apiEndpoint = "https://api.shadowfront.com/saves";
        public float syncInterval = 300f;

        float syncTimer;
        bool isSyncing;

        public event Action<bool> OnCloudSaveComplete;
        public event Action<bool> OnCloudLoadComplete;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(transform.root);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }

        void Update()
        {
            if (!cloudSavesEnabled) return;

            syncTimer += Time.deltaTime;
            if (syncTimer >= syncInterval)
            {
                AutoSync();
                syncTimer = 0f;
            }
        }

        public void SaveToCloud(string saveJson)
        {
            if (!cloudSavesEnabled) return;
            StartCoroutine(SaveToCloudRoutine(saveJson));
        }

        System.Collections.IEnumerator SaveToCloudRoutine(string saveJson)
        {
            isSyncing = true;
            var data = new CloudSaveData
            {
                userId = PlayerPrefs.GetString("UserId", "guest"),
                saveData = saveJson,
                timestamp = DateTime.UtcNow.ToString("O"),
                version = 1
            };

            // Simulated cloud save
            yield return new WaitForSeconds(1f);

            Debug.Log("[CloudSave] Cloud save completed (simulated)");
            isSyncing = false;
            OnCloudSaveComplete?.Invoke(true);
        }

        public void LoadFromCloud()
        {
            if (!cloudSavesEnabled) return;
            StartCoroutine(LoadFromCloudRoutine());
        }

        System.Collections.IEnumerator LoadFromCloudRoutine()
        {
            isSyncing = true;

            // Simulated cloud load
            yield return new WaitForSeconds(1f);

            Debug.Log("[CloudSave] Cloud load completed (simulated)");
            isSyncing = false;
            OnCloudLoadComplete?.Invoke(true);
        }

        void AutoSync()
        {
            if (isSyncing) return;
            var savePath = System.IO.Path.Combine(Application.persistentDataPath, "save.json");
            if (System.IO.File.Exists(savePath))
            {
                string json = System.IO.File.ReadAllText(savePath);
                SaveToCloud(json);
            }
        }

        public void EnableCloudSaves(bool enabled)
        {
            cloudSavesEnabled = enabled;
            PlayerPrefs.SetInt("CloudSavesEnabled", enabled ? 1 : 0);
        }

        public bool IsSyncing => isSyncing;
    }
}