using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
            this.settings = settings;
            this.runnerApi = runnerApi;
            
            // Setup UI
            if (logoText != null) logoText.text = Constants.Name;
            if (versionText != null) versionText.text = Constants.Version;
            
            // Load user data asynchronously
            runnerApi.GetUserData(OnUserDataReceived);
            
            // Update button states based on settings
            UpdateSettings();
            
            // Ensure level buttons exist and have listeners
            EnsureLevelButtons();
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
        
        private void EnsureLevelButtons()
        {
            // Ensure we have 32 level buttons
            if (levelButtons == null)
            {
                levelButtons = new List<Button>();
            }
            
            // If no buttons are provided, we cannot create them without a parent.
            // This method assumes buttons are already created by editor tool.
            // Just ensure each button has the correct click listener.
            for (int i = 0; i < levelButtons.Count; i++)
            {
                int level = i + 1;
                Button btn = levelButtons[i];
                if (btn != null)
                {
                    // Remove existing listeners and add our own
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => OnLevelButtonClicked(level));
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