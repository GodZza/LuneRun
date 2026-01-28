using UnityEngine;
using com.playchilla.runner.track;
using com.playchilla.runner;
using shared.math;

namespace LuneRun.Tests
{
    /// <summary>
    /// DynamicTrack 单元测试场景
    /// 专门测试动态轨道系统的核心功能，不依赖完整的游戏流程
    /// </summary>
    public class DynamicTrackUnitTest : MonoBehaviour
    {
        [Header("测试配置")]
        [SerializeField] private bool autoRunOnStart = true;
        [SerializeField] private int testLevelId = 1;

        [Header("轨道配置")]
        [SerializeField] private int loadForward = 6;
        [SerializeField] private int keepBackward = 2;

        [Header("测试参数")]
        [SerializeField] private float testSpeed = 20f;
        [SerializeField] private float testDistance = 500f;

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

            Debug.Log("========== 开始测试 ==========");
            _isTesting = true;
            _testTimer = 0f;
            _playerPosition = new Vec3(0, 1, 0);
            _testResult = "测试进行中...";

            // 记录初始段数
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

            // 分析测试结果
            bool isPassed = AnalyzeTestResult(actualDistance, finalSegments);

            if (isPassed)
            {
                _testResult = $"✓ 测试通过 - 移动了 {actualDistance:F1} 单位，最终 {finalSegments} 个轨道段";
                Debug.Log("========== 测试结果: 通过 ==========");
            }
            else
            {
                _testResult = $"✗ 测试失败 - 移动了 {actualDistance:F1} 单位，最终 {finalSegments} 个轨道段";
                Debug.LogError("========== 测试结果: 失败 ==========");
            }

            PrintTestStatistics();
            Debug.Log("===========================================");
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

            GUILayout.BeginArea(new Rect(10, 10, 450, 400));
            GUILayout.BeginVertical("Box");

            GUILayout.Label("DynamicTrack 单元测试", titleStyle);
            GUILayout.Label("================================", normalStyle);
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
                float deltaZ = testSpeed * Time.deltaTime;
                _playerPosition.z += deltaZ;
                _testTimer += Time.deltaTime;

                _dynamicTrack.Update(_playerPosition);

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
        }
    }
}
