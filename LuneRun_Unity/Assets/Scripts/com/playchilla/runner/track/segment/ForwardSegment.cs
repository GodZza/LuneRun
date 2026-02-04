using UnityEngine;
using com.playchilla.runner;
using com.playchilla.runner.track;
using shared.math;
using UnityEngine.UIElements;

namespace com.playchilla.runner.track.segment
{
    public class ForwardSegment : Segment
    {
        private const float PartLength = 10f; // Placeholder
        private object _material;

        public ForwardSegment(string name, float y, Part connectPart, Vector3 direction,
                             float totalRotationY, float totalRotationZ, int totalParts, Materials materials,
                             int levelId, bool addStartPart = true, bool addEndPart = true)
            : base(CreateConnectPartIfNull(connectPart, y, direction, (Material)materials.GetMaterial("part")), name, levelId)
        {
            var material = (Material)materials?.GetMaterial("part");

            var part = this.GetConnectPart();
            var currentPosition = part.GetPos();
            currentPosition.y = y;

            var rotationYIncrement = totalRotationY / totalParts;
            var rotationZIncrement = totalRotationZ / totalParts;
            var baseRotationX = (-totalRotationY) * 0.5f;

            const int smoothTransitionParts = 5; // 用于平滑过渡的部件数量

            for (var partIndex=0;partIndex<totalParts; partIndex++)
            {
                var partMesh = CreateMesh(material);
                partMesh.name = "PartMesh " + GetParts().Count;// "SubMesh";
                partMesh.transform.LookAt(direction);
                partMesh.transform.Rotate(Vector3.right, (partIndex + 1) * rotationZIncrement);
                partMesh.transform.Rotate(Vector3.up, (partIndex + 1) * rotationYIncrement);

                var rotateX = baseRotationX;

                // 起始部分平滑过渡
                if (addStartPart && partIndex < smoothTransitionParts)
                {
                    rotateX = baseRotationX * partIndex / smoothTransitionParts;
                }

                // 结束部分平滑过渡
                if (addEndPart && partIndex >= totalParts - smoothTransitionParts)
                {
                    rotateX = baseRotationX * (1 - (partIndex - (totalParts - smoothTransitionParts)) / smoothTransitionParts);
                }
                partMesh.transform.Rotate(Vector3.forward, rotateX);

                var directionOffset = partMesh.transform.forward * Part.Length;
                currentPosition +=directionOffset; //incrementBy

                var newPart = new Part(this, currentPosition, directionOffset, partMesh.transform.up, partMesh, GetParts().Count, rotationZIncrement);
                this.AddPart(newPart);
            }
        }

        private static Part CreateConnectPartIfNull(Part connectPart, float y, Vector3 dir, Material material)
        {
            if (connectPart != null)  return connectPart;
            var mesh = CreateMesh(material);
            mesh.name = "CreateConnectPartIfNull";
            return new Part(null, new Vector3(0, y, 0), dir, Vector3.up, mesh, 0, 0);
        }

        public static GameObject CreateMesh(Material material)
        {
            //internal const _cube:away3d.primitives.CubeGeometry=new away3d.primitives.CubeGeometry(com.playchilla.runner.track.Part.Length * 1.1, 0.2, com.playchilla.runner.track.Part.Length - 0.5);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // 2. 根据原代码中的参数设置立方体的缩放（Scale）
            // 原参数: (Length * 1.1, 0.2, Length - 0.5)
            // 在Unity中，缩放是通过Transform组件的localScale实现的
            cube.transform.localScale = new Vector3(Part.Length * 1.1f, 0.2f, Part.Length - 0.5f);

            if (material != null)
            {
                if(cube.TryGetComponent<MeshRenderer>(out var renderer))
                {
                    renderer.material = material;
                }
            }

            return cube;
        }
    }
}