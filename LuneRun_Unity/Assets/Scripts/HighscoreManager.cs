using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace LuneRun
{
    [System.Serializable]
    public class HighscoreEntry
    {
        public int rank;
        public string playerName;
        public float score;
        public DateTime date;
        
        public HighscoreEntry(int rank, string playerName, float score, DateTime date)
        {
            this.rank = rank;
            this.playerName = playerName;
            this.score = score;
            this.date = date;
        }
    }
    
    public class HighscoreManager : MonoBehaviour
    {
        public static HighscoreManager Instance { get; private set; }
        
        [SerializeField] private GameObject highscorePanel;
        [SerializeField] private Text titleText;
        [SerializeField] private Transform entriesContainer;
        [SerializeField] private GameObject entryPrefab;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button actionButton;
        [SerializeField] private Text actionButtonText;
        
        private Action closeCallback;
        private Action actionCallback;
        private int currentLevelId;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (highscorePanel != null)
            {
                highscorePanel.SetActive(false);
            }
            
            // Setup button listeners
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
            
            if (actionButton != null)
            {
                actionButton.onClick.AddListener(OnActionButtonClicked);
            }
        }
        
        // Show highscore dialog for a specific level
        public void ShowLevelHighscore(int levelId, int userId, IRunnerApi runnerApi, 
                                      Action onClose, string actionButtonLabel, Action onAction, bool isLastUnlocked)
        {
            currentLevelId = levelId;
            closeCallback = onClose;
            actionCallback = onAction;
            
            if (titleText != null)
            {
                titleText.text = $"LEVEL {levelId}";
            }
            
            if (actionButtonText != null)
            {
                actionButtonText.text = actionButtonLabel;
            }
            
            // Clear existing entries
            ClearEntries();
            
            // Fetch highscore data from API
            runnerApi.GetHighscore(levelId, OnHighscoreReceived);
            
            // Show panel
            if (highscorePanel != null)
            {
                highscorePanel.SetActive(true);
            }
            
            // Position panel (center of screen)
            RepositionPanel();
        }
        
        private void OnHighscoreReceived(List<HighscoreEntry> entries)
        {
            if (entries == null) return;
            
            // Populate entries
            for (int i = 0; i < entries.Count; i++)
            {
                if (i >= 10) break; // Show top 10 only
                
                HighscoreEntry entry = entries[i];
                CreateEntryUI(entry, i + 1);
            }
            
            // If no entries, show "No scores yet" message
            if (entries.Count == 0)
            {
                CreateEmptyEntryUI();
            }
        }
        
        private void CreateEntryUI(HighscoreEntry entry, int displayRank)
        {
            if (entryPrefab == null || entriesContainer == null) return;
            
            GameObject entryObj = Instantiate(entryPrefab, entriesContainer);
            Text[] texts = entryObj.GetComponentsInChildren<Text>();
            
            if (texts.Length >= 3)
            {
                texts[0].text = displayRank.ToString();
                texts[1].text = entry.playerName;
                texts[2].text = FormatScore(entry.score);
            }
            
            // Highlight player's own entry
            if (entry.playerName == "Local" || entry.playerName == PlayerPrefs.GetString("PlayerName", ""))
            {
                Image bg = entryObj.GetComponent<Image>();
                if (bg != null)
                {
                    bg.color = new Color(0.2f, 0.4f, 0.8f, 0.3f);
                }
            }
        }
        
        private void CreateEmptyEntryUI()
        {
            if (entryPrefab == null || entriesContainer == null) return;
            
            GameObject entryObj = Instantiate(entryPrefab, entriesContainer);
            Text[] texts = entryObj.GetComponentsInChildren<Text>();
            
            if (texts.Length >= 3)
            {
                texts[0].text = "";
                texts[1].text = "No scores yet";
                texts[2].text = "";
            }
        }
        
        private void ClearEntries()
        {
            if (entriesContainer == null) return;
            
            foreach (Transform child in entriesContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        private string FormatScore(float score)
        {
            // Format as time (original game uses seconds)
            int minutes = Mathf.FloorToInt(score / 60);
            float seconds = score % 60;
            return $"{minutes:00}:{seconds:00.00}";
        }
        
        private void RepositionPanel()
        {
            if (highscorePanel == null) return;
            
            RectTransform rect = highscorePanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
            }
        }
        
        private void OnCloseButtonClicked()
        {
            HideHighscore();
            closeCallback?.Invoke();
        }
        
        private void OnActionButtonClicked()
        {
            actionCallback?.Invoke();
        }
        
        public void HideHighscore()
        {
            if (highscorePanel != null)
            {
                highscorePanel.SetActive(false);
            }
        }
        
        // Submit a new score
        public void SubmitScore(int levelId, float score, IRunnerApi runnerApi, Action<bool> callback)
        {
            runnerApi.SubmitScore(levelId, score, callback);
        }
        
        // Get local highscore for a level
        public float GetLocalHighscore(int levelId)
        {
            return PlayerPrefs.GetFloat($"Highscore_{levelId}", 0f);
        }
        
        // Set local highscore
        public void SetLocalHighscore(int levelId, float score)
        {
            PlayerPrefs.SetFloat($"Highscore_{levelId}", score);
            PlayerPrefs.Save();
        }
    }
    
    // Extended IRunnerApi interface for highscore operations
    public interface IRunnerApiExtended : IRunnerApi
    {
        void GetHighscore(int levelId, Action<List<HighscoreEntry>> callback);
        void SubmitScore(int levelId, float score, Action<bool> callback);
    }
}