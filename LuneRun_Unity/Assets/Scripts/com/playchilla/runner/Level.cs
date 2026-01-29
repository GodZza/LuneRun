using UnityEngine;
using com.playchilla.runner.api;
using com.playchilla.runner.player;
using com.playchilla.runner.track;
using com.playchilla.gameapi.api;
using shared.input;
using com.playchilla.runner.track.entity;
using shared.math;
using com.playchilla.runner.ambient;
using System;

namespace com.playchilla.runner
{
    public partial class Level : MonoBehaviour, IScoreCallback
    {
        private int _levelId;
        private bool _isHardware;
        private Player _player;

        private bool _hasCompleted = false;

        public int GetLevelId()
        {
            return _levelId;
        }
        private Materials _materials;
        private GameObject _gameCont; // Placeholder for game container
        private World _world; // World instance
        private PlayerView _playerView;
        private com.playchilla.runner.track.DynamicTrack _dynamicTrack; // 动态轨道系统
        private KeyboardInput _keyboard; // Reference for input synchronization
        private Camera _camera;
        private MouseInput _mouse;
        private Settings _settings;
        private IRunnerApi _runnerApi;
        private double _horizDist;
        private Light _light1; // dir
        private Light _light2; // point
        private TrackView _trackView;
        private int _tutorialStep;
        private Curtain _curtain;
        public Level Initialize(Camera camera, MouseInput mi, KeyboardInput ki, int levelId, bool showChapter, bool isHardware, Settings settings, IRunnerApi runnerApi)
        {
            _gameCont = new GameObject("GameContainer");

            _isHardware = isHardware;
            _camera = camera;
            _mouse = mi;
            _keyboard = ki;
            _settings = settings;
            _runnerApi = runnerApi;

            _levelId = levelId;
            _generateForLevel = levelId;

            // 设置摄像头
            _camera.fieldOfView = 90;
            _camera.nearClipPlane = 0.01f;
            _camera.farClipPlane = 40000f;
            _horizDist = _camera.farClipPlane / Math.Sqrt(3);
            _showChapter = showChapter;

            _light1 = Utils.New<Light>("light1-dir");
            _light1.type = LightType.Directional;
            _light1.color = new Color32(0xA0, 0xA0, 0x00, 0xFF); //10526720
            //_light1.diffuse = 0;
            //_light1.specular = 0.2;
            //_light1.ambient = 0.6;

            _light2 = Utils.New<Light>("light2-point");
            _light2.type = LightType.Point;
            _light2.range = 1000;
            //_light2.fallOff = 2000;
            _light2.color = new Color32(0xA0, 0xA0, 0xA0,0xFF);
            //_light2.diffuse = 0.6;
            //_light2.specular = 0.2;
            //_light2.ambient = 0.6;

            if(isHardware) // 支持硬件加速
            {

            }
            else
            {
                UnityEngine.Object.DestroyImmediate(_light2.gameObject);
            }

            // Create materials instance
            _materials = new Materials();

            // Create dynamic track system (creates track internally)
            _dynamicTrack = new com.playchilla.runner.track.DynamicTrack(this, 6, 2);
            _trackView = Utils.New<TrackView>().Initialize(_dynamicTrack.GetTrack());

            // 玩家
            var startPos = _dynamicTrack.GetTrack().GetStartPos().add(new Vec3(0, 0, 10));
            _player = Utils.New<Player>().Initialize(this, _keyboard, _mouse, startPos);
            _playerView = Utils.New<PlayerView>().Initialize(this, _player, _camera, _materials, _keyboard);

            UpdateLevelInfo();
            SetupGround();

            _tutorialStep = this._levelId == 1 || !this._settings.HasSeenTutorial() ? 0 : -1;

            SetupSkybox(); 
            SetupChapter();

            _noah = Utils.New<Noah>().Initialize(new Light[] { _light1, _light2 });
            _curtain = Utils.New<Curtain>().Initialize(3, 1);

            Debug.Log($"[Level] Initialized level {levelId} with world, player, and complete arm system");
            return this;
        }

        void SetupGround()
        {

        }

        void SetupSkybox() { }

        void SetupChapter()
        {

        }
        void UpdateTutorial()
        {

        }

        void UpdateLevelInfo() { }
        void UpdateTimeInfo(bool visible) { }

        Noah GetNoah() => null;

        void CenterGround()
        {

        }

        void Destroy()
        {

        }

        // render ,renderTick , update
        int _startTick;
        int _generateForLevel;
        double _time;
        bool _isFirst;
        Noah _noah;
        bool _showChapter;
        public void Tick(int tick)
        {
            if (this._startTick == -1) 
            {
                this._startTick = tick;
            }
            //this._world.tick(arg1);
            //if (this._chapter == null) 
            //{
            //    this._updateTutorial();
            //}
            //else 
            //{
            //    this._chapter.tick(arg1);
            //    if (this._chapter.isDone())
            //    {
            //        this._chapter = null;
            //    }
            //}
            //com.playchilla.runner.Engine.pt.start("player.tick");
            this._player.Tick(tick);
            //com.playchilla.runner.Engine.pt.stop("player.tick");
            //com.playchilla.runner.Engine.pt.start("dynamicTrack.update");
            var loc1 = 10 + (this._levelId< 25 ? this._levelId / 32 * 8 : this._levelId / 32 * 16);
            if (this._generateForLevel == 1) 
            {
                loc1 = 17;
            }
            if (this._generateForLevel == 20) 
            {
                loc1 = 2;
            }
            if (this._dynamicTrack.Update(this._player.GetCurrentPart(), this._generateForLevel, loc1)) 
            {
                _generateForLevel += 1;
            }
            var segmentLevelId =this._player.GetCurrentPart().segment.GetLevelId();

            if (segmentLevelId != this._levelId && segmentLevelId != -1) 
            {
                this._time = com.playchilla.runner.Tick.ticksToSec(tick - this._startTick);
                this._isFirst = false;
                this._startTick = tick;
                var time = (int)Math.Round(this._time* 1000);
                //this._runnerApi.score(this._levelId, loc3, this);
                _levelId += 1;
                this.UpdateLevelInfo();
                if (this._levelId > 32) 
                {
                    this._hasCompleted = true;
                }
                this._showChapter = true;
                this.SetupChapter();
            }
            //com.playchilla.runner.Engine.pt.stop("dynamicTrack.update");
            //com.playchilla.runner.Engine.pt.start("ambience.tick");
            //this._ambience.tick(arg1); //------------------------------------------
            //com.playchilla.runner.Engine.pt.stop("ambience.tick");
            //com.playchilla.runner.Engine.pt.start("center ground");
            this.CenterGround();
            //com.playchilla.runner.Engine.pt.stop("center ground");
            //this._moon.rotationY = this._moon.rotationY + 0.02;
            this._noah?.Tick(tick);
        }


        private void AddTestEntities()
        {
            // Add a few speed entities along the track for testing
            for (int i = 0; i < 5; i++)
            {
                Vec3 position = new Vec3(0, 5, 10 + i * 20); // Position along Z axis
                SpeedEntity speedEntity = new SpeedEntity(position, this, null); // Segment is null for now
                _world.AddEntity(speedEntity);
            }
            Debug.Log("[Level] Added test entities");
        }
        
        public Player GetPlayer()
        {
            return _player;
        }

        public Player getPlayer()
        {
            return _player;
        }

        public Materials GetMaterials()
        {
            return _materials;
        }
        
        public GameObject GetGameCont()
        {
            return _gameCont;
        }
        
        public World GetWorld()
        {
            return _world;
        }

        public bool HasFailed() => this._player.IsDead();
        public bool HasCompleted() => this._hasCompleted;
        
        // IScoreCallback implementation (stubs)
        void IScoreCallback.Score(Score score, bool isNewHighscore)
        {
            Debug.Log($"[Level] Score posted: {score.GetScore()}, new highscore: {isNewHighscore}");
        }
        
        void IScoreCallback.ScoreError(ErrorResponse error)
        {
            Debug.LogError($"[Level] Score error: {error.GetMessage()}");
        }
        
        // Keep original methods for backward compatibility
        public void OnScorePosted(Score score)
        {
            Debug.Log($"[Level] Score posted: {score.GetScore()}");
        }
        
        public void OnScoreError(ErrorResponse error)
        {
            Debug.LogError($"[Level] Score error: {error.GetMessage()}");
        }

        public void TopPush(string arg1, System.Action arg2)
        {
            // _top.info.text = arg1;
            // _top.info.alpha = 0;
            // gs.TweenLite.to(_top.info, 0.4, new { alpha = 1, ease = gs.easing.Linear.easeIn });
            // gs.TweenLite.to(this._top.info, 0.4, {"alpha":0, "ease":gs.easing.Linear.easeIn, "delay":5, "onComplete":arg2, "overwrite":false});
            return;
        }
        public void topPush(string arg1, System.Action arg2)
        {
            TopPush(arg1, arg2);
        }

        public com.playchilla.runner.player.PlayerView GetPlayerView()
        {
            return this._playerView;
        }
        public com.playchilla.runner.player.PlayerView getPlayerView()
        {
            return this._playerView;
        }

        public Track GetTrack()
        {
            return null;
        }

        public Track getTrack()
        {
            return null;
        }

        // Set track externally (for when track is created after Level)
        public void SetTrack(Track track)
        {
            //_track = track;
            //// Also set track in player
            //if (_player != null)
            //{
            //    var playerField = typeof(Player).GetField("_track", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //    if (playerField != null)
            //    {
            //        playerField.SetValue(_player, track);
            //        Debug.Log($"[Level] Set track to player and level with {track.GetSegments().Count} segments");
            //    }
            //}
            //else
            //{
            //    Debug.Log($"[Level] Set track to level (player not yet created) with {track.GetSegments().Count} segments");
            //}
        }

        /// <summary>
        /// 获取动态轨道系统
        /// </summary>
        public com.playchilla.runner.track.DynamicTrack GetDynamicTrack()
        {
            return _dynamicTrack;
        }

        private void Update()
        {
            // Update keyboard input from Unity (every frame)
            UpdateKeyboardInput();

            // Update dynamic track system (check if we need to load/unload segments)
            if (_dynamicTrack != null && _player != null)
            {
                Vec3 playerPos = _player.GetPos();
                _dynamicTrack.Update(playerPos);
            }

            // Flash physics system: update at 30fps (33ms per tick) to match Flash frame rate
            // This prevents the game from running 2x faster than intended
            if (_player != null) // 添加 null 检查
            {
                _accumulatedTime += Time.deltaTime * 1000; // Convert to milliseconds

                while (_accumulatedTime >= 33) // 33ms = 30fps
                {
                    _player.Tick(33); // Flash uses ~33ms per frame (30fps)
                    _accumulatedTime -= 33;
                }
            }

            if (_world != null)
            {
                // Update world every frame (for smooth visuals)
                int deltaTime = Mathf.RoundToInt(Time.deltaTime * 1000);
                _world.Tick(deltaTime);
                // Render with interpolation (simplified - no interpolation for now)
                _world.Render(deltaTime, 0f);

                // Update player view with complete arm system
                if (_playerView != null)
                {
                    int currentTime = (int)(Time.time * 1000);
                    _playerView.Render(currentTime, 0.0);
                }
            }
        }

        private float _accumulatedTime = 0f;

#if DEBUG
        // 测试模拟字段和方法
        private bool _simulateSpacePress = false;
        private bool _simulateSpaceRelease = false;

        public void SetSpaceKeySimulation(bool press, bool release)
        {
            _simulateSpacePress = press;
            _simulateSpaceRelease = release;
        }

        public bool GetSpaceKeySimulationPressed()
        {
            return _simulateSpacePress;
        }

        private void ApplyTestInputSimulation()
        {
            if (_keyboard == null)
            {
                return;
            }

            if (_simulateSpacePress)
            {
                _keyboard.SetPress(32);
            }
            if (_simulateSpaceRelease)
            {
                _keyboard.SetRelease(32);
            }
        }

        private void ResetSimulationFlags()
        {
            _simulateSpacePress = false;
            _simulateSpaceRelease = false;
        }
#endif

        private void UpdateKeyboardInput()
        {
            // 检查 _keyboardInput 是否已初始化（单元测试可能没有初始化）
            if (_keyboard == null)
            {
                // 在单元测试模式下，不需要输入同步
                return;
            }

            // Clear previous frame's pressed/released state
            _keyboard.Reset();

#if DEBUG
            // 应用测试模拟输入（测试扩展功能）
            ApplyTestInputSimulation();
            ResetSimulationFlags();
#endif

            // Synchronize Unity Input with Flash KeyboardInput
            // Space key (code 32 in Flash/Air)
            if (Input.GetKey(KeyCode.Space))
            {
                _keyboard.SetPress(32);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                _keyboard.SetRelease(32);
            }
        }

    }
}

public class TrackView : MonoBehaviour
{
    public TrackView Initialize(Track t) 
    {
        return this;
    }
}