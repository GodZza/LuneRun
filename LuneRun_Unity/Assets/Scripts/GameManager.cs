using UnityEngine;

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

        // References to other managers
        private MenuManager menuManager;
        private LevelManager levelManager;
        private AudioManager audioManager;

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
            // TODO: Show login UI
            Debug.Log("Enter Login");
        }

        private void UpdateLogin()
        {
            // TODO: Handle login logic
            // For now, skip to main menu
            if (Input.anyKeyDown)
            {
                currentState = GameState.MainMenu;
                EnterMenu();
            }
        }

        private void EnterMenu()
        {
            // Create menu manager if not exists
            if (menuManager == null)
            {
                GameObject menuObj = new GameObject("MenuManager");
                menuManager = menuObj.AddComponent<MenuManager>();
                // TODO: Initialize menu with settings and runner API
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
                    currentState = selectedLevel != 1 ? GameState.Game : GameState.Prelude;
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
            // TODO: Set up render settings
            initialize3D = true;
        }

        private void UpdatePrelude()
        {
            // TODO: Implement prelude sequence
            if (Input.anyKeyDown)
            {
                currentState = GameState.Game;
                EnterGame();
            }
        }

        private void EnterGame()
        {
            if (levelManager == null)
            {
                GameObject levelObj = new GameObject("LevelManager");
                levelManager = levelObj.AddComponent<LevelManager>();
                // TODO: Initialize level with view, selectedLevel, runner API, settings, isHardware
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
            currentState = levelId != 1 ? GameState.Game : GameState.Prelude;
            showPostlude = false;
            Initialize3DView();
        }
    }
}