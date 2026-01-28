using System.Collections.Generic;
using UnityEngine;
using shared.math;
using com.playchilla.runner.track.generator;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.track
{
    /// <summary>
    /// DynamicTrack - 动态轨道管理系统
    /// 根据玩家位置自动生成和卸载轨道段，实现无限轨道
    /// </summary>
    public class DynamicTrack
    {
        private Track _track;
        private TrackGenerator _generator;
        private global::shared.math.Random _random;
        private Level _level;
        private Materials _materials;
        private int _levelId;

        // 配置参数
        private int _loadForward = 6;      // 向前加载的轨道段数量
        private int _keepBackward = 2;     // 向后保留的轨道段数量
        private double _loadDistance = 200; // 触发加载的距离阈值
        private double _removeDistance = 300; // 触发卸载的距离阈值

        private int _segmentCount = 0;     // 已生成的轨道段总数

        // 调试选项
        private bool _debugMode = true;    // 启用详细调试日志
        private int _loadedSegments = 0;    // 统计：已加载的段数
        private int _removedSegments = 0;   // 统计：已移除的段数

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
            _levelId = level.GetLevelId();
            _materials = level.GetMaterials();

            // 如果 Level 还没有初始化 Materials，创建一个默认实例
            if (_materials == null)
            {
                _materials = new Materials();
                Debug.Log("[DynamicTrack] Level.GetMaterials() returned null, using default Materials instance");
            }

            // 根据关卡ID创建随机数生成器（确保相同关卡生成相同轨道）
            _random = new global::shared.math.Random((uint)_levelId);

            // 创建轨道和生成器
            _track = new Track();
            _generator = new TrackGenerator(_materials);

            // 初始化生成器
            InitializeGenerators();

            // 生成初始轨道
            GenerateInitialTrack();

            Debug.Log($"[DynamicTrack] Created with levelId={_levelId}, loadForward={_loadForward}, keepBackward={_keepBackward}");
        }

        /// <summary>
        /// 初始化所有轨道段生成器
        /// </summary>
        private void InitializeGenerators()
        {
            // 添加各种生成器
            _generator.AddSegmentGenerator(new ForwardGenerator(_track, _random, _materials, _level));
            _generator.AddSegmentGenerator(new HillGenerator(_track, _random, _materials));
            _generator.AddSegmentGenerator(new CurveGenerator(_track, _random, _materials));
            _generator.AddSegmentGenerator(new HoleGenerator(_track, _random, _materials));
            _generator.AddSegmentGenerator(new IslandGenerator(_track, _random, _materials));
            _generator.AddSegmentGenerator(new LongJumpGenerator(_track, _random, _materials));
            _generator.AddSegmentGenerator(new LoopGenerator(_track, _random, _materials));
            _generator.AddSegmentGenerator(new SlopeGenerator(_track, _random, _materials, true)); // up slope

            Debug.Log($"[DynamicTrack] Initialized {8} segment generators");
        }

        /// <summary>
        /// 生成初始轨道段
        /// </summary>
        private void GenerateInitialTrack()
        {
            // 根据关卡ID生成不同数量的初始段
            int initialSegments = _loadForward + _keepBackward;
            double difficulty = GetDifficulty();

            Debug.Log($"[DynamicTrack] Generating initial track with {initialSegments} segments, difficulty={difficulty:F2}");

            // 创建起始连接部分并设置到 Track
            Part connectPart = CreateStartConnectPart();
            _track.SetConnectPart(connectPart);

            Debug.Log($"[DynamicTrack] Initial segments before generation: {_track.GetSegments().Count}");

            // 生成初始轨道段
            for (int i = 0; i < initialSegments; i++)
            {
                _generator.Generate(_track, _random, difficulty, _segmentCount, _levelId);
                _segmentCount++;
                _loadedSegments++;

                Debug.Log($"[DynamicTrack] Generated segment {i+1}/{initialSegments}, total segments: {_track.GetSegments().Count}");
            }

            Debug.Log($"[DynamicTrack] Initial track generated with {_track.GetSegments().Count} segments");
        }

        /// <summary>
        /// 创建起始连接部分
        /// </summary>
        private Part CreateStartConnectPart()
        {
            Vec3 startPos = new Vec3(0, 0, 0);
            Vec3 startDir = new Vec3(0, 0, 1);
            Vec3 startUp = new Vec3(0, 1, 0);

            Part connectPart = new Part(null, startPos, startDir, startUp, new GameObject(), 0, 0);
            _track.GetSegments().Insert(0, new ForwardSegment("Start", 0, connectPart,
                                                             startDir, 0, 0, 5, _materials, _levelId));

            return connectPart;
        }

        /// <summary>
        /// 更新动态轨道（每帧调用）
        /// </summary>
        /// <param name="playerPosition">玩家当前位置</param>
        public void Update(Vec3 playerPosition)
        {
            if (playerPosition == null)
            {
                Debug.LogWarning("[DynamicTrack.Update] Player position is null, skipping update");
                return;
            }

            // 检查是否需要加载新轨道段（添加循环限制，防止死循环）
            int loadAttempts = 0;
            int maxLoadAttempts = 10; // 单次 Update 最多加载 10 个段

            while (ShouldLoadMore(playerPosition) && loadAttempts < maxLoadAttempts)
            {
                LoadNextSegment();
                loadAttempts++;
            }

            if (loadAttempts >= maxLoadAttempts)
            {
                Debug.LogWarning($"[DynamicTrack] Reached max load attempts ({maxLoadAttempts}), stopping to prevent infinite loop");
            }

            // 检查是否需要移除旧轨道段（添加循环限制，防止死循环）
            int removeAttempts = 0;
            int maxRemoveAttempts = 10; // 单次 Update 最多移除 10 个段

            while (ShouldRemoveOld(playerPosition) && removeAttempts < maxRemoveAttempts)
            {
                RemoveOldestSegment();
                removeAttempts++;
            }

            if (removeAttempts >= maxRemoveAttempts)
            {
                Debug.LogWarning($"[DynamicTrack] Reached max remove attempts ({maxRemoveAttempts}), stopping to prevent infinite loop");
            }
        }

        /// <summary>
        /// 检查是否需要加载新轨道段
        /// </summary>
        private bool ShouldLoadMore(Vec3 playerPosition)
        {
            List<Segment> segments = _track.GetSegments();
            if (segments.Count == 0)
                return true;

            // 获取最后一个轨道段
            Segment lastSegment = segments[segments.Count - 1];
            if (lastSegment == null || lastSegment.GetLastPart() == null)
                return true;

            // 计算玩家到最后一个段的距离
            Vec3 lastSegmentEnd = lastSegment.GetLastPart().pos;
            double distance = playerPosition.sub(lastSegmentEnd).length();

            bool shouldLoad = distance < _loadDistance;

            if (Time.frameCount % 60 == 0 && shouldLoad)
            {
                Debug.Log($"[DynamicTrack] Player near end (distance={distance:F2}), should load more, segments={_track.GetSegments().Count}");
            }

            if (_debugMode && shouldLoad)
            {
                Debug.Log($"[DynamicTrack] ShouldLoadMore: distance={distance:F2} < {_loadDistance:F2} = {shouldLoad}");
            }

            return shouldLoad;
        }

        /// <summary>
        /// 检查是否需要移除旧轨道段
        /// </summary>
        private bool ShouldRemoveOld(Vec3 playerPosition)
        {
            List<Segment> segments = _track.GetSegments();
            if (segments.Count <= _keepBackward)
                return false;

            // 获取第一个轨道段
            Segment firstSegment = segments[0];
            if (firstSegment == null || firstSegment.GetLastPart() == null)
                return false;

            // 计算玩家到第一个段的距离
            Vec3 firstSegmentEnd = firstSegment.GetLastPart().pos;
            double distance = playerPosition.sub(firstSegmentEnd).length();

            bool shouldRemove = distance > _removeDistance;

            if (Time.frameCount % 60 == 0 && shouldRemove)
            {
                Debug.Log($"[DynamicTrack] Player far from start (distance={distance:F2}), should remove old segment");
            }

            return shouldRemove;
        }

        /// <summary>
        /// 加载下一个轨道段
        /// </summary>
        private void LoadNextSegment()
        {
            double difficulty = GetDifficulty();

            Debug.Log($"[DynamicTrack] Loading segment #{_segmentCount}, difficulty={difficulty:F2}");

            // 使用生成器生成新轨道段
            _generator.Generate(_track, _random, difficulty, _segmentCount, _levelId);
            _segmentCount++;
            _loadedSegments++;

            if (_debugMode)
            {
                var lastSegment = _track.GetSegments()[_track.GetSegments().Count - 1];
                Debug.Log($"[DynamicTrack] Generated segment type: {lastSegment.GetType().Name}");
            }

            // 连接新段
            ConnectLastSegments();

            Debug.Log($"[DynamicTrack] Segment loaded, total segments={_track.GetSegments().Count}, total loaded={_loadedSegments}");
        }

        /// <summary>
        /// 连接最后两个轨道段
        /// </summary>
        private void ConnectLastSegments()
        {
            List<Segment> segments = _track.GetSegments();
            if (segments.Count < 2)
                return;

            Segment prevSegment = segments[segments.Count - 2];
            Segment currSegment = segments[segments.Count - 1];

            Part prevLastPart = prevSegment.GetLastPart();
            Part currConnectPart = currSegment.GetConnectPart();

            if (prevLastPart != null && currConnectPart != null)
            {
                currConnectPart.previous = prevLastPart;
                prevLastPart.next = currConnectPart;
            }
        }

        /// <summary>
        /// 移除最旧的轨道段
        /// </summary>
        private void RemoveOldestSegment()
        {
            List<Segment> segments = _track.GetSegments();
            if (segments.Count == 0)
                return;

            Segment oldestSegment = segments[0];
            _removedSegments++;

            if (_debugMode)
            {
                Debug.Log($"[DynamicTrack] Removing segment type: {oldestSegment.GetType().Name}, total removed={_removedSegments}");
            }

            Debug.Log($"[DynamicTrack] Removing segment: {oldestSegment}, remaining before={segments.Count}");

            // 移除段
            _track.RemoveSegment(oldestSegment);

            Debug.Log($"[DynamicTrack] Segment removed, remaining segments={_track.GetSegments().Count}, total removed={_removedSegments}");
        }

        /// <summary>
        /// 获取当前难度（0-1）
        /// </summary>
        private double GetDifficulty()
        {
            // 难度基于关卡ID：levelId / 50，最大为1.0
            double difficulty = (double)_levelId / 50.0;
            return System.Math.Min(1.0, difficulty);
        }

        /// <summary>
        /// 获取轨道实例
        /// </summary>
        public Track GetTrack()
        {
            return _track;
        }

        /// <summary>
        /// 获取已生成的轨道段总数
        /// </summary>
        public int GetSegmentCount()
        {
            return _segmentCount;
        }

        /// <summary>
        /// 获取起始位置
        /// </summary>
        public Vec3 GetStartPos()
        {
            Part connectPart = _track.GetConnectPart();
            if (connectPart != null)
            {
                return connectPart.pos.add(new Vec3(0, 2, 0)); // 在连接点上方2单位
            }

            return new Vec3(0, 2, 0); // 默认位置
        }

        /// <summary>
        /// 获取统计信息（用于验证）
        /// </summary>
        public string GetStatistics()
        {
            return $"DynamicTrack Statistics:\n" +
                   $"  Level ID: {_levelId}\n" +
                   $"  Difficulty: {GetDifficulty():F2}\n" +
                   $"  Total Generated: {_segmentCount}\n" +
                   $"  Total Loaded: {_loadedSegments}\n" +
                   $"  Total Removed: {_removedSegments}\n" +
                   $"  Current Segments: {_track.GetSegments().Count}\n" +
                   $"  Load Forward: {_loadForward}\n" +
                   $"  Keep Backward: {_keepBackward}\n" +
                   $"  Load Distance: {_loadDistance:F1}\n" +
                   $"  Remove Distance: {_removeDistance:F1}";
        }

        /// <summary>
        /// 设置调试模式
        /// </summary>
        public void SetDebugMode(bool enabled)
        {
            _debugMode = enabled;
            Debug.Log($"[DynamicTrack] Debug mode: {(enabled ? "ENABLED" : "DISABLED")}");
        }
    }
}
