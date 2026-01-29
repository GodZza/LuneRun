using UnityEngine;

namespace shared.timer
{
    /// <summary>
    /// Ticker - 时间步进器
    /// 对应 AS 代码中的 Ticker 类
    /// </summary>
    public class Ticker : ITicker
    {
        // 对应 AS 中的 internal var
        private ITickable _tickable;
        private float _ticksPerSecond;
        private int _maxTicks;
        private ITickHook _tickHook;
        private ITimer _timer;
        private int _tickIntervalMs;
        private int _startTime;
        private int _tick = 0;
        private int _lastTick = 0;

        /// <summary>
        /// 构造函数
        /// 对应 AS: public function Ticker(arg1:shared.timer.ITickable, arg2:Number, arg3:int, arg4:shared.timer.ITickHook=null, arg5:shared.timer.ITimer=null)
        /// </summary>
        public Ticker(ITickable tickable, float ticksPerSecond, int maxTicks, ITickHook tickHook = null, ITimer timer = null)
        {
            _tickable = tickable;
            _ticksPerSecond = ticksPerSecond;
            _maxTicks = maxTicks;
            _tickHook = tickHook;
            _timer = timer != null ? timer : new SystemTimer();
            _tickIntervalMs = (int)(1000 / _ticksPerSecond);
            Reset();
        }

        /// <summary>
        /// 重置 ticker
        /// 对应 AS: public function reset():void
        /// </summary>
        public void Reset()
        {
            _startTime = _timer.GetTime();
            _lastTick = 0;
        }

        /// <summary>
        /// 获取当前 tick 编号
        /// 对应 AS: public function getTick():int
        /// </summary>
        public int GetTick()
        {
            return _tick;
        }

        /// <summary>
        /// 获取插值因子（0-1）
        /// 对应 AS: public function getAlpha():Number
        /// </summary>
        public float GetAlpha()
        {
            float elapsed = _timer.GetTime() - _startTime - _lastTick * _tickIntervalMs;
            return Mathf.Min(elapsed / _tickIntervalMs, 1f);
        }

        /// <summary>
        /// 步进 ticker（执行 tick）
        /// 对应 AS: public function step():void
        /// </summary>
        public void Step()
        {
            int elapsedTime = _timer.GetTime() - _startTime;
            float timeSinceLastTick = elapsedTime - _lastTick * _tickIntervalMs;
            bool stop = false;
            int maxTick = _tick + _maxTicks;

            while (timeSinceLastTick >= _tickIntervalMs && !stop && _tick < maxTick)
            {
                _tick++;

                // 执行 preTick
                if (_tickHook != null)
                {
                    _tickHook.PreTick(_tick);
                }

                // 执行 tick
                stop = !_tickable.tick(_tick);

                // 执行 postTick
                if (_tickHook != null)
                {
                    _tickHook.PostTick(_tick);
                }

                _lastTick++;
                timeSinceLastTick -= _tickIntervalMs;
            }
        }
    }
}
