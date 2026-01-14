using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    public class HillGenerator : SegmentGenerator, ISegmentGenerator
    {
        public HillGenerator(Track track, shared.math.Random rnd, Materials materials) 
            : base(track, rnd, materials)
        {
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (difficulty < 0.15)
                return false;
                
            if (_rnd.NextDouble() > 0.05 + 0.2 * difficulty)
                return false;
                
            return true;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            double y = GetNextY(difficulty);
            int parts = 10 + (int)(_rnd.NextDouble() * 20);
            AddForwardSegment(y, 0, 0, parts, segmentCount);
        }
    }
}