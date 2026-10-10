using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Shadowfront.Input
{
    [DefaultExecutionOrder(-100)]
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool FirePressed { get; private set; }
        public bool AimHeld { get; private set; }
        public bool ReloadPressed { get; private set; }
        public bool CrouchPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool WeaponSwitchNext { get; private set; }
        public bool WeaponSwitchPrev { get; private set; }
        public bool PausePressed { get; private set; }
        
        [Header("Mobile")]
        public bool UseMobileControls = true;
        public RectTransform LeftJoystickArea;
        public RectTransform RightLookArea;
        
        Vector2 leftStart; int leftFinger = -1;
        Vector2 rightStart; int rightFinger = -1;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
        }
        
        void Update()
        {
            // Keyboard fallback
            if(!UseMobileControls || Application.isEditor)
            {
                MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
                LookInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
                FirePressed = Input.GetMouseButton(0);
                AimHeld = Input.GetMouseButton(1);
                ReloadPressed = Input.GetKeyDown(KeyCode.R);
                CrouchPressed = Input.GetKeyDown(KeyCode.LeftControl);
                InteractPressed = Input.GetKeyDown(KeyCode.E);
                WeaponSwitchNext = Input.GetKeyDown(KeyCode.Alpha2) || Input.mouseScrollDelta.y > 0;
                WeaponSwitchPrev = Input.GetKeyDown(KeyCode.Alpha1) || Input.mouseScrollDelta.y < 0;
                PausePressed = Input.GetKeyDown(KeyCode.Escape);
            }
            
            // Mobile touch handling
            if(UseMobileControls && Touchscreen.current != null)
            {
                var touches = Touchscreen.current.touches;
                for(int i=0;i<touches.Count;i++)
                {
                    var t = touches[i];
                    if(!t.isInProgress) continue;
                    var pos = t.position.ReadValue();
                    
                    // Left joystick area (bottom-left 40% width, 60% height)
                    if(leftFinger==-1 || leftFinger==t.touchId.ReadValue())
                    {
                        if(leftFinger==-1 && IsInLeftArea(pos)) { leftFinger=t.touchId.ReadValue(); leftStart=pos; }
                        if(leftFinger==t.touchId.ReadValue())
                        {
                            var delta = pos - leftStart;
                            MoveInput = Vector2.ClampMagnitude(delta / 100f, 1f);
                            if(t.phase.ReadValue()==UnityEngine.InputSystem.TouchPhase.Ended || t.phase.ReadValue()==UnityEngine.InputSystem.TouchPhase.Canceled)
                                leftFinger=-1;
                        }
                    }
                    
                    // Right look area (bottom-right 60% width, full height)
                    if(rightFinger==-1 || rightFinger==t.touchId.ReadValue())
                    {
                        if(rightFinger==-1 && IsInRightArea(pos)) { rightFinger=t.touchId.ReadValue(); rightStart=pos; }
                        if(rightFinger==t.touchId.ReadValue())
                        {
                            var delta = pos - rightStart;
                            LookInput = delta * 0.1f;
                            if(t.phase.ReadValue()==UnityEngine.InputSystem.TouchPhase.Ended || t.phase.ReadValue()==UnityEngine.InputSystem.TouchPhase.Canceled)
                                rightFinger=-1;
                        }
                    }
                }
                if(leftFinger==-1) MoveInput = Vector2.zero;
                if(rightFinger==-1) LookInput = Vector2.zero;
            }
        }
        
        bool IsInLeftArea(Vector2 pos)
        {
            float w=Screen.width, h=Screen.height;
            return pos.x < w*0.4f && pos.y < h*0.6f;
        }
        bool IsInRightArea(Vector2 pos)
        {
            float w=Screen.width, h=Screen.height;
            return pos.x > w*0.4f;
        }
        
        public void ConsumeFire(){ FirePressed=false; }
    }
}
