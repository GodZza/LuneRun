using UnityEngine;
using shared.math;
using com.playchilla.runner.track.segment;
using UnityEngine.UIElements;

namespace com.playchilla.runner.track
{
    public class Part
    {
        private readonly Vec3 _pos;
        public readonly Vec3 dir;
        public readonly Vec3 right;
        public readonly Vec3 normal;
        public const int Length = 6;

        public double zRot;

        public Part next;
        public Part previous;
        public GameObject mesh;

        public Segment segment;
        public int partIndex;

        public Part(Segment segment, Vec3 pos, Vec3 dir, Vec3 normal, GameObject mesh, int index, double zRot)
        {
            this.segment = segment;
            this._pos = pos;
            this.dir = dir;
            this.dir.normalizeSelf();

            this.normal = normal;
            this.normal.normalizeSelf();

            this.right = dir.cross(normal);
            this.zRot = zRot;
            this.partIndex = index;
            this.mesh = mesh;
            if (mesh != null)
            {
                mesh.transform.localPosition = this._pos;
            }
        }

        public Vec3 GetPos()
        {
            return _pos;
        }


        public bool hasPassedForward(Vec3Const arg1)
        {
            var loc1 =this._pos.add(this.dir.scale(Length* 0.5));
            return arg1.sub(loc1).dot(this.dir) > 0;
        }

        public bool hasPassedBackward(Vec3Const arg1)
        {
            var loc1 = this._pos.sub(this.dir.scale(Length * 0.5));
            return arg1.sub(loc1).dot(this.dir) < 0;
        }
        public Vec3 GetSurface(Vec3 rayOrigin, Vec3 rayDirection)
        {
            if (mesh == null) return null;

            var v3 = GetSurface((Vector3)rayOrigin, (Vector3)rayDirection);
            if (v3 == null) return null;
            var v = v3.Value;
            return new Vec3(v.x, v.y, v.z);
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
            var intersection = RayTriangleIntersection(rayOrigin, rayDirection,
                worldVertices[0], worldVertices[1], worldVertices[2]);

            if (intersection != null)
            {
                return intersection.Value;
            }

            // 三角形2：顶点0,2,3
            intersection = RayTriangleIntersection(rayOrigin, rayDirection,
                worldVertices[0], worldVertices[2], worldVertices[3]);

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