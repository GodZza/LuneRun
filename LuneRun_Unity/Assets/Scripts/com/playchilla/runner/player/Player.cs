using UnityEngine;
using shared.input;
using shared.math;
using com.playchilla.runner.track;
using com.playchilla.runner.track.entity;
using Random = UnityEngine.Random;
using System.Linq;


namespace com.playchilla.runner.player
{
    public class Player : MonoBehaviour
    {
        private Vector3 _pos;
        private Vector3 _startPos;
        private Vector3 _vel;

        private const float _MaxSpeed = 3.8f;
        private const float _g = 0.14f; // per tick

        private Track _track;
        private KeyboardInput _keyboard;
        private Part _currentPart;
        private int _fallTime;
        private bool _onGround;
        private IPlayerListener _listener;

        private MouseInput _mouse;
        private bool _hasCompleted;
        private bool _dead;
        private Level _level;
        private float _speed;


        private World _world => _level.GetWorld(); // 用于判断碰撞加速带

        private bool _breathOn;



        public Player Initialize(Level level, KeyboardInput keyboard, MouseInput mouse, Vec3 startPos)
        {
            _level = level;
            _track = level.GetTrack();
            _keyboard = keyboard;
            _mouse = mouse;

            _startPos = startPos.clone();
            _pos = startPos.clone();
            _pos.y = 151;

            _vel = new Vec3(0, 0, 0); // Initialize velocity
            _speed = 0; // Initialize speed
            // Initialize breath sound placeholder

            Debug.Log($"[Player.Initialize] Player initialized at pos=({_pos.x}, {_pos.y}, {_pos.z}), track={(_track != null ? "found" : "null")}");
            return this;
        }
        
        public Vec3 GetPos() => _pos; //?s
        
        public bool IsOnGround() => _onGround;
        public double GetSpeedAlpha() => _vel.magnitude / _MaxSpeed;
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
        public void Tick(int tick)
        {
            _currentPart ??= _track.GetClosestPart(_pos);


            // Flash physics system - restore full physics calculation
            _setWantedSpeeds();
            _clip();
            _entityInteraction();

            if(_pos.y < 1)
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

        internal void _entityInteraction()
        {
            var colliders = UnityEngine.Physics.OverlapSphere(_pos, 1);
            if (colliders == null || colliders.Length == 0) return;

            var closestCollider = colliders.OrderBy(col => Vector3.Distance(_pos, col.transform.position)).FirstOrDefault();

            if (closestCollider != null)
            {
                float closestDistance = Vector3.Distance(_pos, closestCollider.transform.position);
                Debug.Log("最近的物体是: " + closestCollider.gameObject.name + "，距离为: " + closestDistance);

                if ((object)closestCollider is SpeedEntity se) // TODO:
                {
                    _vel *= 1.2f;
                    se.Remove();
                }
            }
        }

        internal void _setWantedSpeeds()
        {
            var down = _keyboard.IsDown(KeyCode.Space) || _mouse.IsDown();
            var up   = _keyboard.IsReleased((int)KeyCode.Space) || _mouse.HasRelease();
            if (_onGround)
            {
                _setWantedVelOnGround(down, up);
            }
            else
            {
                _setWantedVelInAir(down);
            }
        }
        
        internal void _setWantedVelOnGround(bool down, bool up)
        {
            if (_currentPart == null)
            {
                // Default behavior if no current part
                _speed = 0;
                _vel = Vector3.forward; // Default forward direction
                if (up)
                {
                    _vel.y = _vel.y + 4;
                    _onJump();
                }
                return;
            }

            _speed = Vector3.Dot(_vel, _currentPart.dir);
            _vel = _currentPart.dir * _speed;
            if (down)
            {
                _fallTime = 0;
                if (_speed < _MaxSpeed)
                {
                    _vel += _currentPart.dir * 0.1f;
                }
                var scale = -0.1f * _currentPart.dir.y;
                if (_speed < _MaxSpeed * 1.4)
                {
                    if (scale > 0)
                    {
                        _vel += _currentPart.dir * scale;
                    }
                    else
                    {
                        _vel += _currentPart.dir * scale;
                    }
                }
            }
            else if (up)
            {
                _vel.y = _vel.y + System.Math.Min(4, 1 + _speed);
                _onJump();
            }
        }
        
        internal void _setWantedVelInAir(bool down)
        {
            if (down)
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
            var newPos = _pos + _vel;
            var part = _track.GetClosestPart(newPos);

            var surfacePos = default(Vector3?);

            if (part != null)
            {
                var loc3 = newPos + part.normal * 14;
                var loc4 = newPos - part.normal * 2;
                surfacePos = part.GetSurface(loc3, loc4);
                var willOnGround = surfacePos != null;
                if (willOnGround)
                {
                    this._currentPart = part;
                }
                if (!_onGround && willOnGround)
                {
                    _onLand();
                }
                _onGround = willOnGround;
            }

            if (_onGround)
            {
                // Stick to surface
                _pos = surfacePos.Value;
                _pos.y += 0.1f; // Slight offset above surface

                // Edge constraint: keep player within track bounds
                var loc7 = _currentPart.GetPos();
                var loc8 = _pos - (loc7);
                var loc9 = Vector3.Dot(loc8,_currentPart.right);
                if (System.Math.Abs(loc9) > 2)
                {
                    var offsetConstraint = _currentPart.right.normalized * (loc9 > 0 ? -0.4f : 0.4f);
                    _pos += (offsetConstraint);
                }
            }
            else
            {
                // Free fall in air
                _pos += (_vel);
            }
        }
        
       
        
        internal void _onJump()
        {
            com.playchilla.runner.Audio.Sound.getSound(SBreath).setVolumeMod(0.25, 200);
            // Audio.Sound.getSound(SBreath).setVolumeMod(0.25, 200);
        }
        
        internal void _onLand()
        {
            var loc1 = _vel;
            if (loc1.sqrMagnitude < Vec3Const.EpsilonSqr) //速度太小了
            {
                return;
            }
            loc1.Normalize();

            // 速度与地面法线夹角
            var impact = (float)System.Math.Abs( Vector3.Dot(loc1, _currentPart.normal));
            _listener?.onLand(impact);
            if (impact > 0.26)
            {
                //再来一次？？ 震动？？
                _listener?.onLand(impact);
            }
        }
        
        public void Destroy()
        {
            com.playchilla.runner.Audio.Sound.getSound(SBreath).stop(0);
        }
        
        // Helper methods for external access
        public void SetPosition(Vec3 pos) { _pos = (pos); }
        public void SetVelocity(Vec3 vel) { _vel = (vel); }

        // Set ground state externally (from PlayerController)
        public void SetOnGround(bool onGround) { _onGround = onGround; }

        // Set velocity magnitude for arm animation speed calculation
        public void SetSpeed(double speed) { _speed = (float)speed; }

        public void SetCurrentPart(Part part) { _currentPart = part; }

        const string SBreath = nameof(SBreath);
    }

}