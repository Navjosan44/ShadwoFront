using UnityEngine;

namespace Shadowfront.Multiplayer
{
    public enum MultiplayerMode { None, Coop, Versus }

    public class MultiplayerManager : MonoBehaviour
    {
        public static MultiplayerManager Instance { get; private set; }

        [Header("Multiplayer Settings")]
        public bool multiplayerEnabled = false;
        public MultiplayerMode currentMode = MultiplayerMode.None;
        public int maxPlayers = 4;

        public bool IsHost { get; private set; }
        public bool IsConnected { get; private set; }
        public int PlayerCount { get; private set; }

        public event System.Action<bool> OnConnectionChanged;
        public event System.Action<int> OnPlayerCountChanged;

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

        public void StartCoop()
        {
            if (!multiplayerEnabled) return;
            currentMode = MultiplayerMode.Coop;
            IsHost = true;
            IsConnected = true;
            PlayerCount = 1;
            Debug.Log("[Multiplayer] Starting Co-op mode (simulated - no real networking)");
            OnConnectionChanged?.Invoke(true);
        }

        public void Disconnect()
        {
            currentMode = MultiplayerMode.None;
            IsHost = false;
            IsConnected = false;
            PlayerCount = 0;
            OnConnectionChanged?.Invoke(false);
            OnPlayerCountChanged?.Invoke(0);
        }

        public void AddPlayer()
        {
            if (IsConnected && PlayerCount < maxPlayers)
            {
                PlayerCount++;
                OnPlayerCountChanged?.Invoke(PlayerCount);
            }
        }

        public void RemovePlayer()
        {
            if (IsConnected && PlayerCount > 1)
            {
                PlayerCount--;
                OnPlayerCountChanged?.Invoke(PlayerCount);
            }
        }

        public void EnableMultiplayer(bool enabled)
        {
            multiplayerEnabled = enabled;
            PlayerPrefs.SetInt("MultiplayerEnabled", enabled ? 1 : 0);
        }

        public bool CanUseOnlineFeatures => multiplayerEnabled;
    }
}