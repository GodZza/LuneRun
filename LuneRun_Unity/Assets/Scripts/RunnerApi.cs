using System;
using System.Collections.Generic;
using UnityEngine;

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
        private bool useMock = true; // Set to false to attempt real API calls
        
        public RunnerApi(string apiUrl, ApiUser user)
        {
            this.apiUrl = apiUrl;
            this.user = user;
            
            // If no real user provided, default to mock mode
            if (user == null)
            {
                useMock = true;
                this.user = new ApiUser(1, "MockUser");
            }
        }
        
        public int GetUserId()
        {
            return user?.id ?? 0;
        }
        
        public void GetUserData(Action<UserData> callback)
        {
            if (useMock)
            {
                // Simulate API delay and return mock data
                UnityEngine.MonoBehaviour mono = FindMonoBehaviour();
                if (mono != null)
                {
                    mono.StartCoroutine(MockGetUserData(callback));
                }
                else
                {
                    // Fallback immediate callback
                    callback?.Invoke(CreateMockUserData());
                }
            }
            else
            {
                // Mock implementation - replace with actual UnityWebRequest call for production
                UnityEngine.Debug.LogWarning("Real RunnerApi.GetUserData not implemented - falling back to mock");
                GetUserData(callback); // Recursively call with mock mode
            }
        }
        
        public void GetHighscore(int levelId, Action<List<HighscoreEntry>> callback)
        {
            if (useMock)
            {
                // Simulate API delay and return mock highscore data
                UnityEngine.MonoBehaviour mono = FindMonoBehaviour();
                if (mono != null)
                {
                    mono.StartCoroutine(MockGetHighscore(levelId, callback));
                }
                else
                {
                    callback?.Invoke(CreateMockHighscoreEntries(levelId));
                }
            }
            else
            {
                // Mock implementation - replace with actual UnityWebRequest call for production
                UnityEngine.Debug.LogWarning("Real RunnerApi.GetHighscore not implemented - falling back to mock");
                GetHighscore(levelId, callback); // Recursively call with mock mode
            }
        }
        
        public void SubmitScore(int levelId, float score, Action<bool> callback)
        {
            if (useMock)
            {
                // Simulate API delay and return mock success/failure
                UnityEngine.MonoBehaviour mono = FindMonoBehaviour();
                if (mono != null)
                {
                    mono.StartCoroutine(MockSubmitScore(levelId, score, callback));
                }
                else
                {
                    callback?.Invoke(true); // Assume success
                }
            }
            else
            {
                // Mock implementation - replace with actual UnityWebRequest call for production
                UnityEngine.Debug.LogWarning("Real RunnerApi.SubmitScore not implemented - falling back to mock");
                SubmitScore(levelId, score, callback); // Recursively call with mock mode
            }
        }
        
        // Helper to find a MonoBehaviour to run coroutines
        private UnityEngine.MonoBehaviour FindMonoBehaviour()
        {
            // Try to find GameManager or create a temporary GameObject
            GameManager gm = GameManager.Instance;
            if (gm != null) return gm;
            
            // Try AudioManager
            AudioManager am = AudioManager.Instance;
            if (am != null) return am;
            
            // Create a temporary GameObject if none found
            GameObject temp = new GameObject("TempCoroutineRunner");
            UnityEngine.MonoBehaviour tempMono = temp.AddComponent<EmptyMonoBehaviour>();
            return tempMono;
        }
        
        // Mock coroutines with simulated delay
        private System.Collections.IEnumerator MockGetUserData(Action<UserData> callback)
        {
            yield return new UnityEngine.WaitForSeconds(0.5f); // Simulate network delay
            callback?.Invoke(CreateMockUserData());
        }
        
        private System.Collections.IEnumerator MockGetHighscore(int levelId, Action<List<HighscoreEntry>> callback)
        {
            yield return new UnityEngine.WaitForSeconds(0.5f); // Simulate network delay
            callback?.Invoke(CreateMockHighscoreEntries(levelId));
        }
        
        private System.Collections.IEnumerator MockSubmitScore(int levelId, float score, Action<bool> callback)
        {
            yield return new UnityEngine.WaitForSeconds(0.8f); // Simulate slower submission
            bool success = UnityEngine.Random.value > 0.2f; // 80% success rate
            callback?.Invoke(success);
        }
        
        // Mock data generators
        private UserData CreateMockUserData()
        {
            UserData userData = new UserData
            {
                userId = user?.id ?? 1,
                userName = user?.name ?? "MockUser",
                scores = new Dictionary<int, float>()
            };
            
            // Populate with some mock scores (unlocked levels)
            for (int i = 1; i <= 32; i++)
            {
                if (UnityEngine.Random.value > 0.5f)
                {
                    userData.scores[i] = UnityEngine.Random.Range(30f, 180f);
                }
            }
            
            return userData;
        }
        
        private List<HighscoreEntry> CreateMockHighscoreEntries(int levelId)
        {
            List<HighscoreEntry> entries = new List<HighscoreEntry>();
            
            for (int i = 1; i <= 10; i++)
            {
                entries.Add(new HighscoreEntry(
                    i,
                    $"Player{i}",
                    UnityEngine.Random.Range(30f, 120f),
                    System.DateTime.Now.AddDays(-UnityEngine.Random.Range(0, 30))
                ));
            }
            
            return entries;
        }
    }
    
    // Empty MonoBehaviour for running coroutines
    public class EmptyMonoBehaviour : UnityEngine.MonoBehaviour { }
    
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