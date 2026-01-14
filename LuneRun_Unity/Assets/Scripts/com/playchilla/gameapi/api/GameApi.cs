using System;

namespace com.playchilla.gameapi.api
{
    public class GameApi
    {
        private string _apiUrl;

        public GameApi(string apiUrl)
        {
            _apiUrl = apiUrl;
            if (!string.IsNullOrEmpty(_apiUrl) && _apiUrl[_apiUrl.Length - 1] != '/')
            {
                _apiUrl += "/";
            }
        }

        public void Register(string username, string password, string email, IRegisterCallback callback)
        {
            // Stub implementation for single-player game
            // Network functionality will be implemented later if needed
            Console.WriteLine($"[GameApi] Register called: {username}");
            // Simulate successful registration after delay?
        }

        public void Login(string username, string password, ILoginCallback callback)
        {
            // Stub implementation
            Console.WriteLine($"[GameApi] Login called: {username}");
        }

        // Internal callback handlers (stubs)
        internal void OnRegisterComplete(object data, object callback)
        {
            var cb = callback as IRegisterCallback;
            if (cb != null)
            {
                // Simulate successful registration
                // var user = ApiUser.Create(data);
                // cb.Register(user);
            }
        }

        internal void OnRegisterError(object data, object callback)
        {
            var cb = callback as IRegisterCallback;
            if (cb != null)
            {
                // var error = ErrorResponse.Create(data);
                // cb.RegisterError(error);
            }
        }

        internal void OnLoginComplete(object data, object callback)
        {
            var cb = callback as ILoginCallback;
            if (cb != null)
            {
                // var user = ApiUser.Create(data);
                // cb.Login(user);
            }
        }

        internal void OnLoginError(object data, object callback)
        {
            var cb = callback as ILoginCallback;
            if (cb != null)
            {
                // var error = ErrorResponse.Create(data);
                // cb.LoginError(error);
            }
        }
    }
}