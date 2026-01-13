using UnityEngine;
using UnityEngine.UI;

namespace LuneRun
{
    public enum GameState
    {
        Login,
        MainMenu,
        Prelude,
        Game
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas uiCanvas;
        
        private GameState currentState = GameState.Login;
        private bool initialize3D = false;
        private bool isHardware = false;
        private int selectedLevel = -1;
        private bool showPostlude = false;
        private GameObject loginPanel;
        private GameObject preludePanel;

        // References to other managers
        private MenuManager menuManager;
        private LevelManager levelManager;
        private AudioManager audioManager;
        
        // Core game objects
        private Settings settings;
        private IRunnerApi runnerApi;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Initialize subsystems
            audioManager = AudioManager.Instance;
            // Skip login if debug flag set
            if (Constants.SkipToGame)
            {
                currentState = GameState.MainMenu;
            }
            else
            {
                // Show login screen (to be implemented)
                EnterLogin();
            }
        }

        private void Update()
        {
            // State machine
            switch (currentState)
            {
                case GameState.Login:
                    UpdateLogin();
                    break;
                case GameState.MainMenu:
                    UpdateMenu();
                    break;
                case GameState.Prelude:
                    UpdatePrelude();
                    break;
                case GameState.Game:
                    UpdateGame();
                    break;
            }
        }

        private void EnterLogin()
        {
            Debug.Log("Enter Login - Creating login UI");
            
            // Ensure canvas exists
            if (uiCanvas == null)
            {
                uiCanvas = FindObjectOfType<Canvas>();
                if (uiCanvas == null)
                {
                    GameObject canvasObj = new GameObject("Canvas");
                    uiCanvas = canvasObj.AddComponent<Canvas>();
                    uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasObj.AddComponent<CanvasScaler>();
                    canvasObj.AddComponent<GraphicRaycaster>();
                }
            }
            
            // Create login panel
            loginPanel = new GameObject("LoginPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            loginPanel.transform.SetParent(uiCanvas.transform, false);
            RectTransform panelRt = loginPanel.GetComponent<RectTransform>();
            panelRt.anchorMin = Vector2.zero;
            panelRt.anchorMax = Vector2.one;
            panelRt.offsetMin = Vector2.zero;
            panelRt.offsetMax = Vector2.zero;
            
            // Set background (semi-transparent black)
            Image panelImg = loginPanel.GetComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.9f);
            
            // Create title text
            GameObject titleObj = new GameObject("TitleText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            titleObj.transform.SetParent(loginPanel.transform, false);
            Text titleText = titleObj.GetComponent<Text>();
            titleText.text = Constants.Name;
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 96;
            titleText.color = new Color(1f, 0.8f, 0f); // Orange-yellow
            titleText.fontStyle = FontStyle.Bold;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0.5f, 0.5f);
            titleRt.anchorMax = new Vector2(0.5f, 0.5f);
            titleRt.pivot = new Vector2(0.5f, 0.5f);
            titleRt.sizeDelta = new Vector2(800, 150);
            titleRt.anchoredPosition = new Vector2(0, 100);
            
            // Create version text
            GameObject versionObj = new GameObject("VersionText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            versionObj.transform.SetParent(loginPanel.transform, false);
            Text versionText = versionObj.GetComponent<Text>();
            versionText.text = Constants.Version;
            versionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            versionText.fontSize = 24;
            versionText.color = new Color(0.7f, 0.7f, 0.7f);
            versionText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform versionRt = versionObj.GetComponent<RectTransform>();
            versionRt.anchorMin = new Vector2(0.5f, 0.5f);
            versionRt.anchorMax = new Vector2(0.5f, 0.5f);
            versionRt.pivot = new Vector2(0.5f, 0.5f);
            versionRt.sizeDelta = new Vector2(400, 50);
            versionRt.anchoredPosition = new Vector2(0, 40);
            
            // Create instruction text
            GameObject instructionObj = new GameObject("InstructionText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            instructionObj.transform.SetParent(loginPanel.transform, false);
            Text instructionText = instructionObj.GetComponent<Text>();
            instructionText.text = "Press any key to start";
            instructionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            instructionText.fontSize = 32;
            instructionText.color = Color.white;
            instructionText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform instructionRt = instructionObj.GetComponent<RectTransform>();
            instructionRt.anchorMin = new Vector2(0.5f, 0.5f);
            instructionRt.anchorMax = new Vector2(0.5f, 0.5f);
            instructionRt.pivot = new Vector2(0.5f, 0.5f);
            instructionRt.sizeDelta = new Vector2(600, 80);
            instructionRt.anchoredPosition = new Vector2(0, -80);
            
            Debug.Log("Login UI created");
        }

        private void UpdateLogin()
        {
            // Handle login logic
            if (Input.anyKeyDown)
            {
                DestroyLoginUI();
                currentState = GameState.MainMenu;
                EnterMenu();
            }
        }
        
        private void DestroyLoginUI()
        {
            if (loginPanel != null)
            {
                Destroy(loginPanel);
                loginPanel = null;
                Debug.Log("Login UI destroyed");
            }
        }

        private void EnterMenu()
        {
            // Ensure UI canvas is visible for menu
            if (uiCanvas != null)
            {
                uiCanvas.enabled = true;
                Debug.Log("UI canvas enabled for menu");
            }
            else
            {
                // Try to find canvas in scene
                Canvas foundCanvas = FindObjectOfType<Canvas>();
                if (foundCanvas != null)
                {
                    uiCanvas = foundCanvas;
                    uiCanvas.enabled = true;
                    Debug.Log("Found and enabled UI canvas: " + uiCanvas.name);
                }
                else
                {
                    Debug.LogWarning("No UI canvas found to enable");
                }
            }
            
            // Create menu manager if not exists
            if (menuManager == null)
            {
                GameObject menuObj = new GameObject("MenuManager");
                menuManager = menuObj.AddComponent<MenuManager>();
                
                // Initialize settings and runner API if not already done
                if (settings == null)
                {
                    settings = Settings.Load();
                }
                if (runnerApi == null)
                {
                    runnerApi = new LocalRunnerApi();
                }
                
                // Initialize the menu manager
                menuManager.Initialize(settings, runnerApi);
            }
        }

        private void UpdateMenu()
        {
            if (menuManager != null)
            {
                menuManager.UpdateMenu();
                selectedLevel = menuManager.GetSelectedLevel();
                if (selectedLevel != -1)
                {
                    // Transition to game or prelude
                    MenuManager.DestroyMenu(menuManager);
                    menuManager = null;
                    // Show prelude for level 1 or if user hasn't seen tutorial
                    if (selectedLevel == 1 || (settings != null && !settings.HasSeenTutorial()))
                    {
                        currentState = GameState.Prelude;
                    }
                    else
                    {
                        currentState = GameState.Game;
                    }
                    showPostlude = false;
                    Initialize3DView();
                }
            }
        }

        private void Initialize3DView()
        {
            // Ensure camera is set up for 3D
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            // Set up render settings for moon environment
            if (mainCamera != null)
            {
                // Clear to black (space background)
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = Color.black;
                
                // Set camera far clip plane for distant objects
                mainCamera.farClipPlane = 1000f;
                
                // Enable HDR for better lighting
                mainCamera.allowHDR = true;
            }
            
            // Adjust lighting for moon environment
            Light mainLight = GameObject.FindObjectOfType<Light>();
            if (mainLight != null && mainLight.type == LightType.Directional)
            {
                // Moon has harsh directional lighting from the sun
                mainLight.intensity = 1.2f;
                mainLight.color = new Color(1f, 0.95f, 0.9f); // Slightly yellowish
                mainLight.shadows = LightShadows.Soft;
            }
            
            // Set quality settings for smooth gameplay
            QualitySettings.vSyncCount = 0; // Disable VSync for maximum FPS
            QualitySettings.antiAliasing = 2; // 2x MSAA for performance/quality balance
            QualitySettings.shadows = ShadowQuality.All; // Enable shadows
            
            initialize3D = true;
        }

        private void UpdatePrelude()
        {
            // Create prelude UI if not exists
            if (preludePanel == null)
            {
                CreatePreludeUI();
            }
            
            // Wait for key press to start game
            if (Input.anyKeyDown)
            {
                // Mark tutorial as seen if this is first time
                if (settings != null && !settings.HasSeenTutorial())
                {
                    settings.SetSeenTutorial();
                    Debug.Log("Tutorial marked as seen");
                }
                
                DestroyPreludeUI();
                currentState = GameState.Game;
                EnterGame();
            }
        }
        
        private void CreatePreludeUI()
        {
            Debug.Log("Creating prelude UI");
            
            // Ensure canvas exists
            if (uiCanvas == null)
            {
                uiCanvas = FindObjectOfType<Canvas>();
                if (uiCanvas == null)
                {
                    GameObject canvasObj = new GameObject("Canvas");
                    uiCanvas = canvasObj.AddComponent<Canvas>();
                    uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasObj.AddComponent<CanvasScaler>();
                    canvasObj.AddComponent<GraphicRaycaster>();
                }
            }
            
            // Create prelude panel
            preludePanel = new GameObject("PreludePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            preludePanel.transform.SetParent(uiCanvas.transform, false);
            RectTransform panelRt = preludePanel.GetComponent<RectTransform>();
            panelRt.anchorMin = Vector2.zero;
            panelRt.anchorMax = Vector2.one;
            panelRt.offsetMin = Vector2.zero;
            panelRt.offsetMax = Vector2.zero;
            
            // Set background (semi-transparent black)
            Image panelImg = preludePanel.GetComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.9f);
            
            // Determine text based on context
            string titleTextStr = "LEVEL " + selectedLevel;
            string storyTextStr;
            
            if (selectedLevel == 1)
            {
                // Level 1 story introduction
                storyTextStr = "Welcome to Lunerun!\n\n" +
                               "You are an astronaut running on the moon.\n" +
                               "Hold SPACE to run, release to jump.\n" +
                               "Avoid obstacles and reach the goal!\n\n" +
                               "Press any key to begin.";
            }
            else if (settings != null && !settings.HasSeenTutorial())
            {
                // First time playing, tutorial introduction
                titleTextStr = "TUTORIAL";
                storyTextStr = "Welcome to Lunerun!\n\n" +
                               "This is your first time playing.\n" +
                               "Hold SPACE to run, release to jump.\n" +
                               "Avoid obstacles and reach the goal!\n\n" +
                               "After this level, you can access all unlocked levels.\n\n" +
                               "Press any key to begin.";
            }
            else
            {
                // Normal level prelude
                storyTextStr = $"Level {selectedLevel}\n\n" +
                               "Hold SPACE to run, release to jump.\n" +
                               "Avoid obstacles and reach the goal!\n\n" +
                               "Press any key to begin.";
            }
            
            // Create title text
            GameObject titleObj = new GameObject("PreludeTitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            titleObj.transform.SetParent(preludePanel.transform, false);
            Text titleText = titleObj.GetComponent<Text>();
            titleText.text = titleTextStr;
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 72;
            titleText.color = Color.yellow;
            titleText.fontStyle = FontStyle.Bold;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0.5f, 0.5f);
            titleRt.anchorMax = new Vector2(0.5f, 0.5f);
            titleRt.pivot = new Vector2(0.5f, 0.5f);
            titleRt.sizeDelta = new Vector2(600, 100);
            titleRt.anchoredPosition = new Vector2(0, 150);
            
            // Create story text
            GameObject storyObj = new GameObject("StoryText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            storyObj.transform.SetParent(preludePanel.transform, false);
            Text storyText = storyObj.GetComponent<Text>();
            storyText.text = storyTextStr;
            storyText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            storyText.fontSize = 28;
            storyText.color = Color.white;
            storyText.alignment = TextAnchor.MiddleCenter;
            storyText.lineSpacing = 1.3f;
            
            RectTransform storyRt = storyObj.GetComponent<RectTransform>();
            storyRt.anchorMin = new Vector2(0.5f, 0.5f);
            storyRt.anchorMax = new Vector2(0.5f, 0.5f);
            storyRt.pivot = new Vector2(0.5f, 0.5f);
            storyRt.sizeDelta = new Vector2(700, 400);
            storyRt.anchoredPosition = new Vector2(0, -50);
            
            Debug.Log("Prelude UI created");
        }
        
        private void DestroyPreludeUI()
        {
            if (preludePanel != null)
            {
                Destroy(preludePanel);
                preludePanel = null;
                Debug.Log("Prelude UI destroyed");
            }
        }

        private void EnterGame()
        {
            // Hide UI canvas during gameplay
            if (uiCanvas != null)
            {
                uiCanvas.enabled = false;
                Debug.Log("UI canvas hidden for gameplay");
            }
            else
            {
                // Try to find canvas in scene
                Canvas foundCanvas = FindObjectOfType<Canvas>();
                if (foundCanvas != null)
                {
                    uiCanvas = foundCanvas;
                    uiCanvas.enabled = false;
                    Debug.Log("Found and hidden UI canvas: " + uiCanvas.name);
                }
                else
                {
                    Debug.LogWarning("No UI canvas found to hide");
                }
            }
            
            if (levelManager == null)
            {
                GameObject levelObj = new GameObject("LevelManager");
                levelManager = levelObj.AddComponent<LevelManager>();
                
                // Ensure settings and runner API exist
                if (settings == null)
                {
                    settings = Settings.Load();
                }
                if (runnerApi == null)
                {
                    runnerApi = new LocalRunnerApi();
                }
                
                // Initialize the level manager
                Camera view = mainCamera != null ? mainCamera : Camera.main;
                levelManager.Initialize(view, selectedLevel, runnerApi, settings, isHardware);
            }
        }

        private void UpdateGame()
        {
            if (levelManager != null)
            {
                levelManager.UpdateLevel();
                if (levelManager.ShouldExit())
                {
                    bool completed = levelManager.IsCompleted();
                    levelManager.DestroyLevel();
                    Destroy(levelManager.gameObject);
                    levelManager = null;
                    showPostlude = completed;
                    currentState = showPostlude ? GameState.Prelude : GameState.MainMenu;
                }
            }
        }

        // Public methods for state transitions
        public void GoToMainMenu()
        {
            currentState = GameState.MainMenu;
            EnterMenu();
        }

        public void StartGame(int levelId)
        {
            selectedLevel = levelId;
            // Show prelude for level 1 or if user hasn't seen tutorial
            if (selectedLevel == 1 || (settings != null && !settings.HasSeenTutorial()))
            {
                currentState = GameState.Prelude;
            }
            else
            {
                currentState = GameState.Game;
            }
            showPostlude = false;
            Initialize3DView();
        }
    }
}