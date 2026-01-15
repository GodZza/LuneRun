using ActionScript;
using UnityEngine;
using com.playchilla.runner.track.entity;
using com.playchilla.runner.player;
using shared.math;
using Random = UnityEngine.Random;

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
        private World world; // _world (entity manager)
        private AudioSource breathSound; // _breath
        private bool breathOn = false; // _breathOn
        
        // Hand cubes for visual representation (simulating hands from original Flash game)
        private GameObject leftHandCube;
        private GameObject rightHandCube;
        
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
            
            // Create hand cubes (simulating hands from original Flash game)
            CreateHandCubes();
        }
        
        // Create two cubes to represent hands (simulating original Flash game)
        private void CreateHandCubes()
        {
            // Create left hand cube (upper arm)
            leftHandCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftHandCube.name = "LeftHandCube";
            leftHandCube.transform.SetParent(transform);
            leftHandCube.transform.localPosition = new Vector3(-0.3f, 1.0f, 0.2f);
            leftHandCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f); // Original Flash: 0.1, 0.1, 0.5
            
            // Create right hand cube (upper arm)
            rightHandCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightHandCube.name = "RightHandCube";
            rightHandCube.transform.SetParent(transform);
            rightHandCube.transform.localPosition = new Vector3(0.3f, 1.0f, 0.2f);
            rightHandCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f);
            
            // Set colors for visibility (red for left, blue for right)
            leftHandCube.GetComponent<Renderer>().material.color = Color.red;
            rightHandCube.GetComponent<Renderer>().material.color = Color.blue;
            
            Debug.Log("Hand cubes created for player (simulating original Flash game)");
        }
        
        // Update hand animation based on player state (simulating original Flash game)
        private void UpdateHandAnimation()
        {
            if (leftHandCube == null || rightHandCube == null) return;
            
            float time = Time.time;
            
            if (isGrounded)
            {
                // On ground animation
                float speedAlpha = GetSpeedAlpha();
                if (speedAlpha > 0.1f)
                {
                    // Swing arms while moving - amplitude depends on speed
                    float swingAmplitude = 30f * speedAlpha;
                    float swing = Mathf.Sin(time * 10f) * swingAmplitude;
                    // Opposite phase for right arm
                    float rightSwing = Mathf.Sin(time * 10f + Mathf.PI) * swingAmplitude;
                    
                    leftHandCube.transform.localRotation = Quaternion.Euler(-10f + swing, 0, 0);
                    rightHandCube.transform.localRotation = Quaternion.Euler(-10f + rightSwing, 0, 0);
                }
                else
                {
                    // Idle position
                    leftHandCube.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    rightHandCube.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else
            {
                // In air animation - arms flailing (original Flash: loc1 = 10 * (time + alpha))
                float wave = Mathf.Sin(time * 20f) * 60f;
                leftHandCube.transform.localRotation = Quaternion.Euler(wave, 0, 0);
                rightHandCube.transform.localRotation = Quaternion.Euler(wave + 180f, 0, 0);
            }
        }
        
        // Set the track generator reference for collision detection
        public void SetTrackGenerator(TrackGenerator trackGenerator)
        {
            track = trackGenerator;
            Debug.Log("PlayerController: TrackGenerator set");
        }
        
        // Set the world reference for entity interaction
        public void SetWorld(World world)
        {
            this.world = world;
            Debug.Log("PlayerController: World set");
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
            
            // Update hand animation (simulating original Flash game hands)
            UpdateHandAnimation();
            
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
            // If we have a track generator, use it for more accurate collision detection
            if (track != null)
            {
                // Calculate distance along track (simplified using Z coordinate)
                float distanceAlongTrack = transform.position.z;
                // Get the track segment at this distance
                TrackSegment segment = track.GetSegmentAtDistance(distanceAlongTrack);
                if (segment != null)
                {
                    // Get position on track at this distance
                    Vector3 trackPosition = track.GetPositionAtDistance(distanceAlongTrack);
                    // Adjust player position to stay on track surface
                    float surfaceHeight = trackPosition.y;
                    float currentHeight = transform.position.y;
                    float heightDiff = currentHeight - surfaceHeight;
                    
                    if (heightDiff > groundCheckDistance)
                    {
                        // Player is above track, apply downward force
                        velocity.y = Mathf.Min(velocity.y, -0.5f);
                    }
                    else if (heightDiff < -groundCheckDistance)
                    {
                        // Player is below track, push upward (should not happen with proper ground detection)
                        velocity.y = Mathf.Max(velocity.y, 1f);
                    }
                    
                    // Update ground state based on proximity to track surface
                    isGrounded = Mathf.Abs(heightDiff) <= groundCheckDistance;
                    
                    // Update track direction based on segment orientation
                    trackDirection = (segment.endPoint - segment.startPoint).normalized;
                }
                else
                {
                    // Fallback to raycast if no segment found
                    RaycastGround();
                }
            }
            else
            {
                // No track generator, use basic raycast detection
                RaycastGround();
            }
        }
        
        // Basic raycast ground detection (fallback)
        private void RaycastGround()
        {
            RaycastHit hit;
            float rayLength = 0.5f;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            Vector3 rayDirection = Vector3.down;
            
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, groundLayer))
            {
                float surfaceHeight = hit.point.y;
                float currentHeight = transform.position.y;
                if (currentHeight > surfaceHeight + 0.1f)
                {
                    velocity.y = Mathf.Min(velocity.y, -0.5f);
                }
            }
        }
        
        // ActionScript _entityInteraction()
        private void EntityInteraction()
        {
            // Check for speed entities etc.
            if (world != null)
            {
                // Convert player position to Vec3 for entity query
                Vec3 playerPos = new Vec3(transform.position.x, transform.position.y, transform.position.z);
                // Search radius (original uses 1.0)
                double radius = 1.0;
                RunnerEntity closest = world.GetClosestEntity(playerPos, radius);
                if (closest != null)
                {
                    // Handle entity interaction based on entity type
                    if (closest is SpeedEntity)
                    {
                        // Speed boost: increase velocity along track direction
                        velocity += trackDirection * 5f;
                        Debug.Log($"Speed boost! Velocity increased. Entity at {closest.GetPos()}");
                        // Mark entity for removal (optional)
                        closest.Remove();
                    }
                    else
                    {
                        Debug.Log($"Entity interaction with {closest.GetType().Name} at {closest.GetPos()}");
                    }
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
    

}