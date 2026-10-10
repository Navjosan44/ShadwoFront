using UnityEngine;

namespace Shadowfront.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float walkSpeed = 4f;
        public float runSpeed = 8f;
        public float crouchSpeed = 2f;
        public float rotationSpeed = 15f;
        public float gravity = -20f;
        public float jumpHeight = 1.5f;
        
        [Header("Camera")]
        public Transform cameraPivot;
        public float cameraDistance = 4f;
        public float minPitch = -40f, maxPitch = 70f;
        public float cameraSmooth = 10f;
        
        [Header("Aim")]
        public float aimFOV = 40f;
        public float normalFOV = 60f;
        public float aimTransitionSpeed = 10f;
        
        CharacterController cc;
        Health health;
        Camera cam;
        Vector3 velocity;
        bool isGrounded;
        float pitch;
        bool isAiming;
        bool isCrouching;
        bool isRunning;
        
        void Awake()
        {
            cc = GetComponent<CharacterController>();
            health = GetComponent<Health>();
            cam = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        void Update()
        {
            HandleMovement();
            HandleCamera();
            HandleAim();
            HandleCrouch();
            ApplyGravity();
        }
        
        void HandleMovement()
        {
            var input = Shadowfront.Input.InputManager.Instance;
            if(input == null) return;
            
            Vector3 move = new Vector3(input.MoveInput.x, 0, input.MoveInput.y);
            if(move.sqrMagnitude > 1f) move.Normalize();
            
            // Relative to camera forward (y-flattened)
            Vector3 camForward = cam.transform.forward; camForward.y = 0; camForward.Normalize();
            Vector3 camRight = cam.transform.right; camRight.y = 0; camRight.Normalize();
            Vector3 targetDir = camForward * move.z + camRight * move.x;
            
            float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
            if(isAiming) currentSpeed = walkSpeed * 0.6f;
            
            if(targetDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            
            cc.Move(targetDir * currentSpeed * Time.deltaTime);
        }
        
        void HandleCamera()
        {
            var input = Shadowfront.Input.InputManager.Instance;
            if(input == null || cameraPivot == null || cam == null) return;
            
            pitch -= input.LookInput.y;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            
            Quaternion targetRot = Quaternion.Euler(pitch, transform.eulerAngles.y, 0);
            cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, targetRot, cameraSmooth * Time.deltaTime);
            
            // Camera collision
            Vector3 desiredPos = cameraPivot.position - cameraPivot.forward * cameraDistance;
            if(Physics.Raycast(cameraPivot.position, -cameraPivot.forward, out var hit, cameraDistance, ~0, QueryTriggerInteraction.Ignore))
                desiredPos = hit.point + hit.normal * 0.2f;
            
            cam.transform.position = Vector3.Lerp(cam.transform.position, desiredPos, cameraSmooth * Time.deltaTime);
            cam.transform.rotation = cameraPivot.rotation;
        }
        
        void HandleAim()
        {
            var input = Shadowfront.Input.InputManager.Instance;
            if(input == null) return;
            isAiming = input.AimHeld;
            float targetFOV = isAiming ? aimFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, aimTransitionSpeed * Time.deltaTime);
        }
        
        void HandleCrouch()
        {
            var input = Shadowfront.Input.InputManager.Instance;
            if(input == null) return;
            if(input.CrouchPressed) isCrouching = !isCrouching;
            // Adjust controller height
            cc.height = isCrouching ? 1f : 2f;
            cc.center = new Vector3(0, cc.height * 0.5f, 0);
        }
        
        void ApplyGravity()
        {
            isGrounded = cc.isGrounded;
            if(isGrounded && velocity.y < 0) velocity.y = -2f;
            velocity.y += gravity * Time.deltaTime;
            cc.Move(velocity * Time.deltaTime);
        }
        
        public bool IsAiming => isAiming;
        public bool IsGrounded => isGrounded;
        public Vector3 Velocity => velocity;
    }
}
