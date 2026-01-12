using UnityEngine;
using UnityEngine.UI;

namespace LuneRun
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool useLocalRunnerApi = true;
        [SerializeField] private bool skipLogin = false;
        
        [Header("References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas mainCanvas;
        
        private GameManager gameManager;
        private AudioManager audioManager;
        private HighscoreManager highscoreManager;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeCoreSystems();
        }
        
        private void Start()
        {
            // Load player settings
            Settings settings = Settings.Load();
            
            // Initialize runner API
            IRunnerApi runnerApi = useLocalRunnerApi ? 
                new LocalRunnerApi() : 
                new RunnerApi(Constants.RunnerApiUrl, null); // TODO: Pass actual user
            
            // Start game flow
            if (skipLogin || Constants.SkipToGame)
            {
                gameManager.StartGame(1); // Start with level 1
            }
            else
            {
                gameManager.GoToMainMenu();
            }
        }
        
        private void InitializeCoreSystems()
        {
            // GameManager
            GameObject gmObj = new GameObject("GameManager");
            gameManager = gmObj.AddComponent<GameManager>();
            
            // AudioManager
            GameObject amObj = new GameObject("AudioManager");
            audioManager = amObj.AddComponent<AudioManager>();
            
            // HighscoreManager
            GameObject hmObj = new GameObject("HighscoreManager");
            highscoreManager = hmObj.AddComponent<HighscoreManager>();
            
            // Ensure camera
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    GameObject camObj = new GameObject("MainCamera");
                    mainCamera = camObj.AddComponent<Camera>();
                    camObj.AddComponent<AudioListener>();
                    camObj.tag = "MainCamera";
                }
            }
            
            // Ensure canvas
            if (mainCanvas == null)
            {
                mainCanvas = FindObjectOfType<Canvas>();
                if (mainCanvas == null)
                {
                    GameObject canvasObj = new GameObject("Canvas");
                    mainCanvas = canvasObj.AddComponent<Canvas>();
                    mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    
                    canvasObj.AddComponent<CanvasScaler>();
                    canvasObj.AddComponent<GraphicRaycaster>();
                }
            }
        }
        
        // Public API for external control
        public void RestartGame()
        {
            // Clean up and restart
            if (gameManager != null)
            {
                // TODO: Implement proper restart
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}