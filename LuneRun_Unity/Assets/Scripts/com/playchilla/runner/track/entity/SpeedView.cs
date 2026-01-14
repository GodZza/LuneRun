using com.playchilla.runner;

namespace com.playchilla.runner.track.entity
{
    public class SpeedView : EntityView
    {
        public SpeedView(RunnerEntity entity, Level level)
        {
            // In Unity, we would instantiate a GameObject with a Sphere mesh and material
            // For now, just initialize base
            Initialize(entity);
            // TODO: Set up visual representation
        }
    }
}