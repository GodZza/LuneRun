using UnityEngine;

namespace com.playchilla.runner.ambient
{
    public class Skyscraper : MonoBehaviour
    {
        private float _baseScaleY;
        private float _wantedScaleY;
        private com.playchilla.runner.Level _level;
        private com.playchilla.runner.track.segment.Segment _segment;

        public void Initialize(Mesh geometry, com.playchilla.runner.Level level, com.playchilla.runner.track.segment.Segment segment)
        {
            _level = level;
            _segment = segment;
            _baseScaleY = Random.Range(0.5f, 8.5f);
            _wantedScaleY = _baseScaleY;
        }

        public void Render(int time, float interpolation)
        {
            float diff = _wantedScaleY - transform.localScale.y;
            diff *= 0.3f;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + diff, transform.localScale.z);
        }

        public void SetYMod(float mod)
        {
            _wantedScaleY = _baseScaleY + mod;
        }
    }
}