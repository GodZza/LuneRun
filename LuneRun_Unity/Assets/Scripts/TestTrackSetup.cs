using UnityEngine;
using System.Reflection;

namespace LuneRun
{
    public class TestTrackSetup : MonoBehaviour
    {
        [Header("轨道设置")]
        [SerializeField] private int testLevelId = 1;
        [SerializeField] private float segmentLength = 10f;
        [SerializeField] private int segmentsPerLevel = 20;
        [SerializeField] private float maxSlopeAngle = 30f;
        
        [Header("玩家设置")]
        [SerializeField] private Vector3 playerStartPosition = new Vector3(0, 0.5f, 0);
        [SerializeField] private float cameraDistance = 20f;
        [SerializeField] private float cameraHeight = 10f;
        
        [Header("调试选项")]
        [SerializeField] private bool showDebugMarkers = true;
        [SerializeField] private bool drawGizmos = true;
        
        private PlayerController playerController;
        private TrackGenerator trackGenerator;
        private Camera mainCamera;
        
        void Start()
        {
            Debug.Log("=== LuneRun 核心玩法测试场景 ===");
            Debug.Log("开始设置测试轨道场景...");
            
            // 确保主相机存在
            SetupCamera();
            
            // 创建轨道生成器
            CreateTrackGenerator();
            
            // 创建玩家
            CreatePlayer();
            
            // 生成测试轨道
            GenerateTestTrack();
            
            // 定位相机
            PositionCamera();
            
            Debug.Log("测试轨道场景设置完成！");
            Debug.Log("操作说明：");
            Debug.Log("- 按住 SPACE 键：加速奔跑");
            Debug.Log("- 松开 SPACE 键：跳跃");
            Debug.Log("- 在空中按住 SPACE：加速下落");
            Debug.Log("- 避免掉落轨道（Y < -50）");
            Debug.Log("- 按 F3 键：切换性能面板");
            Debug.Log("- 按 F11 键：切换全屏");
            Debug.Log("- 按 R 键：重启场景");
            Debug.Log("- 按 H 键：显示帮助");
            Debug.Log("===============================");
        }
        
        void SetupCamera()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("MainCamera");
                cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
                cameraObj.tag = "MainCamera";
                mainCamera = cameraObj.GetComponent<Camera>();
                
                // 设置相机属性
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.2f); // 深蓝色太空背景
                mainCamera.farClipPlane = 1000f;
                mainCamera.nearClipPlane = 0.1f;
                mainCamera.fieldOfView = 60f;
                
                Debug.Log("创建了新的主相机");
            }
        }
        
        void CreateTrackGenerator()
        {
            GameObject trackGenObj = new GameObject("TrackGenerator");
            trackGenerator = trackGenObj.AddComponent<TrackGenerator>();
            
            // 使用反射设置私有序列化字段
            SetPrivateField(trackGenerator, "segmentLength", segmentLength);
            SetPrivateField(trackGenerator, "segmentsPerLevel", segmentsPerLevel);
            SetPrivateField(trackGenerator, "maxSlopeAngle", maxSlopeAngle);
            
            Debug.Log("创建了轨道生成器");
            Debug.Log($"参数：段数={segmentsPerLevel}, 长度={segmentLength}, 最大坡度={maxSlopeAngle}°");
        }
        
        void CreatePlayer()
        {
            GameObject playerObj = new GameObject("Player");
            playerController = playerObj.AddComponent<PlayerController>();
            
            // 创建默认设置
            // 如果Settings类有静态Load方法，可以使用
            Settings settings = null;
            try
            {
                // 尝试加载设置
                settings = Settings.Load();
            }
            catch
            {
                // 如果失败，创建默认设置
                settings = new Settings();
            }
            
            playerController.Initialize(settings);
            playerController.transform.position = playerStartPosition;
            
            Debug.Log("创建了玩家角色");
            Debug.Log($"初始位置: {playerStartPosition}");
        }
        
        void GenerateTestTrack()
        {
            if (trackGenerator != null)
            {
                // 调用TrackGenerator的GenerateTrack方法
                trackGenerator.GenerateTrack(testLevelId);
                
                Debug.Log($"生成了测试轨道，关卡ID: {testLevelId}");
                
                // 显示轨道总长度
                float totalLength = trackGenerator.GetTotalLength();
                Debug.Log($"轨道总长度: {totalLength} 单位");
                
                // 在轨道起点和终点放置标记
                if (showDebugMarkers)
                {
                    CreateTrackMarkers(totalLength);
                }
            }
            else
            {
                Debug.LogError("轨道生成器未找到！");
            }
        }
        
        void CreateTrackMarkers(float totalLength)
        {
            // 轨道起点标记（绿色方块）
            GameObject startMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            startMarker.name = "TrackStartMarker";
            startMarker.transform.position = Vector3.zero + Vector3.up * 3f; // 抬高一点
            startMarker.transform.localScale = new Vector3(2f, 2f, 2f);
            
            Material greenMat = new Material(Shader.Find("Standard"));
            greenMat.color = Color.green;
            startMarker.GetComponent<Renderer>().material = greenMat;
            Destroy(startMarker.GetComponent<Collider>()); // 移除碰撞体避免干扰
            
            // 添加发光效果
            Light startLight = startMarker.AddComponent<Light>();
            startLight.color = Color.green;
            startLight.intensity = 2f;
            startLight.range = 10f;
            
            // 轨道终点标记（红色方块）
            GameObject endMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            endMarker.name = "TrackEndMarker";
            
            // 获取终点位置（轨道最后一个点）
            Vector3 endPosition = trackGenerator.GetPositionAtDistance(totalLength - 0.1f) + Vector3.up * 3f;
            
            endMarker.transform.position = endPosition;
            endMarker.transform.localScale = new Vector3(2f, 2f, 2f);
            
            Material redMat = new Material(Shader.Find("Standard"));
            redMat.color = Color.red;
            endMarker.GetComponent<Renderer>().material = redMat;
            Destroy(endMarker.GetComponent<Collider>());
            
            // 添加发光效果
            Light endLight = endMarker.AddComponent<Light>();
            endLight.color = Color.red;
            endLight.intensity = 2f;
            endLight.range = 10f;
            
            Debug.Log($"轨道起点标记（绿色）在: {startMarker.transform.position}");
            Debug.Log($"轨道终点标记（红色）在: {endPosition}");
            
            // 添加方向箭头（从起点指向终点）
            CreateDirectionArrow(Vector3.zero + Vector3.up * 2f, endPosition);
        }
        
        void CreateDirectionArrow(Vector3 from, Vector3 to)
        {
            GameObject arrow = new GameObject("DirectionArrow");
            arrow.transform.position = (from + to) * 0.5f;
            arrow.transform.LookAt(to);
            
            // 创建箭头身体（圆柱）
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.SetParent(arrow.transform);
            cylinder.transform.localScale = new Vector3(0.5f, Vector3.Distance(from, to) * 0.5f, 0.5f);
            cylinder.transform.localPosition = Vector3.zero;
            cylinder.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            
            Material arrowMat = new Material(Shader.Find("Standard"));
            arrowMat.color = Color.yellow;
            cylinder.GetComponent<Renderer>().material = arrowMat;
            Destroy(cylinder.GetComponent<Collider>());
        }
        
        void PositionCamera()
        {
            if (mainCamera != null && playerController != null)
            {
                // 第三人称视角：相机在玩家后方上方
                Vector3 cameraOffset = new Vector3(0, cameraHeight, -cameraDistance);
                mainCamera.transform.position = playerController.transform.position + cameraOffset;
                mainCamera.transform.LookAt(playerController.transform.position);
                
                Debug.Log($"相机初始位置: {mainCamera.transform.position}");
                Debug.Log($"相机朝向玩家位置");
            }
        }
        
        void Update()
        {
            // 更新玩家控制器（核心玩法循环）
            if (playerController != null)
            {
                playerController.Tick();
            }
            
            // 简单的相机跟随
            if (mainCamera != null && playerController != null)
            {
                Vector3 targetPosition = playerController.transform.position + new Vector3(0, cameraHeight, -cameraDistance);
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * 3f);
            }
            
            // 调试控制
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("重新启动场景...");
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
            
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("显示帮助信息");
                Debug.Log("SPACE: 奔跑/跳跃, F3: 性能面板, F11: 全屏, R: 重启, H: 帮助");
            }
            
            // 显示玩家状态
            if (playerController != null)
            {
                // 每60帧显示一次状态
                if (Time.frameCount % 60 == 0)
                {
                    Debug.Log($"玩家状态: 速度={playerController.GetSpeed():F2}, 地面={playerController.IsOnGround()}, 位置={playerController.transform.position:F1}");
                }
            }
        }
        
        // 辅助方法：设置私有字段
        private void SetPrivateField(object obj, string fieldName, object value)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                Debug.LogWarning($"无法设置私有字段: {fieldName}");
            }
        }
        
        // 辅助方法：在编辑器中绘制轨道路径
        void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            
            if (trackGenerator != null)
            {
                Gizmos.color = Color.yellow;
                
                // 绘制轨道路径
                float totalLength = trackGenerator.GetTotalLength();
                for (float dist = 0; dist < totalLength; dist += segmentLength / 2f)
                {
                    Vector3 pos = trackGenerator.GetPositionAtDistance(dist);
                    Gizmos.DrawSphere(pos, 0.3f);
                }
                
                // 绘制轨道方向线
                Gizmos.color = Color.cyan;
                for (float dist = 0; dist < totalLength; dist += segmentLength)
                {
                    Vector3 pos = trackGenerator.GetPositionAtDistance(dist);
                    Vector3 dir = trackGenerator.GetDirectionAtDistance(dist);
                    Gizmos.DrawRay(pos, dir * 3f);
                }
            }
            
            // 绘制玩家位置
            if (playerController != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(playerController.transform.position, 1f);
                
                // 绘制玩家前方方向
                Gizmos.color = Color.magenta;
                Vector3 forward = playerController.transform.forward;
                Gizmos.DrawRay(playerController.transform.position, forward * 2f);
            }
        }
        
        void OnGUI()
        {
            // 简单的屏幕提示
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 16;
            style.normal.textColor = Color.white;
            
            GUI.Label(new Rect(10, 10, 500, 30), "LuneRun 核心玩法测试", style);
            GUI.Label(new Rect(10, 40, 500, 30), "按住 SPACE 奔跑，松开跳跃", style);
            GUI.Label(new Rect(10, 70, 500, 30), "按 R 重启，H 显示帮助", style);
            
            if (playerController != null)
            {
                GUI.Label(new Rect(10, 100, 500, 30), $"速度: {playerController.GetSpeed():F2}", style);
                GUI.Label(new Rect(10, 130, 500, 30), $"是否在地面: {playerController.IsOnGround()}", style);
                GUI.Label(new Rect(10, 160, 500, 30), $"位置: {playerController.transform.position:F1}", style);
            }
        }
    }
}