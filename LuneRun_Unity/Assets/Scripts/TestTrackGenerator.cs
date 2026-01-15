using UnityEngine;

namespace LuneRun
{
    public class TestTrackGenerator : TrackGenerator
    {
        [Header("测试轨道参数")]
        public int testSegmentsPerLevel = 20;
        public float testSegmentLength = 10f;
        public float testMaxSlopeAngle = 30f;
        
        // 这个类主要用于在Inspector中调整轨道参数
        // 实际的轨道生成由TrackGenerator基类处理
    }
}