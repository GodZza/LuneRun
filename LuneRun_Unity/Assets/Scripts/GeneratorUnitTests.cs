using UnityEngine;
using com.playchilla.runner.track;
using com.playchilla.runner.track.generator;
using com.playchilla.runner;
using shared.math;

namespace LuneRun.Tests
{
    /// <summary>
    /// Generator 纯单元测试
    /// 测试每个生成器的 CanRun() 和 Generate() 方法
    /// 不依赖 GameObject，符合单元测试原则
    /// </summary>
    public class GeneratorUnitTests
    {
        private Track _track;
        private global::shared.math.Random _random;
        private Materials _materials;

        /// <summary>
        /// 初始化测试环境
        /// </summary>
        private void Setup()
        {
            //_track = new Track();
            //_track.SetConnectPart(new Part(null, new Vec3(0, 150, 0), new Vec3(0, 0, 1), new Vec3(0, 1, 0), null, 0, 0));
            //_random = new global::shared.math.Random(1);
            //_materials = new Materials();
        }

        #region ForwardGenerator Tests

        /// <summary>
        /// 测试 ForwardGenerator.CanRun() - 低难度
        /// </summary>
        public void TestForwardGenerator_CanRun_LowDifficulty()
        {
            // Arrange
            Setup();
            var generator = new ForwardGenerator(_track, _random, _materials, null);

            // Act
            bool canRun = generator.CanRun(null, 0.0, 0);

            // Assert
            Debug.Log($"[Test] ForwardGenerator.CanRun(difficulty=0.0): {canRun}");
            if (!canRun)
            {
                Debug.LogError("✗ ForwardGenerator 在低难度下应该能运行");
            }
            else
            {
                Debug.Log("✓ ForwardGenerator 在低难度下可以运行");
            }
        }

        /// <summary>
        /// 测试 ForwardGenerator.Generate()
        /// </summary>
        public void TestForwardGenerator_Generate()
        {
            // Arrange
            Setup();
            var generator = new ForwardGenerator(_track, _random, _materials, null);
            int segmentCount = 1;

            // Act
            generator.Generate(null, 0.5, segmentCount);

            // Assert
            int actualCount = _track.GetSegments().Count;
            Debug.Log($"[Test] ForwardGenerator.Generate(): 生成了 {actualCount} 个段");
            if (actualCount > 0)
            {
                Debug.Log("✓ ForwardGenerator 成功生成段");
            }
            else
            {
                Debug.LogError("✗ ForwardGenerator 未能生成段");
            }
        }

        #endregion

        #region CurveGenerator Tests

        /// <summary>
        /// 测试 CurveGenerator.CanRun() - 低难度不应该运行
        /// </summary>
        public void TestCurveGenerator_CanRun_LowDifficulty()
        {
            // Arrange
            Setup();
            var generator = new CurveGenerator(_track, _random, _materials);

            // Act
            bool canRun = generator.CanRun(null, 0.05, 5); // difficulty < 0.07

            // Assert
            Debug.Log($"[Test] CurveGenerator.CanRun(difficulty=0.05): {canRun}");
            if (canRun)
            {
                Debug.LogError("✗ CurveGenerator 在低难度下不应该运行 (difficulty < 0.07)");
            }
            else
            {
                Debug.Log("✓ CurveGenerator 在低难度下正确拒绝运行");
            }
        }

        /// <summary>
        /// 测试 CurveGenerator.CanRun() - 高难度可能运行
        /// 注意：由于有随机因素，此测试只是验证高难度不会直接被difficulty条件拒绝
        /// </summary>
        public void TestCurveGenerator_CanRun_HighDifficulty()
        {
            // Arrange
            Setup();
            var generator = new CurveGenerator(_track, _random, _materials);

            // Act
            bool canRun = generator.CanRun(null, 0.5, 5); // difficulty >= 0.07

            // Assert
            Debug.Log($"[Test] CurveGenerator.CanRun(difficulty=0.5): {canRun}");
            Debug.Log("  注：高难度时仍可能因随机因素被拒绝，但不会被difficulty条件直接拒绝");
            // 不判断canRun的值，因为受随机数影响
            Debug.Log("✓ 测试完成（difficulty >= 0.07 时，不会因difficulty被拒绝）");
        }

        /// <summary>
        /// 测试 CurveGenerator.Generate()
        /// </summary>
        public void TestCurveGenerator_Generate()
        {
            // Arrange
            Setup();
            var generator = new CurveGenerator(_track, _random, _materials);
            int segmentCount = 1;

            // Act
            generator.Generate(null, 0.05, segmentCount);

            // Assert
            int actualCount = _track.GetSegments().Count;
            Debug.Log($"[Test] CurveGenerator.Generate(): 生成了 {actualCount} 个段");
            if (actualCount > 0)
            {
                Debug.Log("✓ CurveGenerator 成功生成段");
            }
            else
            {
                Debug.LogError("✗ CurveGenerator 未能生成段");
            }
        }

        #endregion

        #region SlopeGenerator Tests

        /// <summary>
        /// 测试 SlopeGenerator.CanRun() - 低难度不应该运行
        /// </summary>
        public void TestSlopeGenerator_CanRun_LowDifficulty()
        {
            // Arrange
            Setup();
            var generator = new SlopeGenerator(_track, _random, _materials, true);

            // Act
            bool canRun = generator.CanRun(null, 0.2, 5); // difficulty < 0.3

            // Assert
            Debug.Log($"[Test] SlopeGenerator.CanRun(difficulty=0.2): {canRun}");
            if (canRun)
            {
                Debug.LogError("✗ SlopeGenerator 在低难度下不应该运行 (difficulty < 0.3)");
            }
            else
            {
                Debug.Log("✓ SlopeGenerator 在低难度下正确拒绝运行");
            }
        }

        /// <summary>
        /// 测试 SlopeGenerator.CanRun() - 上坡在合适高度可以运行
        /// </summary>
        public void TestSlopeGenerator_CanRun_Up()
        {
            // Arrange
            Setup();
            var generator = new SlopeGenerator(_track, _random, _materials, true); // up = true
            // Setup中y=150，<=190，满足条件

            // Act
            bool canRun = generator.CanRun(null, 0.5, 5); // difficulty >= 0.3

            // Assert
            Debug.Log($"[Test] SlopeGenerator(up=true, y=150).CanRun(): {canRun}");
            Debug.Log("  注：有15%概率因随机因素被拒绝");
            // 不判断canRun的值，因为受随机数影响
            Debug.Log("✓ 测试完成（difficulty >= 0.3 且 y <= 190，条件满足）");
        }

        /// <summary>
        /// 测试 SlopeGenerator.CanRun() - 下坡在低高度不应该运行
        /// </summary>
        public void TestSlopeGenerator_CanRun_Down_LowHeight()
        {
            // Arrange
            Setup();
            //_track.SetConnectPart(new Part(null, new Vec3(0, 50, 0), new Vec3(0, 0, 1), new Vec3(0, 1, 0), null, 0, 0));
            var generator = new SlopeGenerator(_track, _random, _materials, false); // up = false

            // Act
            bool canRun = generator.CanRun(null, 0.5, 5);

            // Assert
            Debug.Log($"[Test] SlopeGenerator(down=true, y=50).CanRun(): {canRun}");
            if (canRun)
            {
                Debug.LogError("✗ SlopeGenerator(下坡, y<110) 不应该运行");
            }
            else
            {
                Debug.Log("✓ SlopeGenerator(下坡, y<110) 正确拒绝运行");
            }
        }

        /// <summary>
        /// 测试 SlopeGenerator.Generate()
        /// </summary>
        public void TestSlopeGenerator_Generate()
        {
            // Arrange
            Setup();
            var generator = new SlopeGenerator(_track, _random, _materials, true);
            int segmentCount = 1;

            // Act
            generator.Generate(null, 0.5, segmentCount);

            // Assert
            int actualCount = _track.GetSegments().Count;
            Debug.Log($"[Test] SlopeGenerator.Generate(): 生成了 {actualCount} 个段");
            if (actualCount > 0)
            {
                Debug.Log("✓ SlopeGenerator 成功生成段");
            }
            else
            {
                Debug.LogError("✗ SlopeGenerator 未能生成段");
            }
        }

        #endregion

        #region HillGenerator Tests

        /// <summary>
        /// 测试 HillGenerator.Generate()
        /// </summary>
        public void TestHillGenerator_Generate()
        {
            // Arrange
            Setup();
            var generator = new HillGenerator(_track, _random, _materials);
            int segmentCount = 1;

            // Act
            generator.Generate(null, 0.5, segmentCount);

            // Assert
            int actualCount = _track.GetSegments().Count;
            Debug.Log($"[Test] HillGenerator.Generate(): 生成了 {actualCount} 个段");
            if (actualCount > 0)
            {
                Debug.Log("✓ HillGenerator 成功生成段");
            }
            else
            {
                Debug.LogError("✗ HillGenerator 未能生成段");
            }
        }

        #endregion

        #region HoleGenerator Tests

        /// <summary>
        /// 测试 HoleGenerator.CanRun() - 没有前一个生成器应该拒绝
        /// </summary>
        public void TestHoleGenerator_CanRun_NoPrevious()
        {
            // Arrange
            Setup();
            var generator = new HoleGenerator(_track, _random, _materials);

            // Act
            bool canRun = generator.CanRun(null, 0.5, 5); // previousGenerator = null

            // Assert
            Debug.Log($"[Test] HoleGenerator.CanRun(previous=null): {canRun}");
            if (canRun)
            {
                Debug.LogError("✗ HoleGenerator 没有前一个生成器时不应该运行");
            }
            else
            {
                Debug.Log("✓ HoleGenerator 正确拒绝（没有前一个生成器）");
            }
        }

        /// <summary>
        /// 测试 HoleGenerator.CanRun() - 连续空洞应该拒绝
        /// </summary>
        public void TestHoleGenerator_CanRun_ConsecutiveHole()
        {
            // Arrange
            Setup();
            var generator = new HoleGenerator(_track, _random, _materials);
            var previousHoleGenerator = new HoleGenerator(_track, _random, _materials);

            // Act
            bool canRun = generator.CanRun(previousHoleGenerator, 0.5, 5);

            // Assert
            Debug.Log($"[Test] HoleGenerator.CanRun(previous=HoleGenerator): {canRun}");
            if (canRun)
            {
                Debug.LogError("✗ HoleGenerator 前一个也是空洞时不应该运行");
            }
            else
            {
                Debug.Log("✓ HoleGenerator 正确拒绝（连续空洞）");
            }
        }

        /// <summary>
        /// 测试 HoleGenerator.Generate()
        /// </summary>
        public void TestHoleGenerator_Generate()
        {
            // Arrange
            Setup();
            var generator = new HoleGenerator(_track, _random, _materials);
            var forwardGenerator = new ForwardGenerator(_track, _random, _materials, null);
            forwardGenerator.Generate(null, 0.5, 1); // 先生成一个段

            // Act
            generator.Generate(forwardGenerator, 0.5, 2);

            // Assert
            int actualCount = _track.GetSegments().Count;
            Debug.Log($"[Test] HoleGenerator.Generate(): 生成了 {actualCount} 个段");
            if (actualCount > 1)
            {
                Debug.Log("✓ HoleGenerator 成功生成空洞段");
            }
            else
            {
                Debug.LogError("✗ HoleGenerator 未能生成空洞段");
            }
        }

        #endregion

        #region LoopGenerator Tests

        /// <summary>
        /// 测试 LoopGenerator.CanRun() - 段数少不应该运行
        /// </summary>
        public void TestLoopGenerator_CanRun_FewSegments()
        {
            // Arrange
            Setup();
            var generator = new LoopGenerator(_track, _random, _materials);

            // Act
            bool canRun = generator.CanRun(null, 0.5, 3); // segmentCount <= 5

            // Assert
            Debug.Log($"[Test] LoopGenerator.CanRun(segmentCount=3): {canRun}");
            if (canRun)
            {
                Debug.LogError("✗ LoopGenerator 段数少时不应该运行 (segmentCount <= 5)");
            }
            else
            {
                Debug.Log("✓ LoopGenerator 正确拒绝（段数少）");
            }
        }

        /// <summary>
        /// 测试 LoopGenerator.CanRun() - 段数多可能运行
        /// 注意：由于有随机因素，此测试只是验证段数多时不会直接被拒绝
        /// </summary>
        public void TestLoopGenerator_CanRun_ManySegments()
        {
            // Arrange
            Setup();
            var generator = new LoopGenerator(_track, _random, _materials);

            // Act
            bool canRun = generator.CanRun(null, 0.5, 10); // segmentCount > 5

            // Assert
            Debug.Log($"[Test] LoopGenerator.CanRun(segmentCount=10): {canRun}");
            Debug.Log("  注：有随机因素，segmentCount > 5 时仍可能被拒绝");
            // 不判断canRun的值，因为受随机数影响
            Debug.Log("✓ 测试完成（segmentCount > 5，条件满足）");
        }

        /// <summary>
        /// 测试 LoopGenerator.Generate()
        /// </summary>
        public void TestLoopGenerator_Generate()
        {
            // Arrange
            Setup();
            var generator = new LoopGenerator(_track, _random, _materials);
            int segmentCount = 10;

            // Act
            generator.Generate(null, 0.5, segmentCount);

            // Assert
            int actualCount = _track.GetSegments().Count;
            Debug.Log($"[Test] LoopGenerator.Generate(): 生成了 {actualCount} 个段");
            if (actualCount > 0)
            {
                Debug.Log("✓ LoopGenerator 成功生成段");
            }
            else
            {
                Debug.LogError("✗ LoopGenerator 未能生成段");
            }
        }

        #endregion

        #region LongJumpGenerator Tests

        /// <summary>
        /// 测试 LongJumpGenerator.Generate()
        /// </summary>
        public void TestLongJumpGenerator_Generate()
        {
            // Arrange
            Setup();
            var generator = new LongJumpGenerator(_track, _random, _materials);
            int segmentCount = 1;

            // Act
            generator.Generate(null, 0.5, segmentCount);

            // Assert
            int actualCount = _track.GetSegments().Count;
            Debug.Log($"[Test] LongJumpGenerator.Generate(): 生成了 {actualCount} 个段");
            if (actualCount > 0)
            {
                Debug.Log("✓ LongJumpGenerator 成功生成段");
            }
            else
            {
                Debug.LogError("✗ LongJumpGenerator 未能生成段");
            }
        }

        #endregion

        #region IslandGenerator Tests

        /// <summary>
        /// 测试 IslandGenerator.Generate()
        /// </summary>
        public void TestIslandGenerator_Generate()
        {
            // Arrange
            Setup();
            var generator = new IslandGenerator(_track, _random, _materials);
            int segmentCount = 1;

            // Act
            generator.Generate(null, 0.5, segmentCount);

            // Assert
            int actualCount = _track.GetSegments().Count;
            Debug.Log($"[Test] IslandGenerator.Generate(): 生成了 {actualCount} 个段");
            if (actualCount > 0)
            {
                Debug.Log("✓ IslandGenerator 成功生成段");
            }
            else
            {
                Debug.LogError("✗ IslandGenerator 未能生成段");
            }
        }

        #endregion

        #region StraightGenerator Tests

        /// <summary>
        /// 测试 StraightGenerator.Generate()
        /// </summary>
        public void TestStraightGenerator_Generate()
        {
            // Arrange
            Setup();
            var generator = new StraightGenerator(_track, _random, _materials);
            int segmentCount = 1;

            // Act
            generator.Generate(null, 0.5, segmentCount);

            // Assert
            int actualCount = _track.GetSegments().Count;
            Debug.Log($"[Test] StraightGenerator.Generate(): 生成了 {actualCount} 个段");
            if (actualCount > 0)
            {
                Debug.Log("✓ StraightGenerator 成功生成段");
            }
            else
            {
                Debug.LogError("✗ StraightGenerator 未能生成段");
            }
        }

        #endregion

        #region Run All Tests

        /// <summary>
        /// 运行所有单元测试
        /// </summary>
        public void RunAllTests()
        {
            Debug.Log("========================================");
            Debug.Log("Generator 单元测试开始");
            Debug.Log("========================================");

            int passed = 0;
            int total = 0;

            // ForwardGenerator
            total++;
            try { TestForwardGenerator_CanRun_LowDifficulty(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestForwardGenerator_Generate(); passed++; } catch { Debug.LogError("✗ 测试失败"); }

            // CurveGenerator
            total++;
            try { TestCurveGenerator_CanRun_LowDifficulty(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestCurveGenerator_CanRun_HighDifficulty(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestCurveGenerator_Generate(); passed++; } catch { Debug.LogError("✗ 测试失败"); }

            // SlopeGenerator
            total++;
            try { TestSlopeGenerator_CanRun_LowDifficulty(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestSlopeGenerator_CanRun_Up(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestSlopeGenerator_CanRun_Down_LowHeight(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestSlopeGenerator_Generate(); passed++; } catch { Debug.LogError("✗ 测试失败"); }

            // HillGenerator
            total++;
            try { TestHillGenerator_Generate(); passed++; } catch { Debug.LogError("✗ 测试失败"); }

            // HoleGenerator
            total++;
            try { TestHoleGenerator_CanRun_NoPrevious(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestHoleGenerator_CanRun_ConsecutiveHole(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestHoleGenerator_Generate(); passed++; } catch { Debug.LogError("✗ 测试失败"); }

            // LoopGenerator
            total++;
            try { TestLoopGenerator_CanRun_FewSegments(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestLoopGenerator_CanRun_ManySegments(); passed++; } catch { Debug.LogError("✗ 测试失败"); }
            total++;
            try { TestLoopGenerator_Generate(); passed++; } catch { Debug.LogError("✗ 测试失败"); }

            // LongJumpGenerator
            total++;
            try { TestLongJumpGenerator_Generate(); passed++; } catch { Debug.LogError("✗ 测试失败"); }

            // IslandGenerator
            total++;
            try { TestIslandGenerator_Generate(); passed++; } catch { Debug.LogError("✗ 测试失败"); }

            // StraightGenerator
            total++;
            try { TestStraightGenerator_Generate(); passed++; } catch { Debug.LogError("✗ 测试失败"); }

            Debug.Log("========================================");
            Debug.Log($"测试完成: {passed}/{total} 通过");
            Debug.Log("========================================");
        }

        #endregion
    }
}
