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
        
        public void Initialize(Settings settings)
        {
            // Ensure CharacterController component
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = gameObject.AddComponent<CharacterController>();
                characterController.height = 2f;
                characterController.radius = 0.5f;
            }
            
            // Reset position
            transform.position = Vector3.zero;
            velocity = Vector3.zero;
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
            }
            
            // Handle running/jumping based on space key
            // In the original game: hold SPACE to run, release to jump
            // Press SPACE while in air to land quicker
            
            if (isGrounded)
            {
                if (inputSpace)
                {
                    // Hold SPACE to run forward
                    velocity.z = runSpeed;
                }
                else
                {
                    // Release SPACE to jump
                    if (spaceReleased)
                    {
                        velocity.y = jumpForce;
                        isJumping = true;
                    }
                    velocity.z = 0f;
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
                
                // Air control
                velocity.z = inputSpace ? runSpeed * 0.5f : 0f;
            }
            
            // Move character
            characterController.Move(velocity * Time.deltaTime);
            
            // Rotate player to face movement direction
            if (velocity.z != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(0, 0, velocity.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
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
            transform.position = Vector3.zero;
            isJumping = false;
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
                velocity.z = runSpeed * 1.5f;
            }
        }
        
        // Detect if player has fallen off track
        public bool HasFallen(float minY)
        {
            return transform.position.y < minY;
        }
    }
}