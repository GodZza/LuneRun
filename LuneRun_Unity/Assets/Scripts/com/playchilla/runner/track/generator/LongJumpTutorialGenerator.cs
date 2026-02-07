using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    // ============================================================================
    // LongJumpTutorialGenerator - 长跳教程生成器
    // ============================================================================
    public class LongJumpTutorialGenerator : SegmentGenerator, ISegmentGenerator
    {
        public LongJumpTutorialGenerator(Track track, global::shared.math.Random rnd, Materials materials) 
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
            var y = GetNextY(difficulty);
            AddForwardSegment(y, 0, 0, 3, segmentCount);
        }
    }
}