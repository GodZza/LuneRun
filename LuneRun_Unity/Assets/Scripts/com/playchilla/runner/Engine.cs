using UnityEngine;
using com.playchilla.runner.player;
using com.playchilla.runner.api;
using com.playchilla.gameapi.api;
using shared.debug;
using away3d.containers;
using away3d.events;
using away3d.core.managers;
using shared.sound;
using LuneRun;

namespace com.playchilla.runner
{
    public class Engine : MonoBehaviour
    {
        // State constants
        internal const int _Login = 1;
        internal const int _MainMenu = 2;
        internal const int _Prelude = 3;
        internal const int _Game = 4;

        // Public static performance timer (simplified)
        public static PerformanceTimer pt = new PerformanceTimer();

        // Internal fields
        internal RemoteAssertHandler _remoteAssertHandler;
        internal Settings _settings;
        internal com.playchilla.runner.api.IRunnerApi _runnerApi;
        internal View3D _view;
        internal bool _initialize3D = false;
        internal bool _isHardware = false;
        internal bool _disposed = false;
        internal object _login; // Placeholder for Login component
        internal RowView _ptView;
        internal int _state = _Login;
        internal bool _showPostlude = false;
        internal int _selectedLevel = -1;
        internal object _menu;
        internal object _game;
        internal object _prelude;
        internal object _bg;

        // For self-created view tracking
        internal int _selfCreated = 0;

        void Awake()
        {
            // Simplified initialization
            _remoteAssertHandler = new RemoteAssertHandler("http://www.playchilla.com/api/assert.php", Constants.Name, Constants.Version);
            shared.debug.Debug.setAssertHandler(_remoteAssertHandler);
            UnityEngine.Debug.Log("Engine initialized");
            _settings = Settings.Load();
            if (Constants.SkipToGame)
            {
                _state = _MainMenu;
            }
            
            // Get reference to GameManager
            //_gameManager = GameManager.Instance;
            //if (_gameManager != null)
            {
                UnityEngine.Debug.Log("Engine: Found GameManager");
            }
        }

        void Start()
        {
            // Start the game loop
            UnityEngine.Debug.Log("Engine started");
        }

        void Update()
        {
            // Main update loop
            pt.Start("onEnterFrame");

            // Update audio
            Audio.Sound.update((int)(Time.time * 1000));
            Audio.Music.update((int)(Time.time * 1000));

            // Handle key events
            if (Input.GetKeyDown(KeyCode.F3))
            {
                if (_ptView != null)
                    _ptView.visible = !_ptView.visible;
            }
            else if (Input.GetKeyDown(KeyCode.F11) && _state != _Login)
            {
                Screen.fullScreen = !Screen.fullScreen;
            }

            // State machine
            switch (_state)
            {
                case _Login:
                    UpdateLogin();
                    break;
                case _MainMenu:
                    UpdateMenu();
                    break;
                case _Prelude:
                    UpdatePrelude();
                    break;
                case _Game:
                    UpdateGame();
                    break;
                default:
                    shared.debug.Debug.Assert(false, "Bad state.");
                    break;
            }

            pt.Stop("onEnterFrame");
        }

        internal void UpdateLogin()
        {
            // Delegate to GameManager if available
            //if (_gameManager != null)
            {
                // Let GameManager handle login state
                return;
            }
            
            // Placeholder for login logic
            // For now, transition to main menu after a delay
            if (Time.time > 5.0f) // Example condition
            {
                UnityEngine.Debug.Log("Engine: Login -> MainMenu");
                _state = _MainMenu;
            }
        }

        internal void UpdateMenu()
        {
            // Delegate to GameManager if available
            //if (_gameManager != null)
            {
                // Let GameManager handle menu state
                return;
            }
            
            // Placeholder for menu logic
            // If a level is selected, transition to game or prelude
            if (_selectedLevel != -1)
            {
                if (_selectedLevel == 1)
                    _state = _Prelude;
                else
                    _state = _Game;
                _selectedLevel = -1;
            }
        }

        internal void UpdatePrelude()
        {
            // Delegate to GameManager if available
            //if (_gameManager != null)
            {
                // Let GameManager handle prelude state
                return;
            }
            
            // Placeholder for prelude logic
            // After prelude, transition to game or main menu
            if (_prelude == null)
            {
                // Create prelude
                // _prelude = new Prelude(_view, _showPostlude);
            }
            // Update prelude
            // if (_prelude.isDone())
            // {
            //     _state = _showPostlude ? _MainMenu : _Game;
            //     _showPostlude = false;
            // }
        }

        internal void UpdateGame()
        {
            // Delegate to GameManager if available
            //if (_gameManager != null)
            {
                // Let GameManager handle game state
                return;
            }
            
            // Placeholder for game logic
            if (!_initialize3D)
            {
                // Wait for 3D initialization
                return;
            }

            if (_game == null)
            {
                _showPostlude = false;
                // Create game instance
                // _game = new Game(_view, _selectedLevel, _runnerApi, _settings, _isHardware);
            }

            // Update game
            // _game.update();

            // Render 3D view
            if (_view != null)
            {
                pt.Start("away3d.render");
                _view.Render();
                pt.Stop("away3d.render");
            }

            // Check if game should exit
            // if (_game.shouldExit())
            // {
            //     // Clean up
            //     _state = _showPostlude ? _Prelude : _MainMenu;
            // }
        }

        internal void OnKeyDown(KeyCode keyCode)
        {
            // Handle key events
            if (keyCode == KeyCode.F3)
            {
                if (_ptView != null)
                    _ptView.visible = !_ptView.visible;
            }
            else if (keyCode == KeyCode.F11 && _state != _Login)
            {
                // Toggle fullscreen (Unity-specific)
                Screen.fullScreen = !Screen.fullScreen;
            }
        }

        internal void CreateView()
        {
            if (_view != null)
            {
                DestroyView();
            }
            _view = new View3D();
            _view.backgroundColor = 0;
            _view.antiAlias = 4;
            // Add to scene? (Unity-specific handling needed)
            _view.stage3DProxy.addEventListener("context3DCreate", OnContext3DCreate);
            _selfCreated++;
        }

        internal void DestroyView()
        {
            if (_view != null)
            {
                _view.stage3DProxy.removeEventListener("context3DCreate", OnContext3DCreate);
                // Remove from scene
                _view.stage3DProxy.dispose();
                _view = null;
            }
        }

        internal void OnContext3DCreate(Stage3DEvent e)
        {
            var proxy = e.currentTarget as Stage3DProxy;
            _isHardware = !proxy.context3D.driverInfo.ToLower().Contains("software");
            if (_selfCreated > 0)
            {
                _selfCreated--;
                return;
            }
            _initialize3D = true;
        }

        void OnDestroy()
        {
            _disposed = true;
            DestroyView();
        }
    }
}