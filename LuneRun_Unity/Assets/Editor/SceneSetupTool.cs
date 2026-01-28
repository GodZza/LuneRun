using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using LuneRun;

namespace LuneRun.Editor
{
    public static class SceneSetupTool
    {
        [MenuItem("Tools/LuneRun/Setup Test Scene")]
        public static void SetupTestScene()
        {
            // Create a new empty scene for testing
            Scene currentScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            // Rename the scene
            EditorSceneManager.SaveScene(currentScene, "Assets/Scenes/Level1Test.unity", true);

            // Mark scene dirty for saving
            EditorSceneManager.MarkSceneDirty(currentScene);

            // Create basic scene objects
            SetupBasicScene();

            // Add TestTrackSetup component
            GameObject testSetupObj = new GameObject("TestTrackSetup");
            TestTrackSetup testSetup = testSetupObj.AddComponent<TestTrackSetup>();

            // Set default values
            SetPrivateField(testSetup, "testLevelId", 1);
            SetPrivateField(testSetup, "segmentLength", 10f);
            SetPrivateField(testSetup, "segmentsPerLevel", 20);
            SetPrivateField(testSetup, "maxSlopeAngle", 30f);
            SetPrivateField(testSetup, "playerStartPosition", new Vector3(0, 0.2f, 0));
            SetPrivateField(testSetup, "cameraDistance", 20f);
            SetPrivateField(testSetup, "cameraHeight", 10f);
            SetPrivateField(testSetup, "showDebugMarkers", true);
            SetPrivateField(testSetup, "drawGizmos", true);

            Debug.Log("Test scene created successfully!");
            Debug.Log("Press ▶️ to play the level.");
            Debug.Log("Controls: Hold SPACE to run, Release to jump");
        }
        
        private static void SetupBasicScene()
        {
            // Create main camera
            GameObject cameraObj = new GameObject("MainCamera");
            Camera mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
            cameraObj.tag = "MainCamera";
            
            // Set camera properties
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
            mainCamera.farClipPlane = 1000f;
            mainCamera.nearClipPlane = 0.1f;
            mainCamera.fieldOfView = 60f;
            
            // Create directional light
            GameObject lightObj = new GameObject("Directional Light");
            Light mainLight = lightObj.AddComponent<Light>();
            mainLight.type = LightType.Directional;
            mainLight.intensity = 1.2f;
            mainLight.color = new Color(1f, 0.95f, 0.9f);
            mainLight.shadows = LightShadows.Soft;
            lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            
            Debug.Log("Basic scene setup completed");
        }
        
        private static void SetPrivateField(object obj, string fieldName, object value)
        {
            System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                Debug.LogWarning($"Could not set private field: {fieldName}");
            }
        }
        
        [MenuItem("Tools/LuneRun/Setup Scene UI")]
        public static void SetupSceneUI()
        {
            // Ensure we are in a scene
            Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!currentScene.IsValid())
            {
                Debug.LogError("No active scene found.");
                return;
            }

            // Mark scene dirty for saving
            EditorSceneManager.MarkSceneDirty(currentScene);

            // Ensure Bootstrapper exists
            EnsureBootstrapper();

            // Ensure Canvas exists
            Canvas canvas = EnsureCanvas();

            // Setup UI hierarchy
            SetupUI(canvas);

            // Ensure managers exist (optional)
            EnsureManagers();

            Debug.Log("Scene UI setup completed. Please save the scene.");
        }

        private static void EnsureBootstrapper()
        {
            GameObject bootstrapperObj = GameObject.Find("Bootstrapper");
            if (bootstrapperObj == null)
            {
                bootstrapperObj = new GameObject("Bootstrapper");
                bootstrapperObj.AddComponent<Bootstrapper>();
                Debug.Log("Created Bootstrapper GameObject.");
            }
            else if (bootstrapperObj.GetComponent<Bootstrapper>() == null)
            {
                bootstrapperObj.AddComponent<Bootstrapper>();
                Debug.Log("Added Bootstrapper component to existing GameObject.");
            }
        }

        private static Canvas EnsureCanvas()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                Debug.Log("Created Canvas.");
            }
            return canvas;
        }

        private static void EnsureManagers()
        {
            // Ensure GameManager exists
            GameManager gm = Object.FindFirstObjectByType<GameManager>();
            if (gm == null)
            {
                GameObject gameManagerObj = new GameObject("GameManager");
                gameManagerObj.AddComponent<GameManager>();
                Debug.Log("Created GameManager GameObject.");
            }

            // Ensure AudioManager exists
            AudioManager am = Object.FindFirstObjectByType<AudioManager>();
            if (am == null)
            {
                GameObject audioManagerObj = new GameObject("AudioManager");
                audioManagerObj.AddComponent<AudioManager>();
                Debug.Log("Created AudioManager GameObject.");
            }

            // Ensure HighscoreManager exists
            HighscoreManager hm = Object.FindFirstObjectByType<HighscoreManager>();
            if (hm == null)
            {
                GameObject highscoreManagerObj = new GameObject("HighscoreManager");
                highscoreManagerObj.AddComponent<HighscoreManager>();
                Debug.Log("Created HighscoreManager GameObject.");
            }
        }

        private static void SetupUI(Canvas canvas)
        {
            // Clean up existing UI to prevent duplicates
            CleanupExistingUI();
            
            // Create main menu panel (full screen)
            GameObject mainMenuPanel = CreatePanel(canvas.transform, "MainMenuPanel");
            RectTransform panelRt = mainMenuPanel.GetComponent<RectTransform>();
            panelRt.anchorMin = Vector2.zero;
            panelRt.anchorMax = Vector2.one;
            panelRt.offsetMin = Vector2.zero;
            panelRt.offsetMax = Vector2.zero;

            // Create logo text (top center)
            Text logoText = CreateText(mainMenuPanel.transform, "LogoText", Constants.Name);
            RectTransform logoTf = logoText.GetComponent<RectTransform>();
            logoTf.anchorMin = new Vector2(0.5f, 0.85f);
            logoTf.anchorMax = new Vector2(0.5f, 0.95f);
            logoTf.pivot = new Vector2(0.5f, 0.5f);
            logoTf.sizeDelta = new Vector2(600, 100);
            logoTf.anchoredPosition = Vector2.zero;
            logoText.fontSize = 72;
            logoText.alignment = TextAnchor.MiddleCenter;
            logoText.color = new Color(1f, 0.8f, 0f); // Orange-yellow
            logoText.fontStyle = FontStyle.Bold;

            // Create version text (below logo)
            Text versionText = CreateText(mainMenuPanel.transform, "VersionText", Constants.Version);
            RectTransform versionTf = versionText.GetComponent<RectTransform>();
            versionTf.anchorMin = new Vector2(0.5f, 0.78f);
            versionTf.anchorMax = new Vector2(0.5f, 0.82f);
            versionTf.pivot = new Vector2(0.5f, 0.5f);
            versionTf.sizeDelta = new Vector2(300, 40);
            versionTf.anchoredPosition = Vector2.zero;
            versionText.fontSize = 20;
            versionText.alignment = TextAnchor.MiddleCenter;
            versionText.color = new Color(0.7f, 0.7f, 0.7f);

            // Create level panel (left side, 8x4 grid)
            GameObject levelPanel = CreatePanel(mainMenuPanel.transform, "LevelPanel");
            RectTransform levelRt = levelPanel.GetComponent<RectTransform>();
            levelRt.anchorMin = new Vector2(0.05f, 0.05f);
            levelRt.anchorMax = new Vector2(0.65f, 0.75f);
            levelRt.offsetMin = Vector2.zero;
            levelRt.offsetMax = Vector2.zero;
            Image levelImg = levelPanel.GetComponent<Image>();
            levelImg.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

            // Add GridLayoutGroup to level panel
            GridLayoutGroup grid = levelPanel.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(80, 80);
            grid.spacing = new Vector2(10, 10);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperLeft;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 8;

            // Create 32 level buttons
            List<Button> levelButtons = new List<Button>();
            for (int i = 1; i <= 32; i++)
            {
                Button levelBtn = CreateButton(levelPanel.transform, "Level" + i + "Button");
                RectTransform btnTf = levelBtn.GetComponent<RectTransform>();
                // Text child already exists
                Text btnText = levelBtn.GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.text = i.ToString();
                    btnText.fontSize = 24;
                    btnText.color = Color.white;
                }
                // Set button colors
                ColorBlock colors = levelBtn.colors;
                colors.normalColor = new Color(0.3f, 0.3f, 0.5f, 1f);
                colors.highlightedColor = new Color(0.4f, 0.4f, 0.7f, 1f);
                colors.pressedColor = new Color(0.2f, 0.2f, 0.4f, 1f);
                colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                levelBtn.colors = colors;
                
                levelButtons.Add(levelBtn);
            }

            // Create side panel (right side, vertical buttons)
            GameObject sidePanel = CreatePanel(mainMenuPanel.transform, "SidePanel");
            RectTransform sideRt = sidePanel.GetComponent<RectTransform>();
            sideRt.anchorMin = new Vector2(0.7f, 0.05f);
            sideRt.anchorMax = new Vector2(0.95f, 0.75f);
            sideRt.offsetMin = Vector2.zero;
            sideRt.offsetMax = Vector2.zero;
            Image sideImg = sidePanel.GetComponent<Image>();
            sideImg.color = new Color(0.2f, 0.2f, 0.25f, 0.8f);

            // Add VerticalLayoutGroup to side panel
            VerticalLayoutGroup vertical = sidePanel.AddComponent<VerticalLayoutGroup>();
            vertical.spacing = 10f;
            vertical.padding = new RectOffset(20, 20, 20, 20);
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            // Create setting/social buttons (8 buttons)
            string[] buttonNames = new string[]
            {
                "Sound",
                "Music",
                "Help",
                "Fullscreen",
                "Tweet",
                "Link",
                "Survivors",
                "PayPal"
            };

            string[] buttonDisplayNames = new string[]
            {
                "Sound",
                "Music",
                "Help",
                "Fullscreen",
                "Tweet",
                "Link",
                "Survivors",
                "PayPal"
            };

            Button[] sideButtons = new Button[buttonNames.Length];
            for (int i = 0; i < buttonNames.Length; i++)
            {
                sideButtons[i] = CreateButton(sidePanel.transform, buttonNames[i] + "Button");
                RectTransform btnTf = sideButtons[i].GetComponent<RectTransform>();
                btnTf.sizeDelta = new Vector2(0, 40); // height 40, width stretched

                Text btnText = sideButtons[i].GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.text = buttonDisplayNames[i];
                    btnText.fontSize = 18;
                    btnText.color = Color.white;
                }

                // Set button colors
                ColorBlock colors = sideButtons[i].colors;
                colors.normalColor = new Color(0.4f, 0.4f, 0.4f, 1f);
                colors.highlightedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                colors.pressedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                sideButtons[i].colors = colors;
            }

            // Create help panel (initially hidden, full screen overlay)
            GameObject helpPanel = CreatePanel(canvas.transform, "HelpPanel");
            helpPanel.SetActive(false);
            RectTransform helpRt = helpPanel.GetComponent<RectTransform>();
            helpRt.anchorMin = Vector2.zero;
            helpRt.anchorMax = Vector2.one;
            helpRt.offsetMin = Vector2.zero;
            helpRt.offsetMax = Vector2.zero;
            Image helpImg = helpPanel.GetComponent<Image>();
            helpImg.color = new Color(0f, 0f, 0f, 0.85f);

            Text helpText = CreateText(helpPanel.transform, "HelpText", 
                "HOW TO PLAY:\n\n" +
                "• Use SPACE or CLICK to jump\n" +
                "• Avoid obstacles and reach the goal\n" +
                "• Complete levels as fast as possible\n\n" +
                "TIP: Timing is everything!");
            RectTransform helpTextTf = helpText.GetComponent<RectTransform>();
            helpTextTf.anchorMin = new Vector2(0.5f, 0.5f);
            helpTextTf.anchorMax = new Vector2(0.5f, 0.5f);
            helpTextTf.pivot = new Vector2(0.5f, 0.5f);
            helpTextTf.sizeDelta = new Vector2(700, 500);
            helpTextTf.anchoredPosition = Vector2.zero;
            helpText.fontSize = 28;
            helpText.alignment = TextAnchor.MiddleCenter;
            helpText.color = Color.white;
            helpText.lineSpacing = 1.2f;

            // Create MenuManager GameObject and attach script
            GameObject menuManagerObj = GameObject.Find("MenuManager");
            if (menuManagerObj == null)
            {
                menuManagerObj = new GameObject("MenuManager");
            }

            MenuManager menuManager = menuManagerObj.GetComponent<MenuManager>();
            if (menuManager == null)
            {
                menuManager = menuManagerObj.AddComponent<MenuManager>();
            }

            // Assign references
            menuManager.mainMenuPanel = mainMenuPanel;
            menuManager.helpPanel = helpPanel;
            menuManager.soundButton = sideButtons[0];
            menuManager.musicButton = sideButtons[1];
            menuManager.helpButton = sideButtons[2];
            menuManager.fullscreenButton = sideButtons[3];
            menuManager.tweetButton = sideButtons[4];
            menuManager.linkButton = sideButtons[5];
            menuManager.survivorsButton = sideButtons[6];
            menuManager.paypalButton = sideButtons[7];
            menuManager.versionText = versionText;
            menuManager.logoText = logoText;
            menuManager.levelButtons = levelButtons;

            // Setup button event listeners
            SetupButtonListeners(menuManager, sideButtons, levelButtons);

            Debug.Log("MenuManager references and event listeners set.");
        }

        private static void CleanupExistingUI()
        {
            // Destroy existing UI GameObjects to prevent duplicates
            string[] uiNames = new string[]
            {
                "MainMenuPanel",
                "LevelPanel",
                "SidePanel",
                "HelpPanel",
                "LogoText",
                "VersionText",
                "MenuManager"
            };
            foreach (string name in uiNames)
            {
                GameObject obj = GameObject.Find(name);
                if (obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }
            // Also destroy level buttons
            for (int i = 1; i <= 32; i++)
            {
                GameObject btn = GameObject.Find("Level" + i + "Button");
                if (btn != null)
                {
                    Object.DestroyImmediate(btn);
                }
            }
        }

        private static void SetupButtonListeners(MenuManager menuManager, Button[] sideButtons, List<Button> levelButtons)
        {
            // Clear existing listeners on side buttons
            foreach (Button btn in sideButtons)
            {
                btn.onClick.RemoveAllListeners();
            }

            // Sound button - instance method
            sideButtons[0].onClick.AddListener(() => menuManager.OnSoundButtonClicked());

            // Music button - instance method
            sideButtons[1].onClick.AddListener(() => menuManager.OnMusicButtonClicked());

            // Help button - instance method
            sideButtons[2].onClick.AddListener(() => menuManager.OnHelpButtonClicked());

            // Fullscreen button - static method
            sideButtons[3].onClick.AddListener(() => MenuManager.OnFullscreenButtonClicked());

            // Tweet button - static method
            sideButtons[4].onClick.AddListener(() => MenuManager.OnTweetButtonClicked());

            // Link button - static method
            sideButtons[5].onClick.AddListener(() => MenuManager.OnLinkButtonClicked());

            // Survivors button - instance method
            sideButtons[6].onClick.AddListener(() => menuManager.OnSurvivorsButtonClicked());

            // PayPal button - static method
            sideButtons[7].onClick.AddListener(() => MenuManager.OnPayPalButtonClicked());

            // Setup level button listeners
            if (levelButtons != null)
            {
                for (int i = 0; i < levelButtons.Count; i++)
                {
                    int level = i + 1;
                    Button btn = levelButtons[i];
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => menuManager.OnLevelButtonClicked(level));
                    }
                }
            }
        }

        private static GameObject CreatePanel(Transform parent, string name)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            Image img = panel.GetComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            return panel;
        }

        private static Text CreateText(Transform parent, string name, string content)
        {
            GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObj.transform.SetParent(parent, false);
            Text text = textObj.GetComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.white;
            return text;
        }

        private static Button CreateButton(Transform parent, string name)
        {
            GameObject buttonObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObj.transform.SetParent(parent, false);

            // Add button text child
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObj.transform.SetParent(buttonObj.transform, false);
            RectTransform textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            Text text = textObj.GetComponent<Text>();
            text.text = name.Replace("Button", "");
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;

            Button button = buttonObj.GetComponent<Button>();
            // Set default colors
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
            button.colors = colors;

            return button;
        }
    }
}