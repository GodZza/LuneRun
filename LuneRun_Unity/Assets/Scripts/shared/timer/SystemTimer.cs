using UnityEngine;

namespace shared.timer
{
    public class SystemTimer : ITimer
    {
        public int GetTime()
        {
            return (int)(Time.time * 1000);
        }
    }
}