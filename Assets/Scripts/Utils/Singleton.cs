using UnityEngine;

namespace Shadowfront.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T _instance;
        public static T Instance
        {
            get
            {
                if(_instance == null) _instance = FindObjectOfType<T>();
                return _instance;
            }
        }
        protected virtual void Awake()
        {
            if(_instance != null && _instance != this) { Destroy(gameObject); return; }
            _instance = this as T;
        }
    }
    
    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go.TryGetComponent(out T comp) ? comp : go.AddComponent<T>();
        }
    }
}
