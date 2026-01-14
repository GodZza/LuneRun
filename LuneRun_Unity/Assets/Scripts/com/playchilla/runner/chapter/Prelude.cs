using shared.timer;

namespace com.playchilla.runner.chapter
{
    public class Prelude : ITickable
    {
        // TODO: Implement Away3D scene in Unity
        // This is a complex class that needs 3D rendering, particles, camera movement, etc.

        bool ITickable.tick(int deltaTime)
        {
            // Return true when prelude is finished
            return Tick(deltaTime);
        }
        
        public bool Tick(int deltaTime)
        {
            // Return true when prelude is finished
            return false;
        }

        // Other methods to be implemented
    }
}