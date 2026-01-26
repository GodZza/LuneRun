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
            _track = level.GetTrack();
            _world = level.GetWorld();
            // Initialize breath sound placeholder
            _breathSound = gameObject.AddComponent<AudioSource>();
            // Additional initialization as needed
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
        public void Tick(int deltaTime)
        {
            if (_track != null)
            {
                if (_currentPart == null)
                {
                    _currentPart = _track.GetClosestPart(_pos);
                }
            }
            _setWantedSpeeds();
            _clip();
            _entityInteraction();
            if (_pos.y < 1)
            {
                _dead = true;
            }
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
            // Simplified collision detection using Unity's CharacterController
            // In original Flash, this used track.getClosestPart() and surface detection
            // For now, we'll rely on the CharacterController component attached to the GameObject
            // The actual ground detection will be handled by Unity's physics
            
            // This method should update _onGround based on collision detection
            // For simplicity, we'll assume the CharacterController handles ground detection
            // and we'll read it from the controller in the UpdatePlayer method of PlayerController
            
            // Position is now synchronized from PlayerController, so we don't update it here
            // Placeholder: update position based on velocity (if not using CharacterController)
            if (_onGround)
            {
                // Stick to surface (simplified)
                // In original: _pos.copy(loc5); where loc5 is surface point
            }
            else
            {
                // _pos.addSelf(_vel); // Disabled - position synchronized from PlayerController
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
        public void SetOnGround(bool onGround) { _onGround = onGround; }
        public void SetCurrentPart(Part part) { _currentPart = part; }
    }
}