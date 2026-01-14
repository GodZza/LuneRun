using UnityEngine;
using shared.math;

namespace com.playchilla.runner.ambient
{
    public class Particle : MonoBehaviour
    {
        private Vec3 _dir;

        public void Initialize(Mesh geometry, Material material, Vec3 direction)
        {
            // Create particle with given geometry and material
            _dir = direction;
        }

        public void Tick(int deltaTime)
        {
            // Move particle along direction
            transform.position += new Vector3((float)_dir.x, (float)_dir.y, (float)_dir.z) * (deltaTime / 1000f);
        }
    }
}