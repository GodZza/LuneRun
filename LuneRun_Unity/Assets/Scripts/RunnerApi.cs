using System;
using System.Collections.Generic;

namespace LuneRun
{
    public interface IRunnerApi
    {
        int GetUserId();
        void GetUserData(Action<UserData> callback);
        void GetHighscore(int levelId, Action<List<HighscoreEntry>> callback);
        void SubmitScore(int levelId, float score, Action<bool> callback);
    }
    
    public class LocalRunnerApi : IRunnerApi
    {
        private int userId = 1;
        private string userName = "Local";
        
        public int GetUserId()
        {
            return userId;
        }
        
        public void GetUserData(Action<UserData> callback)
        {
            // Return dummy user data
            UserData userData = new UserData
            {
                userId = userId,
                userName = userName,
                scores = new Dictionary<int, float>()
            };
            
            // Populate with some dummy scores
            for (int i = 1; i <= 32; i++)
            {
                if (UnityEngine.Random.value > 0.7f)
                {
                    userData.scores[i] = UnityEngine.Random.Range(30f, 180f);
                }
            }
            
            callback?.Invoke(userData);
        }
        
        public void GetHighscore(int levelId, Action<List<HighscoreEntry>> callback)
        {
            // Return dummy highscore entries
            List<HighscoreEntry> entries = new List<HighscoreEntry>();
            
            for (int i = 1; i <= 10; i++)
            {
                entries.Add(new HighscoreEntry(
                    i,
                    $"Player{i}",
                    UnityEngine.Random.Range(30f, 120f),
                    DateTime.Now.AddDays(-UnityEngine.Random.Range(0, 30))
                ));
            }
            
            callback?.Invoke(entries);
        }
        
        public void SubmitScore(int levelId, float score, Action<bool> callback)
        {
            // Simulate successful submission
            UnityEngine.Debug.Log($"Submitted score {score} for level {levelId}");
            callback?.Invoke(true);
        }
    }
    
    public class RunnerApi : IRunnerApi
    {
        private string apiUrl;
        private ApiUser user;
        
        public RunnerApi(string apiUrl, ApiUser user)
        {
            this.apiUrl = apiUrl;
            this.user = user;
        }
        
        public int GetUserId()
        {
            return user?.id ?? 0;
        }
        
        public void GetUserData(Action<UserData> callback)
        {
            // TODO: Implement actual API call
            UnityEngine.Debug.LogWarning("Real RunnerApi.GetUserData not implemented");
            callback?.Invoke(new UserData());
        }
        
        public void GetHighscore(int levelId, Action<List<HighscoreEntry>> callback)
        {
            // TODO: Implement actual API call
            UnityEngine.Debug.LogWarning("Real RunnerApi.GetHighscore not implemented");
            callback?.Invoke(new List<HighscoreEntry>());
        }
        
        public void SubmitScore(int levelId, float score, Action<bool> callback)
        {
            // TODO: Implement actual API call
            UnityEngine.Debug.LogWarning("Real RunnerApi.SubmitScore not implemented");
            callback?.Invoke(false);
        }
    }
    
    public class ApiUser
    {
        public int id;
        public string name;
        
        public ApiUser(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
    
    public class UserData
    {
        public int userId;
        public string userName;
        public Dictionary<int, float> scores;
        
        public float GetScore(int levelId)
        {
            return scores != null && scores.ContainsKey(levelId) ? scores[levelId] : 0f;
        }
    }
}