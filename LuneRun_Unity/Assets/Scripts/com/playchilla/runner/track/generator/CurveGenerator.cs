using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    public class CurveGenerator : SegmentGenerator, ISegmentGenerator
    {
        public CurveGenerator(Track track, global::shared.math.Random rnd, Materials materials) 
            : base(track, rnd, materials)
        {
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (difficulty < 0.07)
                return false;

            if (_rnd.NextDouble() > 0.4 + 0.45 * difficulty)
                return false;

            return true;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            var direction = (2 * _rnd.NextDouble() - 1);
            var rotationY = (0.2 + difficulty) * direction * 180;
            int parts = 10 + (int)(10 * Mathf.Abs((float)direction)) + (int)(_rnd.NextDouble() * (15 - 10 * difficulty));
            var y = GetNextY(difficulty);
            
            AddForwardSegment(y, 0, 0, 2, segmentCount);
            AddForwardSegment(y, (float)rotationY, 0, parts, segmentCount);
        }
    }
}