using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;
using com.playchilla.runner.track.segment;
using System;

namespace com.playchilla.runner.track.generator
{
    // ============================================================================
    // SegmentGenerator - 轨道段生成器基类
    // ============================================================================
    public class SegmentGenerator
    {
        protected Track _track;
        protected global::shared.math.Random _rnd;
        protected Materials _materials;
        protected Segment _segment; // 存储最后生成的段，供TrackGenerator访问

        public SegmentGenerator(Track track, global::shared.math.Random rnd, Materials materials)
        {
            _track = track;
            _rnd = rnd;
            _materials = materials;
        }

        public Segment AddForwardSegment(float y, float rotationY, float rotationZ, int parts, int segmentCount, bool addStartPart = true, bool addEndPart = true)
        {
            var connectPart = _track.GetConnectPart();
            _segment = new ForwardSegment("XSegment", y, connectPart, connectPart.dir, rotationY, rotationZ, parts, _materials, segmentCount, addStartPart, addEndPart);
            return _track.AddSegment(_segment);
        }

        public Segment AddHoleSegment(int parts, int segmentCount)
        {
            var connectPart = _track.GetConnectPart();
            _segment = new HoleSegment(connectPart, connectPart.dir, parts, segmentCount);
            return _track.AddSegment(_segment);
        }

        public float GetNextY(double difficulty)
        {
            var lastSegment = _track.GetLastSegment();
            var lastPart = lastSegment.GetLastPart();

            if(lastSegment is not HoleSegment)
            {
                return lastPart.GetPos().y;
            }

            // If last segment is a HoleSegment, calculate Y position based on hole depth
            var lastSolidPart = GetLastSolidPart(lastSegment);
            var solidY = lastSolidPart.GetPos().y;
            //var loc5 = lastPart.GetPos();
            var parts = lastSegment.GetNumberOfParts();


            var targetY = Math.Min(195, solidY + (1 - 1.0 * parts / 40) * 25);
            var progress = Math.Min(parts / 70f, 1);
            var minY = Math.Max(50, targetY - targetY * 2 * progress);
            var y = minY + _rnd.NextDouble() * (targetY - minY);
            return (float)y;
        }

        public Part GetLastSolidPart(Segment segment)
        {

            while (segment is HoleSegment)
            {
                segment = segment.GetPreviousSegment();
            }
            return segment?.GetLastPart();
        }
    }
}