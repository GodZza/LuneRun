using UnityEngine;
using com.playchilla.runner;
using com.playchilla.runner.track;

namespace DebugUI
{
    /// <summary>
    /// 动态轨道调试UI - 显示实时统计信息
    /// 将此脚本附加到场景中的任意GameObject上即可启用
    /// </summary>
    public class DynamicTrackDebugUI : MonoBehaviour
    {
        private Level _level;
        private com.playchilla.runner.track.DynamicTrack _dynamicTrack;
        private bool _showUI = true;
        private int _updateInterval = 60; // 每60帧更新一次
        private int _frameCounter = 0;

        void Start()
        {
            // 查找Level组件
            _level = FindObjectOfType<Level>();

            if (_level == null)
            {
                Debug.LogError("[DynamicTrackDebugUI] Level component not found in scene!");
                enabled = false;
                return;
            }

            _dynamicTrack = _level.GetDynamicTrack();

            if (_dynamicTrack == null)
            {
                Debug.LogError("[DynamicTrackDebugUI] DynamicTrack not found! Make sure Level has initialized it.");
                enabled = false;
                return;
            }

            Debug.Log("[DynamicTrackDebugUI] Initialized successfully");
        }

        void Update()
        {
            // 键盘快捷键
            if (Input.GetKeyDown(KeyCode.S))
            {
                _showUI = !_showUI;
                Debug.Log($"[DynamicTrackDebugUI] UI: {(_showUI ? "SHOWN" : "HIDDEN")}");
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                if (_dynamicTrack != null)
                {
                    //_dynamicTrack.SetDebugMode(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                if (_dynamicTrack != null)
                {
                    Debug.Log(_dynamicTrack.GetStatistics());
                }
            }
        }
    }
}
