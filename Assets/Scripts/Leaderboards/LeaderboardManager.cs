using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shadowfront.Leaderboards
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
        public int level;
        public string missionName;
        public string timestamp;
    }

    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager Instance { get; private set; }

        [Header("Leaderboard Settings")]
        public bool leaderboardsEnabled = false;
        public string apiEndpoint = "https://api.shadowfront.com/leaderboards";

        public List<LeaderboardEntry> GlobalLeaderboard { get; private set; } = new List<LeaderboardEntry>();
        public List<LeaderboardEntry> FriendsLeaderboard { get; private set; } = new List<LeaderboardEntry>();
        public LeaderboardEntry PlayerEntry { get; private set; }

        public event Action OnLeaderboardUpdated;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(transform.root);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(transform.root);
            InitializeMockLeaderboard();
        }

        void InitializeMockLeaderboard()
        {
            GlobalLeaderboard = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { playerName = "Shadow", score = 50000, level = 15, missionName = "Industrial Zone", timestamp = DateTime.UtcNow.ToString("O") },
                new LeaderboardEntry { playerName = "Ghost", score = 45000, level = 14, missionName = "The Docks", timestamp = DateTime.UtcNow.ToString("O") },
                new LeaderboardEntry { playerName = "Phantom", score = 40000, level = 13, missionName = "Sector 7", timestamp = DateTime.UtcNow.ToString("O") },
                new LeaderboardEntry { playerName = "Reaper", score = 35000, level = 12, missionName = "The Docks", timestamp = DateTime.UtcNow.ToString("O") },
                new LeaderboardEntry { playerName = "Spectre", score = 30000, level = 11, missionName = "Sector 7", timestamp = DateTime.UtcNow.ToString("O") }
            };

            PlayerEntry = new LeaderboardEntry
            {
                playerName = PlayerPrefs.GetString("Username", "Guest"),
                score = PlayerPrefs.GetInt("TotalScore", 0),
                level = PlayerPrefs.GetInt("PlayerLevel", 1),
                missionName = "Training Grounds",
                timestamp = DateTime.UtcNow.ToString("O")
            };
        }

        public void SubmitScore(int score, int level, string missionName)
        {
            if (!leaderboardsEnabled) return;
            StartCoroutine(SubmitScoreRoutine(score, level, missionName));
        }

        System.Collections.IEnumerator SubmitScoreRoutine(int score, int level, string missionName)
        {
            yield return new WaitForSeconds(1f);

            PlayerEntry.score += score;
            PlayerEntry.level = level;
            PlayerEntry.missionName = missionName;
            PlayerEntry.timestamp = DateTime.UtcNow.ToString("O");
            PlayerEntry.playerName = PlayerPrefs.GetString("Username", "Guest");

            PlayerPrefs.SetInt("TotalScore", PlayerEntry.score);
            PlayerPrefs.Save();

            Debug.Log($"[Leaderboard] Score {PlayerEntry.score} submitted");
            OnLeaderboardUpdated?.Invoke();
        }

        public List<LeaderboardEntry> GetTopN(int count)
        {
            List<LeaderboardEntry> sorted = new List<LeaderboardEntry>(GlobalLeaderboard);
            sorted.Sort((a, b) => b.score.CompareTo(a.score));
            return sorted.GetRange(0, Math.Min(count, sorted.Count));
        }

        public void EnableLeaderboards(bool enabled)
        {
            leaderboardsEnabled = enabled;
            PlayerPrefs.SetInt("LeaderboardsEnabled", enabled ? 1 : 0);
        }
    }
}