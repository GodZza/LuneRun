using UnityEngine;
using UnityEngine.UI;
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
        
        // Level buttons (1-32)
        public List<Button> levelButtons = new();
        private int selectedLevel = -1;
        private bool isLastUnlocked = false;
        
        // References
        private Settings settings;
        private IRunnerApi runnerApi;
        private UserData currentUserData;
        
        public void Initialize(Settings settings, IRunnerApi runnerApi)
        {
            Debug.Log("[LuneRun] MenuManager.Initialize - Starting menu initialization");
            this.settings = settings;
            this.runnerApi = runnerApi;
            
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
            
            int levelButtonCount = levelButtons?.Count(btn => btn != null) ?? 0;
            if (levelButtonCount < 32)
            {
                missingRefs.Add($"levelButtons ({levelButtonCount}/32)");
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
            
            // Ensure we have 32 level buttons
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
            
            // Ensure each button has the correct click listener
            for (int i = 0; i < levelButtons.Count; i++)
            {
                int level = i + 1;
                Button btn = levelButtons[i];
                if (btn != null)
                {
                    // Remove existing listeners and add our own
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => OnLevelButtonClicked(level));
                    Debug.Log($"[LuneRun] MenuManager.EnsureLevelButtons - Set click listener for Level {level} button");
                }
                else if (i < 32) // Only warn for first 32 levels
                {
                    Debug.LogWarning($"[LuneRun] MenuManager.EnsureLevelButtons - Level {level} button is null");
                }
            }
            
            // If we still don't have enough buttons, warn the user
            if (validButtonCount < 32)
            {
                Debug.LogWarning($"[LuneRun] MenuManager.EnsureLevelButtons - Only {validButtonCount} level buttons found. UI may not be properly set up.");
                Debug.LogWarning("[LuneRun] Please run 'Tools/LuneRun/Setup Scene UI' in the Unity editor to create the menu UI.");
            }
            
            Debug.Log("[LuneRun] MenuManager.EnsureLevelButtons - Completed");
        }
        
        private void FindLevelButtonsInScene()
        {
            // Clear the list first
            levelButtons.Clear();
            
            // Try to find buttons by name pattern "LevelXButton"
            for (int i = 1; i <= 32; i++)
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
            // Determine if last level is unlocked
            isLastUnlocked = userData.GetScore(32) != 0f;
            
            // Update button backgrounds
            if (levelButtons != null && levelButtons.Count >= 32)
            {
                for (int i = 0; i < 32; i++)
                {
                    bool unlocked = userData.GetScore(i + 1) != 0f;
                    Button btn = levelButtons[i];
                    if (btn != null)
                    {
                        // Update button appearance based on unlocked status
                        // For now, just enable/disable interactable
                        btn.interactable = unlocked || (runnerApi is LocalRunnerApi);
                    }
                }
            }
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
                    isLastUnlocked: (level == 32 && isLastUnlocked)
                );
            }
            else
            {
                // If not unlocked or HighscoreManager not available, just select level
                selectedLevel = level;
            }
        }
    }
}