using UnityEngine;
using System.Collections.Generic;
using com.playchilla.runner.track;
using com.playchilla.runner.track.segment;
using shared.math;

namespace LuneRun
{
    public class TrackSegment : MonoBehaviour
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public float length;
        public float slopeAngle; // in degrees
        
        // Flash-compatible Part objects
        public List<Part> parts = new List<Part>();
        
        public void Initialize(Vector3 start, Vector3 end, float angle, com.playchilla.runner.track.Track flashTrack)
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
            
            // Create Flash-compatible Part objects
            CreateParts(flashTrack);
        }
        
        private void CreateParts(com.playchilla.runner.track.Track flashTrack)
        {
            // Number of parts per segment (approximate based on length)
            int partsPerSegment = Mathf.CeilToInt(length);
            
            // Calculate direction and normal
            Vector3 dir = (endPoint - startPoint).normalized;
            Vector3 normal = Vector3.up;
            
            // Adjust normal based on slope
            if (Mathf.Abs(slopeAngle) > 0.1f)
            {
                Vector3 right = Vector3.Cross(normal, dir);
                normal = Vector3.Cross(dir, right).normalized;
            }
            
            // Create parts along the segment
            for (int i = 0; i <= partsPerSegment; i++)
            {
                float t = (float)i / partsPerSegment;
                Vector3 partPos = Vector3.Lerp(startPoint, endPoint, t);
                
                // Create Part object for Flash compatibility
                Vec3 posVec = new Vec3(partPos.x, partPos.y, partPos.z);
                Vec3 dirVec = new Vec3(dir.x, dir.y, dir.z);
                Vec3 normalVec = new Vec3(normal.x, normal.y, normal.z);
                
                Part part = new Part(null, posVec, dirVec, normalVec, null, i, 0);
                part.segment = null; // Will be set by Segment.AddPart
                parts.Add(part);
            }
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
        private com.playchilla.runner.track.Track flashTrack; // Flash-compatible track
        
        public void GenerateTrack(int levelId, com.playchilla.runner.track.Track existingFlashTrack = null)
        {
            ClearTrack();
            
            // If a Flash track is provided, use it (for integration)
            flashTrack = existingFlashTrack;
            
            // Seed random based on level
            UnityEngine.Random.InitState(levelId);
            
            // Determine number of segments: for infinite mode, generate longer track
            int numSegments = segmentsPerLevel;
            if (levelId == Constants.Tab3InfiniteLevelId)
            {
                numSegments = 200; // Much longer track for infinite mode
                Debug.Log("[LuneRun] Generating infinite mode track with " + numSegments + " segments");
            }
            
            // Create Flash-compatible segments
            List<com.playchilla.runner.track.segment.Segment> flashSegments = new List<com.playchilla.runner.track.segment.Segment>();
            
            // Generate segments
            for (int i = 0; i < numSegments; i++)
            {
                // Determine slope for this segment
                float slopeAngle = UnityEngine.Random.Range(-maxSlopeAngle, maxSlopeAngle);
                
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
                
                segment.Initialize(currentPosition, endPoint, slopeAngle, flashTrack);
                segments.Add(segment);
                
                // Create Flash-compatible Segment and add parts
                if (flashTrack != null)
                {
                    // For first segment, create initial connect part at start
                    Part connectPart = null;
                    if (i == 0)
                    {
                        connectPart = new Part(null, 
                            new Vec3(currentPosition.x, currentPosition.y, currentPosition.z),
                            new Vec3(0, 0, 1), 
                            new Vec3(0, 1, 0), 
                            null, 0, 0);
                    }
                    else if (flashSegments.Count > 0)
                    {
                        // Use last part from previous segment as connect part
                        com.playchilla.runner.track.segment.Segment prevSegment = flashSegments[flashSegments.Count - 1];
                        connectPart = prevSegment.GetLastPart();
                    }
                    
                    com.playchilla.runner.track.segment.Segment flashSegment = new com.playchilla.runner.track.segment.Segment(connectPart, "Segment_" + i, levelId);
                    
                    // Add all parts from Unity segment to Flash segment
                    foreach (Part part in segment.parts)
                    {
                        part.segment = flashSegment;
                        flashSegment.AddPart(part);
                    }
                    
                    flashSegments.Add(flashSegment);
                }
                
                // Update current position for next segment
                currentPosition = endPoint;
            }
            
            // Link Flash segments together
            if (flashTrack != null && flashSegments.Count > 0)
            {
                for (int i = 0; i < flashSegments.Count; i++)
                {
                    if (i + 1 < flashSegments.Count)
                    {
                        flashSegments[i].SetNextSegment(flashSegments[i + 1]);
                        flashTrack.AddSegment(flashSegments[i]);
                    }
                }
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