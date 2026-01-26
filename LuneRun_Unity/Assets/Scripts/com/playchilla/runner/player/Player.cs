using UnityEngine;
using shared.input;
using shared.math;
using com.playchilla.runner.track;
using com.playchilla.runner.track.entity;
using Random = UnityEngine.Random;


namespace com.playchilla.runner.player
{
    public class Player : MonoBehaviour
    {
        private Level _level;
        private KeyboardInput _keyboard;
        private MouseInput _mouse;
        private Vec3 _pos;
        private Vec3 _startPos;
        private World _world;
        private Track _track;
        private bool _onGround;
        private bool _dead;
        private bool _hasCompleted;
        private double _speed;
        private Vec3 _vel;
        private Part _currentPart;
        private IPlayerListener _listener;
        private AudioSource _breathSound;
        private bool _breathOn;
        private int _fallTime;
        
        private const double _MaxSpeed = 3.8;
        private const double _g = 0.14; // per tick
        
        public void Initialize(Level level, KeyboardInput keyboard, MouseInput mouse, Vec3 startPos)
        {
            _level = level;
            _keyboard = keyboard;
            _mouse = mouse;
            _pos = startPos.clone();
            _startPos = startPos.clone();
            _vel = new Vec3(0, 0, 0); // Initialize velocity
            _speed = 0; // Initialize speed
            _track = null; // Don't get track yet - will be set via SetTrack()
            _world = level.GetWorld();
            // Initialize breath sound placeholder
            _breathSound = gameObject.AddComponent<AudioSource>();
            Debug.Log($"[Player.Initialize] Player initialized, track will be set later");
        }
        
        public Vec3 GetPos() => _pos;
        
        public bool IsOnGround() => _onGround;
        public double GetSpeedAlpha() => _vel.length() / _MaxSpeed;
        public double GetSpeedY() => _vel.y;
        public Part GetCurrentPart() => _currentPart;
        public void SetListener(IPlayerListener listener) => _listener = listener;
        public bool HasCompleted() => _hasCompleted;
        public bool IsDead() => _dead;
        public double GetSpeed() => _speed;
        public Vec3 GetForwardDir() => _vel;
        
        // Main update method called each frame (or tick)
        // In Unity test mode, physics is handled by PlayerController
        // This method is only used for arm animation state
        public void Tick(int deltaTime)
        {
            // Debug: Show current position and state
            if (Time.frameCount % 30 == 0)
            {
                Debug.Log($"[Player.Tick] pos=({_pos.x:F2}, {_pos.y:F2}, {_pos.z:F2}), vel=({_vel.x:F2}, {_vel.y:F2}, {_vel.z:F2}), _onGround={_onGround}, _currentPart={(_currentPart != null ? "found" : "null")}");
            }

            // Flash physics system - restore full physics calculation
            _setWantedSpeeds();
            _clip();
            _entityInteraction();

            if (_onGround)
            {
                if (_breathOn && Random.value > 0.99)
                {
                    _breathOn = false;
                    // Audio.Sound.getSound(SBreath).setVolume(0, 500);
                }
                else if (!_breathOn && Random.value > 0.99)
                {
                    _breathOn = true;
                    // Audio.Sound.getSound(SBreath).setVolume(GetSpeedAlpha(), 500);
                }
            }
        }
        
        internal void _setWantedSpeeds()
        {
            bool loc1 = _keyboard.IsDown((int)KeyCode.Space) || _mouse.IsDown();
            if (_onGround)
            {
                bool loc2 = _keyboard.IsReleased((int)KeyCode.Space) || _mouse.HasRelease();
                _setWantedVelOnGround(loc1, loc2);
            }
            else
            {
                _setWantedVelInAir(loc1);
            }
        }
        
        internal void _setWantedVelOnGround(bool arg1, bool arg2)
        {
            if (_currentPart == null)
            {
                // Default behavior if no current part
                _speed = 0;
                _vel.copy(new Vec3(0, 0, 1)); // Default forward direction
                if (arg2)
                {
                    _vel.y = _vel.y + 4;
                    _onJump();
                }
                return;
            }

            _speed = _vel.dot(_currentPart.dir);
            _vel.copy(_currentPart.dir.scale(_speed));
            if (arg1)
            {
                _fallTime = 0;
                if (_speed < _MaxSpeed)
                {
                    _vel.addSelf(_currentPart.dir.scale(0.1));
                }
                double loc1 = -0.1 * _currentPart.dir.y;
                if (_speed < _MaxSpeed * 1.4)
                {
                    if (loc1 > 0)
                    {
                        _vel.addSelf(_currentPart.dir.scale(loc1));
                    }
                    else
                    {
                        _vel.addSelf(_currentPart.dir.scale(loc1));
                    }
                }
            }
            else if (arg2)
            {
                _vel.y = _vel.y + System.Math.Min(4, 1 + _speed);
                _onJump();
            }
        }
        
        internal void _setWantedVelInAir(bool arg1)
        {
            if (arg1)
            {
                _vel.y = System.Math.Min(0, _vel.y);
                _vel.y = _vel.y - 2 * _g;
            }
            _fallTime++;
            _vel.y = _vel.y - _g;
        }
        
        internal void _clip()
        {
            // Flash physics system: use track.getClosestPart() for collision detection
            Vec3 loc1 = _pos.add(_vel);
            Part loc2 = _track.GetClosestPart(loc1);

            if (loc2 != null)
            {
                // Calculate distance to surface using dot product with normal
                Vec3 offset = loc1.sub(loc2.pos);
                double distanceToSurface = offset.dot(loc2.normal);

                // Calculate horizontal distance to part center
                Vec3 horizontalOffset = offset.clone();
                horizontalOffset.y = 0;
                double horizontalDist = horizontalOffset.length();

                // Debug output
                if (Time.frameCount % 30 == 0)
                {
                    Debug.Log($"[Player._clip] loc1=({loc1.x:F2}, {loc1.y:F2}, {loc1.z:F2}), loc2.pos=({loc2.pos.x:F2}, {loc2.pos.y:F2}, {loc2.pos.z:F2}), distanceToSurface={distanceToSurface:F2}, horizontalDist={horizontalDist:F2}");
                }

                // Ground detection: player is within vertical and horizontal bounds of the part
                // Vertical: between -1 and 2 units above/below surface
                // Horizontal: within 3 units of part center
                bool loc6 = distanceToSurface >= -1 && distanceToSurface < 3 && horizontalDist < 3;

                if (loc6)
                {
                    _currentPart = loc2;
                }

                if (!_onGround && loc6)
                {
                    _onLand();
                    Debug.Log($"[Player._clip] Player landed! distanceToSurface={distanceToSurface:F2}");
                }

                _onGround = loc6;

                if (_onGround)
                {
                    // Stick to surface
                    _pos.copy(loc2.pos);
                    _pos.y += 0.1f; // Slight offset above surface

                    // Edge constraint: keep player within track bounds
                    Vec3 loc7 = _currentPart.GetPos();
                    Vec3 loc8 = _pos.sub(loc7);
                    // Use direction as proxy for right (perpendicular to forward and up)
                    Vec3 right = new Vec3(-loc2.dir.z, 0, loc2.dir.x);
                    double loc9 = loc8.dot(right);
                    if (System.Math.Abs(loc9) > 2)
                    {
                        Vec3 offsetConstraint = right.rescale(loc9 > 0 ? -0.4 : 0.4);
                        _pos.addSelf(offsetConstraint);
                    }
                }
                else
                {
                    // Free fall in air
                    _pos.addSelf(_vel);
                }
            }
            else
            {
                // No track found, free fall
                Debug.LogWarning($"[Player._clip] No closest part found at pos=({loc1.x:F2}, {loc1.y:F2}, {loc1.z:F2})");
                _pos.addSelf(_vel);
            }
        }
        
        internal void _entityInteraction()
        {
            if (_world == null) return; // Safety check

            RunnerEntity closest = _world.GetClosestEntity(_pos, 1);
            if (closest is SpeedEntity)
            {
                _vel.scaleSelf(1.2);
                closest.Remove();
            }
        }
        
        internal void _onJump()
        {
            // Audio.Sound.getSound(SBreath).setVolumeMod(0.25, 200);
        }
        
        internal void _onLand()
        {
            Vec3 loc1 = _vel.clone();
            if (loc1.lengthSqr() < Vec3Const.EpsilonSqr)
            {
                return;
            }
            loc1.normalizeSelf();

            // Check if currentPart is available
            if (_currentPart != null)
            {
                double loc2 = System.Math.Abs(loc1.dot(_currentPart.normal));
                if (_listener != null)
                {
                    _listener.onLand(loc2);
                }
            }
        }
        
        public void Destroy()
        {
            if (_breathSound != null)
            {
                _breathSound.Stop();
            }
        }
        
        // Helper methods for external access
        public void SetPosition(Vec3 pos) { _pos.copy(pos); }
        public void SetVelocity(Vec3 vel) { _vel.copy(vel); }

        // Set ground state externally (from PlayerController)
        public void SetOnGround(bool onGround) { _onGround = onGround; }

        // Set velocity magnitude for arm animation speed calculation
        public void SetSpeed(double speed) { _speed = speed; }

        public void SetCurrentPart(Part part) { _currentPart = part; }
    }
}