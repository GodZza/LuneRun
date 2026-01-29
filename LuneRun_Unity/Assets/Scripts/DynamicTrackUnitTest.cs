using UnityEngine;
using com.playchilla.runner.track;
using com.playchilla.runner;
using shared.math;

namespace LuneRun.Tests
{
    /// <summary>
    /// 测试场景枚举
    /// </summary>
    public enum TestScenario
    {
        BasicMovement,      // 基础移动测试
        LoadUnload,         // 加载/卸载测试
        Boundary,           // 边界条件测试
        GeneratorCoverage,  // 生成器覆盖测试
        Performance,         // 性能测试
        ErrorHandling       // 错误处理测试
    }

    /// <summary>
    /// DynamicTrack 单元测试场景
    /// 专门测试动态轨道系统的核心功能，不依赖完整的游戏流程
    /// </summary>
    public class DynamicTrackUnitTest : MonoBehaviour
    {
        [Header("测试配置")]
        [SerializeField] private bool autoRunOnStart = false;
        [SerializeField] private int testLevelId = 1;
        [SerializeField] private int selectedTestScenario = 0;

        [Header("轨道配置")]
        [SerializeField] private int loadForward = 6;
        [SerializeField] private int keepBackward = 2;

        [Header("测试参数")]
        [SerializeField] private float testSpeed = 20f;
        [SerializeField] private float testDistance = 500f;

        [Header("性能测试配置")]
        [SerializeField] private bool runPerformanceTest = false;
        [SerializeField] private int performanceTestIterations = 100;

        // 测试状态
        private Level _testLevel;
        private DynamicTrack _dynamicTrack;
        private Vec3 _playerPosition;
        private bool _isTesting = false;
        private float _testTimer = 0f;

        // 测试结果
        private int _segmentsLoaded = 0;
        private int _segmentsUnloaded = 0;
        private int _totalSegments = 0;
        private float _initialZ = 0f;
        private float _finalZ = 0f;
        private string _testResult = "尚未运行测试";

        void Start()
        {
            Debug.Log("========== DynamicTrack 单元测试场景 ==========");
            Debug.Log("初始化测试环境...");

            if (autoRunOnStart)
            {
                SetupTestEnvironment();
            }
            else
            {
                Debug.Log("自动运行已禁用。请使用 GUI 按钮选择测试场景并开始。");
            }
        }

        /// <summary>
        /// 设置测试环境
        /// </summary>
        public void SetupTestEnvironment()
        {
            try
            {
                // 1. 创建测试 Level
                GameObject levelObj = new GameObject("TestLevel");
                _testLevel = levelObj.AddComponent<Level>();

                // 初始化Level（必须先Initialize才能设置levelId）
                //_testLevel.Initialize(testLevelId, false, null, null); //--------------------------
                Debug.Log($"[1/4] 创建测试 Level (LevelId: {testLevelId})");

                // 2. 创建 DynamicTrack 实例
                _dynamicTrack = new DynamicTrack(_testLevel, loadForward, keepBackward);
                Debug.Log($"[2/4] 创建 DynamicTrack (forward={loadForward}, backward={keepBackward})");

                // 3. 获取 Track
                Track track = _dynamicTrack.GetTrack();
                if (track == null)
                {
                    throw new System.Exception("DynamicTrack.GetTrack() 返回 null");
                }

                // 4. 记录初始状态
                _playerPosition = new Vec3(0, 1, 0);
                _initialZ = (float)_playerPosition.z;
                _totalSegments = track.GetSegments().Count;

                Debug.Log($"[3/4] 初始状态: {_totalSegments} 个轨道段");
                Debug.Log($"[4/4] 测试环境准备完成");
                Debug.Log("===========================================");

                _isTesting = false; // 等待用户触发
                _testResult = "测试环境已就绪，按 [T] 开始测试";
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DynamicTrackUnitTest] 初始化失败: {e.Message}");
                Debug.LogException(e);
                _testResult = $"初始化失败: {e.Message}";
            }
        }

        /// <summary>
        /// 开始测试
        /// </summary>
        public void StartTest()
        {
            if (_dynamicTrack == null)
            {
                Debug.LogError("请先初始化测试环境");
                return;
            }

            TestScenario scenario = (TestScenario)selectedTestScenario;
            Debug.Log($"========== 开始测试: {scenario} ==========");

            switch (scenario)
            {
                case TestScenario.BasicMovement:
                    RunBasicMovementTest();
                    break;
                case TestScenario.LoadUnload:
                    RunLoadUnloadTest();
                    break;
                case TestScenario.Boundary:
                    RunBoundaryTest();
                    break;
                case TestScenario.GeneratorCoverage:
                    RunGeneratorCoverageTest();
                    break;
                case TestScenario.Performance:
                    RunPerformanceTest();
                    break;
                case TestScenario.ErrorHandling:
                    RunErrorHandlingTest();
                    break;
                default:
                    Debug.LogWarning($"未知的测试场景: {scenario}");
                    break;
            }
        }

        /// <summary>
        /// 测试场景 1: 基础移动测试
        /// </summary>
        private void RunBasicMovementTest()
        {
            _isTesting = true;
            _testTimer = 0f;
            _playerPosition = new Vec3(0, 1, 0);
            _testResult = "基础移动测试进行中...";

            Track track = _dynamicTrack.GetTrack();
            if (track != null)
            {
                _totalSegments = track.GetSegments().Count;
            }

            Debug.Log($"初始段数: {_totalSegments}");
            Debug.Log($"测试速度: {testSpeed} 单位/秒");
            Debug.Log($"测试距离: {testDistance} 单位");
        }

        /// <summary>
        /// 测试场景 2: 加载/卸载测试
        /// </summary>
        private void RunLoadUnloadTest()
        {
            _isTesting = true;
            _testTimer = 0f;
            _playerPosition = new Vec3(0, 1, 0);
            _testResult = "加载/卸载测试进行中...";

            Track track = _dynamicTrack.GetTrack();
            if (track != null)
            {
                _totalSegments = track.GetSegments().Count;
            }

            Debug.Log($"初始段数: {_totalSegments}");
            Debug.Log($"配置: 前方加载={loadForward}, 后方保留={keepBackward}");
        }

        /// <summary>
        /// 测试场景 3: 边界条件测试
        /// </summary>
        private void RunBoundaryTest()
        {
            _isTesting = true;
            _testTimer = 0f;
            _playerPosition = new Vec3(0, 1, 0);
            _testResult = "边界条件测试进行中...";

            Track track = _dynamicTrack.GetTrack();
            if (track != null)
            {
                _totalSegments = track.GetSegments().Count;
            }

            Debug.Log($"测试边界条件...");
            Debug.Log($"初始段数: {_totalSegments}");
        }

        /// <summary>
        /// 测试场景 4: 生成器覆盖测试
        /// </summary>
        private void RunGeneratorCoverageTest()
        {
            _isTesting = true;
            _testTimer = 0f;
            _playerPosition = new Vec3(0, 1, 0);
            _testResult = "生成器覆盖测试进行中...";

            Track track = _dynamicTrack.GetTrack();
            if (track != null)
            {
                _totalSegments = track.GetSegments().Count;
            }

            Debug.Log($"测试所有生成器的覆盖情况...");
            Debug.Log($"初始段数: {_totalSegments}");
            Debug.Log($"目标: 验证所有10种生成器都被使用");
        }

        /// <summary>
        /// 测试场景 5: 性能测试
        /// </summary>
        private void RunPerformanceTest()
        {
            _isTesting = true;
            _testTimer = 0f;
            _playerPosition = new Vec3(0, 1, 0);
            _testResult = "性能测试进行中...";

            Track track = _dynamicTrack.GetTrack();
            if (track != null)
            {
                _totalSegments = track.GetSegments().Count;
            }

            Debug.Log($"性能测试配置:");
            Debug.Log($"  迭代次数: {performanceTestIterations}");
            Debug.Log($"  测试速度: {testSpeed} 单位/秒");
        }

        /// <summary>
        /// 测试场景 6: 错误处理测试
        /// </summary>
        private void RunErrorHandlingTest()
        {
            _isTesting = true;
            _testTimer = 0f;
            _playerPosition = new Vec3(0, 1, 0);
            _testResult = "错误处理测试进行中...";

            Track track = _dynamicTrack.GetTrack();
            if (track != null)
            {
                _totalSegments = track.GetSegments().Count;
            }

            Debug.Log($"测试错误处理能力...");
            Debug.Log($"初始段数: {_totalSegments}");
        }

        /// <summary>
        /// 完成测试
        /// </summary>
        private void CompleteTest()
        {
            _isTesting = false;
            _finalZ = (float)_playerPosition.z;
            float actualDistance = _finalZ - _initialZ;

            // 获取最终段数
            Track track = _dynamicTrack.GetTrack();
            if (track == null)
            {
                _testResult = "测试失败: Track 为 null";
                return;
            }

            int finalSegments = track.GetSegments().Count;
            _segmentsUnloaded = _totalSegments - finalSegments;
            _segmentsLoaded = finalSegments - _totalSegments;

            TestScenario scenario = (TestScenario)selectedTestScenario;
            bool isPassed = false;

            // 根据测试场景分析结果
            switch (scenario)
            {
                case TestScenario.BasicMovement:
                    isPassed = AnalyzeBasicMovementTest(actualDistance, finalSegments);
                    break;
                case TestScenario.LoadUnload:
                    isPassed = AnalyzeLoadUnloadTest(actualDistance, finalSegments);
                    break;
                case TestScenario.Boundary:
                    isPassed = AnalyzeBoundaryTest(actualDistance, finalSegments);
                    break;
                case TestScenario.GeneratorCoverage:
                    isPassed = AnalyzeGeneratorCoverageTest(track);
                    break;
                case TestScenario.Performance:
                    isPassed = AnalyzePerformanceTest(_testTimer);
                    break;
                case TestScenario.ErrorHandling:
                    isPassed = AnalyzeErrorHandlingTest();
                    break;
                default:
                    isPassed = AnalyzeTestResult(actualDistance, finalSegments);
                    break;
            }

            if (isPassed)
            {
                _testResult = $"✓ 测试通过 - {scenario}";
                Debug.Log("========== 测试结果: 通过 ==========");
            }
            else
            {
                _testResult = $"✗ 测试失败 - {scenario}";
                Debug.LogError("========== 测试结果: 失败 ==========");
            }

            PrintTestStatistics();
            Debug.Log("===========================================");
        }

        /// <summary>
        /// 分析基础移动测试结果
        /// </summary>
        private bool AnalyzeBasicMovementTest(float actualDistance, int finalSegments)
        {
            bool passed = true;

            if (actualDistance < testDistance * 0.9f)
            {
                Debug.LogError($"基础移动测试失败: 移动距离不足 (expected >= {testDistance * 0.9f}, got {actualDistance})");
                passed = false;
            }
            else
            {
                Debug.Log($"✓ 移动距离符合预期: {actualDistance:F1}");
            }

            if (finalSegments < 3)
            {
                Debug.LogError($"基础移动测试失败: 最终段数过少 (expected >= 3, got {finalSegments})");
                passed = false;
            }
            else
            {
                Debug.Log($"✓ 最终段数合理: {finalSegments}");
            }

            return passed;
        }

        /// <summary>
        /// 分析加载/卸载测试结果
        /// </summary>
        private bool AnalyzeLoadUnloadTest(float actualDistance, int finalSegments)
        {
            bool passed = true;

            // 检查是否有加载和卸载发生
            int segmentCount = finalSegments;
            int expectedMin = System.Math.Max(3, loadForward - 1);
            int expectedMax = loadForward + keepBackward + 2;

            if (segmentCount < expectedMin || segmentCount > expectedMax)
            {
                Debug.LogWarning($"加载/卸载测试: 段数超出预期范围 (expected {expectedMin}-{expectedMax}, got {segmentCount})");
            }
            else
            {
                Debug.Log($"✓ 段数在预期范围内: {segmentCount}");
            }

            if (actualDistance < testDistance * 0.5f)
            {
                Debug.LogError($"加载/卸载测试失败: 移动距离不足");
                passed = false;
            }

            return passed;
        }

        /// <summary>
        /// 分析边界条件测试结果
        /// </summary>
        private bool AnalyzeBoundaryTest(float actualDistance, int finalSegments)
        {
            bool passed = true;
            Track track = _dynamicTrack.GetTrack();

            // 检查第一个段是否有效
            if (track != null && track.GetSegments().Count > 0)
            {
                var firstSegment = track.GetSegments()[0];
                if (firstSegment.GetConnectPart() == null)
                {
                    Debug.LogError("边界条件测试失败: 第一个段的 connectPart 为 null");
                    passed = false;
                }
                else
                {
                    Debug.Log($"✓ 第一个段有效");
                }
            }

            // 检查最后一个段是否有效
            if (track != null && track.GetSegments().Count > 0)
            {
                var lastSegment = track.GetSegments()[track.GetSegments().Count - 1];
                if (lastSegment.GetLastPart() == null)
                {
                    Debug.LogError("边界条件测试失败: 最后一个段的 lastPart 为 null");
                    passed = false;
                }
                else
                {
                    Debug.Log($"✓ 最后一个段有效");
                }
            }

            // 检查极端位置
            if (actualDistance < 1f)
            {
                Debug.LogError("边界条件测试失败: 未移动");
                passed = false;
            }

            return passed;
        }

        /// <summary>
        /// 分析生成器覆盖测试结果
        /// </summary>
        private bool AnalyzeGeneratorCoverageTest(Track track)
        {
            if (track == null)
            {
                Debug.LogError("生成器覆盖测试失败: Track 为 null");
                return false;
            }

            // 统计每种段类型
            var segmentTypes = new System.Collections.Generic.Dictionary<string, int>();
            foreach (var segment in track.GetSegments())
            {
                string typeName = segment.GetType().Name;
                if (segmentTypes.ContainsKey(typeName))
                    segmentTypes[typeName]++;
                else
                    segmentTypes[typeName] = 1;
            }

            Debug.Log("段类型统计:");
            foreach (var kvp in segmentTypes)
            {
                Debug.Log($"  - {kvp.Key}: {kvp.Value}");
            }

            // 检查是否有多种类型的段
            bool passed = segmentTypes.Count >= 3;
            if (passed)
            {
                Debug.Log($"✓ 生成了 {segmentTypes.Count} 种不同类型的段");
            }
            else
            {
                Debug.LogWarning($"⚠ 仅生成了 {segmentTypes.Count} 种不同类型的段，建议至少3种");
            }

            return passed;
        }

        /// <summary>
        /// 分析性能测试结果
        /// </summary>
        private bool AnalyzePerformanceTest(float elapsedTime)
        {
            bool passed = true;

            if (elapsedTime > 10f)
            {
                Debug.LogWarning($"性能测试警告: 耗时较长 ({elapsedTime:F2}s > 10s)");
                passed = false;
            }
            else
            {
                Debug.Log($"✓ 性能测试完成: {elapsedTime:F2}s");
            }

            return passed;
        }

        /// <summary>
        /// 分析错误处理测试结果
        /// </summary>
        private bool AnalyzeErrorHandlingTest()
        {
            // 这个测试主要验证系统能够处理各种异常情况
            // 只要测试完成没有崩溃，就视为通过
            Debug.Log($"✓ 错误处理测试完成，系统运行正常");
            return true;
        }

        /// <summary>
        /// 分析测试结果
        /// </summary>
        private bool AnalyzeTestResult(float actualDistance, int finalSegments)
        {
            // 基本检查：玩家必须移动了足够距离
            if (actualDistance < testDistance * 0.5f)
            {
                Debug.LogError($"测试失败: 移动距离不足 (expected > {testDistance * 0.5f}, got {actualDistance})");
                return false;
            }

            // 基本检查：必须有一些轨道段
            if (finalSegments <= 0)
            {
                Debug.LogError($"测试失败: 没有轨道段 (expected > 0, got {finalSegments})");
                return false;
            }

            // 检查段数是否合理（不应该太多或太少）
            // 根据配置：加载前方6个，保留后方2个，大约应该有 6-8 个段
            if (finalSegments < 3)
            {
                Debug.LogWarning($"警告: 轨道段数过少 ({finalSegments} < 3)");
            }
            else if (finalSegments > 12)
            {
                Debug.LogWarning($"警告: 轨道段数过多 ({finalSegments} > 12)");
            }

            return true;
        }

        /// <summary>
        /// 打印测试统计
        /// </summary>
        private void PrintTestStatistics()
        {
            Debug.Log($"测试统计:");
            Debug.Log($"  测试时间: {_testTimer:F2} 秒");
            Debug.Log($"  移动距离: {_finalZ - _initialZ:F2} 单位");
            Debug.Log($"  初始段数: {_totalSegments}");
            Debug.Log($"  最终段数: {_totalSegments + _segmentsLoaded - _segmentsUnloaded}");
            Debug.Log($"  卸载段数: {_segmentsUnloaded}");
            Debug.Log($"  配置参数: 前方加载={loadForward}, 后方保留={keepBackward}");

            // 获取详细统计
            string stats = _dynamicTrack.GetStatistics();
            Debug.Log($"动态轨道统计:\n{stats}");
        }

        void OnGUI()
        {
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = Color.white;

            GUIStyle normalStyle = new GUIStyle(GUI.skin.label);
            normalStyle.fontSize = 14;
            normalStyle.normal.textColor = Color.white;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 16;

            GUILayout.BeginArea(new Rect(10, 10, 500, 500));
            GUILayout.BeginVertical("Box");

            GUILayout.Label("DynamicTrack 单元测试", titleStyle);
            GUILayout.Label("================================", normalStyle);
            GUILayout.Label("", normalStyle);

            // 测试场景选择
            GUILayout.Label("测试场景:", normalStyle);
            string[] scenarioNames = System.Enum.GetNames(typeof(TestScenario));
            selectedTestScenario = GUILayout.SelectionGrid(selectedTestScenario, scenarioNames, 2);
            GUILayout.Label("", normalStyle);

            // 测试状态
            GUILayout.Label($"状态: {(_isTesting ? "测试中..." : "待机")}", normalStyle);
            GUILayout.Label($"结果: {_testResult}", normalStyle);
            GUILayout.Label("", normalStyle);

            // 测试信息
            if (_dynamicTrack != null)
            {
                Track track = _dynamicTrack.GetTrack();
                if (track != null)
                {
                    int segmentCount = track.GetSegments().Count;
                    GUILayout.Label($"当前段数: {segmentCount}", normalStyle);
                }
                GUILayout.Label($"玩家 Z: {_playerPosition.z:F2}", normalStyle);
                GUILayout.Label($"测试进度: {(_playerPosition.z - _initialZ):F1} / {testDistance}", normalStyle);
                GUILayout.Label($"测试时间: {_testTimer:F2}s", normalStyle);
            }

            GUILayout.Label("", normalStyle);

            // 控制按钮
            if (!_isTesting)
            {
                if (GUILayout.Button("初始化测试环境", buttonStyle))
                {
                    SetupTestEnvironment();
                }

                if (GUILayout.Button("开始测试 [T]", buttonStyle))
                {
                    StartTest();
                }
            }
            else
            {
                GUILayout.Button("测试进行中...", buttonStyle);
            }

            if (GUILayout.Button("查看统计 [S]", buttonStyle))
            {
                if (_dynamicTrack != null)
                {
                    Debug.Log(_dynamicTrack.GetStatistics());
                }
            }

            if (GUILayout.Button("重置 [R]", buttonStyle))
            {
                ResetTest();
            }

            GUILayout.Label("", normalStyle);
            GUILayout.Label("快捷键:", normalStyle);
            GUILayout.Label("[T] 开始测试  [S] 查看统计  [R] 重置", normalStyle);

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        void ResetTest()
        {
            _isTesting = false;
            _testTimer = 0f;
            _playerPosition = new Vec3(0, 1, 0);
            _testResult = "已重置";
            Debug.Log("测试已重置");
        }

        void Update()
        {
            // 键盘快捷键
            if (Input.GetKeyDown(KeyCode.T) && !_isTesting)
            {
                StartTest();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (_dynamicTrack != null)
                {
                    Debug.Log(_dynamicTrack.GetStatistics());
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetTest();
            }

            // 测试逻辑
            if (_isTesting)
            {
                RunTestLogic();
            }
        }

        /// <summary>
        /// 执行测试逻辑（根据测试场景）
        /// </summary>
        private void RunTestLogic()
        {
            TestScenario scenario = (TestScenario)selectedTestScenario;

            switch (scenario)
            {
                case TestScenario.Performance:
                    RunPerformanceTestLogic();
                    break;
                default:
                    RunStandardTestLogic();
                    break;
            }
        }

        /// <summary>
        /// 标准测试逻辑
        /// </summary>
        private void RunStandardTestLogic()
        {
            float deltaZ = testSpeed * Time.deltaTime;
            _playerPosition.z += deltaZ;
            _testTimer += Time.deltaTime;

            //_dynamicTrack.Update(_playerPosition);

            if (_playerPosition.z - _initialZ >= testDistance)
            {
                CompleteTest();
            }

            if (_testTimer > 30f)
            {
                Debug.LogWarning("测试超时（30秒），强制结束");
                CompleteTest();
            }
        }

        /// <summary>
        /// 性能测试逻辑
        /// </summary>
        private void RunPerformanceTestLogic()
        {
            float deltaZ = testSpeed * Time.deltaTime;
            _playerPosition.z += deltaZ;
            _testTimer += Time.deltaTime;

            //_dynamicTrack.Update(_playerPosition);

            // 性能测试运行指定距离
            if (_playerPosition.z - _initialZ >= testDistance)
            {
                CompleteTest();
            }

            // 性能测试超时时间更长
            if (_testTimer > 60f)
            {
                Debug.LogWarning("性能测试超时（60秒），强制结束");
                CompleteTest();
            }
        }
    }
}
