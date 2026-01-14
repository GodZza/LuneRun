using System.Collections.Generic;
using shared.math;

namespace shared.algorithm.spatial
{
    public class SpatialHash : IDDACallback
    {
        private double _cellSize;
        private uint _maxBuckets;
        private DDA _dda;
        private Dictionary<uint, List<SpatialHashValue>> _hash = new Dictionary<uint, List<SpatialHashValue>>();
        private uint _timeStamp = 1;
        private List<SpatialHashValue> _rayCastResult;

        public SpatialHash(double cellSize, uint maxBuckets)
        {
            _cellSize = cellSize;
            _maxBuckets = maxBuckets;
            _dda = new DDA(cellSize, cellSize);
        }

        public double GetCellSize()
        {
            return _cellSize;
        }

        public void Add(SpatialHashValue value)
        {
            value.cx1 = (int)(value.x1 / _cellSize);
            value.cy1 = (int)(value.y1 / _cellSize);
            value.cx2 = (int)(value.x2 / _cellSize);
            value.cy2 = (int)(value.y2 / _cellSize);

            for (int cy = value.cy1; cy <= value.cy2; cy++)
            {
                for (int cx = value.cx1; cx <= value.cx2; cx++)
                {
                    AddToBucket(value, cx, cy);
                }
            }
        }

        public void Remove(SpatialHashValue value)
        {
            for (int cy = value.cy1; cy <= value.cy2; cy++)
            {
                for (int cx = value.cx1; cx <= value.cx2; cx++)
                {
                    RemoveFromBucket(value, cx, cy);
                }
            }
        }

        public void Update(SpatialHashValue value)
        {
            // Simplified: remove and re-add
            Remove(value);
            Add(value);
        }

        public List<SpatialHashValue> GetOverlappingXY(double x1, double y1, double x2, double y2)
        {
            var result = new List<SpatialHashValue>();
            int cx1 = (int)(x1 / _cellSize);
            int cy1 = (int)(y1 / _cellSize);
            int cx2 = (int)(x2 / _cellSize);
            int cy2 = (int)(y2 / _cellSize);

            for (int cy = cy1; cy <= cy2; cy++)
            {
                for (int cx = cx1; cx <= cx2; cx++)
                {
                    uint key = GetKey(cx, cy);
                    if (_hash.TryGetValue(key, out var bucket))
                    {
                        foreach (var val in bucket)
                        {
                            if (val.timeStamp >= _timeStamp)
                                continue;
                            val.timeStamp = _timeStamp;
                            if (x1 < val.x2 && x2 > val.x1 && y1 < val.y2 && y2 > val.y1)
                                result.Add(val);
                        }
                    }
                }
            }
            _timeStamp++;
            return result;
        }

        public List<SpatialHashValue> GetOverlapping(SpatialHashValue value)
        {
            return GetOverlappingXY(value.x1, value.y1, value.x2, value.y2);
        }

        public List<SpatialHashValue> RayCast(Vec2 from, Vec2 to)
        {
            _rayCastResult = new List<SpatialHashValue>();
            _dda.Run(from.x, from.y, to.x, to.y, this);
            _timeStamp++;
            return _rayCastResult;
        }

        public bool OnTraverse(int cellX, int cellY)
        {
            uint key = GetKey(cellX, cellY);
            if (_hash.TryGetValue(key, out var bucket))
            {
                foreach (var val in bucket)
                {
                    if (val.timeStamp >= _timeStamp)
                        continue;
                    val.timeStamp = _timeStamp;
                    _rayCastResult.Add(val);
                }
            }
            return true;
        }

        private void AddToBucket(SpatialHashValue value, int cx, int cy)
        {
            uint key = GetKey(cx, cy);
            if (!_hash.TryGetValue(key, out var bucket))
            {
                bucket = new List<SpatialHashValue>();
                _hash[key] = bucket;
            }
            bucket.Add(value);
        }

        private void RemoveFromBucket(SpatialHashValue value, int cx, int cy)
        {
            uint key = GetKey(cx, cy);
            if (_hash.TryGetValue(key, out var bucket))
            {
                bucket.Remove(value);
                if (bucket.Count == 0)
                    _hash.Remove(key);
            }
        }

        private uint GetKey(int cx, int cy)
        {
            return (uint)(cx + cy * _maxBuckets);
        }
    }
}