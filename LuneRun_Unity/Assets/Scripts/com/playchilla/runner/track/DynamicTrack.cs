using System.Collections.Generic;
using UnityEngine;
using shared.math;
using com.playchilla.runner.track.generator;
using com.playchilla.runner.track.segment;
using System;
using UnityEngine.Assertions;

namespace com.playchilla.runner.track
{
   


    /// <summary>
    /// DynamicTrack - 动态轨道管理系统
    /// 根据玩家位置自动生成和卸载轨道段，实现无限轨道
    /// </summary>
    public class DynamicTrack
    {
        private Track _track = new();
        private shared.math.Random _rnd = new(1);
        private TrackGenerator _generator;
        private Level _level;
        

        // 配置参数
        private int _loadForward = 6;      // 向前加载的轨道段数量
        private int _keepBackward = 2;     // 向后保留的轨道段数量
        private double _loadDistance = 200; // 触发加载的距离阈值
        private double _removeDistance = 300; // 触发卸载的距离阈值

        private int _segmentCount = 0;     // 已生成的轨道段总数（删除旧的段数，不会降）
        private int _lastGenerateLevelId = -1;  // 上次生成时的levelId
        private int _generatedForLevel = 0;

        private int _loadedSegments = 0;    // 统计：已加载的段数
        private int _removedSegments = 0;   // 统计：已移除的段数

        private ForwardGenerator _forwardGen;
        private LongJumpGenerator _longJumpGen;
        private TutorialGenerator _tutorialGenerator;
        private LongJumpTutorialGenerator _longJumpTutorial;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="level">关卡引用</param>
        /// <param name="loadForward">向前加载的段数</param>
        /// <param name="keepBackward">向后保留的段数</param>
        public DynamicTrack(Level level, int loadForward, int keepBackward)
        {
            _level = level;
            _loadForward = loadForward;
            _keepBackward = keepBackward;

            // 初始化生成器
            InitializeGenerators();
        }

        /// <summary>
        /// 初始化所有轨道段生成器
        /// </summary>
        private void InitializeGenerators()
        {
            var materials = this._level.GetMaterials();

            _forwardGen = new(_track, _rnd, materials, _level);
            _longJumpGen = new(_track, _rnd, materials);

            _generator = new TrackGenerator(materials);
            // 添加默认开始赛道?
            _track.AddSegment(new ForwardSegment("ForwardSegment", 150, null, new Vec3(0, 0, 1), 0, 0, 20, materials, -1));

            _generator.AddSegmentGenerator(new ForwardGenerator (_track, _rnd, materials, _level));
            _generator.AddSegmentGenerator(new HoleGenerator    (_track, _rnd, materials));
            _generator.AddSegmentGenerator(new SlopeGenerator   (_track, _rnd, materials, true)); // up slope
            _generator.AddSegmentGenerator(new SlopeGenerator   (_track, _rnd, materials, false)); // down slope            
            _generator.AddSegmentGenerator(new CurveGenerator   (_track, _rnd, materials));
            _generator.AddSegmentGenerator(new HillGenerator    (_track, _rnd, materials));
            _generator.AddSegmentGenerator(new LoopGenerator    (_track, _rnd, materials));
            _generator.AddSegmentGenerator(new LongJumpGenerator(_track, _rnd, materials));
            _generator.AddSegmentGenerator(new IslandGenerator  (_track, _rnd, materials));

            _tutorialGenerator = new TutorialGenerator(_track, _rnd, materials);
            _longJumpTutorial = new LongJumpTutorialGenerator(_track, _rnd, materials);

            Debug.Log($"[DynamicTrack] Initialized x segment generators");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerStandPart">玩家站在的Part</param>
        public void TryRemove(Part playerStandPart)
        {
            if(playerStandPart == null) return;

            // 删除part之前的 segment

            var segment = playerStandPart.segment.GetPreviousSegment();
            if(segment == null) return;
            var keep = _keepBackward;
            while(segment != null && --keep > 0)
            {
                segment = segment.GetPreviousSegment();
                if(segment != null)
                {
                    _track.RemoveSegment(segment);
                }
            }
        }

        /// <summary>
        /// 这里使用了分帧处理，每次调用生成一段轨道。直到 _segmentCount >= maxSegment
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="maxSegment"></param>
        public void TryAdd(int levelId, int maxSegment)
        {
            // 如果levelId改变了，更新随机数seed（与AS代码一致）
            if (levelId != _lastGenerateLevelId)
            {
                uint seed = (uint)(levelId + 1);

                // 特殊levelId的seed处理
                if (levelId == 26)
                {
                    seed = 101;
                }
                else if (levelId == 28)
                {
                    seed = 100;
                }
                else if (levelId == 32)
                {
                    seed = 105;
                }

                _rnd.SetSeed(seed);
                _lastGenerateLevelId = levelId;
                _segmentCount = 0;  // 重置生成的段数
            }

            if(_segmentCount >= maxSegment)
            {
                return;
            }
            if(_track.GetSegments().Count >= this._keepBackward + this._loadForward)
            {
                return;
            }

            var difficulty = 1 - Math.Max(0, Math.Min((32f - levelId) / 32f, 1));
            Assert.IsTrue(difficulty >= 0 && difficulty <= 1, $"Difficulty out of range: {difficulty}");
            if(levelId == 1)
            {
                this._tutorialGenerator.Generate(null, 0, levelId);
            }else if(levelId == 20)
            {
                this._longJumpTutorial.Generate(null, 0, levelId);
            }
            else
            {
                if(levelId == 32 && _segmentCount == maxSegment -1)
                {
                    _longJumpGen.SetHoleParts(100);
                    _longJumpGen.Generate(_generator.GetLastGenerator(), 0.1, levelId);
                }
                else if(levelId == 33)
                {
                    _forwardGen.SetParts(40);
                    _forwardGen.Generate(_generator.GetLastGenerator(), 0.1, levelId);
                }
                else if(_segmentCount != 0)
                {
                    _generator.Generate(this._track, this._rnd, difficulty, 1, levelId);
                }
                else if(levelId == 17)
                {
                    this._forwardGen.SetParts(300);
                    this._forwardGen.Generate(_generator.GetLastGenerator(), difficulty, levelId);
                }
            }

            _segmentCount += 1;
        }

        /// <summary>
        /// 返回true表示levelId的关卡已经生成完毕
        /// </summary>
        /// <param name="part"></param>
        /// <param name="levelId">关卡id</param>
        /// <param name="maxSegment">本关卡（levelId） 生成的最大段数</param>
        /// <returns></returns>
        public bool Update(Part part, int levelId, int maxSegment)
        {
            this.TryRemove(part);             // 移除玩家后方旧的段
            this.TryAdd(levelId, maxSegment); // 添加玩家前方新的段
            return this._segmentCount>= maxSegment;
        }



        /// <summary>
        /// 获取轨道实例
        /// </summary>
        public Track GetTrack()
        {
            return _track;
        }
        Part _lastLevelPart;
        public bool IsLastLevelPart(Part part)
        {
            return part == this._lastLevelPart;
        }


        /// <summary>
        /// 获取统计信息（用于验证）
        /// </summary>
        public string GetStatistics()
        {
            return $"DynamicTrack Statistics:\n" +
                   $"  Total Generated: {_segmentCount}\n" +
                   $"  Total Loaded: {_loadedSegments}\n" +
                   $"  Total Removed: {_removedSegments}\n" +
                   $"  Current Segments: {_track.GetSegments().Count}\n" +
                   $"  Load Forward: {_loadForward}\n" +
                   $"  Keep Backward: {_keepBackward}\n" +
                   $"  Load Distance: {_loadDistance:F1}\n" +
                   $"  Remove Distance: {_removeDistance:F1}";
        }

    }
}
