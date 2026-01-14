using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    public class LongJumpTutorialGenerator : SegmentGenerator, ISegmentGenerator
    {
        public LongJumpTutorialGenerator(Track track, shared.math.Random rnd, Materials materials) 
            : base(track, rnd, materials)
        {
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            // Tutorial generator only runs in specific conditions
            return segmentCount < 5 && difficulty < 0.5;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            double y = GetNextY(difficulty);
            AddForwardSegment(y, 0, 0, 3, segmentCount);
        }
    }
}