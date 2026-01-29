//using UnityEngine;
//using com.playchilla.runner;
//using com.playchilla.runner.track;
//using shared.math;

//namespace LuneRun.Tests
//{
//    /// <summary>
//    /// 纯 DynamicTrack 单元测试
//    /// 不依赖 Level、Player 等复杂对象，直接测试轨道生成逻辑
//    /// 符合单元测试原则：隔离、快速、可重复
//    /// </summary>
//    public class PureDynamicTrackTest : MonoBehaviour
//    {
//        private Track _track;
//        private com.playchilla.runner.track.TrackGenerator _generator;
//        private global::shared.math.Random _random;
//        private Materials _materials;

//        void Start()
//        {
//            Debug.Log("========== Pure DynamicTrack Unit Test ==========");
//            TestTrackGeneration();
//            TestAllGenerators();
//            TestSegmentConnections();
//            TestDynamicLoading();
//            Cleanup();
//            Debug.Log("=============================================");
//            Debug.Log("所有测试完成！3秒后自动关闭...");
//            Destroy(gameObject, 3f);
//        }

//        private void Cleanup()
//        {
//            // 清理轨道中所有段的 GameObject
//            if (_track != null)
//            {
//                foreach (var segment in _track.GetSegments())
//                {
//                    if (segment.GetConnectPart() != null && segment.GetConnectPart().visual != null)
//                    {
//                        Destroy(segment.GetConnectPart().visual);
//                    }
//                    foreach (var part in segment.GetParts())
//                    {
//                        if (part.visual != null)
//                        {
//                            Destroy(part.visual);
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 测试 1: 轨道段生成
//        /// </summary>
//        private void TestTrackGeneration()
//        {
//            Debug.Log("\n[Test 1] 轨道段生成测试");
//            Debug.Log("----------------------------------");

//            try
//            {
//                // 初始化
//                _materials = new Materials();
//                _track = new Track();
//                _track.SetConnectPart(new Part(null, new Vec3(0, 0, 0), new Vec3(0, 0, 1), new Vec3(0, 1, 0), new GameObject(), 0, 0));
//                _generator = new com.playchilla.runner.track.TrackGenerator(_materials);
//                _random = new global::shared.math.Random(1); 

//                // 添加生成器
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.ForwardGenerator(_track, _random, _materials, null));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.HillGenerator(_track, _random, _materials));

//                // 循环生成直到有 10 个段（因为每次 Generate 只生成一个段，且受 CanRun 条件限制）
//                Debug.Log("生成轨道段...");
//                int attempts = 0;
//                int targetSegments = 10;
//                while (_track.GetSegments().Count < targetSegments && attempts < 50)
//                {
//                    _generator.Generate(_track, _random, 0.8, _track.GetSegments().Count, 1);
//                    attempts++;
//                }

//                // 验证
//                int segmentCount = _track.GetSegments().Count;
//                Debug.Log($"✓ 尝试了 {attempts} 次生成，实际生成了 {segmentCount} 个段");

//                if (segmentCount < targetSegments)
//                {
//                    Debug.LogWarning($"⚠ 未达到目标段数 {targetSegments}，实际 {segmentCount}");
//                }
//                else
//                {
//                    Debug.Log($"✓ 成功生成 {segmentCount} 个段");
//                }
//            }
//            catch (System.Exception e)
//            {
//                Debug.LogError($"✗ 测试失败: {e.Message}");
//                Debug.LogException(e);
//            }
//        }

//        /// <summary>
//        /// 测试 1.5: 所有生成器测试
//        /// </summary>
//        private void TestAllGenerators()
//        {
//            Debug.Log("\n[Test 1.5] 所有生成器测试");
//            Debug.Log("----------------------------------");

//            try
//            {
//                // 重新初始化轨道
//                _track = new Track();
//                _track.SetConnectPart(new Part(null, new Vec3(0, 150, 0), new Vec3(0, 0, 1), new Vec3(0, 1, 0), new GameObject(), 0, 0));
//                _generator = new com.playchilla.runner.track.TrackGenerator(_materials);
//                _random = new global::shared.math.Random(1);

//                // 添加所有生成器
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.ForwardGenerator(_track, _random, _materials, null));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.HoleGenerator(_track, _random, _materials));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.SlopeGenerator(_track, _random, _materials, true));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.SlopeGenerator(_track, _random, _materials, false));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.CurveGenerator(_track, _random, _materials));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.HillGenerator(_track, _random, _materials));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.LoopGenerator(_track, _random, _materials));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.LongJumpGenerator(_track, _random, _materials));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.IslandGenerator(_track, _random, _materials));
//                _generator.AddSegmentGenerator(new com.playchilla.runner.track.generator.StraightGenerator(_track, _random, _materials));

//                // 生成 30 个段以测试所有生成器
//                Debug.Log("使用所有生成器生成 30 个段...");
//                int attempts = 0;
//                int targetSegments = 30;
//                while (_track.GetSegments().Count < targetSegments && attempts < 100)
//                {
//                    _generator.Generate(_track, _random, 0.8, _track.GetSegments().Count, 1);
//                    attempts++;
//                }

//                // 验证
//                int segmentCount = _track.GetSegments().Count;
//                Debug.Log($"✓ 尝试了 {attempts} 次生成，实际生成了 {segmentCount} 个段");

//                if (segmentCount < targetSegments)
//                {
//                    Debug.LogWarning($"⚠ 未达到目标段数 {targetSegments}，实际 {segmentCount}");
//                }
//                else
//                {
//                    Debug.Log($"✓ 成功生成 {segmentCount} 个段");

//                    // 统计每种生成器生成的段
//                    var generatorNames = new System.Collections.Generic.Dictionary<string, int>();
//                    foreach (var segment in _track.GetSegments())
//                    {
//                        string typeName = segment.GetType().Name;
//                        if (generatorNames.ContainsKey(typeName))
//                            generatorNames[typeName]++;
//                        else
//                            generatorNames[typeName] = 1;
//                    }

//                    Debug.Log("各类型段分布:");
//                    foreach (var kvp in generatorNames)
//                    {
//                        Debug.Log($"  - {kvp.Key}: {kvp.Value}");
//                    }
//                }
//            }
//            catch (System.Exception e)
//            {
//                Debug.LogError($"✗ 测试失败: {e.Message}");
//                Debug.LogException(e);
//            }
//        }

//        /// <summary>
//        /// 测试 2: 段连接
//        /// </summary>
//        private void TestSegmentConnections()
//        {
//            Debug.Log("\n[Test 2] 段连接测试");
//            Debug.Log("----------------------------------");

//            try
//            {
//                if (_track == null || _track.GetSegments().Count == 0)
//                {
//                    Debug.LogWarning("跳过：没有可测试的段");
//                    return;
//                }

//                var segments = _track.GetSegments();

//                // 检查第一个段
//                var firstSegment = segments[0];
//                if (firstSegment.GetConnectPart() == null)
//                {
//                    Debug.LogError("✗ 第一个段的 connectPart 为 null");
//                }
//                else
//                {
//                    Debug.Log($"✓ 第一个段的 connectPart 存在");
//                }

//                if (firstSegment.GetLastPart() == null)
//                {
//                    Debug.LogError("✗ 第一个段的 lastPart 为 null");
//                }
//                else
//                {
//                    Debug.Log($"✓ 第一个段的 lastPart 存在");
//                }

//                // 检查最后两个段的连接
//                if (segments.Count >= 2)
//                {
//                    var secondLastSegment = segments[segments.Count - 2];
//                    var lastSegment = segments[segments.Count - 1];

//                    if (lastSegment.GetConnectPart() != null &&
//                        secondLastSegment.GetLastPart() != null)
//                    {
//                        Debug.Log($"✓ 段连接存在（最后两个段）");
//                    }
//                    else
//                    {
//                        Debug.LogWarning("⚠ 部分段连接可能缺失");
//                    }
//                }
//            }
//            catch (System.Exception e)
//            {
//                Debug.LogError($"✗ 测试失败: {e.Message}");
//                Debug.LogException(e);
//            }
//        }

//        /// <summary>
//        /// 测试 3: 动态加载/卸载
//        /// </summary>
//        private void TestDynamicLoading()
//        {
//            Debug.Log("\n[Test 3] 动态加载/卸载测试");
//            Debug.Log("----------------------------------");

//            try
//            {
//                if (_track == null || _track.GetSegments().Count == 0)
//                {
//                    Debug.LogWarning("跳过：没有可测试的段");
//                    return;
//                }

//                var segments = _track.GetSegments();
//                Debug.Log($"初始段数: {segments.Count}");

//                // 模拟玩家向前移动
//                Vec3 playerPos = new Vec3(0, 1, 0);

//                for (int step = 0; step < 5; step++)
//                {
//                    // 移动玩家
//                    playerPos.z += 50;
//                    Debug.Log($"步骤 {step + 1}: 玩家位置 Z={playerPos.z}");

//                    // 获取最后一段的终点
//                    var lastSegment = segments[segments.Count - 1];
//                    var lastPart = lastSegment.GetLastPart();

//                    if (lastPart == null)
//                    {
//                        Debug.LogError($"✗ 步骤 {step + 1}: lastPart 为 null");
//                        continue;
//                    }

//                    // 计算距离
//                    double distance = playerPos.sub(lastPart.pos).length();
//                    Debug.Log($"  距离最后一段: {distance:F2}");

//                    // 距离小于 200 时应该加载新段
//                    bool shouldLoad = distance < 200;
//                    Debug.Log($"  是否应该加载: {shouldLoad}");

//                    if (shouldLoad && segments.Count < 15)
//                    {
//                        // 加载新段
//                        _generator.Generate(_track, _random, 0.5, segments.Count, 1);
//                        Debug.Log($"  ✓ 已加载新段，当前总数: {segments.Count}");
//                    }
//                    else
//                    {
//                        Debug.Log($"  - 不需要加载新段");
//                    }
//                }

//                Debug.Log($"✓ 最终段数: {segments.Count}");
//            }
//            catch (System.Exception e)
//            {
//                Debug.LogError($"✗ 测试失败: {e.Message}");
//                Debug.LogException(e);
//            }
//        }
//    }
//}
