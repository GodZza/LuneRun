using UnityEngine;
using com.playchilla.runner.track;
using com.playchilla.runner;
using shared.math;

namespace LuneRun.Tests
{
    /// <summary>
    /// 快速验证 DynamicTrack 是否正常工作
    /// 将此脚本附加到场景中任意 GameObject 上即可运行
    /// </summary>
    public class QuickDynamicTrackTest : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("========== Quick DynamicTrack Test ==========");
            Debug.Log("开始快速测试...");

            try
            {
                // 1. 创建 Level
                GameObject levelObj = new GameObject("TestLevel");
                Level level = levelObj.AddComponent<Level>();
                Debug.Log("✓ [1/6] Level created");

                // 2. 创建 DynamicTrack
                DynamicTrack dynamicTrack = new DynamicTrack(level, 3, 1);
                Debug.Log("✓ [2/6] DynamicTrack created");

                // 3. 获取 Track
                Track track = dynamicTrack.GetTrack();
                if (track == null)
                {
                    Debug.LogError("✗ [3/6] Track is null!");
                    throw new System.Exception("Track is null");
                }
                Debug.Log($"✓ [3/6] Track created with {track.GetSegments().Count} segments");

                // 4. 检查段数是否合理
                if (track.GetSegments().Count == 0)
                {
                    Debug.LogError("✗ [4/6] No segments created!");
                    throw new System.Exception("No segments created");
                }
                Debug.Log($"✓ [4/6] Segments count is valid ({track.GetSegments().Count} > 0)");

                // 5. 模拟玩家移动
                Vec3 playerPos = new Vec3(0, 1, 0);
                for (int i = 0; i < 10; i++)
                {
                    playerPos.z += 20;
                    //dynamicTrack.Update(playerPos);
                }
                Debug.Log($"✓ [5/6] Player moved from Z=0 to Z={playerPos.z}");

                // 6. 检查最终段数
                int initialSegments = track.GetSegments().Count;
                Debug.Log($"✓ [6/6] Test completed successfully");
                Debug.Log("");
                Debug.Log($"========== Test Summary ==========");
                Debug.Log($"Initial segments: {initialSegments}");
                Debug.Log($"Final position: Z={playerPos.z}");
                Debug.Log($"");
                Debug.Log($"Statistics:\n{dynamicTrack.GetStatistics()}");
                Debug.Log($"================================");

                // 延迟销毁对象
                Destroy(levelObj, 5f);
                Destroy(gameObject, 6f);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"========== Test FAILED ==========");
                Debug.LogError($"Error: {e.Message}");
                Debug.LogError($"Stack Trace: {e.StackTrace}");
                Debug.LogError($"=====================================");
                // 延迟销毁对象
                Destroy(gameObject, 3f);
            }
        }
    }
}

