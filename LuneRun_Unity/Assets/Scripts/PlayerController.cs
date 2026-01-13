using UnityEngine;

namespace LuneRun
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float jumpForce = 15f;
        [SerializeField] private float gravity = 30f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private LayerMask groundLayer;
        
        private CharacterController characterController;
        private Vector3 velocity;
        private bool isGrounded;
        private bool isJumping;
        private bool spacePressed;
        private bool spaceReleased;
        
        // Input state
        private bool inputSpace;
        private float horizontalSpeed = 0f;
        
        public void Initialize(Settings settings)
        {
            // Ensure CharacterController component
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = gameObject.AddComponent<CharacterController>();
                characterController.height = 2f;
                characterController.radius = 0.5f;
                characterController.stepOffset = 0.3f;
                characterController.slopeLimit = 45f;
            }
            else
            {
                characterController.stepOffset = 0.3f;
                characterController.slopeLimit = 45f;
            }
            
            // Set ground layer to include all layers
            groundLayer = -1;
            
            // Reset position slightly above track surface
            transform.position = new Vector3(0, 0.3f, 0);
            velocity = Vector3.zero;
            horizontalSpeed = 0f;
            
            Debug.Log("PlayerController initialized at position: " + transform.position);
        }
        
        public void UpdatePlayer()
        {
            // Gather input
            inputSpace = Input.GetKey(KeyCode.Space);
            spacePressed = Input.GetKeyDown(KeyCode.Space);
            spaceReleased = Input.GetKeyUp(KeyCode.Space);
            
            // Ground check
            isGrounded = characterController.isGrounded;
            
            // Apply gravity
            if (!isGrounded)
            {
                velocity.y -= gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = -2f; // Small downward force to keep grounded
                isJumping = false; // Landed
            }
            
            // Handle horizontal speed based on ground state and input
            if (isGrounded)
            {
                if (inputSpace)
                {
                    // Hold SPACE to run forward
                    horizontalSpeed = runSpeed;
                }
                else
                {
                    // Not holding space - stop running
                    horizontalSpeed = 0f;
                    
                    // Release SPACE to jump (only if previously holding)
                    if (spaceReleased)
                    {
                        velocity.y = jumpForce;
                        isJumping = true;
                        // Keep horizontal speed from before jump (runSpeed if was running)
                        horizontalSpeed = runSpeed;
                    }
                }
            }
            else
            {
                // In air
                if (spacePressed)
                {
                    // Press SPACE while in air to land quicker (increase gravity)
                    velocity.y -= gravity * 2f * Time.deltaTime;
                }
                
                // Air control: if holding space, allow some horizontal control
                if (inputSpace)
                {
                    horizontalSpeed = runSpeed * 0.5f;
                }
                // If not holding space, keep current horizontal speed (no change)
            }
            
            // Apply horizontal speed
            velocity.z = horizontalSpeed;
            
            // Move character
            characterController.Move(velocity * Time.deltaTime);
            
            // Rotate player to face movement direction
            if (velocity.z != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(0, 0, velocity.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
            
            // Debug logging (uncomment for testing)
            // Debug.Log($"Player: grounded={isGrounded}, inputSpace={inputSpace}, hSpeed={horizontalSpeed}, velocity={velocity}");
        }
        
        public void Interpolate(float alpha)
        {
            // Interpolation for smooth rendering between physics ticks
            // In Unity, this is handled automatically by CharacterController and Update
            // We could implement custom interpolation if needed
        }
        
        public void Reset()
        {
            velocity = Vector3.zero;
            transform.position = new Vector3(0, 0.3f, 0);
            isJumping = false;
            horizontalSpeed = 0f;
            Debug.Log("PlayerController reset at position: " + transform.position);
        }
        
        public bool IsOnGround()
        {
            return isGrounded;
        }
        
        // Call this when player lands on a slope to gain speed
        public void SlopeBoost()
        {
            if (isGrounded && inputSpace)
            {
                horizontalSpeed = runSpeed * 1.5f;
            }
        }
        
        // Detect if player has fallen off track
        public bool HasFallen(float minY)
        {
            return transform.position.y < minY;
        }
    }
}