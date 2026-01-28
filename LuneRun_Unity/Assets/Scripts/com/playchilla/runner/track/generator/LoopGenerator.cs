using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    public class LoopGenerator : SegmentGenerator, ISegmentGenerator
    {
        public LoopGenerator(Track track, global::shared.math.Random rnd, Materials materials) 
            : base(track, rnd, materials)
        {
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            return segmentCount > 5 && _rnd.NextDouble() < 0.08 + 0.02 * difficulty;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            double y = GetNextY(difficulty);
            double parts = 35 + _rnd.NextDouble() * 30;
            AddForwardSegment(y, 10, -360, (int)parts, segmentCount);
        }
    }
}