using UnityEngine;
using com.playchilla.runner;
using com.playchilla.runner.player;
using com.playchilla.runner.track;
using com.playchilla.runner.track.entity;
using shared.math;

namespace LuneRun.Tests
{
    /// <summary>
    /// 核心玩法测试场景
    /// 验证游戏的基本功能：移动、跳跃、重力、碰撞、死亡判定
    /// </summary>
    public class CoreGameplayTest : MonoBehaviour
    {
        [Header("测试配置")]
        [SerializeField] private bool autoRunOnStart = true;
        [SerializeField] private int testLevelId = 1;

        [Header("测试参数")]
        [SerializeField] private float testDuration = 30f; // 测试持续时间（秒）

        // 测试状态
        private Level _testLevel;
        private Player _player;
        private bool _isTesting = false;
        private float _testTimer = 0f;
        private string _testResult = "尚未运行测试";

        // 测试结果
        private int _jumpCount = 0;
        private int _landCount = 0;
        private int _speedEntityCount = 0;
        private float _maxSpeed = 0f;
        private float _totalDistance = 0f;
        private bool _playerDied = false;

        // 测试用例
        private Vec3 _startPos;
        private Vec3 _lastPos;

        // 测试日志和错误收集
        private System.Collections.Generic.List<string> _testLogs = new System.Collections.Generic.List<string>();
        private System.Collections.Generic.List<string> _testErrors = new System.Collections.Generic.List<string>();

        void Start()
        {
            LogMessage("========== 核心玩法测试场景 ==========");
            LogMessage("初始化测试环境...");

            if (autoRunOnStart)
            {
                SetupTestEnvironment();
            }
            else
            {
                LogMessage("自动运行已禁用。请使用 GUI 按钮开始测试。");
            }
        }

        /// <summary>
        /// 记录测试日志
        /// </summary>
        private void LogMessage(string message)
        {
            _testLogs.Add($"[{Time.time:F2}] {message}");
            Debug.Log(message);
        }

        /// <summary>
        /// 记录测试错误
        /// </summary>
        private void LogError(string message)
        {
            _testErrors.Add($"[{Time.time:F2}] {message}");
            Debug.LogError(message);
        }

        /// <summary>
        /// 记录测试警告
        /// </summary>
        private void LogWarning(string message)
        {
            _testLogs.Add($"[WARNING] [{Time.time:F2}] {message}");
            Debug.LogWarning(message);
        }

        void Update()
        {
            if (!_isTesting)
            {
                // 按 T 键开始测试
                if (Input.GetKeyDown(KeyCode.T))
                {
                    StartTest();
                }
                return;
            }

            _testTimer += Time.deltaTime;

            // 记录统计数据
            UpdateTestStatistics();

            // 测试结束条件
            if (_testTimer >= testDuration || _playerDied || _player.IsDead())
            {
                EndTest();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                LogMessage("用户手动结束测试");
                EndTest();
            }

            // 模拟按键（测试用）
            // 按住空格键加速
            if (Input.GetKey(KeyCode.Space))
            {
                // 空格键由 Level.Update() 中的 UpdateKeyboardInput() 处理
            }
            // 松开空格键跳跃
            if (Input.GetKeyUp(KeyCode.Space))
            {
                // 松开空格键由 Level.Update() 中的 UpdateKeyboardInput() 处理
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
                _testLevel.Initialize(testLevelId, false, null, null);
                LogMessage($"[1/5] 创建测试 Level (LevelId: {testLevelId})");

                // 2. 获取 Player
                _player = _testLevel.GetPlayer();
                if (_player == null)
                {
                    throw new System.Exception("无法获取 Player");
                }
                LogMessage($"[2/5] 获取 Player");

                // 3. 添加速度实体（测试收集物品功能）
                AddTestSpeedEntities();
                LogMessage($"[3/5] 添加测试实体");

                // 4. 记录初始状态
                _startPos = _player.GetPos().clone();
                _lastPos = _startPos.clone();
                _testTimer = 0f;
                LogMessage($"[4/5] 初始位置: ({_startPos.x:F2}, {_startPos.y:F2}, {_startPos.z:F2})");

                // 5. 注册死亡检测回调
                // 注意：Player.IsDead() 方法返回 _dead 状态
                // 我们需要在 Update() 中轮询检查

                LogMessage($"[5/5] 测试环境准备完成");
                LogMessage("===========================================");

                _isTesting = false;
                _testResult = "测试环境已就绪，按 [T] 开始测试，按 [空格] 加速，按 [ESC] 结束测试";
            }
            catch (System.Exception e)
            {
                LogError($"[CoreGameplayTest] 初始化失败: {e.Message}");
                LogError($"[CoreGameplayTest] 堆栈跟踪: {e.StackTrace}");
                _testResult = $"初始化失败: {e.Message}";
                PrintTestReport();
            }
        }

        /// <summary>
        /// 添加测试速度实体
        /// </summary>
        private void AddTestSpeedEntities()
        {
            World world = _testLevel.GetWorld();
            if (world == null) return;

            // 在轨道上添加一些速度实体
            for (int i = 0; i < 5; i++)
            {
                Vec3 position = new Vec3(0, 2, 10 + i * 20);
                SpeedEntity speedEntity = new SpeedEntity(position, _testLevel, null);
                world.AddEntity(speedEntity);
            }
        }

        /// <summary>
        /// 开始测试
        /// </summary>
        public void StartTest()
        {
            if (_player == null)
            {
                LogError("请先初始化测试环境");
                return;
            }

            _isTesting = true;
            _testTimer = 0f;
            _testResult = "测试进行中...";

            // 重置统计数据
            _jumpCount = 0;
            _landCount = 0;
            _speedEntityCount = 0;
            _maxSpeed = 0f;
            _totalDistance = 0f;
            _playerDied = false;

            LogMessage("========== 开始核心玩法测试 ==========");
            LogMessage("测试内容：");
            LogMessage("1. 玩家移动（加速）");
            LogMessage("2. 跳跃功能");
            LogMessage("3. 重力效果");
            LogMessage("4. 碰撞检测");
            LogMessage("5. 收集物品");
            LogMessage("===========================================");
        }

        /// <summary>
        /// 更新测试统计
        /// </summary>
        private void UpdateTestStatistics()
        {
            if (_player == null) return;

            Vec3 currentPos = _player.GetPos();
            double currentSpeed = _player.GetSpeed();

            // 记录最大速度
            if (currentSpeed > _maxSpeed)
            {
                _maxSpeed = (float)currentSpeed;
            }

            // 计算移动距离
            Vec3 movement = currentPos.sub(_lastPos);
            _totalDistance += (float)movement.length();

            // 检测跳跃（离开地面）
            bool wasOnGround = _lastPos.y <= currentPos.y + 0.1f;
            bool isOnGround = _player.IsOnGround();
            if (wasOnGround && !isOnGround && currentPos.y > _startPos.y + 0.5f)
            {
                _jumpCount++;
                LogMessage($"[跳跃] 第 {_jumpCount} 次跳跃，速度: {currentSpeed:F2}");
            }

            // 检测落地
            if (!wasOnGround && isOnGround)
            {
                _landCount++;
                LogMessage($"[落地] 第 {_landCount} 次落地，位置: ({currentPos.x:F2}, {currentPos.y:F2}, {currentPos.z:F2})");
            }

            // 检测死亡
            if (_player.IsDead())
            {
                _playerDied = true;
                LogWarning($"[死亡] 玩家死亡，位置: ({currentPos.x:F2}, {currentPos.y:F2}, {currentPos.z:F2})");
                LogError($"[致命错误] 玩家死亡，测试失败！");
            }

            // 定期输出状态
            if (Time.frameCount % 60 == 0)
            {
                LogMessage($"[状态] 速度: {currentSpeed:F2}, 位置: ({currentPos.x:F2}, {currentPos.y:F2}, {currentPos.z:F2}), 在地面: {isOnGround}");
            }

            _lastPos = currentPos.clone();
        }

        /// <summary>
        /// 结束测试
        /// </summary>
        public void EndTest()
        {
            _isTesting = false;

            Vec3 finalPos = _player.GetPos() != null ? _player.GetPos() : _startPos;

            // 输出测试结果
            LogMessage("========== 核心玩法测试结果 ==========");
            LogMessage($"测试时长: {_testTimer:F2} 秒");
            LogMessage($"跳跃次数: {_jumpCount}");
            LogMessage($"落地次数: {_landCount}");
            LogMessage($"最大速度: {_maxSpeed:F2}");
            LogMessage($"移动距离: {_totalDistance:F2}");
            LogMessage($"起始位置: ({_startPos.x:F2}, {_startPos.y:F2}, {_startPos.z:F2})");
            LogMessage($"结束位置: ({finalPos.x:F2}, {finalPos.y:F2}, {finalPos.z:F2})");
            LogMessage($"是否死亡: {_playerDied}");

            // 判断测试结果
            bool testPassed = !_playerDied && _maxSpeed > 0 && _totalDistance > 10;
            _testResult = testPassed ? "测试通过" : "测试失败";

            LogMessage($"测试结果: {_testResult}");

            // 如果测试失败，记录详细错误信息
            if (!testPassed)
            {
                LogError("========== 测试失败原因分析 ==========");
                if (_playerDied)
                {
                    LogError("- 玩家死亡：玩家掉出了轨道或发生了致命错误");
                }
                if (_maxSpeed <= 0)
                {
                    LogError("- 玩家无法加速：速度始终为0，可能的原因：");
                    LogError("  1. 空格键输入未正确处理");
                    LogError("  2. 玩家未在轨道上");
                    LogError("  3. 物理计算未正确执行");
                }
                if (_totalDistance <= 10)
                {
                    LogError("- 移动距离不足：玩家移动距离小于10，可能的原因：");
                    LogError("  1. 玩家初始位置错误");
                    LogError("  2. 轨道未正确生成");
                    LogError("  3. 碰撞检测失败");
                }
                LogError("===========================================");
            }

            LogMessage("===========================================");

            // 打印完整测试报告
            PrintTestReport();
        }

        /// <summary>
        /// 打印完整测试报告
        /// </summary>
        public void PrintTestReport()
        {
            Debug.Log("\n");
            Debug.Log("╔═══════════════════════════════════════════════════════════════╗");
            Debug.Log("║                   核心玩法测试完整报告                          ║");
            Debug.Log("╚═══════════════════════════════════════════════════════════════╝");
            Debug.Log("\n");

            Debug.Log("【测试配置】");
            Debug.Log($"  Level ID: {testLevelId}");
            Debug.Log($"  测试时长: {testDuration} 秒");
            Debug.Log($"  实际时长: {_testTimer:F2} 秒");
            Debug.Log("\n");

            Debug.Log("【测试结果】");
            Debug.Log($"  状态: {_testResult}");
            Debug.Log($"  跳跃次数: {_jumpCount}");
            Debug.Log($"  落地次数: {_landCount}");
            Debug.Log($"  最大速度: {_maxSpeed:F2}");
            Debug.Log($"  移动距离: {_totalDistance:F2}");
            Debug.Log($"  玩家死亡: {_playerDied}");
            Debug.Log("\n");

            Debug.Log("【位置信息】");
            if (_startPos != null)
            {
                Debug.Log($"  起始位置: ({_startPos.x:F2}, {_startPos.y:F2}, {_startPos.z:F2})");
            }
            if (_player != null && _player.GetPos() != null)
            {
                Vec3 finalPos = _player.GetPos();
                Debug.Log($"  结束位置: ({finalPos.x:F2}, {finalPos.y:F2}, {finalPos.z:F2})");
            }
            Debug.Log("\n");

            if (_testErrors.Count > 0)
            {
                Debug.Log("【错误列表】(" + _testErrors.Count + " 个错误)");
                for (int i = 0; i < _testErrors.Count; i++)
                {
                    Debug.Log($"  [{i + 1}] {_testErrors[i]}");
                }
                Debug.Log("\n");
            }

            Debug.Log("【详细日志】(" + _testLogs.Count + " 条记录)");
            for (int i = 0; i < _testLogs.Count; i++)
            {
                Debug.Log($"  {i + 1}. {_testLogs[i]}");
            }
            Debug.Log("\n");

            Debug.Log("╔═══════════════════════════════════════════════════════════════╗");
            Debug.Log("║                      报告结束                                   ║");
            Debug.Log("╚═══════════════════════════════════════════════════════════════╝");
            Debug.Log("\n");
        }

        void OnGUI()
        {
            // 显示测试状态和结果
            GUI.Box(new Rect(10, 10, 400, 350), "核心玩法测试");

            GUI.Label(new Rect(20, 40, 380, 20), $"状态: {_testResult}");
            GUI.Label(new Rect(20, 70, 380, 20), $"测试时长: {_testTimer:F2} / {testDuration} 秒");

            if (_isTesting)
            {
                GUI.Label(new Rect(20, 100, 380, 20), $"跳跃次数: {_jumpCount}");
                GUI.Label(new Rect(20, 130, 380, 20), $"落地次数: {_landCount}");
                GUI.Label(new Rect(20, 160, 380, 20), $"最大速度: {_maxSpeed:F2}");
                GUI.Label(new Rect(20, 190, 380, 20), $"移动距离: {_totalDistance:F2}");
                GUI.Label(new Rect(20, 220, 380, 20), $"是否死亡: {_playerDied}");

                if (_player != null)
                {
                    Vec3 pos = _player.GetPos();
                    GUI.Label(new Rect(20, 250, 380, 20), $"位置: ({pos.x:F2}, {pos.y:F2}, {pos.z:F2})");
                    GUI.Label(new Rect(20, 280, 380, 20), $"速度: {_player.GetSpeed():F2}");
                    GUI.Label(new Rect(20, 310, 380, 20), $"在地面: {_player.IsOnGround()}");
                }
            }

            if (!_isTesting)
            {
                if (GUI.Button(new Rect(20, 130, 380, 30), "开始测试 (T)"))
                {
                    StartTest();
                }
            }
            else
            {
                if (GUI.Button(new Rect(20, 340, 380, 30), "结束测试 (ESC)"))
                {
                    EndTest();
                }
            }
        }
    }
}
