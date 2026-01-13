using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LuneRun
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField] private Transform trackParent;
        
        private int levelId;
        private bool isHardware;
        private Settings settings;
        private IRunnerApi runnerApi;
        
        // Game state
        private bool hasFailed = false;
        private bool hasCompleted = false;
        private bool shouldExit = false;
        private float playerDistance = 0f;
        
        // References
        private PlayerController playerController;
        private TrackGenerator trackGenerator;
        
        public void Initialize(Camera view, int levelId, IRunnerApi runnerApi, Settings settings, bool isHardware)
        {
            Debug.Log($"[LuneRun] LevelManager.Initialize - level {levelId}, camera {view}");
            this.viewCamera = view;
            this.levelId = levelId;
            this.runnerApi = runnerApi;
            this.settings = settings;
            this.isHardware = isHardware;
            
            // Generate track first
            trackGenerator = GetComponent<TrackGenerator>();
            if (trackGenerator == null)
            {
                Debug.Log("[LuneRun] Adding TrackGenerator component");
                trackGenerator = gameObject.AddComponent<TrackGenerator>();
            }
            trackGenerator.GenerateTrack(levelId);
            
            // Create player and place on track start
            GameObject playerObj = new GameObject("Player");
            playerController = playerObj.AddComponent<PlayerController>();
            playerController.Initialize(settings);
            
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
                
                // Update player distance along track (simplified using Z coordinate)
                playerDistance = Mathf.Max(playerDistance, playerController.transform.position.z);
                
                // Check for failure: player fell off track
                if (playerController.transform.position.y < -50f)
                {
                    FailLevel();
                }
                
                // Check for completion: reached end of track
                if (trackGenerator != null && playerDistance >= trackGenerator.GetTotalLength())
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
                runnerApi.SubmitScore(levelId, score, (success) =>
                {
                    Debug.Log($"Score submission {(success ? "succeeded" : "failed")}");
                });
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
    }
}