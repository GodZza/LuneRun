using com.playchilla.gameapi.api;
using shared.debug;

namespace com.playchilla.runner.api
{
    public class LocalRunnerApi : IRunnerApi
    {
        private ApiUser _apiUser;
        
        public LocalRunnerApi(ApiUser apiUser)
        {
            Debug.Assert(apiUser != null, "Trying to create local runner api without user.");
            _apiUser = apiUser;
        }
        
        public int GetUserId()
        {
            return (int)_apiUser.GetId();
        }
        
        public void GetUserData(IGetUserDataCallback callback)
        {
            // Stub implementation - simulate async load
            UnityEngine.Debug.Log("[LocalRunnerApi] Loading user data");
            var userData = Load();
            callback.GetUserData(userData);
        }
        
        public void Score(int levelId, double scoreValue, IScoreCallback callback)
        {
            // Stub implementation
            UnityEngine.Debug.Log($"[LocalRunnerApi] Recording score for level {levelId}: {scoreValue}");
            var score = new Score(GetUserId(), _apiUser.GetName(), scoreValue);
            callback.Score(score, true);
        }
        
        public void GetHighscore(int levelId, IGetHighscoreCallback callback)
        {
            // Stub implementation
            UnityEngine.Debug.Log($"[LocalRunnerApi] Getting highscore for level {levelId}");
            var scores = new System.Collections.Generic.List<Score>();
            var highscore = new Highscore(levelId, scores);
            callback.GetHighscore(highscore);
        }
        
        public void GetSurvivors(int levelId, IGetSurvivorsCallback callback)
        {
            // Stub implementation
            UnityEngine.Debug.Log($"[LocalRunnerApi] Getting survivors for level {levelId}");
            var scores = new System.Collections.Generic.List<Score>();
            var highscore = new Highscore(levelId, scores);
            callback.GetSurvivors(highscore);
        }
        
        private UserData Load()
        {
            // Simulate loading from local storage
            return new UserData(GetUserId(), new System.Collections.Generic.List<Score>());
        }
        
        private void Save(UserData userData)
        {
            // Simulate saving to local storage
            UnityEngine.Debug.Log("[LocalRunnerApi] Saved user data");
        }
    }
}