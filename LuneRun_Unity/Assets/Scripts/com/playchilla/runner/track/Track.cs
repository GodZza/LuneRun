using System.Collections.Generic;
using UnityEngine;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.track
{
    public class Track
    {
        private List<Segment> _segments = new List<Segment>();
        private Part _connectPart;

        public Part GetConnectPart()
        {
            return _connectPart;
        }

        public Segment GetLastSegment()
        {
            return _segments.Count > 0 ? _segments[_segments.Count - 1] : null;
        }

        public List<Segment> GetSegments()
        {
            return _segments;
        }

        public Segment AddSegment(Segment segment)
        {
            _segments.Add(segment);
            return segment;
        }

        public void RemoveSegment(Segment segment)
        {
            _segments.Remove(segment);
        }

        /// <summary>
        /// Finds the closest part to the given position.
        /// </summary>
        /// <param name="position">Position to check (Vec3).</param>
        /// <returns>The closest part, or null if no parts exist.</returns>
        public Part GetClosestPart(shared.math.Vec3 position)
        {
            if (_segments.Count == 0)
                return null;

            Part closest = null;
            double minDistSqr = double.MaxValue;

            foreach (Segment segment in _segments)
            {
                foreach (Part part in segment.GetParts())
                {
                    double distSqr = position.distanceSqr(part.pos);
                    if (distSqr < minDistSqr)
                    {
                        minDistSqr = distSqr;
                        closest = part;
                    }
                }
            }

            return closest;
        }
    }
}