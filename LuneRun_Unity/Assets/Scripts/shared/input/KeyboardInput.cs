using System.Collections.Generic;
using UnityEngine;

namespace shared.input
{
    public class KeyboardInput
    {
        private List<int> _pressed = new ();
        private List<int> _released = new ();
        private Dictionary<int, bool> _isDown = new Dictionary<int, bool>();

        public KeyboardInput()
        {
        }

        public void Reset()
        {
            _pressed.Clear();
            _released.Clear();
        }
        public void SetPress(KeyCode keyCode) => SetPress((int)keyCode);
        public void SetPress(int keyCode)
        {
            _pressed.Add(keyCode);
            _isDown[keyCode] = true;
        }
        public void SetRelease(KeyCode keyCode) => SetRelease((int)keyCode);
        public void SetRelease(int keyCode)
        {
            _released.Add(keyCode);
            _isDown[keyCode] = false;
        }

        public bool IsDown(int keyCode)
        {
            return _isDown.ContainsKey(keyCode) && _isDown[keyCode];
        }

        public bool IsPressed(int keyCode)
        {
            return _pressed.Contains(keyCode);
        }

        public bool IsReleased(int keyCode)
        {
            return _released.Contains(keyCode);
        }

        public bool IsPressed(KeyCode keyCode) => IsReleased((int)keyCode);

        public List<int> GetPressed()
        {
            return _pressed;
        }
    }
}