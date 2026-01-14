using UnityEngine;

namespace com.playchilla.runner.highscore
{
    public class LevelHighscoreGui : HighscoreGui, com.playchilla.runner.api.IGetHighscoreCallback
    {
        private int _levelId;
        private com.playchilla.runner.api.IRunnerApi _runnerApi;
        private bool _isLastUnlocked;
        private GameObject _nextButton;
        private GameObject _prevButton;

        // TODO: Implement UI

        public void Initialize(int myUserId, int levelId, com.playchilla.runner.api.IRunnerApi runnerApi, 
                               System.Action onClose, string playCaption, System.Action onPlay, bool isLastUnlocked)
        {
            _levelId = levelId;
            _runnerApi = runnerApi;
            _isLastUnlocked = isLastUnlocked;
            base.Initialize(myUserId, onClose, playCaption, onPlay);
            // Create next/prev buttons
        }

        public void Load(int levelId)
        {
            _runnerApi.GetHighscore(levelId, this);
            // Update button visibility
        }

        public void GetHighscore(com.playchilla.runner.api.Highscore highscore)
        {
            Setup("Level " + highscore.GetLevelId(), highscore);
            // Update button visibility
        }

        public void GetHighscoreError(com.playchilla.gameapi.api.ErrorResponse error)
        {
            SetError(error.GetMessage());
        }

        private void OnNextLevel()
        {
            if (_levelId == 32) return;
            _levelId++;
            Load(_levelId);
        }

        private void OnPrevLevel()
        {
            if (_levelId == 1) return;
            _levelId--;
            Load(_levelId);
        }
    }
}