using UnityEngine;
using shared.math;
using com.playchilla.runner;

namespace com.playchilla.runner.player
{
    public class PlayerCam : MonoBehaviour
    {
        private Camera _camera;
        private Player _player;
        private Vec3 _lookOffset = new Vec3();
        private Vec3 _wantedLookPos = new Vec3();
        private double _lastTime;
        private double _lerp = 1;

        public void Initialize(Camera camera, Player player)
        {
            _camera = camera;
            _player = player;
            camera.nearClipPlane = 0.1f;
            // Additional initialization as needed
        }

        public void RenderTick(int deltaTime)
        {
            UpdateWantedLookPos(deltaTime);
            _lookOffset.scaleSelf(0.9);
            // Debug assertions omitted
        }

        public void Render(int time, double alpha)
        {
            double currentTime = time + alpha;
            double deltaTime = currentTime - _lastTime;
            
            Vec3 lookDir = new Vec3(0, 0, 1);
            lookDir.addSelf(_lookOffset);
            lookDir.normalizeSelf();
            
            Vec3 forwardVec = new Vec3(0, 0, 1);
            forwardVec = new Vec3(forwardVec.x * (1 - _lerp) + lookDir.x * _lerp, forwardVec.y * (1 - _lerp) + lookDir.y * _lerp, forwardVec.z * (1 - _lerp) + lookDir.z * _lerp);
            
            // In Unity, we would set camera rotation to look at position + forwardVec
            // Simplified implementation
            _lastTime = currentTime;
        }

        public void OnLand(double impact)
        {
            _lookOffset.x = impact * (UnityEngine.Random.value * 0.2 - 0.1);
            _lookOffset.y = impact * -0.8;
            _lerp = 1;
        }

        public void SetLookOffset(Vec3Const offset, double lerp)
        {
            _lerp = lerp;
            _lookOffset.copy(offset);
        }

        private void UpdateWantedLookPos(int deltaTime)
        {
            // Simplified: look at player position
            Vec3 playerPos = _player.GetPos();
            _wantedLookPos.copy(playerPos);
            _wantedLookPos.addXYZSelf(0, 0, 1);
        }
    }
}