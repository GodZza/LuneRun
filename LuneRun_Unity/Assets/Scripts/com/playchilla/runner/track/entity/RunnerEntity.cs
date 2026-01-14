using shared.algorithm.spatial;
using shared.math;

namespace com.playchilla.runner.track.entity
{
    public class RunnerEntity : SpatialHashValue
    {
        protected Vec3 _pos = new Vec3();
        internal double _radius;
        internal bool _remove = false;

        public RunnerEntity(Vec3Const position, double radius)
            : base(position.x - radius, position.z - radius, position.x + radius, position.z + radius)
        {
            _pos.Copy(position);
            _radius = radius;
        }

        public virtual Vec3Const GetPos()
        {
            return _pos;
        }

        public virtual bool CanRemove()
        {
            return _remove;
        }

        public virtual void Remove()
        {
            _remove = true;
        }

        public virtual void Tick(int deltaTime)
        {
            // Base implementation does nothing
        }
    }
}