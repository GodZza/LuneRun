using UnityEngine;
using com.playchilla.runner.api;
using com.playchilla.runner.player;
using com.playchilla.gameapi.api;
using shared.input;

namespace com.playchilla.runner
{
    public class Level : MonoBehaviour, IScoreCallback
    {
        private int _levelId;
        private bool _isHardware;
        private Player _player;
        private Materials _materials;
        private object _gameCont; // Placeholder for game container
        private object _world; // Placeholder for World instance
        private PlayerView _playerView;
        
        public void Initialize(int levelId, bool isHardware, Settings settings, IRunnerApi runnerApi)
        {
            _levelId = levelId;
            _isHardware = isHardware;
            // Initialize other components as needed
        }
        
        public Player GetPlayer()
        {
            return _player;
        }
        
        public Materials GetMaterials()
        {
            return _materials;
        }
        
        public object GetGameCont()
        {
            return _gameCont;
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

    }
}