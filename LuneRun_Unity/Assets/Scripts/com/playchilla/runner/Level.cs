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
            
            // Create world instance
            _world = new World(this, _gameCont);
            
            // Add some test entities (speed entities) for demonstration
            // This should be replaced with proper entity generation based on level design
            AddTestEntities();
            
            Debug.Log($"[Level] Initialized level {levelId} with world");
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

        private void Update()
        {
            if (_world != null)
            {
                // Convert delta time to milliseconds (original game uses integer ticks)
                int deltaTime = Mathf.RoundToInt(Time.deltaTime * 1000);
                _world.Tick(deltaTime);
                // Render with interpolation (simplified - no interpolation for now)
                _world.Render(deltaTime, 0f);
            }
        }

    }
}