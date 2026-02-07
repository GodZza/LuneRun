using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.track.generator
{
    // ============================================================================
    // HoleGenerator - 空洞生成器
    // ============================================================================
    public class HoleGenerator : SegmentGenerator, ISegmentGenerator
    {
        private int _parts = -1;

        public HoleGenerator(Track track, global::shared.math.Random rnd, Materials materials)
            : base(track, rnd, materials)
        {
        }

        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            if (previousGenerator == null) // 不能一开始就创建洞
                return false;
            
            if (previousGenerator is HoleGenerator || _track.GetLastSegment() is HoleSegment) // 上一个是洞，不再创建
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
            if (previousGenerator is CurveGenerator || previousGenerator is LoopGenerator)
            {
                AddForwardSegment(GetNextY(difficulty), 0, 0, 3, segmentCount);
            }

            var lastSegment = _track.GetLastSegment();
            var currentSegment = lastSegment;
            var currentSegmentParts = currentSegment.GetParts();
            var warningMaterials = _materials.GetMaterialVector("Warning");
            var warningMaterialCount = warningMaterials.Count;
            var currentPartIndex = currentSegmentParts.Count - 1;

            for(var i = 1; i < warningMaterialCount; ++i)
            {
                if(currentPartIndex < 0)
                {
                    currentSegment = currentSegment.GetPreviousSegment();
                    if(currentSegment == null || currentSegment is HoleSegment)
                    {
                        break;
                    }
                    currentSegmentParts = currentSegment.GetParts();
                    currentPartIndex = currentSegmentParts.Count - 1;
                }
                var targetPart = currentSegmentParts[currentPartIndex--];
                targetPart.mesh.material = warningMaterials[warningMaterialCount - i];

            }

            var parts = _parts != -1 ? _parts : 10 + 15 * _rnd.NextDouble();
            AddHoleSegment((int)parts, segmentCount);
        }
    }
}