using UnityEngine;
using UnityEngine.InputSystem;

namespace Shadowfront.Input
{
    public class SimpleInput : MonoBehaviour
    {
        public Vector2 Move { get; private set; }
        public void OnMove(InputValue v){ Move=v.Get<Vector2>(); }
        public bool Fire { get; private set; }
        public void OnFire(){ Fire=true; }
    }
}
