using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.track.generator
{
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

        public Segment AddForwardSegment(double y, double rotationY, double rotationZ, int parts, int segmentCount, bool addStartPart = true, bool addEndPart = true)
        {
            Part connectPart = _track.GetConnectPart();
            if (connectPart == null)
            {
                Debug.LogError("[SegmentGenerator] connectPart is null! Call _track.SetConnectPart() first.");
                // Create default connect part
                connectPart = new Part(null, new Vec3(0, 0, 0), new Vec3(0, 0, 1), new Vec3(0, 1, 0), new GameObject(), 0, 0);
            }
            _segment = new ForwardSegment("XSegment", y, connectPart, connectPart.dir, rotationY, rotationZ, parts, _materials, segmentCount, addStartPart, addEndPart);
            _track.AddSegment(_segment);
            return _segment;
        }

        public Segment AddHoleSegment(int parts, int segmentCount)
        {
            Part connectPart = _track.GetConnectPart();
            if (connectPart == null)
            {
                Debug.LogError("[SegmentGenerator] connectPart is null! Call _track.SetConnectPart() first.");
                // Create default connect part
                connectPart = new Part(null, new Vec3(0, 0, 0), new Vec3(0, 0, 1), new Vec3(0, 1, 0), new GameObject(), 0, 0);
            }
            _segment = new HoleSegment(connectPart, connectPart.dir, parts, segmentCount);
            _track.AddSegment(_segment);
            return _segment;
        }

        public double GetNextY(double difficulty)
        {
            // Simplified implementation
            Segment lastSegment = _track.GetLastSegment();
            if (lastSegment == null)
                return 0;
            
            Part lastPart = lastSegment.GetLastPart();
            if (lastPart == null)
                return 0;
                
            return lastPart.GetPos().y;
        }

        public Part GetLastSolidPart(Segment segment)
        {
            // Simplified implementation
            return segment?.GetLastPart();
        }
    }
}