using UnityEngine;

namespace shared.timer
{
    /// <summary>
    /// Unity 版本的 Ticker - 使用 Unity 的时间系统模拟 AS 的固定时间步长
    /// </summary>
    public class UnityTicker : ITicker
    {
        private readonly ITickable _tickable;
        private readonly float _ticksPerSecond;
        private readonly int _maxTicks;
        private readonly ITickHook _tickHook;

        private float _tickInterval;         // 每个 tick 的时间间隔（秒）
        private int _tick = 0;              // 当前 tick 编号
        private int _lastTick = 0;          // 上次执行的 tick
        private float _accumulatedTime = 0f; // 累积的时间
        private float _lastTickTime = 0f;   // 上次 tick 发生的时间

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tickable">可 tick 的对象（如 Level）</param>
        /// <param name="ticksPerSecond">每秒多少个 tick（如 25）</param>
        /// <param name="maxTicks">最大 tick 数</param>
        /// <param name="tickHook">tick 钩子（可选）</param>
        public UnityTicker(ITickable tickable, float ticksPerSecond, int maxTicks, ITickHook tickHook = null)
        {
            _tickable = tickable;
            _ticksPerSecond = ticksPerSecond;
            _maxTicks = maxTicks;
            _tickHook = tickHook;
            _tickInterval = 1f / _ticksPerSecond;
            _lastTickTime = Time.time;
        }

        /// <summary>
        /// 获取当前 tick 编号
        /// </summary>
        public int GetTick()
        {
            return _tick;
        }

        /// <summary>
        /// 获取插值因子（0-1）
        /// 表示当前帧距离下一个 tick 的比例
        /// </summary>
        public float GetAlpha()
        {
            // 计算从上次 tick 开始经过的时间
            float timeSinceLastTick = Time.time - _lastTickTime;
            
            // 计算比例，限制在 0-1 之间
            return Mathf.Clamp01(timeSinceLastTick / _tickInterval);
        }

        /// <summary>
        /// 步进 ticker - 在 Unity 的 Update 中调用
        /// </summary>
        public void Step()
        {
            // 累积时间
            _accumulatedTime += Time.deltaTime;

            // 执行 tick（可能一次执行多个）
            while (_accumulatedTime >= _tickInterval)
            {
                // 更新上次 tick 时间
                _lastTickTime = Time.time - _accumulatedTime + _tickInterval;
                
                // 减去一个 tick 间隔
                _accumulatedTime -= _tickInterval;
                
                // 增加 tick 编号
                _tick++;

                // PreTick 钩子
                _tickHook?.PreTick(_tick);

                // 执行游戏逻辑
                bool continueTick = _tickable.tick(_tick);

                // PostTick 钩子
                _tickHook?.PostTick(_tick);

                // 如果 tick 返回 false，停止
                if (!continueTick)
                {
                    break;
                }

                // 检查最大 tick 限制
                if (_tick >= _tick + _maxTicks)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 重置 ticker
        /// </summary>
        public void Reset()
        {
            _tick = 0;
            _lastTick = 0;
            _accumulatedTime = 0f;
            _lastTickTime = Time.time;
        }

        /// <summary>
        /// 获取当前累积时间（用于调试）
        /// </summary>
        public float GetAccumulatedTime()
        {
            return _accumulatedTime;
        }

        /// <summary>
        /// 获取 tick 间隔（用于调试）
        /// </summary>
        public float GetTickInterval()
        {
            return _tickInterval;
        }
    }
}
