using UnityEngine;

namespace LuneRun.Tests
{
    /// <summary>
    /// Generator 单元测试运行器
    /// 运行 GeneratorUnitTests 中的所有单元测试
    /// </summary>
    public class GeneratorTestRunner : MonoBehaviour
    {
        [Header("测试配置")]
        [SerializeField] private bool autoRunOnStart = true;

        void Start()
        {
            if (autoRunOnStart)
            {
                RunTests();
            }
        }

        /// <summary>
        /// 运行所有测试
        /// </summary>
        [ContextMenu("运行所有测试")]
        public void RunTests()
        {
            Debug.Log("========================================");
            Debug.Log("Generator 单元测试 - 开始运行");
            Debug.Log("========================================");

            var tests = new GeneratorUnitTests();
            tests.RunAllTests();

            Debug.Log("测试将在3秒后自动销毁对象...");
            Destroy(gameObject, 3f);
        }

        /// <summary>
        /// 运行单个测试
        /// </summary>
        public void RunSingleTest(string testName)
        {
            Debug.Log($"========================================");
            Debug.Log($"运行单个测试: {testName}");
            Debug.Log($"========================================");

            var tests = new GeneratorUnitTests();

            switch (testName)
            {
                case "ForwardGenerator_CanRun":
                    tests.TestForwardGenerator_CanRun_LowDifficulty();
                    break;
                case "ForwardGenerator_Generate":
                    tests.TestForwardGenerator_Generate();
                    break;
                case "CurveGenerator_CanRun_Low":
                    tests.TestCurveGenerator_CanRun_LowDifficulty();
                    break;
                case "CurveGenerator_CanRun_High":
                    tests.TestCurveGenerator_CanRun_HighDifficulty();
                    break;
                case "SlopeGenerator_CanRun_Low":
                    tests.TestSlopeGenerator_CanRun_LowDifficulty();
                    break;
                case "SlopeGenerator_CanRun_Up":
                    tests.TestSlopeGenerator_CanRun_Up();
                    break;
                case "SlopeGenerator_CanRun_Down":
                    tests.TestSlopeGenerator_CanRun_Down_LowHeight();
                    break;
                case "HoleGenerator_CanRun_NoPrevious":
                    tests.TestHoleGenerator_CanRun_NoPrevious();
                    break;
                case "HoleGenerator_CanRun_Consecutive":
                    tests.TestHoleGenerator_CanRun_ConsecutiveHole();
                    break;
                case "LoopGenerator_CanRun_Few":
                    tests.TestLoopGenerator_CanRun_FewSegments();
                    break;
                case "LoopGenerator_CanRun_Many":
                    tests.TestLoopGenerator_CanRun_ManySegments();
                    break;
                default:
                    Debug.LogWarning($"未知的测试: {testName}");
                    break;
            }

            Debug.Log("========================================");
        }
    }
}
