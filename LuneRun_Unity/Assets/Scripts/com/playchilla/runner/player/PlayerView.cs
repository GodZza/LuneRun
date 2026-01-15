using UnityEngine;
using shared.input;
using shared.math;
using shared.render;
using com.playchilla.runner;

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

        public void Initialize(Level level, Player player, Camera camera, Materials materials, KeyboardInput keyboard)
        {
            _level = level;
            _player = player;
            _camera = camera;
            _playerCamera = gameObject.AddComponent<PlayerCam>();
            _playerCamera.Initialize(camera, player);
            _smoothPos = new SmoothPos3(player.GetPos());
            _smoothDir = new SmoothDir3();
            _keyboard = keyboard;
            
            // 创建手部立方体（根据原Flash游戏尺寸）
            CreateHandCubes();
            
            // Set player listener
            player.SetListener(this);
            
            // Position at player start
            Vec3 playerPos = player.GetPos();
            transform.position = new Vector3((float)playerPos.x, (float)playerPos.y, (float)playerPos.z);
        }

        private void CreateHandCubes()
        {
            // 上臂立方体 (尺寸: 0.1, 0.1, 0.5) - 原代码使用 CubeGeometry(0.1, 0.1, 0.5)
            _upperArmLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _upperArmRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            // 设置尺寸
            _upperArmLeft.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f);
            _upperArmRight.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f);
            
            // 设置父对象
            _upperArmLeft.transform.SetParent(transform);
            _upperArmRight.transform.SetParent(transform);
            
            // 初始位置（相对于玩家中心）
            // 原Flash代码：左右手臂分别位于身体两侧
            _upperArmLeft.transform.localPosition = new Vector3(-0.3f, 1.0f, 0.2f);
            _upperArmRight.transform.localPosition = new Vector3(0.3f, 1.0f, 0.2f);
            
            // 下臂立方体 (尺寸: 0.1, 0.1, 0.5) - 原代码同样使用 CubeGeometry(0.1, 0.1, 0.5)
            _lowerArmLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _lowerArmRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            _lowerArmLeft.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f);
            _lowerArmRight.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f);
            
            _lowerArmLeft.transform.SetParent(_upperArmLeft.transform);
            _lowerArmRight.transform.SetParent(_upperArmRight.transform);
            
            // 下臂连接到上臂末端
            _lowerArmLeft.transform.localPosition = new Vector3(0, 0, 0.25f);
            _lowerArmRight.transform.localPosition = new Vector3(0, 0, 0.25f);
            
            // 手指立方体 (尺寸: 0.03, 0.03, 0.15) - 原代码 CubeGeometry(0.03, 0.03, 0.15)
            _leftFingers = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _rightFingers = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            _leftFingers.transform.localScale = new Vector3(0.03f, 0.03f, 0.15f);
            _rightFingers.transform.localScale = new Vector3(0.03f, 0.03f, 0.15f);
            
            _leftFingers.transform.SetParent(_lowerArmLeft.transform);
            _rightFingers.transform.SetParent(_lowerArmRight.transform);
            
            // 手指连接到下臂末端
            // 原Flash代码：away3d.tools.helpers.MeshHelper.applyPosition(this._rightFingers, 0, 0, this._fingerGeometry.depth * 0.5);
            _leftFingers.transform.localPosition = new Vector3(0, 0, 0.25f + 0.075f);
            _rightFingers.transform.localPosition = new Vector3(0, 0, 0.25f + 0.075f);
            
            // 设置材质颜色（临时，可后续替换为真实材质）
            SetCubeColor(_upperArmLeft, Color.red);
            SetCubeColor(_upperArmRight, Color.red);
            SetCubeColor(_lowerArmLeft, Color.green);
            SetCubeColor(_lowerArmRight, Color.green);
            SetCubeColor(_leftFingers, Color.blue);
            SetCubeColor(_rightFingers, Color.blue);
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
            // Play footstep sound - placeholder
            // Audio.Sound.getSound(Audio.GetRandom(...)).Play(_player.GetSpeedAlpha());
        }

        void IPlayerListener.onLand(double impact)
        {
            _playerCamera.OnLand(impact);
        }
        
        public void OnLand(double impact)
        {
            _playerCamera.OnLand(impact);
        }

        public void Render(int time, double alpha)
        {
            double currentTime = time + alpha;
            Vec3 smoothPosition = _smoothPos.GetPos(_player.GetPos(), time, alpha);
            transform.position = new Vector3((float)smoothPosition.x, (float)smoothPosition.y, (float)smoothPosition.z);
            
            // 更新手部动画
            UpdateHandAnimation(time, alpha);
            
            // Update camera
            if (_firstPerson)
            {
                _playerCamera.Render(time, alpha);
            }
            
            _lastTime = currentTime;
        }

        public void RenderTick(int deltaTime)
        {
            if (_firstPerson)
            {
                _playerCamera.RenderTick(deltaTime);
            }
        }

        private void UpdateHandAnimation(int time, double alpha)
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

        private void AnimateInAir(int time, double alpha)
        {
            // 原Flash代码：loc1 = 10 * (arg1 + arg2);
            // this._upperArmLeft.rotateTo(loc1, 0, 0);
            // this._upperArmRight.rotateTo(loc1 + 180, 0, 0);
            // this._lowerArmLeft.rotateTo(70, 0, 0);
            // this._lowerArmRight.rotateTo(70, 0, 0);
            
            float loc1 = 10f * (time + (float)alpha);
            
            if (_upperArmLeft != null)
                _upperArmLeft.transform.localRotation = Quaternion.Euler(loc1, 0, 0);
            if (_upperArmRight != null)
                _upperArmRight.transform.localRotation = Quaternion.Euler(loc1 + 180f, 0, 0);
            if (_lowerArmLeft != null)
                _lowerArmLeft.transform.localRotation = Quaternion.Euler(70f, 0, 0);
            if (_lowerArmRight != null)
                _lowerArmRight.transform.localRotation = Quaternion.Euler(70f, 0, 0);
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

        private void UpdateCamera()
        {
            if (_firstPerson)
            {
                return;
            }
            
            // Camera control via keyboard - simplified for Unity
            float pitchDelta = 0;
            float yawDelta = 0;
            
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

        private void ApplyHeadShake(int time, double alpha)
        {
            // Simplified head shake
        }

        public PlayerCam getCam()
        {
            return _playerCamera;
        }

        public PlayerCam GetCam()
        {
            return _playerCamera;
        }
        
        // Unity Update 方法，用于实时更新动画（如果使用Render方法，可以省略）
        private void Update()
        {
            // 如果使用Render方法，可以注释掉这里
            // UpdateHandAnimation((int)(Time.time * 1000), Time.deltaTime);
            
            // Alternative: update animation based on Unity's Update loop
            // This ensures arm cubes are animated even if Render isn't called
            if (_player != null)
            {
                int currentTime = (int)(Time.time * 1000);
                float alpha = Time.deltaTime;
                UpdateHandAnimation(currentTime, alpha);
            }
        }

    }
}