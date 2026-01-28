using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.track.generator
{
    public class HoleGenerator : SegmentGenerator, ISegmentGenerator
    {
        private int _parts = -1;

        public HoleGenerator(Track track, global::shared.math.Random rnd, Materials materials)
            : base(track, rnd, materials)
        {
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (previousGenerator == null)
                return false;

            if (previousGenerator is HoleGenerator || _track.GetLastSegment() is HoleSegment)
                return false;

            if (_rnd.NextDouble() > 0.85 + 0.15 * difficulty)
                return false;

            return true;
        }

        public void SetParts(int parts)
        {
            _parts = parts;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            int parts = _parts != -1 ? _parts : (int)(10 + 15 * _rnd.NextDouble());
            var lastSegment = _track.GetLastSegment();

            if (previousGenerator is CurveGenerator || previousGenerator is LoopGenerator)
            {
                AddForwardSegment(GetNextY(difficulty), 0, 0, 3, segmentCount);
            }

            // Simplified: skip warning material logic for now
            AddHoleSegment(parts, segmentCount);
        }
    }
}