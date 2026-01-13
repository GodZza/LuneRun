using UnityEngine;

namespace LuneRun
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float maxSpeed = 3.8f;
        [SerializeField] private float gravity = 3.5f;
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
        private bool spaceDown;
        private bool spaceReleasedFlag;
        private float currentSpeed = 0f;
        private Vector3 trackDirection = Vector3.forward;
        
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
            currentSpeed = 0f;
            trackDirection = Vector3.forward;
            
            Debug.Log("PlayerController initialized at position: " + transform.position);
        }
        
        public void UpdatePlayer()
        {
            // Gather input - track both down and released states
            bool prevSpaceDown = spaceDown;
            spaceDown = Input.GetKey(KeyCode.Space);
            spacePressed = Input.GetKeyDown(KeyCode.Space);
            spaceReleasedFlag = Input.GetKeyUp(KeyCode.Space);
            inputSpace = spaceDown;
            
            // Ground check
            isGrounded = characterController.isGrounded;
            
            // Apply gravity (always, but adjust based on ground state)
            velocity.y -= gravity * Time.deltaTime;
            
            // If grounded, reset vertical velocity to small downward force
            if (isGrounded)
            {
                velocity.y = Mathf.Max(velocity.y, -2f);
                isJumping = false;
            }
            
            // Calculate speed along track direction
            currentSpeed = Vector3.Dot(velocity, trackDirection);
            
            // Handle wanted speeds based on input and ground state
            if (isGrounded)
            {
                if (spaceDown)
                {
                    // Hold SPACE to run forward - accelerate toward max speed
                    if (currentSpeed < maxSpeed)
                    {
                        velocity += trackDirection * 0.1f;
                    }
                    
                    // Slope effect: -0.1 * trackDirection.y
                    float slopeEffect = -0.1f * trackDirection.y;
                    if (currentSpeed < maxSpeed * 1.4f)
                    {
                        velocity += trackDirection * slopeEffect;
                    }
                }
                else if (spaceReleasedFlag && prevSpaceDown)
                {
                    // Release SPACE to jump - vertical impulse based on current speed
                    float jumpImpulse = Mathf.Min(4f, 1f + currentSpeed);
                    velocity.y += jumpImpulse;
                    isJumping = true;
                }
                
                // Update current speed after velocity changes
                currentSpeed = Vector3.Dot(velocity, trackDirection);
            }
            else
            {
                // In air
                if (spacePressed)
                {
                    // Press SPACE while in air to land quicker (increase downward velocity)
                    velocity.y = Mathf.Min(0f, velocity.y);
                    velocity.y -= 2f * gravity * Time.deltaTime;
                }
            }
            
            // Move character
            characterController.Move(velocity * Time.deltaTime);
            
            // Rotate player to face track direction (if moving forward)
            if (currentSpeed > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(trackDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
            
            // Debug logging (uncomment for testing)
            // Debug.Log($"Player: grounded={isGrounded}, spaceDown={spaceDown}, released={spaceReleasedFlag}, speed={currentSpeed}, velocity={velocity}");
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
            currentSpeed = 0f;
            trackDirection = Vector3.forward;
            Debug.Log("PlayerController reset at position: " + transform.position);
        }
        
        public bool IsOnGround()
        {
            return isGrounded;
        }
        
        // Set the direction of the current track segment
        public void SetTrackDirection(Vector3 direction)
        {
            trackDirection = direction.normalized;
        }
        
        // Call this when player lands on a slope to gain speed (simplified version)
        public void SlopeBoost()
        {
            if (isGrounded && spaceDown)
            {
                // Increase speed by 20% (similar to original's 1.2x scaling)
                velocity *= 1.2f;
            }
        }
        
        // Detect if player has fallen off track
        public bool HasFallen(float minY)
        {
            return transform.position.y < minY;
        }
    }
}