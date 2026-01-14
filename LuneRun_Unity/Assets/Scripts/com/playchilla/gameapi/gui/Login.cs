using UnityEngine;
using com.playchilla.gameapi.api;

namespace com.playchilla.gameapi.gui
{
    public class Login : MonoBehaviour, ILoginCallback, IRegisterCallback
    {
        private GameApi _gameApi;
        private bool _exit;
        private bool _signIn = true;
        private ApiUser _user;
        
        // UI references (stubs)
        private object _dialog; // Placeholder for dialog UI
        
        public void Initialize(string apiUrl)
        {
            _gameApi = new GameApi(apiUrl);
            // Initialize UI components here
            // For now, just set up basic state
            _exit = false;
            _signIn = true;
            _user = null;
        }
        
        public bool IsDone()
        {
            return _exit;
        }
        
        public ApiUser GetUser()
        {
            return _user;
        }
        
        // ILoginCallback implementation
        void ILoginCallback.Login(ApiUser user)
        {
            _user = user;
            _exit = true;
        }

        void ILoginCallback.LoginError(ErrorResponse error)
        {
            // Show error message
            Debug.LogError("Login error: " + error.GetMessage());
        }

        // IRegisterCallback implementation
        void IRegisterCallback.Register(ApiUser user)
        {
            _user = user;
            _exit = true;
        }

        void IRegisterCallback.RegisterError(ErrorResponse error)
        {
            // Show error message
            Debug.LogError("Register error: " + error.GetMessage());
        }
        
        // Internal event handlers (stubs)
        private void OnOk()
        {
            // Validate input and call appropriate API method
            // For now, just simulate success
            _exit = true;
        }
        
        private void OnSkip()
        {
            _exit = true;
        }
        
        private void OnSignUp()
        {
            _signIn = false;
        }
        
        private void OnSignIn()
        {
            _signIn = true;
        }
    }
}