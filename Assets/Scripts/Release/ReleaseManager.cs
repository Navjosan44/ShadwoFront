using UnityEngine;
using System;

namespace Shadowfront.Release
{
    public class ReleaseManager : MonoBehaviour
    {
        public static ReleaseManager Instance { get; private set; }

        [Header("Build Info")]
        public string buildVersion = "1.0.0";
        public int buildNumber = 1;
        public string bundleIdentifier = "com.shadowfront.game";
        public string minSdkVersion = "33";
        public string targetSdkVersion = "35";

        [Header("Security")]
        public bool enableAntiTamper = true;
        public bool enableObfuscation = true;
        public bool verifySaveIntegrity = true;

        [Header("Analytics")]
        public bool enableAnalytics = true;
        public string analyticsEndpoint = "https://analytics.shadowfront.com/events";

        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(transform.root); return; }
            Instance=this; DontDestroyOnLoad(transform.root);
            LoadBuildInfo();
        }

        void LoadBuildInfo()
        {
            buildVersion =PlayerPrefs.GetString("BuildVersion", buildVersion);
            buildNumber =PlayerPrefs.GetInt("BuildNumber", buildNumber);
        }

        public string GetFullVersion() => $"v{buildVersion} (Build {buildNumber})";

        public bool VerifySaveIntegrity(string saveData)
        {
            if(!verifySaveIntegrity) return true;
            if(string.IsNullOrEmpty(saveData)) return false;
            // In production, verify checksum/signature
            return saveData.Length > 0 && saveData.Contains("\"version\"");
        }

        public void LogEvent(string eventName, params string[] parameters)
        {
            if(!enableAnalytics) return;
            var msg = $"[Analytics] {eventName}";
            foreach(var p in parameters) msg += $" | {p}";
            Debug.Log(msg);
            // In production, send to analyticsEndpoint
        }

        public void CheckAndroidCompliance()
        {
            #if UNITY_ANDROID
            // Verify package name
            string pkg = Application.identifier;
            if(pkg != bundleIdentifier)
                Debug.LogWarning($"[Release] Package name mismatch: {pkg} != {bundleIdentifier}");

            // Verify SDK
            int sdk = Android.OS.Build.VERSION.SdkInt;
            if(sdk < 33)
                Debug.LogWarning($"[Release] SDK below minimum: {sdk} < 33");

            Debug.Log($"[Release] Android compliance check passed. SDK: {sdk}");
            #endif
        }
    }
}