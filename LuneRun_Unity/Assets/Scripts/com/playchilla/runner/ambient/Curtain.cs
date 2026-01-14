using UnityEngine;

namespace com.playchilla.runner.ambient
{
    public class Curtain : MonoBehaviour
    {
        private float _fadeTime;
        private bool _removeOnOpen;
        
        public void Initialize(float fadeTime, float delay, bool removeOnOpen = false)
        {
            _fadeTime = fadeTime;
            _removeOnOpen = removeOnOpen;
            
            // Start fade out after delay
            Invoke("StartFadeOut", delay);
        }
        
        private void StartFadeOut()
        {
            // Use Unity's animation system or coroutine for fading
            // For now, just log
            Debug.Log($"[Curtain] Fading out over {_fadeTime} seconds");
            
            // Simulate completion
            Invoke("OnOpen", _fadeTime);
        }
        
        private void OnOpen()
        {
            if (_removeOnOpen)
            {
                Destroy(gameObject);
            }
        }
        
        public void Close(System.Action callback = null)
        {
            // Fade in
            Debug.Log($"[Curtain] Closing (fading in) over {_fadeTime} seconds");
            
            if (callback != null)
            {
                Invoke(callback.Method.Name, _fadeTime);
            }
        }
    }
}