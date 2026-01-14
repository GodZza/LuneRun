using UnityEngine;
using UnityEngine.UI;

namespace LuneRun
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private bool spawnManagers = true;
        [SerializeField] private bool loadMainMenu = true;
        
        private void Awake()
        {
            if (spawnManagers)
            {
                EnsureManagers();
            }
        }
        
        private void Start()
        {
            if (loadMainMenu)
            {
                // Start the game in main menu state
                GameManager.Instance.GoToMainMenu();
            }
        }
        
        private void EnsureManagers()
        {
            // Create GameManager if not exists
            if (GameManager.Instance == null)
            {
                GameObject gameManagerObj = new GameObject("GameManager");
                gameManagerObj.AddComponent<GameManager>();
            }
            
            // Create AudioManager if not exists
            if (AudioManager.Instance == null)
            {
                GameObject audioManagerObj = new GameObject("AudioManager");
                audioManagerObj.AddComponent<AudioManager>();
            }
            
            // Create HighscoreManager if not exists
            if (HighscoreManager.Instance == null)
            {
                GameObject highscoreManagerObj = new GameObject("HighscoreManager");
                highscoreManagerObj.AddComponent<HighscoreManager>();
            }
            
            // Create Engine (Flash-style entry point)
            if (FindObjectOfType<com.playchilla.runner.Engine>() == null)
            {
                GameObject engineObj = new GameObject("Engine");
                engineObj.AddComponent<com.playchilla.runner.Engine>();
                DontDestroyOnLoad(engineObj);
            }
            
            // Ensure camera exists
            if (Camera.main == null)
            {
                GameObject cameraObj = new GameObject("MainCamera");
                cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
                cameraObj.tag = "MainCamera";
            }
            
            // Ensure canvas exists for UI
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
        }
    }
}