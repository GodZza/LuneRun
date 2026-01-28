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
        private Track _track;
        private List<ISegmentGenerator> _generators = new List<ISegmentGenerator>();
        private ISegmentGenerator _lastGenerator;
        private int _levelId;

        public TrackGenerator(Materials materials)
        {
            _materials = materials;
        }

        /// <summary>
        /// 添加轨道段生成器
        /// </summary>
        public void AddSegmentGenerator(ISegmentGenerator generator)
        {
            _generators.Add(generator);
        }

        public ISegmentGenerator GetLastGenerator()
        {
            return _lastGenerator;
        }

        /// <summary>
        /// 生成轨道段
        /// </summary>
        public void Generate(Track track, global::shared.math.Random rnd, double difficulty, int count, int levelId)
        {
            _track = track;
            _levelId = levelId;

            // 遍历所有生成器，找到可以运行的生成器
            foreach (var generator in _generators)
            {
                if (generator.CanRun(_lastGenerator, difficulty, count))
                {
                    var generatorName = generator.GetType().Name.Replace("Generator", "");
                    generator.Generate(_lastGenerator, difficulty, levelId);
                    _lastGenerator = generator;

                    Debug.Log($"[TrackGenerator] Generated {generatorName} segment, total segments: {_track.GetSegments().Count}");
                    break;
                }
            }
        }
    }
}