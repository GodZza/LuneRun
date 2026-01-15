using ActionScript;
using UnityEngine;

namespace LuneRun
{
    // Interface matching ActionScript's IPlayerListener
    public interface IPlayerListener
    {
        void OnLand(float impact);
    }
    
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float maxSpeed = 3.8f;
        [SerializeField] private float gravity = 3.5f; // per second, converted from per-tick _g = 0.14
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
        
        // Additional fields to match ActionScript Player class
        private TrackGenerator track; // _track
        private KeyboardInput keyboard; // _keyboard (custom)
        private MouseInput mouse; // _mouse (custom)
        private TrackSegment currentPart; // _currentPart
        private int fallTime = 0; // _fallTime
        private IPlayerListener listener = null; // _listener
        private bool hasCompleted = false; // _hasCompleted
        private bool dead = false; // _dead
        private LevelManager level; // _level (reference)
        private float speed = 0f; // _speed (scalar speed along track)
        private EntityWorld world; // _world (entity manager)
        private AudioSource breathSound; // _breath
        private bool breathOn = false; // _breathOn
        
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
            
            // Initialize breath sound (placeholder)
            breathSound = gameObject.AddComponent<AudioSource>();
            // Load audio clip? Will be handled by AudioManager
            
            Debug.Log("PlayerController initialized at position: " + transform.position);
        }
        
        // Main update method - corresponds to ActionScript's tick()
        public void UpdatePlayer()
        {
            Tick();
        }
        
        // ActionScript Player.tick() equivalent
        public void Tick()
        {
            if (characterController == null)
            {
                Debug.LogError("CharacterController 为空！");
                return;
            }
            
            // Gather input
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
            speed = currentSpeed;
            
            // Handle wanted speeds based on input and ground state
            SetWantedSpeeds();
            
            // Collision detection (clip)
            Clip();
            
            // Entity interaction
            EntityInteraction();
            
            // Breath sound logic (simplified)
            if (isGrounded)
            {
                if (breathOn && Random.value > 0.99f)
                {
                    breathOn = false;
                    // Audio.Sound.getSound(SBreath).setVolume(0, 500);
                }
                else if (!breathOn && Random.value > 0.99f)
                {
                    breathOn = true;
                    // Audio.Sound.getSound(SBreath).setVolume(GetSpeedAlpha(), 500);
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
            
            // Debug logging（每30帧输出一次）
            if (Time.frameCount % 30 == 0)
            {
                Debug.Log($"Player: grounded={isGrounded}, spaceDown={spaceDown}, released={spaceReleasedFlag}, speed={currentSpeed:F2}, velocity={velocity}");
            }
        }
        
        // ActionScript _setWantedSpeeds()
        private void SetWantedSpeeds()
        {
            // In ActionScript: loc1 = this._keyboard.isDown(flash.ui.Keyboard.SPACE) || this._mouse.isDown();
            // We'll implement keyboard only for now
            bool loc1 = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
            
            if (isGrounded)
            {
                // loc2 = this._keyboard.isReleased(flash.ui.Keyboard.SPACE) || this._mouse.hasRelease();
                bool loc2 = Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0);
                SetWantedVelOnGround(loc1, loc2);
            }
            else
            {
                SetWantedVelInAir(loc1);
            }
        }
        
        // ActionScript _setWantedVelOnGround()
        private void SetWantedVelOnGround(bool arg1, bool arg2)
        {
            speed = velocity.Dot(trackDirection);
            velocity = trackDirection * speed;
            
            if (arg1)
            {
                fallTime = 0;
                if (speed < maxSpeed)
                {
                    velocity += trackDirection * 0.1f;
                }
                float loc1 = -0.1f * trackDirection.y;
                if (speed < maxSpeed * 1.4f)
                {
                    velocity += trackDirection * loc1;
                }
            }
            else if (arg2)
            {
                velocity.y += Mathf.Min(4f, 1f + speed);
                OnJump();
            }
            
            // Update speed after changes
            speed = velocity.Dot(trackDirection);
            currentSpeed = speed;
        }
        
        // ActionScript _setWantedVelInAir()
        private void SetWantedVelInAir(bool arg1)
        {
            if (arg1)
            {
                velocity.y = Mathf.Min(0f, velocity.y);
                velocity.y -= 2f * gravity * Time.deltaTime;
            }
            fallTime++;
            velocity.y -= gravity * Time.deltaTime;
        }
        
        // ActionScript _clip()
        private void Clip()
        {
            // Simplified collision with track
            // In ActionScript, this uses _track.getClosestPart(), surface detection, etc.
            // For now, we'll keep basic ground detection via CharacterController
            // TODO: Implement proper track collision
            // Basic raycast implementation for track collision detection
            RaycastHit hit;
            float rayLength = 0.5f; // Adjust based on player size
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Slight offset from feet
            Vector3 rayDirection = Vector3.down;
            
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, groundLayer))
            {
                // If we hit the track, adjust vertical velocity to stay on surface
                float surfaceHeight = hit.point.y;
                float currentHeight = transform.position.y;
                if (currentHeight > surfaceHeight + 0.1f)
                {
                    // Apply downward force to stick to track
                    velocity.y = Mathf.Min(velocity.y, -0.5f);
                }
                // Optionally, adjust track direction based on surface normal
                // trackDirection = Vector3.ProjectOnPlane(trackDirection, hit.normal).normalized;
            }
        }
        
        // ActionScript _entityInteraction()
        private void EntityInteraction()
        {
            // Check for speed entities etc.
            // TODO: Implement entity collision
            // Basic entity interaction - check for nearby entities
            if (world != null)
            {
                // Example: Get closest entity within radius
                object closestEntity = world.GetClosestEntity(transform.position, 1.0f);
                if (closestEntity != null)
                {
                    // Handle entity interaction (e.g., speed boost, obstacle)
                    Debug.Log("Entity interaction detected");
                }
            }
        }
        
        // ActionScript _onJump()
        private void OnJump()
        {
            // Audio.Sound.getSound(SBreath).setVolumeMod(0.25, 200);
        }
        
        // ActionScript _onLand()
        private void OnLand()
        {
            // In ActionScript: calculates impact and notifies listener
            if (listener != null)
            {
                // Simplified impact calculation
                float impact = Mathf.Abs(velocity.y) / 10f;
                listener.OnLand(impact);
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
            transform.position = new Vector3(0, 0.3f, 0);
            isJumping = false;
            currentSpeed = 0f;
            speed = 0f;
            trackDirection = Vector3.forward;
            fallTime = 0;
            dead = false;
            hasCompleted = false;
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
        
        // ActionScript API translation methods
        
        public float GetSpeedAlpha()
        {
            return velocity.magnitude / maxSpeed;
        }
        
        public float GetSpeedY()
        {
            return velocity.y;
        }
        
        public TrackSegment GetCurrentPart()
        {
            return currentPart;
        }
        
        public void SetListener(IPlayerListener playerListener)
        {
            listener = playerListener;
        }
        
        public bool HasCompleted()
        {
            return hasCompleted;
        }
        
        public bool IsDead()
        {
            return dead;
        }
        
        public float GetSpeed()
        {
            return speed;
        }
        
        public Vector3 GetForwardDir()
        {
            return velocity.normalized;
        }
        
        public Vector3 GetPos()
        {
            return transform.position;
        }
        
        // ActionScript destroy() equivalent
        public void Destroy()
        {
            // Stop breath sound
            // Audio.Sound.getSound(SBreath).stop(0);
            if (breathSound != null)
            {
                breathSound.Stop();
            }
        }
        
        // Helper extension method for Vector3.Dot
        // (Unity already has Vector3.Dot, but we'll keep for clarity)
    }
    
    // Placeholder classes for Input (to be implemented)
    public class KeyboardInput
    {
        public bool IsDown(KeyCode key) { return Input.GetKey(key); }
        public bool IsReleased(KeyCode key) { return Input.GetKeyUp(key); }
    }
    
    public class MouseInput
    {
        public bool IsDown() { return Input.GetMouseButton(0); }
        public bool HasRelease() { return Input.GetMouseButtonUp(0); }
    }
    
    // Placeholder for entity world
    public class EntityWorld
    {
        public object GetClosestEntity(Vector3 pos, float radius) { return null; }
    }
}