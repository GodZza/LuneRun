using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    public class StraightGenerator : SegmentGenerator, ISegmentGenerator
    {
        public StraightGenerator(Track track, global::shared.math.Random rnd, Materials materials) 
            : base(track, rnd, materials)
        {
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            // Always can run as fallback
            return true;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            var y = GetNextY(difficulty);
            int parts = 5 + (int)(_rnd.NextDouble() * 10);
            AddForwardSegment(y, 0, 0, parts, segmentCount);
        }
    }
}