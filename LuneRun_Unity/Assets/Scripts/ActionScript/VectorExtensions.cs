using UnityEngine;

namespace ActionScript
{
    /// <summary>
    /// Extension methods for Unity Vector2/Vector3 to mimic Flash vector operations.
    /// </summary>
    public static class VectorExtensions
    {
        // ---------- Vector2 extensions ----------
        
        /// <summary>
        /// Dot product of two vectors (instance method for Flash style).
        /// </summary>
        public static float Dot(this Vector2 a, Vector2 b)
        {
            return Vector2.Dot(a, b);
        }
        
        /// <summary>
        /// Returns a normalized copy (Flash style Normalize() that returns new vector).
        /// Unity already has .normalized property, but this provides same API.
        /// </summary>
        public static Vector2 Normalize(this Vector2 v)
        {
            return v.normalized;
        }
        
        /// <summary>
        /// Scale vector by individual x,y factors (Flash style Scale(float sx, float sy)).
        /// </summary>
        public static Vector2 Scale(this Vector2 v, float sx, float sy)
        {
            return new Vector2(v.x * sx, v.y * sy);
        }
        
        /// <summary>
        /// Rescale vector to given length while preserving direction.
        /// If original length is zero, returns zero vector.
        /// </summary>
        public static Vector2 Rescale(this Vector2 v, float newLength)
        {
            float len = v.magnitude;
            if (len > 0)
                return v * (newLength / len);
            return Vector2.zero;
        }
        
        /// <summary>
        /// Rescale vector in-place to given length (modifies the vector).
        /// Returns the modified vector for chaining.
        /// </summary>
        public static Vector2 RescaleSelf(this ref Vector2 v, float newLength)
        {
            float len = v.magnitude;
            if (len > 0)
                v *= (newLength / len);
            else
                v = Vector2.zero;
            return v;
        }
        
        // ---------- Vector3 extensions ----------
        
        /// <summary>
        /// Dot product of two vectors (instance method for Flash style).
        /// </summary>
        public static float Dot(this Vector3 a, Vector3 b)
        {
            return Vector3.Dot(a, b);
        }
        
        /// <summary>
        /// Cross product of two vectors (instance method for Flash style).
        /// </summary>
        public static Vector3 Cross(this Vector3 a, Vector3 b)
        {
            return Vector3.Cross(a, b);
        }
        
        /// <summary>
        /// Returns a normalized copy (Flash style Normalize() that returns new vector).
        /// Unity already has .normalized property, but this provides same API.
        /// </summary>
        public static Vector3 Normalize(this Vector3 v)
        {
            return v.normalized;
        }
        
        /// <summary>
        /// Scale vector by individual x,y,z factors (Flash style Scale(float sx, float sy, float sz)).
        /// </summary>
        public static Vector3 Scale(this Vector3 v, float sx, float sy, float sz)
        {
            return new Vector3(v.x * sx, v.y * sy, v.z * sz);
        }
        
        /// <summary>
        /// Rescale vector to given length while preserving direction.
        /// If original length is zero, returns zero vector.
        /// </summary>
        public static Vector3 Rescale(this Vector3 v, float newLength)
        {
            float len = v.magnitude;
            if (len > 0)
                return v * (newLength / len);
            return Vector3.zero;
        }
        
        /// <summary>
        /// Rescale vector in-place to given length (modifies the vector).
        /// Returns the modified vector for chaining.
        /// </summary>
        public static Vector3 RescaleSelf(this ref Vector3 v, float newLength)
        {
            float len = v.magnitude;
            if (len > 0)
                v *= (newLength / len);
            else
                v = Vector3.zero;
            return v;
        }
    }
}