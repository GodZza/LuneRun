using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    public class HoleGenerator : SegmentGenerator, ISegmentGenerator
    {
        public HoleGenerator(Track track, shared.math.Random rnd, Materials materials) 
            : base(track, rnd, materials)
        {
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (difficulty < 0.2)
                return false;
                
            if (_rnd.NextDouble() > 0.02 + 0.08 * difficulty)
                return false;
                
            return true;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            int parts = 5 + (int)(_rnd.NextDouble() * 10);
            AddHoleSegment(parts, segmentCount);
        }
    }
}