using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    public class IslandGenerator : SegmentGenerator, ISegmentGenerator
    {
        private HoleGenerator _holeGenerator;

        public IslandGenerator(Track track, shared.math.Random rnd, Materials materials) 
            : base(track, rnd, materials)
        {
            _holeGenerator = new HoleGenerator(track, rnd, materials);
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (segmentCount < 20)
                return false;
                
            if (_rnd.NextDouble() > 0.02 + 0.06 * difficulty)
                return false;
                
            return true;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            int islandCount = 3 + (int)(_rnd.NextDouble() * 6 * difficulty);
            for (int i = 0; i < islandCount; i++)
            {
                AddForwardSegment(GetNextY(difficulty), 0, 0, _rnd.NextDouble() < 0.1 ? 4 : 7, segmentCount);
                _holeGenerator.Generate(this, difficulty, segmentCount);
            }
        }
    }
}