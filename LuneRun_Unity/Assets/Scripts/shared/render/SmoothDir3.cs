using shared.math;

namespace shared.render
{
    public class SmoothDir3
    {
        private Vec3 _beginDir = new Vec3();
        private Vec3 _endDir = new Vec3();
        private int _lastTick = -1;

        public SmoothDir3()
        {
        }

        public Vec3 GetDir(Vec3Const targetDir, int tick, double alpha)
        {
            if (_lastTick != tick)
            {
                _beginDir.copy(_endDir);
                _endDir.copy(targetDir);
                _lastTick = tick;
            }
            return _beginDir.lerp(_endDir, alpha);
        }
    }
}