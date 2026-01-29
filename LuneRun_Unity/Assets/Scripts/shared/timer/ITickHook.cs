namespace shared.timer
{
    /// <summary>
    /// ITickHook - Tick 钩子接口
    /// 对应 AS 代码中的 ITickHook
    /// </summary>
    public interface ITickHook
    {
        void PreTick(int deltaTime);
        void PostTick(int deltaTime);
    }
}
