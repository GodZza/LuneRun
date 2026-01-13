using UnityEngine;

namespace ActionScript
{
    /// <summary>
    /// Mimics Flash MouseInput for mouse state tracking.
    /// </summary>
    public class MouseInput
    {
        private bool _mouseDown;
        private bool _mousePressed;
        private bool _mouseReleased;
        private Vector2 _position;
        private Vector2 _pressPosition;
        private Vector2 _releasePosition;
        
        public MouseInput()
        {
            Reset();
        }
        
        public void Update()
        {
            // Update mouse states
            bool currentDown = Input.GetMouseButton(0);
            _mousePressed = !_mouseDown && currentDown;
            _mouseReleased = _mouseDown && !currentDown;
            _mouseDown = currentDown;
            
            // Update position
            _position = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }
        
        public bool IsDown()
        {
            return Input.GetMouseButton(0);
        }
        
        public bool HasRelease()
        {
            return Input.GetMouseButtonUp(0);
        }
        
        public void SetPress(Vector2 position)
        {
            _pressPosition = position;
            _mousePressed = true;
            _mouseDown = true;
        }
        
        public void SetRelease(Vector2 position)
        {
            _releasePosition = position;
            _mouseReleased = true;
            _mouseDown = false;
        }
        
        public void SetPosition(Vector2 position)
        {
            _position = position;
        }
        
        public Vector2 GetPosition()
        {
            return _position;
        }
        
        public void Reset()
        {
            _mouseDown = false;
            _mousePressed = false;
            _mouseReleased = false;
            _position = Vector2.zero;
            _pressPosition = Vector2.zero;
            _releasePosition = Vector2.zero;
        }
    }
}