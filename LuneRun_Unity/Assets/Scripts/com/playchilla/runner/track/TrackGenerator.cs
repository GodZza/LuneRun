using System.Collections.Generic;
using UnityEngine;
using shared.math;
using com.playchilla.runner.track.generator;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.track
{
    public class TrackGenerator
    {
        private Materials _materials;
        private List<ISegmentGenerator> _generators = new List<ISegmentGenerator>();
        private ISegmentGenerator _lastGenerator;

        public TrackGenerator(Materials materials)
        {
            _materials = materials;
        }

        public void AddSegmentGenerator(ISegmentGenerator generator)
        {
            _generators.Add(generator);
        }

        public ISegmentGenerator GetLastGenerator()
        {
            return _lastGenerator;
        }

        public void Generate(Track track, shared.math.Random rnd, double difficulty, int count, int levelId)
        {
            // Simplified generation logic
            // In real implementation, would select appropriate generator based on conditions
            foreach (var generator in _generators)
            {
                if (generator.CanRun(_lastGenerator, difficulty, count))
                {
                    generator.Generate(_lastGenerator, difficulty, levelId);
                    _lastGenerator = generator;
                    break;
                }
            }
        }
    }
}