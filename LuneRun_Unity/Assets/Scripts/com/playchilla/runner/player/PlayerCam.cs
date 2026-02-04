using UnityEngine;
using shared.math;
using com.playchilla.runner;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.player
{
    public class PlayerCam : MonoBehaviour
    {
        private Camera _camera;
        private Player _player;
        private Vector3 _lookOffset;
        private Vector3 _wantedLookPos;
        private float _lerp = 1;

        public PlayerCam Initialize(Camera camera, Player player)
        {
            _camera = camera;
            _player = player;
            camera.nearClipPlane = 0.1f;
            camera.transform.SetParent(this.transform);
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localScale = Vector3.one;
            camera.transform.localRotation = Quaternion.identity;
            // Additional initialization as needed
            return this;
        }

        public void RenderTick(int tick)
        {
            UpdateWantedLookPos(tick);
            _lookOffset = _lookOffset * 0.9f;
        }

        public void Tick(float deltaTime) // 通过PlayerView调用，而不是自更新。（只有设置为第一人称视角才会调用）
        {
            var lookDir    = (Vector3.forward + _lookOffset).normalized;
            var forwardVec = Vector3.Lerp(Vector3.forward, lookDir, _lerp);
            var lookAt     = transform.position + forwardVec;

            transform.LookAt(lookAt);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="impact">撞击力</param>
        public void OnLand(float impact)
        {
            _lookOffset.x = impact * (UnityEngine.Random.value * 0.2f - 0.1f);
            _lookOffset.y = impact * -0.8f;
            _lerp = 1;
        }

        public void SetLookOffset(Vec3 offset, double lerp)
        {
            _lerp = (float)lerp;
            _lookOffset = offset;
        }

        private void UpdateWantedLookPos(int tick)
        {
            var part = _player.GetCurrentPart();
            if(part == null){
                _wantedLookPos = _player.GetPos() + Vector3.forward;
                return;
            }

            var segment = part.segment.GetNextSegment();
            if (segment == null)
            {
                _wantedLookPos = segment.GetLastPart().GetPos();
                return;
            }

            while (segment is HoleSegment)
            {
                segment = segment.GetNextSegment();
            }
            if(segment == null)
            {
                return;
            }

            var lookPos = _player.IsOnGround() ? segment.GetFirstPart().GetPos() : segment.GetLastPart().GetPos();
            _wantedLookPos = lookPos;
        }
    }
}