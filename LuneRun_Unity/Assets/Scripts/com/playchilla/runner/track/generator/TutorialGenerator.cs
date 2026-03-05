using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace com.playchilla.runner.track.generator
{
    /// <summary>
    /// 教程关卡生成器 - 根据步骤生成不同的轨道片段
    /// 继承自SegmentGenerator并实现ISegmentGenerator接口
    /// </summary>
    public class TutorialGenerator : SegmentGenerator, ISegmentGenerator
    {
        private HoleGenerator _hg;
        private LoopGenerator _loopGenerator;
        private int _step = 0;
        public TutorialGenerator(Track track, global::shared.math.Random rnd, Materials materials)
            : base(track, rnd, materials)
        {
            this._hg = new HoleGenerator(track, rnd, materials);
            this._loopGenerator = new LoopGenerator(track, rnd, materials);
        }
        /// <summary>
        /// 检查是否可以运行生成器
        /// </summary>
        /// <param name="previousGenerator">上一个生成器</param>
        /// <param name="difficulty">位置参数</param>
        /// <param name="segmentCount">关卡标识</param>
        /// <returns>始终返回true，教程生成器是强制运行的</returns>
        public bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            return true;
        }

        public void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount)
        {
            // 使用switch结构重构复杂的if-else逻辑[1](@ref)
            switch (_step)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    // 前10步生成直线段
                    AddForwardSegment(150, 0, 0, 20, segmentCount);
                    break;

                case 10:
                    // 第10步生成坑洞
                    _hg.Generate(this, 0, segmentCount);
                    break;

                case 11:
                    // 第11步生成50长度的直线段
                    AddForwardSegment(150, 0, 0, 50, segmentCount);
                    break;

                case 12:
                    // 第12步生成坑洞
                    _hg.Generate(this, 0, segmentCount);
                    break;

                case 13:
                    // 第13步生成100长度的直线段
                    AddForwardSegment(150, 0, 0, 100, segmentCount);
                    break;

                case 14:
                    // 第14步生成坑洞
                    _hg.Generate(this, 0, segmentCount);
                    break;

                case 15:
                    // 第15步生成60或100长度的直线段
                    AddForwardSegment(60, 0, 0, 100, segmentCount);
                    break;

                case 16:
                    // 第16步生成循环轨道
                    _loopGenerator.Generate(previousGenerator, difficulty, segmentCount);
                    break;

                case 17:
                    // 第17步生成60或100长度的直线段
                    AddForwardSegment(60, 0, 0, 100, segmentCount);
                    break;

                default:
                    // 超出定义范围的步骤不做任何操作
                    break;
            }

            // 更新步骤计数器[1](@ref)
            _step++;
        }
    }
}
