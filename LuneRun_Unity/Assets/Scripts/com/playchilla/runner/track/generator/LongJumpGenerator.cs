using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    // ============================================================================
    // LongJumpGenerator - ³¤ÌøÉú³ÉÆ÷
    // ============================================================================
    public class LongJumpGenerator : SegmentGenerator, ISegmentGenerator
    {
        private HoleGenerator _holeGenerator;
        private int _holeParts = -1;

        public LongJumpGenerator(Track track, global::shared.math.Random rnd, Materials materials)
            : base(track, rnd, materials)
        {
            _holeGenerator = new HoleGenerator(track, rnd, materials);
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (segmentCount < 20)
                return false;

            if (_rnd.NextDouble() > 0.05)
                return false;

            return true;
        }

        public void SetHoleParts(int holeParts)
        {
            _holeParts = holeParts;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            AddForwardSegment(GetNextY(difficulty), 0, -35, 10, segmentCount);
            AddForwardSegment(GetNextY(difficulty), 0, 35, 15, segmentCount);
            AddForwardSegment(GetNextY(difficulty), 0, 35, 10, segmentCount);
            AddForwardSegment(GetNextY(difficulty), 0, -55, 15, segmentCount);
            AddForwardSegment(GetNextY(difficulty), 0, 0, 2, segmentCount);

            _holeGenerator.SetParts(_holeParts != -1 ? _holeParts : (int)(50 + 30 * difficulty));
            _holeGenerator.Generate(this, difficulty, segmentCount);
        }
    }
}