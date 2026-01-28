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
        /// <returns>实际生成的段数</returns>
        public int Generate(Track track, global::shared.math.Random rnd, double difficulty, int count, int levelId)
        {
            _track = track;
            _levelId = levelId;

            int generatedCount = 0;

            // 无限循环，直到生成足够的段
            for (;;)
            {
                ISegmentGenerator generator;

                // 随机选择生成器
                do
                {
                    int index = (int)(rnd.NextDouble() * _generators.Count);
                    generator = _generators[index];
                }
                while (!generator.CanRun(_lastGenerator, difficulty, count));

                // 生成段
                generator.Generate(_lastGenerator, difficulty, levelId);
                _lastGenerator = generator;

                var generatorName = generator.GetType().Name.Replace("Generator", "");
                Debug.Log($"[TrackGenerator] Generated {generatorName} segment ({generatedCount + 1}/target), total segments: {_track.GetSegments().Count}");

                generatedCount++;

                // 如果生成了足够的段，返回
                if (generatedCount >= count)
                {
                    return generatedCount;
                }
            }
        }
    }
}