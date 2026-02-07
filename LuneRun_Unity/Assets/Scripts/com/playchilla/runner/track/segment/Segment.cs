using System.Collections.Generic;
using UnityEngine;
using shared.debug;
using com.playchilla.runner.track;
using Debug =  shared.debug.Debug;
namespace com.playchilla.runner.track.segment
{

    // ============================================================================
    // Segment - ¹ìµÀ¶Î»ùÀà
    // ============================================================================
    public class Segment
    {
        private List<Part> _parts = new List<Part>();
        private Part _connectPart;
        private Segment _nextSegment;
        private bool _isRemoved = false;
        private string _name;
        private int _levelId = -1;

        public Segment(Part connectPart, string name, int levelId)
        {
            Debug.Assert(connectPart != null, "Segment without connect part.");
            _connectPart = connectPart;
            _name = name;
            _levelId = levelId;
        }

        public void AddPart(Part part)
        {
            var lastPart = GetLastPart();
            if (lastPart == null)
            {
                _connectPart.next = part;
            }
            else
            {
                lastPart.next = part;
                part.previous = lastPart;
            }
            _parts.Add(part);
        }

        public List<Part> GetParts()
        {
            return _parts;
        }

        public Part GetLastPart()
        {
            return _parts.Count > 0 ? _parts[_parts.Count - 1] : null;
        }

        public Part GetConnectPart()
        {
            return _connectPart;
        }

        public Segment GetNextSegment()
        {
            return _nextSegment;
        }

        public Segment GetPreviousSegment()
        {
            return _connectPart != null ? _connectPart.segment : null;
        }

        public Part GetFirstPart()
        {
            return _parts.Count > 0 ? _parts[0] : null;
        }

        public virtual int GetNumberOfParts()
        {
            return _parts.Count;
        }

        public void SetNextSegment(Segment nextSegment)
        {
            Debug.Assert(nextSegment != this, "Trying to set next segment to self.");
            _nextSegment = nextSegment;
        }

        public void Remove()
        {
            _isRemoved = true;
            _nextSegment._connectPart = null;
        }

        public bool IsRemoved()
        {
            return _isRemoved;
        }

        public override string ToString()
        {
            return _name;
        }

        public int GetLevelId()
        {
            return _levelId;
        }
    }
}