using UnityEngine;

namespace ActionScript
{
    /// <summary>
    /// Mimics Flash Stage for stage properties and display state.
    /// </summary>
    public class Stage : DisplayObject
    {
        private static Stage _instance;
        public static Stage Instance => _instance ??= new Stage();
        
        public float stageWidth => Screen.width;
        public float stageHeight => Screen.height;
        
        public string displayState
        {
            get => Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? "fullScreen" : "normal";
            set
            {
                if (value == "fullScreen")
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                else
                    Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }
        
        public string align { get; set; } = "topLeft";
        public string scaleMode { get; set; } = "noScale";
        
        public float mouseX => Input.mousePosition.x;
        public float mouseY => Screen.height - Input.mousePosition.y; // Flash uses top-left origin
        
        private Stage() : base(null)
        {
            // Stage doesn't need a GameObject, it's a conceptual representation
        }
        
        public new void AddEventListener(string type, System.Action<Event> listener)
        {
            base.AddEventListener(type, listener);
        }
        
        public new void RemoveEventListener(string type, System.Action<Event> listener)
        {
            base.RemoveEventListener(type, listener);
        }
        
        public new bool HasEventListener(string type)
        {
            return base.HasEventListener(type);
        }
        
        // Static convenience methods for global stage access
        public static float GetStageWidth() => Instance.stageWidth;
        public static float GetStageHeight() => Instance.stageHeight;
        public static void SetFullScreen(bool fullscreen)
        {
            Instance.displayState = fullscreen ? "fullScreen" : "normal";
        }
    }
}