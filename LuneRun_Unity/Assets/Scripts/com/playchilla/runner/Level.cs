using UnityEngine;
using com.playchilla.runner.api;
using com.playchilla.runner.player;
using com.playchilla.runner.track;
using com.playchilla.gameapi.api;
using shared.input;
using com.playchilla.runner.track.entity;
using shared.math;

namespace com.playchilla.runner
{
    public class Level : MonoBehaviour, IScoreCallback
    {
        private int _levelId;
        private bool _isHardware;
        private Player _player;
        private Materials _materials;
        private GameObject _gameCont; // Placeholder for game container
        private World _world; // World instance
        private PlayerView _playerView;
        private Track _track;
        private KeyboardInput _keyboardInput; // Reference for input synchronization
        
        public void Initialize(int levelId, bool isHardware, Settings settings, IRunnerApi runnerApi)
        {
            _levelId = levelId;
            _isHardware = isHardware;
            
            // Create game container if not exists
            if (_gameCont == null)
            {
                _gameCont = new GameObject("GameContainer");
                _gameCont.transform.SetParent(transform);
            }
            
            // Create materials instance
            _materials = new Materials();

            // Create track instance BEFORE player (player needs it)
            _track = new Track();
            
            // Create world instance (must be created before player initialization)
            _world = new World(this, _gameCont);

            // Create input instances
            KeyboardInput keyboard = new KeyboardInput();
            MouseInput mouse = new MouseInput();

            // Save keyboard reference for input synchronization
            _keyboardInput = keyboard;

            // Create player instance (now track is available)
            GameObject playerObj = new GameObject("Player");
            playerObj.transform.SetParent(_gameCont.transform);
            _player = playerObj.AddComponent<Player>();
            Vec3 startPos = new Vec3(0, 1, 0); // Start 1 unit above track (was 2, too high)
            _player.Initialize(this, keyboard, mouse, startPos);
            Debug.Log($"[Level] Player initialized at position: ({startPos.x}, {startPos.y}, {startPos.z})");

            // Get main camera
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                // Create a camera if none exists
                GameObject cameraObj = new GameObject("MainCamera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
            }

            // Create player view instance with complete arm system
            GameObject playerViewObj = new GameObject("PlayerView");
            playerViewObj.transform.SetParent(_gameCont.transform);
            _playerView = playerViewObj.AddComponent<PlayerView>();
            Debug.Log($"[Level] Created PlayerView, initializing with player={_player != null}, camera={mainCamera != null}, materials={_materials != null}");
            _playerView.Initialize(this, _player, mainCamera, _materials, keyboard);
            
            // Add some test entities (speed entities) for demonstration
            // This should be replaced with proper entity generation based on level design
            // Disabled for now - causing visual clutter
            // AddTestEntities();
            
            Debug.Log($"[Level] Initialized level {levelId} with world, player, and complete arm system");
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
            return _track;
        }

        public Track getTrack()
        {
            return _track;
        }

        // Set track externally (for when track is created after Level)
        public void SetTrack(Track track)
        {
            _track = track;
            Debug.Log($"[Level] Track set with {track.GetSegments().Count} segments");
        }

        private void Update()
        {
            // Convert delta time to milliseconds (original game uses integer ticks)
            int deltaTime = Mathf.RoundToInt(Time.deltaTime * 1000);

            // Update keyboard input from Unity
            UpdateKeyboardInput();

            if (_player != null)
            {
                _player.Tick(deltaTime);
            }

            if (_world != null)
            {
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

        private void UpdateKeyboardInput()
        {
            // Clear previous frame's pressed/released state
            _keyboardInput.Reset();

            // Synchronize Unity Input with Flash KeyboardInput
            // Space key (code 32 in Flash/Air)
            if (Input.GetKey(KeyCode.Space))
            {
                _keyboardInput.SetPress(32);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                _keyboardInput.SetRelease(32);
            }
        }

    }
}