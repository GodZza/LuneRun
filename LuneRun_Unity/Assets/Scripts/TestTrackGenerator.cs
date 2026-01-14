using UnityEngine;

namespace LuneRun
{
    public class TestTrackGenerator : TrackGenerator
    {
        [Header("测试轨道参数")]
        public int testSegmentsPerLevel = 20;
        public float testSegmentLength = 10f;
        public float testMaxSlopeAngle = 30f;
        
        protected  void Awake()
        {
            // 通过反射设置父类的私有序列化字段
            System.Reflection.FieldInfo segmentsPerLevelField = typeof(TrackGenerator).GetField("segmentsPerLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            System.Reflection.FieldInfo segmentLengthField = typeof(TrackGenerator).GetField("segmentLength", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            System.Reflection.FieldInfo maxSlopeAngleField = typeof(TrackGenerator).GetField("maxSlopeAngle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (segmentsPerLevelField != null) segmentsPerLevelField.SetValue(this, testSegmentsPerLevel);
            if (segmentLengthField != null) segmentLengthField.SetValue(this, testSegmentLength);
            if (maxSlopeAngleField != null) maxSlopeAngleField.SetValue(this, testMaxSlopeAngle);
            
            // 调用基类的Awake（MonoBehaviour）
            //base.Awake();
        }
    }
}