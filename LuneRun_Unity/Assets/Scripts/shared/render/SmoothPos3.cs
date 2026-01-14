using shared.math;

namespace shared.render
{
    public class SmoothPos3
    {
        private Vec3 _beginPos = new Vec3();
        private Vec3 _endPos = new Vec3();
        private Vec3 _dir = new Vec3();
        private Vec3 _temp = new Vec3();
        private int _lastTick = -1;

        public SmoothPos3(Vec3Const startPos)
        {
            _endPos.copy(startPos);
        }

        public Vec3 GetPos(Vec3Const targetPos, int currentTick, double interpolation)
        {
            if (_lastTick != currentTick)
            {
                _beginPos.copy(_endPos);
                _endPos.copy(targetPos);
                _dir.copy(_endPos);
                _dir.subSelf(_beginPos);
                _lastTick = currentTick;
            }
            _temp.copy(_dir);
            _temp.scaleSelf(interpolation);
            _temp.addSelf(_beginPos);
            return _temp;
        }
    }
}