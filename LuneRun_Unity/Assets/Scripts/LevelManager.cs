using UnityEngine;

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
            this.viewCamera = view;
            this.levelId = levelId;
            this.runnerApi = runnerApi;
            this.settings = settings;
            this.isHardware = isHardware;
            
            // Create player
            GameObject playerObj = new GameObject("Player");
            playerController = playerObj.AddComponent<PlayerController>();
            playerController.Initialize(settings);
            
            // Generate track
            trackGenerator = GetComponent<TrackGenerator>();
            if (trackGenerator == null)
            {
                trackGenerator = gameObject.AddComponent<TrackGenerator>();
            }
            trackGenerator.GenerateTrack(levelId);
            
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
                playerController.UpdatePlayer();
                
                // Update player distance along track (simplified using Z coordinate)
                playerDistance = Mathf.Max(playerDistance, playerController.transform.position.z);
                
                // Check for failure: player fell off track
                if (playerController.transform.position.y < -10f)
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
            // TODO: Implement highscore UI
            Debug.Log("Show highscore for level " + levelId);
        }
        
        private void HideHighscore()
        {
            // TODO: Hide highscore UI
        }
        
        // Called when level is completed (e.g., player reaches end)
        public void CompleteLevel()
        {
            hasCompleted = true;
            // Show curtain effect
            // TODO: Implement curtain
            shouldExit = true;
        }
        
        // Called when player fails (e.g., falls off track)
        public void FailLevel()
        {
            hasFailed = true;
            // Restart level
            // TODO: Implement restart logic
            Debug.Log("Level failed, restarting");
            hasFailed = false;
            // Reset player position
            playerController?.Reset();
        }
    }
}