using UnityEngine;
using shared.math;
using shared.render;

namespace com.playchilla.runner.track.entity
{
    public class EntityView : MonoBehaviour
    {
        private SmoothPos3 _smoothPos;
        protected RunnerEntity _entity;

        public virtual void Initialize(RunnerEntity entity)
        {
            _entity = entity;
            _smoothPos = new SmoothPos3(entity.GetPos());
            transform.position = new Vector3((float)_entity.GetPos().x, (float)_entity.GetPos().y, (float)_entity.GetPos().z);
        }

        public void Render(int deltaTime, float interpolation)
        {
            if (_entity == null) return;
            Vec3 targetPos = (Vec3)_entity.GetPos();
            Vec3 smoothed = _smoothPos.GetPos(targetPos, deltaTime, interpolation);
            transform.position = new Vector3((float)smoothed.x, (float)smoothed.y, (float)smoothed.z);
        }

        public bool CanRemove()
        {
            return _entity?.CanRemove() ?? true;
        }

        public RunnerEntity Entity => _entity;
    }
}