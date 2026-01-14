using UnityEngine;

namespace com.playchilla.runner.highscore
{
    public class HighscoreGui : MonoBehaviour
    {
        private int _myUserId;
        private System.Action _onClose;
        private System.Action _onPlay;
        private string _titleText;
        private com.playchilla.runner.api.Highscore _highscore;

        // TODO: Implement UI elements

        public void Initialize(int myUserId, System.Action onClose, string playCaption, System.Action onPlay)
        {
            _myUserId = myUserId;
            _onClose = onClose;
            _onPlay = onPlay;
            _titleText = "Loading...";
            // Setup UI
        }

        public void Tick(int deltaTime)
        {
            // Handle dragging etc.
        }

        public void Setup(string title, com.playchilla.runner.api.Highscore highscore)
        {
            _titleText = title;
            _highscore = highscore;
            // Update UI
        }

        public void SetError(string message)
        {
            // Show error
        }

        public com.playchilla.runner.api.Highscore GetHighscoreData()
        {
            return _highscore;
        }

        public void Destroy()
        {
            // Cleanup
        }
    }
}