using UnityEngine;

namespace shared.debug
{
    public class Debug
    {
        public static void Assert(bool condition, string message = null)
        {
            if (!condition)
            {
                UnityEngine.Debug.LogError("[Assert] " + message);
            }
        }
        
        public static void setAssertHandler(RemoteAssertHandler handler) { }
    }
}