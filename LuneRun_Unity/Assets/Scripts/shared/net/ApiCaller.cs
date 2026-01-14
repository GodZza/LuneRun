using System;

namespace shared.net
{
    public class ApiCaller
    {
        public ApiCaller(string url, object urlVariables, Action<object, object> onCompleteCallback, Action<object, object> onErrorCallback, object custom)
        {
            // Stub implementation for single-player game
            // Network functionality will be implemented later if needed
            Console.WriteLine($"[ApiCaller] Created for URL: {url}");
        }

        public bool IsComplete()
        {
            return true;
        }

        public bool HasError()
        {
            return false;
        }

        public string GetError()
        {
            return null;
        }

        public string GetAsString()
        {
            return "";
        }
    }
}