using UnityEngine;
using shared.input;
using shared.math;
using shared.render;
using com.playchilla.runner;
using System;

namespace com.playchilla.runner.player
{
    public class PlayerView : MonoBehaviour, IPlayerListener
    {
        private Level _level;
        private Player _player;
        private Camera _camera;
        private PlayerCam _playerCamera;
        private SmoothPos3 _smoothPos;
        private SmoothDir3 _smoothDir;
        private KeyboardInput _keyboard;

        // Body parts GameObjects - 手部立方体
        private GameObject _leftArm;
        private GameObject _rightArm;
        private GameObject _leftLeg;
        private GameObject _rightLeg;

        private GameObject _upperArmLeft;
        private GameObject _upperArmRight;
        private GameObject _lowerArmLeft;
        private GameObject _lowerArmRight;
        private GameObject _rightFingers;
        private GameObject _leftFingers;
        
        private Vec3 _headOffset = new Vec3(0, 2, 0);
        private double _lastTime;
        private double _roll;
        private double _lastRoll;
        private double _yaw;
        private double _pitch;
        private bool _firstPerson;

        private double _disposX=0;
        private double _disposY=0;
        private double _lastDisposX;
        private double _period=0;

        public PlayerView Initialize(Level level, Player player, Camera camera, Materials materials, KeyboardInput keyboard)
        {
            _level = level;
            _player = player;
            _camera = camera;
            _playerCamera = Utils.New<PlayerCam>().Initialize(camera, player);
            _smoothPos = new SmoothPos3(player.GetPos());
            _smoothDir = new SmoothDir3();
            _keyboard = keyboard;
            
            // 创建手部立方体（根据原Flash游戏尺寸）
            CreateHandCubes();
            CreateLegCubes();
            
            // Set player listener
            player.SetListener(this);
            
            // Position at player start
            var playerPos = player.GetPos();
            transform.position = playerPos;
            _playerCamera.transform.SetParent(transform, false);
            _playerCamera.transform.localPosition = new Vector3(0, (float)_headOffset.y, 0);
            return this;
        }

        private void CreateHandCubes()
        {
            Debug.Log("[PlayerView] Creating hand cubes for complete arm system");

            _leftArm = new GameObject(nameof(_leftArm));
            _rightArm = new GameObject(nameof(_rightArm));

            //_upperArmLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // 上臂立方体 (尺寸: 0.1, 0.1, 0.5) - 原代码使用 CubeGeometry(0.1, 0.1, 0.5)
            float debugScale = 1.0f;
            
            _upperArmLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _upperArmRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            // 设置尺寸（临时放大）
            _upperArmLeft.transform.localScale = new Vector3(0.1f * debugScale, 0.1f * debugScale, 0.5f * debugScale);
            _upperArmRight.transform.localScale = new Vector3(0.1f * debugScale, 0.1f * debugScale, 0.5f * debugScale);
            
            // 设置父对象
            _upperArmLeft.transform.SetParent(transform);
            _upperArmRight.transform.SetParent(transform);
            
            // 初始位置（相对于玩家中心）
            // 原Flash代码：左右手臂分别位于身体两侧
            _upperArmLeft.transform.localPosition = new Vector3(-0.3f * debugScale, 1.0f * debugScale, 0.2f * debugScale);
            _upperArmRight.transform.localPosition = new Vector3(0.3f * debugScale, 1.0f * debugScale, 0.2f * debugScale);
            
            // 下臂立方体 (尺寸: 0.1, 0.1, 0.5) - 原代码同样使用 CubeGeometry(0.1, 0.1, 0.5)
            _lowerArmLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _lowerArmRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            _lowerArmLeft.transform.localScale = new Vector3(0.1f * debugScale, 0.1f * debugScale, 0.5f * debugScale);
            _lowerArmRight.transform.localScale = new Vector3(0.1f * debugScale, 0.1f * debugScale, 0.5f * debugScale);
            
            _lowerArmLeft.transform.SetParent(_upperArmLeft.transform);
            _lowerArmRight.transform.SetParent(_upperArmRight.transform);
            
            // 下臂连接到上臂末端
            _lowerArmLeft.transform.localPosition = new Vector3(0, 0, 0.25f * debugScale);
            _lowerArmRight.transform.localPosition = new Vector3(0, 0, 0.25f * debugScale);
            
            // 手指立方体 (尺寸: 0.03, 0.03, 0.15) - 原代码 CubeGeometry(0.03, 0.03, 0.15)
            _leftFingers = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _rightFingers = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            _leftFingers.transform.localScale = new Vector3(0.03f * debugScale, 0.03f * debugScale, 0.15f * debugScale);
            _rightFingers.transform.localScale = new Vector3(0.03f * debugScale, 0.03f * debugScale, 0.15f * debugScale);
            
            _leftFingers.transform.SetParent(_lowerArmLeft.transform);
            _rightFingers.transform.SetParent(_lowerArmRight.transform);
            
            // 手指连接到下臂末端
            // 原Flash代码：away3d.tools.helpers.MeshHelper.applyPosition(this._rightFingers, 0, 0, this._fingerGeometry.depth * 0.5);
            _leftFingers.transform.localPosition = new Vector3(0, 0, (0.25f + 0.075f) * debugScale);
            _rightFingers.transform.localPosition = new Vector3(0, 0, (0.25f + 0.075f) * debugScale);
            
            // 设置材质颜色（临时，可后续替换为真实材质）
            // 使用更鲜艳的颜色
            SetCubeColor(_upperArmLeft, new Color(1, 0, 0, 1)); // 亮红色
            SetCubeColor(_upperArmRight, new Color(1, 0, 0, 1)); // 亮红色
            SetCubeColor(_lowerArmLeft, new Color(0, 1, 0, 1)); // 亮绿色
            SetCubeColor(_lowerArmRight, new Color(0, 1, 0, 1)); // 亮绿色
            SetCubeColor(_leftFingers, new Color(0, 0, 1, 1)); // 亮蓝色
            SetCubeColor(_rightFingers, new Color(0, 0, 1, 1)); // 亮蓝色
            
            Debug.Log($"[PlayerView] Hand cubes created: upper arms={_upperArmLeft != null}/{_upperArmRight != null}, lower arms={_lowerArmLeft != null}/{_lowerArmRight != null}, fingers={_leftFingers != null}/{_rightFingers != null}");
        }

        private void CreateLegCubes()
        {
            Debug.Log("TODO");
        }


        private void SetCubeColor(GameObject cube, Color color)
        {
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = color;
            }
        }

        private void OnFootDown()
        {
            // TODO: Play footstep sound - placeholder
            // Audio.Sound.getSound(Audio.GetRandom(...)).Play(_player.GetSpeedAlpha());
            //com.playchilla.runner.Audio.Sound.getSound(com.playchilla.runner.Audio.getRandom([SFootstep1, SFootstep2, SFootstep3, SFootstep4, SFootstep5])).play(this._player.getSpeedAlpha());
        }

        void IPlayerListener.onLand(float impact)
        {
            _playerCamera.OnLand(impact);
        }
        

        public void Render(int tick, double alpha)
        {
            if (_player == null)
            {
                Debug.LogWarning("[PlayerView] Render: _player is null!");
                return;
            }
            
            var currentTime = tick + alpha;
            var smoothPosition = _smoothPos.GetPos(_player.GetPos(), tick, alpha);
            transform.position = smoothPosition;
            
            // 更新手部动画
            UpdateHandAnimation(tick, alpha);

            var part = _player.GetCurrentPart();
            if (part != null)
            {
                var delta = tick + alpha - _lastTime;

                var x = default(Vector3);
                if (_player.IsOnGround())
                {
                    x = part.dir;
                }
                else
                {
                    var nextForward = _player.GetPos() + new Vector3((float)(100 * part.dir.x), 0, (float)(100 * part.dir.z));
                    nextForward.y -= 30;
                    x = (nextForward - _player.GetPos()).normalized;
                }
                var forwardVector = Vector3.Lerp(this.transform.forward, x, (float)(0.15 * delta));
                var upVector = Vector3.Lerp(transform.up, part.normal, (float)(0.2 * delta));

                this.transform.LookAt(this.transform.position + this.transform.forward, upVector);
            }

            if (_player.IsOnGround())
            {
                ApplyHeadShake(tick, alpha);
                AnimateOnGround(tick, alpha);
            }
            else
            {
                AnimateInAir(tick, alpha);
            }

            if (_firstPerson)
            {
                //_playerCamera.Render(tick, alpha); // OLD
                
            }
            UpdateCamera();
            
            _lastTime = currentTime;
        }

        public void RenderTick(int deltaTime)
        {
            if (_firstPerson)
            {
                _playerCamera.RenderTick(deltaTime);
            }

            var part = _player.GetCurrentPart();
            if(part != null)
            {
                _lastRoll = _roll;
                _roll = _roll + 0.1 * (part.zRot - _roll);
            }
        }

        private void UpdateCamera() // 这里默认就是第一人称的，暂时不管 TODO:
        {
            if (_firstPerson)
            {
                return;
            }
            // 原Flash代码
            //var loc1:*= shared.math.Conv.deg2rad(5);
            //var loc2:*= shared.math.Conv.deg2rad(5);
            //if (this._keyboard.isDown(flash.ui.Keyboard.LEFT))
            //{
            //    this._yaw = this._yaw - loc1;
            //}
            //if (this._keyboard.isDown(flash.ui.Keyboard.RIGHT))
            //{
            //    this._yaw = this._yaw + loc1;
            //}
            //if (this._keyboard.isDown(flash.ui.Keyboard.UP))
            //{
            //    this._pitch = this._pitch + loc2;
            //}
            //if (this._keyboard.isDown(flash.ui.Keyboard.DOWN))
            //{
            //    this._pitch = this._pitch - loc2;
            //}
            //if (this._keyboard.isDown(flash.ui.Keyboard.W))
            //{
            //    this._camera.moveForward(20);
            //}
            //if (this._keyboard.isDown(flash.ui.Keyboard.S))
            //{
            //    this._camera.moveBackward(20);
            //}
            //if (this._keyboard.isDown(flash.ui.Keyboard.A))
            //{
            //    this._camera.moveLeft(20);
            //}
            //if (this._keyboard.isDown(flash.ui.Keyboard.D))
            //{
            //    this._camera.moveRight(20);
            //}
            //if (this._keyboard.isDown(flash.ui.Keyboard.SHIFT))
            //{
            //    this._camera.moveUp(4);
            //}
            //if (this._keyboard.isDown(flash.ui.Keyboard.CONTROL))
            //{
            //    this._camera.moveDown(4);
            //}
            //var loc3:*= new flash.geom.Vector3D();
            //loc3.x = 10 * Math.sin(this._yaw) * Math.cos(this._pitch) + this._camera.x;
            //loc3.z = 10 * Math.cos(this._yaw) * Math.cos(this._pitch) + this._camera.z;
            //loc3.y = 10 * Math.sin(this._pitch) + this._camera.y;
            //this._camera.lookAt(loc3);



            // Camera control via keyboard - simplified for Unity
            float pitchDelta = Mathf.Deg2Rad * (5);
            float yawDelta = Mathf.Deg2Rad * (5);

            if (_keyboard.IsDown(KeyCode.LeftArrow))
            {

            }

            if (Input.GetKey(KeyCode.LeftArrow))
                yawDelta -= 5f;
            if (Input.GetKey(KeyCode.RightArrow))
                yawDelta += 5f;
            if (Input.GetKey(KeyCode.UpArrow))
                pitchDelta += 5f;
            if (Input.GetKey(KeyCode.DownArrow))
                pitchDelta -= 5f;

            _yaw += yawDelta * Mathf.Deg2Rad;
            _pitch += pitchDelta * Mathf.Deg2Rad;

            // WASD movement
            float moveSpeed = 20f;
            if (Input.GetKey(KeyCode.W))
                _camera.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.S))
                _camera.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.A))
                _camera.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.D))
                _camera.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.LeftShift))
                _camera.transform.Translate(Vector3.up * 4f * Time.deltaTime);
            if (Input.GetKey(KeyCode.LeftControl))
                _camera.transform.Translate(Vector3.down * 4f * Time.deltaTime);

            // Look at target
            Vector3 lookTarget = _camera.transform.position + _camera.transform.forward * 10f;
            _camera.transform.LookAt(lookTarget);
        }

        private void AnimateInAir(int tick, double alpha)
        {
            // 原Flash代码：loc1 = 10 * (arg1 + arg2);
            // this._upperArmLeft.rotateTo(loc1, 0, 0);
            // this._upperArmRight.rotateTo(loc1 + 180, 0, 0);
            // this._lowerArmLeft.rotateTo(70, 0, 0);
            // this._lowerArmRight.rotateTo(70, 0, 0);

            float loc1 = 10f * (tick + (float)alpha);

            if (_upperArmLeft != null)
                _upperArmLeft.transform.localRotation = Quaternion.Euler(loc1, 0, 0);
            if (_upperArmRight != null)
                _upperArmRight.transform.localRotation = Quaternion.Euler(loc1 + 180f, 0, 0);
            if (_lowerArmLeft != null)
                _lowerArmLeft.transform.localRotation = Quaternion.Euler(70f, 0, 0);
            if (_lowerArmRight != null)
                _lowerArmRight.transform.localRotation = Quaternion.Euler(70f, 0, 0);
        }
        private void UpdateHandAnimation(int time, double alpha) //?
        {
            if (_player == null) return;
            
            // 获取玩家状态
            bool isOnGround = _player.IsOnGround();
            float speedAlpha = (float)_player.GetSpeedAlpha();
            
            if (isOnGround)
            {
                AnimateOnGround(time, alpha);
            }
            else
            {
                AnimateInAir(time, alpha);
            }
        }


        private void AnimateOnGround(int time, double alpha)
        {
            // 原Flash代码：
            // if (this._player.getSpeedAlpha() > 0.1) {
            //     loc1 = -10 - 60 * (-this._disposX);
            //     loc2 = 20 - 40 * (-this._disposX);
            //     loc3 = -10 - 60 * this._disposX;
            //     loc4 = 20 - 40 * this._disposX;
            //     this._upperArmLeft.rotateTo(loc1, 0, 0);
            //     this._lowerArmLeft.rotateTo(loc2, 0, 0);
            //     this._upperArmRight.rotateTo(loc3, 0, 0);
            //     this._lowerArmRight.rotateTo(loc4, 0, 0);
            // } else {
            //     this._upperArmLeft.rotateTo(0, 0, 0);
            //     this._lowerArmLeft.rotateTo(0, 0, 0);
            //     this._upperArmRight.rotateTo(0, 0, 0);
            //     this._lowerArmRight.rotateTo(0, 0, 0);
            // }
            
            if (_player == null) return;
            
            float speedAlpha = (float)_player.GetSpeedAlpha();
            
            if (speedAlpha > 0.1f)
            {
                // 简化：使用固定的摆动动画
                float wave = Mathf.Sin(time * 0.1f + (float)alpha) * 30f;
                float wave2 = Mathf.Cos(time * 0.1f + (float)alpha) * 20f;
                
                if (_upperArmLeft != null)
                    _upperArmLeft.transform.localRotation = Quaternion.Euler(-10f + wave, 0, 0);
                if (_lowerArmLeft != null)
                    _lowerArmLeft.transform.localRotation = Quaternion.Euler(20f + wave2, 0, 0);
                if (_upperArmRight != null)
                    _upperArmRight.transform.localRotation = Quaternion.Euler(-10f - wave, 0, 0);
                if (_lowerArmRight != null)
                    _lowerArmRight.transform.localRotation = Quaternion.Euler(20f - wave2, 0, 0);
            }
            else
            {
                // 静止状态
                if (_upperArmLeft != null)
                    _upperArmLeft.transform.localRotation = Quaternion.Euler(0, 0, 0);
                if (_lowerArmLeft != null)
                    _lowerArmLeft.transform.localRotation = Quaternion.Euler(0, 0, 0);
                if (_upperArmRight != null)
                    _upperArmRight.transform.localRotation = Quaternion.Euler(0, 0, 0);
                if (_lowerArmRight != null)
                    _lowerArmRight.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void ApplyHeadShake(int tick, double alpha)
        {
            if (!_player.IsOnGround()) return;

            var speedAlpha = Math.Min(0.9f, _player.GetSpeedAlpha());
            var tt = tick + alpha;
            var timeDelta = tt - _lastTime;

            // 更新周期，速度越快周期增加越快
            _period += (0.6f + speedAlpha) * timeDelta;

            // 计算X轴和Y轴的位移
            _disposX = Math.Sin(_period / 4f);
            _disposY = Math.Sin(_period / 2f + Mathf.PI * 0.5f);

            // 应用位移到当前位置
            Vector3 pos = transform.position;
            pos.x += (float)(0.4f * speedAlpha * _disposX);
            pos.y += (float)(0.3f * speedAlpha * _disposY);
            transform.position = pos;

            // 如果X轴位移方向发生变化（穿过零点），触发脚步落地事件
            if (_disposX * _lastDisposX < 0)
            {
                OnFootDown();
            }

            // 记录当前位移用于下一次比较
            _lastDisposX = _disposX;

        }





        public PlayerCam getCam()
        {
            return _playerCamera;
        }

        
        // Unity Update 方法，用于实时更新动画（如果使用Render方法，可以省略）
        private void Update()
        {
            Update(Time.deltaTime);

        }




        void Update(float deltaTime)
        {
            if (_player == null)
            {
                Debug.LogWarning("[PlayerView] Render: _player is null!");
                return;
            }

            //var smoothPosition = _smoothPos.GetPos(_player.GetPos(), time, alpha);
            var smoothPosition = Vector3.Lerp(transform.position, _player.GetPos(), 0.5f); // 临时的
            transform.position = smoothPosition;

            // 更新手部动画
            UpdateHandAnimation(deltaTime);

            var part = _player.GetCurrentPart();
            if (part != null)
            {
                var x = default(Vector3);
                if (_player.IsOnGround())
                {
                    x = part.dir;
                }
                else
                {
                    var nextForward = _player.GetPos() + new Vector3((float)(100 * part.dir.x), 0, (float)(100 * part.dir.z));
                    nextForward.y -= 30;
                    x = (nextForward - _player.GetPos()).normalized;
                }
                var forwardVector = Vector3.Lerp(this.transform.forward, x, 0.15f * deltaTime);
                var upVector = Vector3.Lerp(transform.up, part.normal, 0.2f * deltaTime);

                this.transform.LookAt(this.transform.position + this.transform.forward, upVector);
            }

            if (_player.IsOnGround())
            {
                ApplyHeadShake(deltaTime);
                AnimateOnGround(deltaTime);
            }
            else
            {
                AnimateInAir(deltaTime);
            }

            if (_firstPerson)
            {
                _playerCamera.Tick(deltaTime);
            }
            UpdateCamera();
        }



        private void AnimateInAir(float detaTime)
        {
            // 原Flash代码：loc1 = 10 * (arg1 + arg2);
            // this._upperArmLeft.rotateTo(loc1, 0, 0);
            // this._upperArmRight.rotateTo(loc1 + 180, 0, 0);
            // this._lowerArmLeft.rotateTo(70, 0, 0);
            // this._lowerArmRight.rotateTo(70, 0, 0);

        }

        private void UpdateHandAnimation(float detaTime)
        {
            if (_player == null) return;

            // 获取玩家状态
            bool isOnGround = _player.IsOnGround();
            float speedAlpha = (float)_player.GetSpeedAlpha();

            if (isOnGround)
            {
                AnimateOnGround(detaTime);
            }
            else
            {
                AnimateInAir(detaTime);
            }
        }
        private void ApplyHeadShake(float detaTime)
        {
            if (!_player.IsOnGround()) return;

            var speedAlpha = Math.Min(0.9f, _player.GetSpeedAlpha());

            // 更新周期，速度越快周期增加越快
            _period += (0.6f + speedAlpha) * detaTime;

            // 计算X轴和Y轴的位移
            _disposX = Math.Sin(_period / 4f);
            _disposY = Math.Sin(_period / 2f + Mathf.PI * 0.5f);

            // 应用位移到当前位置
            Vector3 pos = transform.position;
            pos.x += (float)(0.4f * speedAlpha * _disposX);
            pos.y += (float)(0.3f * speedAlpha * _disposY);
            transform.position = pos;

            // 如果X轴位移方向发生变化（穿过零点），触发脚步落地事件
            if (_disposX * _lastDisposX < 0)
            {
                OnFootDown();
            }

            // 记录当前位移用于下一次比较
            _lastDisposX = _disposX;

        }


        private void AnimateOnGround(float deltaTime)
        {
            // 原Flash代码：
            // if (this._player.getSpeedAlpha() > 0.1) {
            //     loc1 = -10 - 60 * (-this._disposX);
            //     loc2 = 20 - 40 * (-this._disposX);
            //     loc3 = -10 - 60 * this._disposX;
            //     loc4 = 20 - 40 * this._disposX;
            //     this._upperArmLeft.rotateTo(loc1, 0, 0);
            //     this._lowerArmLeft.rotateTo(loc2, 0, 0);
            //     this._upperArmRight.rotateTo(loc3, 0, 0);
            //     this._lowerArmRight.rotateTo(loc4, 0, 0);
            // } else {
            //     this._upperArmLeft.rotateTo(0, 0, 0);
            //     this._lowerArmLeft.rotateTo(0, 0, 0);
            //     this._upperArmRight.rotateTo(0, 0, 0);
            //     this._lowerArmRight.rotateTo(0, 0, 0);
            // }

        }
    }
}