using System.Collections.Generic;
using UnityEngine;
using com.playchilla.runner.track.segment;
using shared.math;
using System.Linq;
using System.IO;

namespace com.playchilla.runner.track
{
    public class TrackView : MonoBehaviour 
    {
        public TrackView Initialize(Track track)
        {
            _track = track;
            return this;
        }
        private List<Segment> _segments = new List<Segment>();
        private Track _track;

        private void Update()
        {
            var wasModified = false;
            // 第一遍循环：检查并移除标记为删除的Segment
            foreach (var segment in _segments)
            {
                if(segment == null) continue;
                if(segment.IsRemoved())
                {
                    Remove(segment);
                    wasModified = true;
                    continue;
                }
                if(segment.GetNextSegment() is not HoleSegment hole)
                {
                    continue;
                }
                //hole.GetLastPart().mesh.visible = false;
            }

            // 第二遍循环：添加新出现的Segment
            foreach (var segment in _track.GetSegments())
            {
                if (segment == null) continue;
                if (_segments.Contains(segment)) continue;

                Add(segment);
                wasModified = true;
            }

            if (wasModified)
            {
                _segments.Clear();
                _segments.AddRange(_track.GetSegments());
            }

        }

        public void Remove(Segment segment)
        {
            if (segment == null) return;
            foreach (var part in segment.GetParts())
            {
                if (part == null) continue;
                if(_dict.TryGetValue(part, out var mesh))
                {
                    // TODO: 删除mesh 
                }
            }
        }

        public void Add(Segment segment)
        {
            if (segment == null) return;
            foreach (var part in segment.GetParts())
            {
                if (part == null) continue;

                // TODO: 这里动态创建mesh
                Mesh mesh = null;
                _dict.Add(part, mesh);
            }
        }

        private Dictionary<Part, Mesh> _dict = new(100);
    }
}