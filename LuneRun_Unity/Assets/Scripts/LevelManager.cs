using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using com.playchilla.gameapi.api;
using com.playchilla.runner;
using com.playchilla.runner.track.entity;
using com.playchilla.runner.player;
using com.playchilla.runner.api;
using shared.math;

namespace LuneRun
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField] private Transform trackParent;
        
        private int levelId;
        private bool isHardware;
        private Settings settings;
        private com.playchilla.runner.api.IRunnerApi runnerApi;
        
        // Game state
        private bool hasFailed = false;
        private bool hasCompleted = false;
        private bool shouldExit = false;
        private float playerDistance = 0f;
        
        // References
        private PlayerController playerController;
        private TrackGenerator trackGenerator;
        private Level level;
        
        public void Initialize(Camera view, int levelId, IRunnerApi runnerApi, Settings settings, bool isHardware)
        {
            Debug.Log($"[LuneRun] LevelManager.Initialize - level {levelId}, camera {view}");
            this.viewCamera = view;
            this.levelId = levelId;
            this.runnerApi = runnerApi;
            this.settings = settings;
            this.isHardware = isHardware;
            
            // Create level instance first (manages world and entities)
            GameObject levelObj = new GameObject("Level");
            level = levelObj.AddComponent<Level>();
            level.Initialize(levelId, isHardware, settings, runnerApi);
            
            // Generate track with Flash track reference
            trackGenerator = GetComponent<TrackGenerator>();
            if (trackGenerator == null)
            {
                Debug.Log("[LuneRun] Adding TrackGenerator component");
                trackGenerator = gameObject.AddComponent<TrackGenerator>();
            }
            trackGenerator.GenerateTrack(levelId, level.GetTrack());
            
            // Create player and place on track start
            GameObject playerObj = new GameObject("Player");
            playerController = playerObj.AddComponent<PlayerController>();
            playerController.Initialize(settings);
            playerController.SetTrackGenerator(trackGenerator);
            playerController.SetWorld(level.GetWorld());
            
            // Place player at track start with slight offset above surface
            playerController.transform.position = new Vector3(0, 0.5f, 0);
            
            // Position camera
            if (viewCamera != null)
            {
                viewCamera.transform.position = new Vector3(0, 10, -20);
                viewCamera.transform.LookAt(Vector3.zero);
            }
        }
        
        public void UpdateLevel()
        {
            if (playerController != null)
            {
                // Update track direction based on player's current position
                if (trackGenerator != null)
                {
                    float distanceAlongTrack = playerController.transform.position.z;
                    Vector3 trackDir = trackGenerator.GetDirectionAtDistance(distanceAlongTrack);
                    playerController.SetTrackDirection(trackDir);
                }
                
                playerController.UpdatePlayer();
                
                // Synchronize playerController state to Level's Player instance for arm animation
                if (level != null)
                {
                    Player levelPlayer = level.GetPlayer();
                    if (levelPlayer != null)
                    {
                        // Update position
                        Vector3 controllerPos = playerController.transform.position;
                        levelPlayer.SetPosition(new Vec3(controllerPos.x, controllerPos.y, controllerPos.z));
                        
                        // Update velocity
                        Vector3 controllerVel = playerController.GetForwardDir() * playerController.GetSpeed();
                        levelPlayer.SetVelocity(new Vec3(controllerVel.x, controllerVel.y, controllerVel.z));
                        
                        // Update ground state
                        levelPlayer.SetOnGround(playerController.IsOnGround());
                        
                        // Update current part from track
                        if (level.GetTrack() != null)
                        {
                            com.playchilla.runner.track.Part closestPart = level.GetTrack().GetClosestPart(
                                new Vec3(controllerPos.x, controllerPos.y, controllerPos.z));
                            if (closestPart != null)
                            {
                                levelPlayer.SetCurrentPart(closestPart);
                            }
                        }
                    }
                }
                
                // Update player distance along track (simplified using Z coordinate)
                playerDistance = Mathf.Max(playerDistance, playerController.transform.position.z);
                
                // Check for failure: player fell off track
                if (playerController.transform.position.y < -50f)
                {
                    FailLevel();
                }
                
                // Check for completion: reached end of track (skip for infinite mode)
                if (trackGenerator != null && playerDistance >= trackGenerator.GetTotalLength() && levelId != Constants.Tab3InfiniteLevelId)
                {
                    CompleteLevel();
                }
            }
            
            // Handle input for highscore display
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ShowHighscore();
            }
        }
        
        public void RenderLevel(float tick, float alpha)
        {
            // In Unity, rendering is automatic
            // We can update interpolated positions here if needed
            if (playerController != null)
            {
                playerController.Interpolate(alpha);
            }
        }
        
        public bool ShouldExit()
        {
            return shouldExit;
        }
        
        public bool IsCompleted()
        {
            return hasCompleted;
        }
        
        public int GetLevelId()
        {
            return levelId;
        }
        
        public bool HasFailed()
        {
            return hasFailed;
        }
        
        public bool HasCompleted()
        {
            return hasCompleted;
        }
        
        public void DestroyLevel()
        {
            // Clean up
            if (playerController != null)
            {
                Destroy(playerController.gameObject);
            }
            if (trackGenerator != null)
            {
                trackGenerator.ClearTrack();
            }
            if (level != null && level.gameObject != null)
            {
                Destroy(level.gameObject);
            }
        }
        
        private void ShowHighscore()
        {
            // Implement highscore UI using HighscoreManager
            if (HighscoreManager.Instance != null && runnerApi != null)
            {
                HighscoreManager.Instance.ShowLevelHighscore(
                    levelId: levelId,
                    userId: runnerApi.GetUserId(),
                    runnerApi: runnerApi,
                    onClose: () =>
                    {
                        Debug.Log("Highscore panel closed");
                    },
                    actionButtonLabel: "Continue",
                    onAction: () =>
                    {
                        HideHighscore();
                    },
                    isLastUnlocked: false
                );
            }
            else
            {
                Debug.LogWarning("Cannot show highscore: HighscoreManager or runnerApi not available");
            }
        }
        
        private void HideHighscore()
        {
            // Hide highscore UI
            if (HighscoreManager.Instance != null)
            {
                HighscoreManager.Instance.HideHighscore();
            }
        }
        
        // Called when level is completed (e.g., player reaches end)
        public void CompleteLevel()
        {
            hasCompleted = true;
            
            // Show curtain effect
            ShowCurtain();
            
            // Submit score to API
            SubmitScore();
            
            // Exit level after curtain animation
            StartCoroutine(ExitAfterCurtain());
        }
        
        private void ShowCurtain()
        {
            // Create a simple curtain effect (black screen fade in)
            GameObject curtain = new GameObject("Curtain", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            
            // Find or create canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            
            curtain.transform.SetParent(canvas.transform, false);
            RectTransform rt = curtain.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            
            Image img = curtain.GetComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0f); // Start transparent
            
            // Animate fade in
            StartCoroutine(FadeCurtain(img));
        }
        
        private System.Collections.IEnumerator FadeCurtain(Image curtainImage)
        {
            float duration = 1.5f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsed / duration);
                curtainImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }
            
            // Destroy curtain after animation
            Destroy(curtainImage.gameObject);
        }
        
        private System.Collections.IEnumerator ExitAfterCurtain()
        {
            yield return new WaitForSeconds(2f); // Wait for curtain animation
            shouldExit = true;
        }
        
        private void SubmitScore()
        {
            if (runnerApi != null && playerDistance > 0)
            {
                // Calculate score based on distance/time (simplified)
                float score = playerDistance; // In original game, score is time in seconds
                
                // Submit score asynchronously
                runnerApi.Score(levelId, score, new ScoreCallback((success) =>
                {
                    Debug.Log($"Score submission {(success ? "succeeded" : "failed")}");
                }));
            }
        }
        
        // Called when player fails (e.g., falls off track)
        public void FailLevel()
        {
            // Prevent multiple consecutive failures
            if (hasFailed) return;
            
            hasFailed = true;
            
            // Restart level with delay
            Debug.Log("Level failed, restarting in 1 second");
            if (playerController != null)
            {
                Debug.Log($"Player position: {playerController.transform.position}, grounded: {playerController.IsOnGround()}");
            }
            
            // Reset player position immediately
            playerController?.Reset();
            
            // Clear and regenerate track
            if (trackGenerator != null)
            {
                trackGenerator.ClearTrack();
                trackGenerator.GenerateTrack(levelId);
            }
            
            // Reset game state after a short delay
            StartCoroutine(DelayedReset());
        }
        
        private System.Collections.IEnumerator DelayedReset()
        {
            yield return new WaitForSeconds(1f);
            hasFailed = false;
            Debug.Log("Level restarted");
        }
        
        // Nested class to implement IScoreCallback
        private class ScoreCallback : IScoreCallback
        {
            private Action<bool> _callback;
            
            public ScoreCallback(Action<bool> callback)
            {
                _callback = callback;
            }
            
            public void Score(Score score, bool isNewHighscore)
            {
                _callback?.Invoke(true);
            }
            
            public void ScoreError(ErrorResponse error)
            {
                Debug.LogError($"Score submission error: {error}");
                _callback?.Invoke(false);
            }
        }
    }
}