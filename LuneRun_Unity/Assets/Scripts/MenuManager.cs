using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LuneRun
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject helpPanel;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button musicButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private Button fullscreenButton;
        [SerializeField] private Button tweetButton;
        [SerializeField] private Button linkButton;
        [SerializeField] private Button survivorsButton;
        [SerializeField] private Button paypalButton;
        [SerializeField] private Text versionText;
        [SerializeField] private Text logoText;
        
        // Level buttons (1-32)
        private readonly List<Button> levelButtons = new();
        private int selectedLevel = -1;
        private bool isLastUnlocked = false;
        
        // References
        private Settings settings;
        private IRunnerApi runnerApi;
        
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
            
            // TODO: Populate level buttons
            // For now, simulate 32 levels
            for (int i = 1; i <= 32; i++)
            {
                // Create or find level button
                // Attach listener with level index
            }
        }
        
        private void UpdateSettings()
        {
            // Update sound/music button icons based on settings
            if (soundButton != null)
            {
                // Set appropriate sprite for on/off
            }
            if (musicButton != null)
            {
                // Set appropriate sprite for on/off
            }
        }
        
        private void OnUserDataReceived(UserData userData)
        {
            // Update level buttons based on unlocked status
            // Determine if last level is unlocked
            isLastUnlocked = userData.GetScore(32) != 0f;
            
            // Update button backgrounds
            for (int i = 1; i <= 32; i++)
            {
                bool unlocked = userData.GetScore(i) != 0f;
                // Update button appearance
            }
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
            if (menu != null && menu.gameObject != null)
            {
                Destroy(menu.gameObject);
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
        
        public static void OnHelpButtonClicked()
        {
            // TODO: Implement help panel
            Debug.Log("Help button clicked");
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
        
        public static void OnSurvivorsButtonClicked()
        {
            // Show survivors/highscores
            Debug.Log("Survivors button clicked");
        }
        
        public static void OnPayPalButtonClicked()
        {
            Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=KM7VFNVAG8CF4&lc=SE&item_name=Lunerun&item_number=10&no_note=0&cn=L%c3%a4gga%20till%20specialinstruktioner%20till%20s%c3%a4ljaren%3a&no_shipping=1&currency_code=SEK&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
        }
        
        public void OnLevelButtonClicked(int level)
        {
            // Check if level is unlocked
            // If local runner API, allow selection even if not unlocked for testing
            if (runnerApi is LocalRunnerApi)
            {
                selectedLevel = level;
            }
            else
            {
                // Show highscore dialog for unlocked levels
                // TODO: Implement highscore dialog
                selectedLevel = level;
            }
        }
    }
}