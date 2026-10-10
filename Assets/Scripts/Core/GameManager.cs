using UnityEngine;

namespace Shadowfront.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public bool IsPaused { get; private set; }
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
        }
        public void Pause(){ IsPaused=true; Time.timeScale=0f; }
        public void Resume(){ IsPaused=false; Time.timeScale=1f; }
    }
}
