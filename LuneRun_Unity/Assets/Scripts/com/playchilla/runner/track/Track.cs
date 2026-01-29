using System.Collections.Generic;
using UnityEngine;
using com.playchilla.runner.track.segment;
using shared.math;
using System.Linq;

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

        public Vec3 GetStartPos()
        {
            if (_segments.Count > 0)
            {
                return _segments[0].GetConnectPart().GetPos();
            }
            return new Vec3(0, 0, 0);
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

            // 更新 connectPart 为新段的终点（LastPart），供下一次生成使用
            if (segment != null)
            {
                Part lastPart = segment.GetLastPart();
                if (lastPart != null)
                {
                    _connectPart = lastPart;
                }
            }

            return segment;
        }

        public void RemoveSegment(Segment segment)
        {
            _segments.Remove(segment);
        }

        public void SetConnectPart(Part part)
        {
            _connectPart = part;
        }

        /// <summary>
        /// Finds the closest part to the given position.
        /// </summary>
        /// <param name="position">Position to check (Vec3).</param>
        /// <returns>The closest part, or null if no parts exist.</returns>
        public Part GetClosestPart(shared.math.Vec3 position)
        {
            if (_segments.Count == 0)
            {
                Debug.LogWarning($"[Track.GetClosestPart] No segments in track");
                return null;
            }

            // 使用 LINQ 的 SelectMany 扁平化嵌套集合，然后通过 OrderBy 按距离排序
            var closestPart = _segments
                .SelectMany(segment => segment.GetParts()) // 将嵌套集合展平
                .Select(part => new { Part = part, DistanceSqr = position.distanceSqr(part.GetPos()) }) // 投影包含距离信息
                .OrderBy(x => x.DistanceSqr) // 按平方距离升序排列
                .FirstOrDefault(); // 取第一个（距离最近的）


            return closestPart?.Part;
        }
    }
}