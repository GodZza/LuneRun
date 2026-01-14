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
    }
}