using UnityEngine;

namespace Shadowfront.Core
{
    public class Bootstrap : MonoBehaviour
    {
        void Start()
        {
            if(GameManager.Instance==null){ var go=new GameObject("GameManager"); go.AddComponent<GameManager>(); }
        }
    }
}
