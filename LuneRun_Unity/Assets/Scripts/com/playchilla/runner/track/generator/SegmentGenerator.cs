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
            Segment lastSegment = _track.GetLastSegment();
            if (lastSegment == null)
                return 0;

            Part lastPart = lastSegment.GetLastPart();
            if (lastPart == null)
                return 0;

            // If last segment is a HoleSegment, calculate Y position based on hole depth
            if (lastSegment is HoleSegment)
            {
                Part lastSolidPart = GetLastSolidPart(lastSegment);
                if (lastSolidPart != null)
                {
                    double solidY = lastSolidPart.GetPos().y;
                    int numParts = lastSegment.GetNumberOfParts();

                    // Calculate target Y based on hole position
                    double targetY = solidY + 25; // Base rise after hole
                    targetY = UnityEngine.Mathf.Min(195, (float)targetY); // Cap at 195

                    double progress = UnityEngine.Mathf.Min(numParts / 70.0f, 1.0f);
                    double minY = UnityEngine.Mathf.Max(50, (float)(targetY - targetY * 2 * progress));

                    // Return random Y between minY and targetY
                    return minY + _rnd.NextDouble() * (targetY - minY);
                }
            }

            return lastPart.GetPos().y;
        }

        public Part GetLastSolidPart(Segment segment)
        {
            if (segment == null)
                return null;

            // If this is not a HoleSegment, return its last part
            if (!(segment is HoleSegment))
                return segment.GetLastPart();

            // Otherwise, traverse backwards to find the last solid segment
            Segment current = segment.GetPreviousSegment();
            while (current != null)
            {
                if (!(current is HoleSegment))
                    return current.GetLastPart();

                current = current.GetPreviousSegment();
            }

            return null;
        }
    }
}