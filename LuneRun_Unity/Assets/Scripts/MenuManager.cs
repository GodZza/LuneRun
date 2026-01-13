using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace LuneRun
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] public GameObject mainMenuPanel;
        [SerializeField] public GameObject helpPanel;
        [SerializeField] public Button soundButton;
        [SerializeField] public Button musicButton;
        [SerializeField] public Button helpButton;
        [SerializeField] public Button fullscreenButton;
        [SerializeField] public Button tweetButton;
        [SerializeField] public Button linkButton;
        [SerializeField] public Button survivorsButton;
        [SerializeField] public Button paypalButton;
        [SerializeField] public Text versionText;
        [SerializeField] public Text logoText;
        
        // Tab buttons
        [SerializeField] private Button tab1Button;
        [SerializeField] private Button tab2Button;
        [SerializeField] private Button tab3Button;
        [SerializeField] private GameObject levelPanel;
        
        // Level buttons (1-32 for tab1, 33-60 for tab2, reused)
        public List<Button> levelButtons = new();
        private int selectedLevel = -1;
        private bool isLastUnlocked = false;
        private int currentTab = 1; // 1, 2, or 3
        
        // References
        private Settings settings;
        private IRunnerApi runnerApi;
        private UserData currentUserData;
        
        public void Initialize(Settings settings, IRunnerApi runnerApi)
        {
            Debug.Log("[LuneRun] MenuManager.Initialize - Starting menu initialization");
            this.settings = settings;
            this.runnerApi = runnerApi;
            
            // Ensure EventSystem exists for UI input
            EnsureEventSystem();
            
            // Check UI references and log warnings if missing
            CheckUIReferences();
            
            // Setup UI
            if (logoText != null) logoText.text = Constants.Name;
            if (versionText != null) versionText.text = Constants.Version;
            
            Debug.Log($"[LuneRun] MenuManager.Initialize - UI refs: logoText={logoText != null}, versionText={versionText != null}, levelButtons count={levelButtons?.Count ?? 0}");
            
            // Load user data asynchronously
            runnerApi.GetUserData(OnUserDataReceived);
            
            // Update button states based on settings
            UpdateSettings();
            
            // Ensure level buttons exist and have listeners
            EnsureLevelButtons();
            
            // Setup tab buttons
            SetupTabButtons();
            
            Debug.Log("[LuneRun] MenuManager.Initialize - Menu initialization completed");
        }
        
        private void UpdateSettings()
        {
            // Update sound/music button icons based on settings
            if (soundButton != null && settings != null)
            {
                // For now, update button text
                Text btnText = soundButton.GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.text = "Sound: " + (settings.GetSoundOn() ? "On" : "Off");
                }
            }
            if (musicButton != null && settings != null)
            {
                Text btnText = musicButton.GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.text = "Music: " + (settings.GetMusicOn() ? "On" : "Off");
                }
            }
        }
        
        private void CheckUIReferences()
        {
            Debug.Log("[LuneRun] MenuManager.CheckUIReferences - Checking UI references");
            
            List<string> missingRefs = new List<string>();
            
            if (mainMenuPanel == null) missingRefs.Add("mainMenuPanel");
            if (helpPanel == null) missingRefs.Add("helpPanel");
            if (soundButton == null) missingRefs.Add("soundButton");
            if (musicButton == null) missingRefs.Add("musicButton");
            if (helpButton == null) missingRefs.Add("helpButton");
            if (fullscreenButton == null) missingRefs.Add("fullscreenButton");
            if (tweetButton == null) missingRefs.Add("tweetButton");
            if (linkButton == null) missingRefs.Add("linkButton");
            if (survivorsButton == null) missingRefs.Add("survivorsButton");
            if (paypalButton == null) missingRefs.Add("paypalButton");
            if (versionText == null) missingRefs.Add("versionText");
            if (logoText == null) missingRefs.Add("logoText");
            
            // Tab buttons
            if (tab1Button == null) missingRefs.Add("tab1Button");
            if (tab2Button == null) missingRefs.Add("tab2Button");
            if (tab3Button == null) missingRefs.Add("tab3Button");
            if (levelPanel == null) missingRefs.Add("levelPanel");
            
            int levelButtonCount = levelButtons?.Count(btn => btn != null) ?? 0;
            if (levelButtonCount < Constants.LevelsPerTab)
            {
                missingRefs.Add($"levelButtons ({levelButtonCount}/{Constants.LevelsPerTab})");
            }
            
            if (missingRefs.Count > 0)
            {
                Debug.LogWarning($"[LuneRun] MenuManager.CheckUIReferences - Missing {missingRefs.Count} UI references: {string.Join(", ", missingRefs)}");
                Debug.LogWarning("[LuneRun] The game menu UI has not been properly set up.");
                Debug.LogWarning("[LuneRun] To fix this, please do one of the following:");
                Debug.LogWarning("[LuneRun] 1. In Unity Editor, go to 'Tools/LuneRun/Setup Scene UI' to create the menu UI");
                Debug.LogWarning("[LuneRun] 2. Or run the game with Constants.SkipToGame = true to skip the menu");
                Debug.LogWarning("[LuneRun] 3. Or manually create UI elements and assign them to MenuManager in the inspector");
            }
            else
            {
                Debug.Log("[LuneRun] MenuManager.CheckUIReferences - All UI references are valid");
            }
        }
        
        private void EnsureLevelButtons()
        {
            Debug.Log("[LuneRun] MenuManager.EnsureLevelButtons - Starting");
            
            // Ensure we have at least 32 level buttons for tab1
            if (levelButtons == null)
            {
                levelButtons = new List<Button>();
                Debug.Log("[LuneRun] MenuManager.EnsureLevelButtons - Created new levelButtons list");
            }
            
            // If the list is empty, try to find buttons in the scene
            if (levelButtons.Count == 0 || levelButtons[0] == null)
            {
                Debug.Log("[LuneRun] MenuManager.EnsureLevelButtons - Level buttons list is empty or null, searching in scene...");
                FindLevelButtonsInScene();
            }
            
            // Log the current state
            int validButtonCount = 0;
            for (int i = 0; i < levelButtons.Count; i++)
            {
                if (levelButtons[i] != null) validButtonCount++;
            }
            Debug.Log($"[LuneRun] MenuManager.EnsureLevelButtons - Found {validButtonCount} valid buttons out of {levelButtons.Count} total");
            
            // Ensure we have enough buttons for tab1 (32)
            if (validButtonCount < Constants.LevelsPerTab)
            {
                Debug.LogWarning($"[LuneRun] MenuManager.EnsureLevelButtons - Only {validButtonCount} level buttons found. Need at least {Constants.LevelsPerTab} for tab1.");
                Debug.LogWarning("[LuneRun] Please run 'Tools/LuneRun/Setup Scene UI' in the Unity editor to create the menu UI.");
                // Attempt to create missing buttons dynamically
                CreateMissingLevelButtons();
            }
            
            // Initialize tab1 as default
            currentTab = 1;
            ReloadLevelButtonsForCurrentTab();
            
            Debug.Log("[LuneRun] MenuManager.EnsureLevelButtons - Completed");
        }
        
        private void FindLevelButtonsInScene()
        {
            // Clear the list first
            levelButtons.Clear();
            
            // Try to find buttons by name pattern "LevelXButton"
            for (int i = 1; i <= Constants.LevelsPerTab; i++)
            {
                string buttonName = "Level" + i + "Button";
                GameObject buttonObj = GameObject.Find(buttonName);
                if (buttonObj != null)
                {
                    Button btn = buttonObj.GetComponent<Button>();
                    if (btn != null)
                    {
                        levelButtons.Add(btn);
                        Debug.Log($"[LuneRun] MenuManager.FindLevelButtonsInScene - Found button for Level {i}: {buttonName}");
                    }
                    else
                    {
                        Debug.LogWarning($"[LuneRun] MenuManager.FindLevelButtonsInScene - GameObject {buttonName} found but no Button component");
                    }
                }
                else
                {
                    // Try alternative search: look for buttons under LevelPanel
                    GameObject levelPanel = GameObject.Find("LevelPanel");
                    if (levelPanel != null)
                    {
                        Button[] allButtons = levelPanel.GetComponentsInChildren<Button>(true);
                        if (allButtons.Length > 0)
                        {
                            // Add all buttons found in LevelPanel
                            levelButtons.AddRange(allButtons);
                            Debug.Log($"[LuneRun] MenuManager.FindLevelButtonsInScene - Found {allButtons.Length} buttons in LevelPanel");
                            break; // Stop searching individually
                        }
                    }
                }
            }
            
            // If still empty, try to find any buttons with "Level" in name
            if (levelButtons.Count == 0)
            {
                Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
                foreach (Button btn in allButtons)
                {
                    if (btn.name.Contains("Level") || (btn.gameObject.name.Contains("Level")))
                    {
                        levelButtons.Add(btn);
                        Debug.Log($"[LuneRun] MenuManager.FindLevelButtonsInScene - Found button by name pattern: {btn.name}");
                    }
                }
            }
        }
        
        private void OnUserDataReceived(UserData userData)
        {
            currentUserData = userData;
            
            // Update level buttons based on unlocked status
            // Determine if last level (60) is unlocked
            isLastUnlocked = userData.GetScore(Constants.TotalLevels) != 0f;
            
            // Update button states for current tab
            ReloadLevelButtonsForCurrentTab();
        }
        
        private bool IsLevelUnlocked(int level)
        {
            if (currentUserData == null) return false;
            return currentUserData.GetScore(level) != 0f;
        }
        
        public void UpdateMenu()
        {
            // Handle input, animations, etc.
            // For now, simple keyboard navigation
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Simulate selecting level 1 for testing
                selectedLevel = 1;
            }
        }
        
        public int GetSelectedLevel()
        {
            return selectedLevel;
        }
        
        public static void DestroyMenu(MenuManager menu)
        {
            // Clean up
            if (menu != null)
            {
                // Destroy UI panels
                if (menu.mainMenuPanel != null)
                {
                    Destroy(menu.mainMenuPanel);
                }
                if (menu.helpPanel != null)
                {
                    Destroy(menu.helpPanel);
                }
                
                // Destroy the menu manager GameObject
                if (menu.gameObject != null)
                {
                    Destroy(menu.gameObject);
                }
            }
        }
        
        // UI event handlers
        public void OnSoundButtonClicked()
        {
            settings.ToggleSound();
            UpdateSettings();
        }
        
        public void OnMusicButtonClicked()
        {
            settings.ToggleMusic();
            UpdateSettings();
        }
        
        public void OnHelpButtonClicked()
        {
            ToggleHelpPanel();
        }
        
        private void ToggleHelpPanel()
        {
            if (helpPanel != null)
            {
                helpPanel.SetActive(!helpPanel.activeSelf);
                Debug.Log($"Help panel {(helpPanel.activeSelf ? "shown" : "hidden")}");
            }
            else
            {
                Debug.LogWarning("Help panel reference is null");
            }
        }
        
        public static void OnFullscreenButtonClicked()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
        
        public static void OnTweetButtonClicked()
        {
            // Open Twitter with pre-filled tweet
            string tweetText = Constants.Name + " is a fun fast paced one-button browser game, http://www.playchilla.com/runner";
            Application.OpenURL("http://twitter.com/home?status=" + UnityEngine.Networking.UnityWebRequest.EscapeURL(tweetText));
        }
        
        public static void OnLinkButtonClicked()
        {
            Application.OpenURL("http://www.playchilla.com/runner");
        }
        
        public void OnSurvivorsButtonClicked()
        {
            // Show survivors/highscores
            Debug.Log("Survivors button clicked - showing highscore panel");
            
            if (runnerApi != null && HighscoreManager.Instance != null)
            {
                int userId = runnerApi.GetUserId();
                bool lastUnlocked = isLastUnlocked;
                
                // Show highscore panel for level 1
                HighscoreManager.Instance.ShowLevelHighscore(
                    levelId: 1,
                    userId: userId,
                    runnerApi: runnerApi,
                    onClose: () =>
                    {
                        Debug.Log("Highscore panel closed");
                    },
                    actionButtonLabel: "Play",
                    onAction: () =>
                    {
                        // Start level 1
                        OnLevelButtonClicked(1);
                    },
                    isLastUnlocked: lastUnlocked
                );
            }
            else
            {
                Debug.LogWarning("Cannot show highscore panel: runnerApi or HighscoreManager.Instance is null");
            }
        }
        
        public static void OnPayPalButtonClicked()
        {
            Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=KM7VFNVAG8CF4&lc=SE&item_name=Lunerun&item_number=10&no_note=0&cn=L%c3%a4gga%20till%20specialinstruktioner%20till%20s%c3%a4ljaren%3a&no_shipping=1&currency_code=SEK&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
        }
        
        public void OnLevelButtonClicked(int level)
        {
            // Check if level is unlocked
            bool unlocked = IsLevelUnlocked(level) || (runnerApi is LocalRunnerApi);
            
            if (unlocked && HighscoreManager.Instance != null)
            {
                // Show highscore dialog with option to play
                HighscoreManager.Instance.ShowLevelHighscore(
                    levelId: level,
                    userId: runnerApi.GetUserId(),
                    runnerApi: runnerApi,
                    onClose: () =>
                    {
                        Debug.Log("Highscore dialog closed");
                    },
                    actionButtonLabel: "Play",
                    onAction: () =>
                    {
                        selectedLevel = level;
                    },
                    isLastUnlocked: (level == Constants.TotalLevels && isLastUnlocked)
                );
            }
            else
            {
                // If not unlocked or HighscoreManager not available, just select level
                selectedLevel = level;
            }
        }
        
        private void EnsureEventSystem()
        {
            if (EventSystem.current == null)
            {
                Debug.Log("[LuneRun] MenuManager.EnsureEventSystem - Creating EventSystem");
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                Debug.Log("[LuneRun] EventSystem created");
            }
            else
            {
                Debug.Log($"[LuneRun] MenuManager.EnsureEventSystem - EventSystem already exists: {EventSystem.current.gameObject.name}");
            }
        }
        
        private void SetupTabButtons()
        {
            if (tab1Button != null)
            {
                tab1Button.onClick.RemoveAllListeners();
                tab1Button.onClick.AddListener(() => SwitchTab(1));
                Debug.Log("[LuneRun] Tab1 button listener set");
            }
            else
            {
                Debug.LogWarning("[LuneRun] Tab1 button reference is null");
            }
            
            if (tab2Button != null)
            {
                tab2Button.onClick.RemoveAllListeners();
                tab2Button.onClick.AddListener(() => SwitchTab(2));
                Debug.Log("[LuneRun] Tab2 button listener set");
            }
            else
            {
                Debug.LogWarning("[LuneRun] Tab2 button reference is null");
            }
            
            if (tab3Button != null)
            {
                tab3Button.onClick.RemoveAllListeners();
                tab3Button.onClick.AddListener(() => SwitchTab(3));
                Debug.Log("[LuneRun] Tab3 button listener set");
            }
            else
            {
                Debug.LogWarning("[LuneRun] Tab3 button reference is null");
            }
            
            // Apply initial highlighting
            UpdateTabAppearance();
        }
        
        private void SwitchTab(int tabIndex)
        {
            if (currentTab == tabIndex) return;
            
            Debug.Log($"[LuneRun] MenuManager.SwitchTab - Switching from tab {currentTab} to tab {tabIndex}");
            currentTab = tabIndex;
            
            // Update button appearances (highlight active tab)
            UpdateTabAppearance();
            
            // Reload level buttons for this tab
            ReloadLevelButtonsForCurrentTab();
        }
        
        private void UpdateTabAppearance()
        {
            Debug.Log($"[LuneRun] MenuManager.UpdateTabAppearance - Active tab: {currentTab}");
            
            // Highlight active tab button, dim inactive tabs
            Color activeColor = new Color(1f, 0.8f, 0f); // Orange-yellow
            Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.8f); // Semi-transparent gray
            
            if (tab1Button != null)
            {
                Image img = tab1Button.GetComponent<Image>();
                if (img != null) img.color = (currentTab == 1) ? activeColor : inactiveColor;
                
                Text btnText = tab1Button.GetComponentInChildren<Text>();
                if (btnText != null) btnText.color = (currentTab == 1) ? Color.white : new Color(0.7f, 0.7f, 0.7f);
            }
            
            if (tab2Button != null)
            {
                Image img = tab2Button.GetComponent<Image>();
                if (img != null) img.color = (currentTab == 2) ? activeColor : inactiveColor;
                
                Text btnText = tab2Button.GetComponentInChildren<Text>();
                if (btnText != null) btnText.color = (currentTab == 2) ? Color.white : new Color(0.7f, 0.7f, 0.7f);
            }
            
            if (tab3Button != null)
            {
                Image img = tab3Button.GetComponent<Image>();
                if (img != null) img.color = (currentTab == 3) ? activeColor : inactiveColor;
                
                Text btnText = tab3Button.GetComponentInChildren<Text>();
                if (btnText != null) btnText.color = (currentTab == 3) ? Color.white : new Color(0.7f, 0.7f, 0.7f);
            }
        }
        
        private void ReloadLevelButtonsForCurrentTab()
        {
            if (levelButtons == null || levelButtons.Count == 0) return;
            
            if (currentTab == 3)
            {
                // Infinite mode tab
                Debug.Log("[LuneRun] MenuManager.ReloadLevelButtonsForCurrentTab - Tab 3 (Infinite mode)");
                
                for (int i = 0; i < levelButtons.Count; i++)
                {
                    Button btn = levelButtons[i];
                    if (btn != null)
                    {
                        if (i == 0)
                        {
                            // First button is Infinite mode
                            btn.gameObject.SetActive(true);
                            Text btnText = btn.GetComponentInChildren<Text>();
                            if (btnText != null)
                            {
                                btnText.text = "Infinite";
                            }
                            
                            btn.onClick.RemoveAllListeners();
                            btn.onClick.AddListener(() => OnLevelButtonClicked(Constants.Tab3InfiniteLevelId));
                            btn.interactable = true; // Infinite mode always accessible
                        }
                        else
                        {
                            // Hide other buttons
                            btn.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                // Normal level tabs (1 and 2)
                int startLevel = (currentTab - 1) * Constants.LevelsPerTab + 1;
                int endLevel = Mathf.Min(startLevel + Constants.LevelsPerTab - 1, Constants.TotalLevels); // Cap at 60
                
                Debug.Log($"[LuneRun] MenuManager.ReloadLevelButtonsForCurrentTab - Tab {currentTab}: levels {startLevel} to {endLevel}");
                
                // Update button labels and bindings
                for (int i = 0; i < levelButtons.Count; i++)
                {
                    Button btn = levelButtons[i];
                    if (btn != null)
                    {
                        int level = startLevel + i;
                        if (level <= endLevel)
                        {
                            // Update button text
                            Text btnText = btn.GetComponentInChildren<Text>();
                            if (btnText != null)
                            {
                                btnText.text = "Level " + level;
                            }
                            
                            // Update click listener
                            btn.onClick.RemoveAllListeners();
                            int capturedLevel = level; // Capture for closure
                            btn.onClick.AddListener(() => OnLevelButtonClicked(capturedLevel));
                            
                            // Update interactable state based on unlocked status
                            bool unlocked = IsLevelUnlocked(level) || (runnerApi is LocalRunnerApi);
                            btn.interactable = unlocked;
                        }
                        else
                        {
                            // Hide or disable extra buttons
                            btn.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        
        private void CreateMissingLevelButtons()
        {
            Debug.Log("[LuneRun] MenuManager.CreateMissingLevelButtons - Creating missing level buttons dynamically");
            
            // Find or create LevelPanel
            GameObject levelPanel = GameObject.Find("LevelPanel");
            if (levelPanel == null)
            {
                // Find Canvas
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    Debug.LogError("[LuneRun] No Canvas found in scene. Cannot create level buttons.");
                    return;
                }
                
                // Create LevelPanel
                levelPanel = new GameObject("LevelPanel", typeof(RectTransform));
                levelPanel.transform.SetParent(canvas.transform, false);
                RectTransform panelRT = levelPanel.GetComponent<RectTransform>();
                panelRT.anchorMin = new Vector2(0.5f, 0.5f);
                panelRT.anchorMax = new Vector2(0.5f, 0.5f);
                panelRT.pivot = new Vector2(0.5f, 0.5f);
                panelRT.sizeDelta = new Vector2(600, 400);
                panelRT.anchoredPosition = Vector2.zero;
                Debug.Log("[LuneRun] Created LevelPanel");
            }
            
            // Create button template
            GameObject buttonTemplate = new GameObject("ButtonTemplate", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonTemplate.SetActive(false);
            buttonTemplate.transform.SetParent(levelPanel.transform, false);
            
            // Set up button appearance
            Image templateImg = buttonTemplate.GetComponent<Image>();
            templateImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            templateImg.sprite = null;
            
            // Add text child
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObj.transform.SetParent(buttonTemplate.transform, false);
            Text textComp = textObj.GetComponent<Text>();
            textComp.text = "Level";
            textComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComp.fontSize = 20;
            textComp.color = Color.white;
            textComp.alignment = TextAnchor.MiddleCenter;
            
            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;
            
            // Create 32 buttons in a grid (for tab1)
            int buttonsToCreate = Constants.LevelsPerTab - levelButtons.Count(btn => btn != null);
            for (int i = 0; i < buttonsToCreate; i++)
            {
                GameObject btnObj = Instantiate(buttonTemplate);
                btnObj.name = "Level" + (i + 1) + "Button";
                btnObj.SetActive(true);
                btnObj.transform.SetParent(levelPanel.transform, false);
                
                // Position in grid (8 columns x 4 rows)
                int col = i % 8;
                int row = i / 8;
                float x = (col - 3.5f) * 70f;
                float y = (1.5f - row) * 70f;
                
                RectTransform rt = btnObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(60f, 60f);
                rt.anchoredPosition = new Vector2(x, y);
                
                Button btn = btnObj.GetComponent<Button>();
                levelButtons.Add(btn);
                Debug.Log($"[LuneRun] Created button: {btnObj.name}");
            }
            
            Destroy(buttonTemplate);
            Debug.Log($"[LuneRun] Created {buttonsToCreate} level buttons");
        }
    }
}