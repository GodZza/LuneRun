using UnityEngine;
using System.Collections.Generic;

namespace LuneRun
{
    public class TrackSegment : MonoBehaviour
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public float length;
        public float slopeAngle; // in degrees
        
        public void Initialize(Vector3 start, Vector3 end, float angle)
        {
            startPoint = start;
            endPoint = end;
            length = Vector3.Distance(start, end);
            slopeAngle = angle;
            
            // Position this GameObject at the midpoint
            transform.position = (start + end) * 0.5f;
            transform.LookAt(end);
            
            // Create visual representation (a simple cube for now)
            GameObject segmentVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            segmentVisual.transform.SetParent(transform);
            segmentVisual.transform.localPosition = Vector3.zero;
            segmentVisual.transform.localRotation = Quaternion.identity;
            segmentVisual.transform.localScale = new Vector3(5f, 0.2f, length);
            
            // Set color based on slope
            Color segmentColor = angle > 0 ? Color.red : (angle < 0 ? Color.blue : Color.gray);
            segmentVisual.GetComponent<Renderer>().material.color = segmentColor;
        }
    }
    
    public class TrackGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject segmentPrefab;
        [SerializeField] private int segmentsPerLevel = 20;
        [SerializeField] private float segmentLength = 10f;
        [SerializeField] private float maxSlopeAngle = 30f;
        
        private List<TrackSegment> segments = new List<TrackSegment>();
        private Vector3 currentPosition = Vector3.zero;
        
        public void GenerateTrack(int levelId)
        {
            ClearTrack();
            
            // Seed random based on level
            Random.InitState(levelId);
            
            // Determine number of segments: for infinite mode, generate longer track
            int numSegments = segmentsPerLevel;
            if (levelId == Constants.Tab3InfiniteLevelId)
            {
                numSegments = 200; // Much longer track for infinite mode
                Debug.Log("[LuneRun] Generating infinite mode track with " + numSegments + " segments");
            }
            
            // Generate segments
            for (int i = 0; i < numSegments; i++)
            {
                // Determine slope for this segment
                float slopeAngle = Random.Range(-maxSlopeAngle, maxSlopeAngle);
                
                // Calculate end point
                float slopeRad = slopeAngle * Mathf.Deg2Rad;
                float horizontal = segmentLength * Mathf.Cos(slopeRad);
                float vertical = segmentLength * Mathf.Sin(slopeRad);
                
                Vector3 endPoint = currentPosition + new Vector3(0, vertical, horizontal);
                
                // Create segment
                GameObject segmentObj;
                if (segmentPrefab != null)
                {
                    segmentObj = Instantiate(segmentPrefab, transform);
                }
                else
                {
                    segmentObj = new GameObject("Segment_" + i);
                    segmentObj.transform.SetParent(transform);
                }
                
                TrackSegment segment = segmentObj.GetComponent<TrackSegment>();
                if (segment == null)
                {
                    segment = segmentObj.AddComponent<TrackSegment>();
                }
                
                segment.Initialize(currentPosition, endPoint, slopeAngle);
                segments.Add(segment);
                
                // Update current position for next segment
                currentPosition = endPoint;
            }
            
            Debug.Log($"Generated track with {segments.Count} segments for level {levelId}");
        }
        
        public void ClearTrack()
        {
            foreach (TrackSegment segment in segments)
            {
                if (segment != null && segment.gameObject != null)
                {
                    Destroy(segment.gameObject);
                }
            }
            segments.Clear();
            currentPosition = Vector3.zero;
        }
        
        // Get the track segment at a given distance along the track
        public TrackSegment GetSegmentAtDistance(float distance)
        {
            float accumulated = 0f;
            foreach (TrackSegment segment in segments)
            {
                accumulated += segment.length;
                if (distance <= accumulated)
                {
                    return segment;
                }
            }
            return segments.Count > 0 ? segments[segments.Count - 1] : null;
        }
        
        // Get position along track at given distance
        public Vector3 GetPositionAtDistance(float distance)
        {
            TrackSegment segment = GetSegmentAtDistance(distance);
            if (segment == null) return Vector3.zero;
            
            // Find distance into this segment
            float segmentStartDist = GetSegmentStartDistance(segment);
            float t = (distance - segmentStartDist) / segment.length;
            
            return Vector3.Lerp(segment.startPoint, segment.endPoint, t);
        }
        
        private float GetSegmentStartDistance(TrackSegment targetSegment)
        {
            float distance = 0f;
            foreach (TrackSegment segment in segments)
            {
                if (segment == targetSegment) return distance;
                distance += segment.length;
            }
            return 0f;
        }
        
        // Get direction at given distance along track
        public Vector3 GetDirectionAtDistance(float distance)
        {
            TrackSegment segment = GetSegmentAtDistance(distance);
            if (segment == null) return Vector3.forward;
            
            // Direction is from start to end, normalized
            Vector3 dir = (segment.endPoint - segment.startPoint).normalized;
            return dir;
        }
        
        // Get total track length
        public float GetTotalLength()
        {
            float total = 0f;
            foreach (TrackSegment segment in segments)
            {
                total += segment.length;
            }
            return total;
        }
        
        // For debugging: draw track in editor
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (TrackSegment segment in segments)
            {
                Gizmos.DrawLine(segment.startPoint, segment.endPoint);
                Gizmos.DrawSphere(segment.startPoint, 0.5f);
            }
            if (segments.Count > 0)
            {
                Gizmos.DrawSphere(segments[segments.Count - 1].endPoint, 0.5f);
            }
        }
    }
}