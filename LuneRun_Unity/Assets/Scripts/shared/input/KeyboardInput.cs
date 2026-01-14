using System.Collections.Generic;

namespace shared.input
{
    public class KeyboardInput
    {
        private List<uint> _pressed = new List<uint>();
        private List<int> _released = new List<int>();
        private Dictionary<int, bool> _isDown = new Dictionary<int, bool>();

        public KeyboardInput()
        {
        }

        public void Reset()
        {
            _pressed.Clear();
            _released.Clear();
        }

        public void SetPress(uint keyCode)
        {
            _pressed.Add(keyCode);
            _isDown[(int)keyCode] = true;
        }

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
            return _pressed.Contains((uint)keyCode);
        }

        public bool IsReleased(int keyCode)
        {
            return _released.Contains(keyCode);
        }

        public List<uint> GetPressed()
        {
            return _pressed;
        }
    }
}