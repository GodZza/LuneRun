using shared.math;

namespace shared.input
{
    public class MouseInput
    {
        private Vec2 _pos = new Vec2();
        private Vec2 _pressPos = new Vec2();
        private bool _hasPos = false;
        private bool _hasPress = false;
        private bool _hasRelease = false;
        private bool _hasWheel = false;
        private bool _isDown = false;

        public MouseInput()
        {
        }

        public void Reset()
        {
            _hasPress = false;
            _hasRelease = false;
            _hasWheel = false;
        }

        public void SetPosition(Vec2Const pos)
        {
            _pos.Copy(pos);
            _hasPos = true;
        }

        public bool HasPos()
        {
            return _hasPos;
        }

        public Vec2Const GetPos()
        {
            // Debug.assert is omitted
            return _pos;
        }

        public void SetPress(Vec2Const pos)
        {
            _pressPos.Copy(pos);
            _hasPress = true;
            _isDown = true;
        }

        public bool HasPress()
        {
            return _hasPress;
        }

        public Vec2Const GetPressPos()
        {
            return _pressPos;
        }

        public void SetRelease(Vec2Const pos)
        {
            _hasRelease = true;
            _isDown = false;
        }

        public bool HasRelease()
        {
            return _hasRelease;
        }

        public void SetWheel(int delta)
        {
            _hasWheel = true;
        }

        public bool HasWheel()
        {
            return _hasWheel;
        }

        public int GetWheelDelta()
        {
            return 0; // placeholder
        }

        public bool IsDown()
        {
            return _isDown;
        }
    }
}