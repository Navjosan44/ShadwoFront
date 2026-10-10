using System;
using UnityEngine;

namespace Shadowfront.Auth
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }

        [Header("Auth Settings")]
        public bool authEnabled = false;

        public string UserId { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }

        public event Action<bool> OnAuthComplete;
        public event Action OnSignOut;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(transform.root);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(transform.root);
            LoadUserSession();
        }

        void LoadUserSession()
        {
            UserId = PlayerPrefs.GetString("UserId", "guest");
            IsAuthenticated = PlayerPrefs.GetInt("IsAuthenticated", 0) == 1;
            Username = PlayerPrefs.GetString("Username", "Guest");
            Email = PlayerPrefs.GetString("Email", "");
        }

        public void SignIn(string username, string password)
        {
            if (!authEnabled) return;
            StartCoroutine(SignInRoutine(username, password));
        }

        System.Collections.IEnumerator SignInRoutine(string username, string password)
        {
            yield return new WaitForSeconds(1f);

            IsAuthenticated = true;
            UserId = Guid.NewGuid().ToString();
            Username = username;
            Email = $"{username}@shadowfront.com";

            PlayerPrefs.SetString("UserId", UserId);
            PlayerPrefs.SetInt("IsAuthenticated", 1);
            PlayerPrefs.SetString("Username", Username);
            PlayerPrefs.SetString("Email", Email);
            PlayerPrefs.Save();

            Debug.Log($"[Auth] Signed in as {Username}");
            OnAuthComplete?.Invoke(true);
        }

        public void SignOut()
        {
            IsAuthenticated = false;
            UserId = "guest";
            Username = "Guest";
            Email = "";

            PlayerPrefs.SetString("UserId", "guest");
            PlayerPrefs.SetInt("IsAuthenticated", 0);
            PlayerPrefs.SetString("Username", "Guest");
            PlayerPrefs.SetString("Email", "");
            PlayerPrefs.Save();

            OnSignOut?.Invoke();
        }

        public void SignUp(string username, string email, string password)
        {
            if (!authEnabled) return;
            StartCoroutine(SignUpRoutine(username, email, password));
        }

        System.Collections.IEnumerator SignUpRoutine(string username, string email, string password)
        {
            yield return new WaitForSeconds(1f);

            IsAuthenticated = true;
            UserId = Guid.NewGuid().ToString();
            Username = username;
            Email = email;

            PlayerPrefs.SetString("UserId", UserId);
            PlayerPrefs.SetInt("IsAuthenticated", 1);
            PlayerPrefs.SetString("Username", Username);
            PlayerPrefs.SetString("Email", Email);
            PlayerPrefs.Save();

            Debug.Log($"[Auth] Signed up as {Username}");
            OnAuthComplete?.Invoke(true);
        }

        public void EnableAuth(bool enabled)
        {
            authEnabled = enabled;
            PlayerPrefs.SetInt("AuthEnabled", enabled ? 1 : 0);
        }

        public bool CanUploadCloud => authEnabled && IsAuthenticated;
    }
}