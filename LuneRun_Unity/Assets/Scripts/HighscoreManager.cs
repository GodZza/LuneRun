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
            
            // Ensure UI elements exist
            EnsureUI();
            
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
        
        private void EnsureUI()
        {
            // Find or create canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            
            // Find or create highscore panel
            if (highscorePanel == null)
            {
                highscorePanel = GameObject.Find("HighscorePanel");
                if (highscorePanel == null)
                {
                    highscorePanel = CreateHighscorePanel(canvas.transform);
                }
            }
            
            // Find sub-elements
            if (titleText == null && highscorePanel != null)
            {
                titleText = highscorePanel.GetComponentInChildren<Text>();
            }
            
            if (entriesContainer == null && highscorePanel != null)
            {
                entriesContainer = highscorePanel.transform.Find("EntriesContainer") as RectTransform;
            }
            
            if (entryPrefab == null)
            {
                // We'll create a simple entry prefab if needed
                entryPrefab = CreateEntryPrefab();
            }
            
            if (closeButton == null && highscorePanel != null)
            {
                closeButton = highscorePanel.GetComponentInChildren<Button>();
            }
            
            if (actionButton == null && highscorePanel != null)
            {
                // Look for second button
                Button[] buttons = highscorePanel.GetComponentsInChildren<Button>();
                if (buttons.Length > 1)
                {
                    actionButton = buttons[1];
                }
            }
            
            if (actionButtonText == null && actionButton != null)
            {
                actionButtonText = actionButton.GetComponentInChildren<Text>();
            }
        }
        
        private GameObject CreateHighscorePanel(Transform parent)
        {
            GameObject panel = new GameObject("HighscorePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            
            Image img = panel.GetComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.2f, 0.95f);
            
            // Create title
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            titleObj.transform.SetParent(panel.transform, false);
            Text titleTextComp = titleObj.GetComponent<Text>();
            titleTextComp.text = "HIGHSCORES";
            titleTextComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleTextComp.fontSize = 48;
            titleTextComp.color = Color.yellow;
            titleTextComp.alignment = TextAnchor.UpperCenter;
            
            RectTransform titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0.5f, 0.9f);
            titleRt.anchorMax = new Vector2(0.5f, 1f);
            titleRt.pivot = new Vector2(0.5f, 0.5f);
            titleRt.sizeDelta = new Vector2(600, 80);
            titleRt.anchoredPosition = Vector2.zero;
            
            // Create entries container
            GameObject containerObj = new GameObject("EntriesContainer", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            containerObj.transform.SetParent(panel.transform, false);
            RectTransform containerRt = containerObj.GetComponent<RectTransform>();
            containerRt.anchorMin = new Vector2(0.1f, 0.1f);
            containerRt.anchorMax = new Vector2(0.9f, 0.8f);
            containerRt.offsetMin = Vector2.zero;
            containerRt.offsetMax = Vector2.zero;
            
            VerticalLayoutGroup layout = containerObj.GetComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 5f;
            layout.padding = new RectOffset(10, 10, 10, 10);
            
            ContentSizeFitter fitter = containerObj.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Create close button
            GameObject closeBtnObj = new GameObject("CloseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            closeBtnObj.transform.SetParent(panel.transform, false);
            RectTransform closeRt = closeBtnObj.GetComponent<RectTransform>();
            closeRt.anchorMin = new Vector2(0.8f, 0.05f);
            closeRt.anchorMax = new Vector2(0.95f, 0.15f);
            closeRt.offsetMin = Vector2.zero;
            closeRt.offsetMax = Vector2.zero;
            
            Button closeBtn = closeBtnObj.GetComponent<Button>();
            closeBtn.colors = ColorBlock.defaultColorBlock;
            
            GameObject closeTextObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            closeTextObj.transform.SetParent(closeBtnObj.transform, false);
            Text closeText = closeTextObj.GetComponent<Text>();
            closeText.text = "CLOSE";
            closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            closeText.fontSize = 24;
            closeText.color = Color.white;
            closeText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform closeTextRt = closeTextObj.GetComponent<RectTransform>();
            closeTextRt.anchorMin = Vector2.zero;
            closeTextRt.anchorMax = Vector2.one;
            closeTextRt.offsetMin = Vector2.zero;
            closeTextRt.offsetMax = Vector2.zero;
            
            return panel;
        }
        
        private GameObject CreateEntryPrefab()
        {
            GameObject prefab = new GameObject("EntryPrefab", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rt = prefab.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(600, 40);
            
            Image img = prefab.GetComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.4f, 0.7f);
            
            // Create rank text
            GameObject rankObj = new GameObject("Rank", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            rankObj.transform.SetParent(prefab.transform, false);
            RectTransform rankRt = rankObj.GetComponent<RectTransform>();
            rankRt.anchorMin = new Vector2(0f, 0f);
            rankRt.anchorMax = new Vector2(0.15f, 1f);
            rankRt.offsetMin = Vector2.zero;
            rankRt.offsetMax = Vector2.zero;
            
            Text rankText = rankObj.GetComponent<Text>();
            rankText.text = "1";
            rankText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            rankText.fontSize = 20;
            rankText.color = Color.white;
            rankText.alignment = TextAnchor.MiddleCenter;
            
            // Create name text
            GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            nameObj.transform.SetParent(prefab.transform, false);
            RectTransform nameRt = nameObj.GetComponent<RectTransform>();
            nameRt.anchorMin = new Vector2(0.15f, 0f);
            nameRt.anchorMax = new Vector2(0.6f, 1f);
            nameRt.offsetMin = Vector2.zero;
            nameRt.offsetMax = Vector2.zero;
            
            Text nameText = nameObj.GetComponent<Text>();
            nameText.text = "Player";
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.fontSize = 20;
            nameText.color = Color.white;
            nameText.alignment = TextAnchor.MiddleLeft;
            
            // Create score text
            GameObject scoreObj = new GameObject("Score", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            scoreObj.transform.SetParent(prefab.transform, false);
            RectTransform scoreRt = scoreObj.GetComponent<RectTransform>();
            scoreRt.anchorMin = new Vector2(0.6f, 0f);
            scoreRt.anchorMax = new Vector2(1f, 1f);
            scoreRt.offsetMin = Vector2.zero;
            scoreRt.offsetMax = Vector2.zero;
            
            Text scoreText = scoreObj.GetComponent<Text>();
            scoreText.text = "00:00.00";
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            scoreText.fontSize = 20;
            scoreText.color = Color.white;
            scoreText.alignment = TextAnchor.MiddleRight;
            
            return prefab;
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