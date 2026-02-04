using UnityEngine;
using shared.math;
using com.playchilla.runner.track.segment;
using UnityEngine.UIElements;

namespace com.playchilla.runner.track
{
    public class Part
    {
        public const int Length = 6;
        public readonly Vector3 dir;
        public readonly Vector3 right;
        public readonly Vector3 normal;
        

        public readonly float zRot;

        public Part next;
        public Part previous;
        public GameObject mesh;

        public Segment segment;
        public int partIndex;

        private readonly Vector3 _pos;

        public Part(Segment segment, Vector3 pos, Vector3 dir, Vector3 normal, GameObject mesh, int index, float zRot)
        {
            this.segment = segment;
            this._pos = pos;
            this.dir = dir.normalized;

            this.normal = normal.normalized;

            this.right = Vector3.Cross(dir, normal);
            this.zRot = zRot;
            this.partIndex = index;
            this.mesh = mesh;
            if (mesh != null)
            {
                mesh.transform.localPosition = this._pos;
            }
        }

        public Vector3 GetPos()
        {
            return _pos;
        }
        /// <summary>
        /// 检查目标点是否已通过本部件的前向检查点（沿方向向前的点）
        /// </summary>
        /// <param name="positionToCheck">要检查的世界坐标系中的位置</param>
        /// <returns>如果目标点已通过前向检查点则返回true，否则返回false</returns>
        public bool hasPassedForward(Vector3 positionToCheck)
        {
            // 计算前向检查点：从部件中心沿方向向量移动半个部件长度
            var forwardCheckPoint = _pos + dir * Part.Length * 0.5f;

            // 通过点积判断目标点是否已通过检查点
            // 点积大于0表示目标点在方向向量的前方
            return Vector3.Dot(positionToCheck - forwardCheckPoint, dir) > 0;
        }
        /// <summary>
        /// 检查目标点是否已通过本部件的后向检查点（沿方向向后的点）
        /// </summary>
        /// <param name="positionToCheck">要检查的世界坐标系中的位置</param>
        /// <returns>如果目标点已通过后向检查点则返回true，否则返回false</returns>
        public bool hasPassedBackward(Vector3 positionToCheck)
        {
            var backwardCheckPoint = _pos - dir * Part.Length * 0.5f;
            return Vector3.Dot(positionToCheck - backwardCheckPoint, dir) < 0;
        }


        /// <summary>
        /// 获取射线与网格表面的交点
        /// </summary>
        /// <param name="rayOrigin">射线起点</param>
        /// <param name="rayDirection">射线方向</param>
        /// <returns>交点坐标，如果没有交点返回null</returns>
        public Vector3? GetSurface(Vector3 rayOrigin, Vector3 rayDirection)
        {
            if ( mesh == null)
            {
                Debug.LogWarning("[SurfaceDetection]  Mesh is null");
                return null;
            }

            var halfLength = -Length * 0.6f;
            var localVertices = new Vector3[4]
            {
                new Vector3(-halfLength, 0, -halfLength), // 左下
                new Vector3(halfLength, 0, -halfLength),  // 右下
                new Vector3(halfLength, 0, halfLength),   // 右上
                new Vector3(-halfLength, 0, halfLength)    // 左上
            };

            // 转换到世界坐标系
            var worldVertices = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                worldVertices[i] = mesh.transform.TransformPoint(localVertices[i]);
            }

            // 将四边形分成两个三角形进行检测
            // 三角形1：顶点0,1,2
            var intersection = RayTriangleIntersection(rayOrigin, rayDirection, worldVertices[0], worldVertices[1], worldVertices[2]);

            if (intersection != null)
            {
                return intersection.Value;
            }

            // 三角形2：顶点0,2,3
            intersection = RayTriangleIntersection(rayOrigin, rayDirection, worldVertices[0], worldVertices[2], worldVertices[3]);

            return intersection;
        }

        /// <summary>
        /// 计算射线与三角形的交点
        /// </summary>
        private Vector3? RayTriangleIntersection(Vector3 rayOrigin, Vector3 rayDirection,
                                               Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // 计算三角形法线[7](@ref)
            Vector3 normal = Vector3.Cross(p2 - p1, p3 - p1);

            // 计算射线与三角形平面的交点参数t[7](@ref)
            float denominator = Vector3.Dot(rayDirection, normal);

            // 如果分母接近0，说明射线与平面平行
            if (Mathf.Abs(denominator) < Mathf.Epsilon)
                return null;

            float t = Vector3.Dot(p1 - rayOrigin, normal) / denominator;

            // 如果t为负，交点在射线起点后面
            if (t < 0) return null;

            // 计算交点坐标
            Vector3 intersectionPoint = rayOrigin + rayDirection * t;

            // 检查交点是否在三角形内部[7](@ref)
            if (IsPointInTriangle(intersectionPoint, p1, p2, p3, normal))
            {
                return intersectionPoint;
            }

            return null;
        }

        /// <summary>
        /// 判断点是否在三角形内部
        /// </summary>
        private bool IsPointInTriangle(Vector3 point, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 normal)
        {
            // 使用重心坐标法或边法判断点是否在三角形内
            // 这里使用边法：点如果在三条边的同一侧，则在三角形内[7](@ref)

            Vector3 edge1 = p2 - p1;
            Vector3 edge2 = p3 - p2;
            Vector3 edge3 = p1 - p3;

            Vector3 toPoint1 = point - p1;
            Vector3 toPoint2 = point - p2;
            Vector3 toPoint3 = point - p3;

            // 计算法线（叉乘）并检查符号一致性
            Vector3 cross1 = Vector3.Cross(edge1, toPoint1);
            Vector3 cross2 = Vector3.Cross(edge2, toPoint2);
            Vector3 cross3 = Vector3.Cross(edge3, toPoint3);

            // 如果三个叉乘结果与法线的点积都同号，则点在三角形内
            float dot1 = Vector3.Dot(cross1, normal);
            float dot2 = Vector3.Dot(cross2, normal);
            float dot3 = Vector3.Dot(cross3, normal);

            return (dot1 >= 0 && dot2 >= 0 && dot3 >= 0) ||
                   (dot1 <= 0 && dot2 <= 0 && dot3 <= 0);
        }
    }
}