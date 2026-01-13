using UnityEngine;

namespace ActionScript
{
    /// <summary>
    /// Mimics Flash KeyboardInput for keyboard state tracking.
    /// </summary>
    public class KeyboardInput
    {
        private readonly bool[] _keyDown = new bool[256];
        private readonly bool[] _keyPressed = new bool[256];
        private readonly bool[] _keyReleased = new bool[256];
        
        public KeyboardInput()
        {
            // Initialize arrays
        }
        
        public void Update()
        {
            // In Flash, this would be called each frame to update key states
            // We'll rely on Unity's Input.GetKey instead
        }
        
        public bool IsDown(int keyCode)
        {
            // Map Flash key codes to Unity KeyCode
            KeyCode unityKey = MapKeyCode(keyCode);
            return Input.GetKey(unityKey);
        }
        
        public bool IsPressed(int keyCode)
        {
            // Key pressed this frame
            KeyCode unityKey = MapKeyCode(keyCode);
            return Input.GetKeyDown(unityKey);
        }
        
        public bool IsReleased(int keyCode)
        {
            // Key released this frame
            KeyCode unityKey = MapKeyCode(keyCode);
            return Input.GetKeyUp(unityKey);
        }
        
        public void SetPress(int keyCode)
        {
            // Simulate key press (used by input system)
            // In Flash, this is called from event handlers
            // We don't need to implement for Unity
        }
        
        public void SetRelease(int keyCode)
        {
            // Simulate key release
        }
        
        public void Reset()
        {
            // Clear all key states
            // Not needed for Unity implementation
        }
        
        private KeyCode MapKeyCode(int flashKeyCode)
        {
            // Map Flash key codes to Unity KeyCode
            // Common Flash key codes (from flash.ui.Keyboard)
            switch (flashKeyCode)
            {
                case 32: return KeyCode.Space; // SPACE
                case 37: return KeyCode.LeftArrow; // LEFT
                case 38: return KeyCode.UpArrow; // UP
                case 39: return KeyCode.RightArrow; // RIGHT
                case 40: return KeyCode.DownArrow; // DOWN
                case 65: return KeyCode.A; // A
                case 68: return KeyCode.D; // D
                case 83: return KeyCode.S; // S
                case 87: return KeyCode.W; // W
                case 16: return KeyCode.LeftShift; // SHIFT
                case 17: return KeyCode.LeftControl; // CONTROL
                case 9: return KeyCode.Tab; // TAB
                case 27: return KeyCode.Escape; // ESCAPE
                case 13: return KeyCode.Return; // ENTER
                default: return (KeyCode)flashKeyCode; // Try direct mapping
            }
        }
    }
}