using System.Collections.Generic;
using UnityEngine;
using com.playchilla.runner.track.segment;
using shared.math;
using System.Linq;
using System.ComponentModel.Design;
using UnityEngine.Assertions;

namespace com.playchilla.runner.track
{
    public class Track
    {

        public List<Segment> GetSegments()
        {
            return _segments;
        }

        public Part GetStartPart()
        {
            return _segments.First().GetFirstPart();
        }
        public int GetTotalParts()
        {
            return _totalParts;
        }

        public Segment GetFirstSegment()
        {
            return _segments.First();
        }
        public Segment GetLastSegment()
        {
            return _segments.Last();
        }
        

        public Vec3 GetStartPos()
        {
            return GetFirstSegment().GetConnectPart().GetPos();
        }
        public Part GetConnectPart()
        {
            return GetLastSegment().GetLastPart();
        }



        public Segment AddSegment(Segment segment)
        {
            if (_segments.Count > 0)
            {
                _segments.Last().SetNextSegment(segment);
            }
            _segments.Add(segment);

            //foreach (var part in segment.GetParts())
            //{
            //    // TODO: 添加到物理世界,用于碰撞
            //    this._partGrid.add(part);
            //}

            _totalParts += segment.GetParts().Count;
            return segment;
        }

        public void RemoveSegment(Segment segment) // 
        {
            var index = _segments.IndexOf(segment);
            if(index < 0)
            {
                UnityEngine.Debug.LogWarning("Trying to remove a segment that doesn\'t exist.");
                return;
            }

            for (var i = 0; i <= index; i++)
            {
                var s = _segments[i];

                //foreach(var part in s.GetParts())
                //{
                //    // TODO: 从物理世界移除
                //    this._partGrid.remove(part);
                //}

                s.Remove();
                _totalParts -= s.GetParts().Count;

                if (_totalParts < 0)
                {
                    UnityEngine.Debug.LogWarning("Negative number of total parts after segment removal.");
                    return;
                }

            }
            _segments.Remove(segment);
        }


        /// <summary>
        /// Finds the closest part to the given position.
        /// </summary>
        /// <param name="position">Position to check (Vec3).</param>
        /// <returns>The closest part, or null if no parts exist.</returns>
        public Part GetClosestPart(Vector3 position)
        {
            //var x = position.x;
            //var z = position.z;
            //var radius = Part.Length * 4;
            //var hitParts = GetOverlappingXY(x - radius, z - radius, x + radius, z + radius);
            //var minDis  = float.MaxValue; // 最接近的距离
            //var minPart = default(Part);  // 最接近的Part
            //foreach (var part in hitParts)
            //{
            //    var partPos =part.GetPos();
            //    var partDis = Vector3.Distance(partPos, position);
            //    if(partDis < minDis)
            //    {
            //        minDis = partDis;
            //        minPart = part;
            //    }
            //}
            //return minPart;

            var x = position.x;
            var z = position.z;
            var radius = Part.Length * 4;

            var closestPart = GetOverlappingXY(x - radius, z - radius, x + radius, z + radius)
                .Select(part => new { Part = part, Distance = Vector3.Distance(position, part.GetPos()) }) // 投影包含距离信息
                .OrderBy(x => x.Distance) // 按平方距离升序排列
                .FirstOrDefault(); // 取第一个（距离最近的）
            return closestPart?.Part;
        }

        public bool IsIntersectingXZ(int centerX, int centerZ, float radius)
        {
            //return this._partGrid.getOverlappingXY(arg1 - arg3, arg2 - arg3, arg1 + arg3, arg2 + arg3).length > 0;
            return GetOverlappingXY(centerX - radius, centerZ - radius, centerX + radius, centerZ + radius).Any();
        }

        /// <summary>
        /// 在指定部件(Part)的特定段(Segment)中，尝试在其某个子部件的垂直方向随机偏移位置生成一个不重叠的新位置。
        /// 该方法会沿着Segment链进行查找，最多尝试40次或直到找到合适位置。
        /// </summary>
        /// <param name="part">作为位置计算基准的部件</param>
        /// <param name="startIndex">在Segment中的起始部件索引</param>
        /// <returns>如果找到合适位置则返回Vector3坐标，否则返回null</returns>
        public Vector3? GetRandomClosePos(Part part, int startIndex)
        {
            var currentSegment = part.segment;
            startIndex = startIndex + part.partIndex;

            var attemptCount = 0; // 尝试次数计数器，防止无限循环
            while (currentSegment != null && attemptCount < 40)
            {
                var parts = currentSegment.GetParts();
                if (startIndex < parts.Count)
                {
                    var pos = parts[startIndex].GetPos();
                    var dir = parts[startIndex].dir;
                    var cro = Vector3.Cross(dir, Vector3.up);

                    var randomOffset = UnityEngine.Random.value * 200 - 100;
                    if (randomOffset > 0)
                    {
                        randomOffset += 25;
                    }
                    else
                    {
                        randomOffset -= 25;
                    }

                    // 应用随机偏移到垂直向量，并叠加到基础位置
                    cro *= randomOffset;
                    cro += pos;

                    // 检查新生成的位置是否与现有部件重叠（10单位范围内）
                    if (this.GetOverlappingXY(cro.x - 10, cro.z - 10, cro.x + 10, cro.z + 10).Any())
                    {
                        return cro;// 无重叠，返回有效位置
                    }
                    return null;// 有重叠，返回null
                }

                startIndex -= parts.Count;
                currentSegment = currentSegment.GetNextSegment();
                ++attemptCount;
            }

            return null;
        }

        /// <summary>
        /// 寻找屏幕范围内 的Part
        /// 传入来的y 其实是z。 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public IEnumerable<Part> GetOverlappingXY(float x1, float y1, float x2, float y2)
        {
            foreach (var segment in _segments)
            {
                foreach(var part in segment.GetParts())
                {
                    var pos = part.GetPos();
                    if(pos.x >= x1 && pos.x <= x2 && pos.y >= y1 && pos.y <= y2)
                        yield return part;
                }
            }
        }

        private int _totalParts;
        private List<Segment> _segments = new List<Segment>();

    }
}