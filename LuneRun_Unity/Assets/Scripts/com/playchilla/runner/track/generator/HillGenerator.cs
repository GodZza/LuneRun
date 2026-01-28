using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    public class HillGenerator : SegmentGenerator, ISegmentGenerator
    {
        private SlopeGenerator _up;
        private SlopeGenerator _down;

        public HillGenerator(Track track, global::shared.math.Random rnd, Materials materials)
            : base(track, rnd, materials)
        {
            _up = new SlopeGenerator(track, rnd, materials, true);
            _down = new SlopeGenerator(track, rnd, materials, false);
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (segmentCount < 3)
                return false;

            if (_rnd.NextDouble() > 0.07 - 0.04 * difficulty)
                return false;

            return true;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            int hillCount = (int)_rnd.NextIntRange(1, 2);

            for (int i = 0; i < hillCount; i++)
            {
                double y = _track.GetConnectPart().GetPos().y;

                if (y > 190)
                {
                    // slopeGen = _down;
                }
                else if (y < 110)
                {
                    // slopeGen = _up;
                }
                else
                {
                    // slopeGen = y > 150 ? _down : _up;
                }

                AddForwardSegment(GetNextY(difficulty), 0, -35, 10, segmentCount);
                AddForwardSegment(GetNextY(difficulty), 0, 35, 15, segmentCount);
                AddForwardSegment(GetNextY(difficulty), 0, 35, 10, segmentCount);
                AddForwardSegment(GetNextY(difficulty), 0, -35, 15, segmentCount);
            }
        }
    }
}