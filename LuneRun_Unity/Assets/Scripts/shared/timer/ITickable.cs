namespace shared.timer
{
    public interface ITickable
    {
        bool tick(int deltaTime);
    }
}