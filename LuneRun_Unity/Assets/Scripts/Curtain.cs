using UnityEngine;
using System;

namespace LuneRun
{
    public class Curtain : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeTime = 3f;
        [SerializeField] private bool removeOnOpen = false;
        
        private float targetAlpha = 0f;
        private float currentAlpha = 0f;
        private float fadeSpeed = 0f;
        private bool isFading = false;
        private Action onCompleteCallback;
        
        private void Awake()
        {
            // Ensure CanvasGroup component
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            // Set initial state
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        
        private void Start()
        {
            // Start opening curtain
            Open();
        }
        
        private void Update()
        {
            if (!isFading) return;
            
            // Update alpha
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
            canvasGroup.alpha = currentAlpha;
            
            // Check if fade complete
            if (Mathf.Approximately(currentAlpha, targetAlpha))
            {
                isFading = false;
                
                if (targetAlpha == 0f && removeOnOpen)
                {
                    // Remove curtain when fully opened
                    Destroy(gameObject);
                }
                
                // Invoke callback
                onCompleteCallback?.Invoke();
                onCompleteCallback = null;
            }
        }
        
        // Open curtain (fade to transparent)
        public void Open()
        {
            targetAlpha = 0f;
            currentAlpha = canvasGroup.alpha;
            fadeSpeed = Mathf.Abs(currentAlpha - targetAlpha) / fadeTime;
            isFading = true;
            onCompleteCallback = null;
            
            // Disable raycast blocking when opening
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        
        // Open with delay
        public void Open(float delay)
        {
            Invoke(nameof(Open), delay);
        }
        
        // Close curtain (fade to black)
        public void Close(Action onComplete = null)
        {
            targetAlpha = 1f;
            currentAlpha = canvasGroup.alpha;
            fadeSpeed = Mathf.Abs(currentAlpha - targetAlpha) / fadeTime;
            isFading = true;
            onCompleteCallback = onComplete;
            
            // Enable raycast blocking when closing
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        
        // Close with custom fade time
        public void Close(float customFadeTime, Action onComplete = null)
        {
            float previousFadeTime = fadeTime;
            fadeTime = customFadeTime;
            Close(onComplete);
            fadeTime = previousFadeTime;
        }
        
        // Immediate show (no fade)
        public void ShowImmediate()
        {
            isFading = false;
            canvasGroup.alpha = 1f;
            targetAlpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        
        // Immediate hide (no fade)
        public void HideImmediate()
        {
            isFading = false;
            canvasGroup.alpha = 0f;
            targetAlpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        
        // Check if curtain is fully open (transparent)
        public bool IsOpen()
        {
            return canvasGroup.alpha <= 0.01f;
        }
        
        // Check if curtain is fully closed (opaque)
        public bool IsClosed()
        {
            return canvasGroup.alpha >= 0.99f;
        }
        
        // Set fade time
        public void SetFadeTime(float time)
        {
            fadeTime = Mathf.Max(0.1f, time);
        }
        
        // Create a curtain instance
        public static Curtain Create(Transform parent = null, float fadeTime = 3f, bool removeOnOpen = false)
        {
            GameObject curtainObj = new GameObject("Curtain");
            
            if (parent != null)
            {
                curtainObj.transform.SetParent(parent, false);
            }
            
            // Add RectTransform for UI
            RectTransform rect = curtainObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            // Add background image
            UnityEngine.UI.Image bg = curtainObj.AddComponent<UnityEngine.UI.Image>();
            bg.color = Color.black;
            
            Curtain curtain = curtainObj.AddComponent<Curtain>();
            curtain.fadeTime = fadeTime;
            curtain.removeOnOpen = removeOnOpen;
            
            return curtain;
        }
    }
}