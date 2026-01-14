using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.track.entity
{
    public class SpeedEntity : RunnerEntity
    {
        private Level _level;
        private Segment _boundToSegment;
        private double _baseY;

        public SpeedEntity(Vec3Const position, Level level, Segment segment) 
            : base(position, 10)
        {
            // Debug.Assert(segment != null, "Trying to bind entity to null segment.");
            _level = level;
            _boundToSegment = segment;
            _baseY = position.y;
        }

        public override void Tick(int deltaTime)
        {
            // Update position with sine wave motion
            _pos.y = _baseY + Mathf.Abs(Mathf.Sin(deltaTime * 0.2f) * 30);
        }

        public override bool CanRemove()
        {
            return base.CanRemove() || (_boundToSegment != null && _boundToSegment.IsRemoved());
        }
    }
}