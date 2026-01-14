namespace shared.timer
{
    public interface ITicker
    {
        int GetTick();
        float GetAlpha();
        void Step();
    }
}