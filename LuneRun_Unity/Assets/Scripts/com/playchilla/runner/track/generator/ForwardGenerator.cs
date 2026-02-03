using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    public class ForwardGenerator : SegmentGenerator, ISegmentGenerator
    {
        private Level _level;
        private int _parts = -1;

        public ForwardGenerator(Track track, global::shared.math.Random rnd, Materials materials, Level level) 
            : base(track, rnd, materials)
        {
            _level = level;
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (segmentCount > 3 && previousGenerator is ForwardGenerator)
                return false;
                
            if (_rnd.NextDouble() > 0.8 - 0.6 * difficulty)
                return false;
                
            return true;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            var y = GetNextY(difficulty);
            var parts = _parts != -1 ? _parts : 13 + (int)(_rnd.NextDouble() * 30);
            
            if (_rnd.NextDouble() < 0.4 * difficulty && segmentCount > 16)
            {
                parts = Mathf.Max(4, parts - (int)(20 + 5 * difficulty));
            }
            
            AddForwardSegment(y, 0, 0, parts, segmentCount);
        }

        public void SetParts(int parts)
        {
            _parts = parts;
        }
    }
}