using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    // ============================================================================
    // SlopeGenerator - Ð±ÆÂÉú³ÉÆ÷
    // ============================================================================
    public class SlopeGenerator : SegmentGenerator, ISegmentGenerator
    {
        private bool _up;

        public SlopeGenerator(Track track, global::shared.math.Random rnd, Materials materials, bool up) 
            : base(track, rnd, materials)
        {
            _up = up;
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (difficulty < 0.3)
                return false;

            if (_rnd.NextDouble() > 0.15)
                return false;

            if (_up && _track.GetConnectPart().GetPos().y > 190)
                return false;

            if (!_up && _track.GetConnectPart().GetPos().y < 110)
                return false;

            return true;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            AddForwardSegment(GetNextY(difficulty), 0, _up ? -45 : 45, 5 + (int)(15 * _rnd.NextDouble()), segmentCount);
            AddForwardSegment(GetNextY(difficulty), 0, _up ? 45 : -45, 5 + (int)(15 * _rnd.NextDouble()), segmentCount);
        }
    }
}