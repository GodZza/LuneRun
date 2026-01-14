using System;
using shared.net;
using shared.util;

namespace com.playchilla.gameapi.api
{
    public class GameApiCaller
    {
        private Action<object, object> _onSuccess;
        private Action<object, object> _onError;

        public GameApiCaller(string url, object urlVariables, Action<object, object> onSuccess, Action<object, object> onError, object custom)
        {
            _onSuccess = onSuccess;
            _onError = onError;
            
            // Add timestamp to avoid caching (as in ActionScript)
            // urlVariables.ts = Math.Round(Math.Random() * 1000000);
            
            // Create ApiCaller instance (network stub)
            var apiCaller = new ApiCaller(url, urlVariables, OnComplete, OnNetError, custom);
        }

        private void OnComplete(object eventObj, object custom)
        {
            // Stub implementation
            // In ActionScript: parse JSON, check success flag, call appropriate callback
            /*
            var data = eventObj.target.data;
            var json = JSON.parse(data);
            var kv = new KeyVal(json);
            if (kv.GetAsBool("success"))
            {
                _onSuccess(json, custom);
            }
            else
            {
                _onError(json, custom);
            }
            */
        }

        private void OnNetError(object errorEvent, object custom)
        {
            // Stub implementation
            /*
            var errorObj = new { success = false, message = errorEvent.text };
            _onError(errorObj, custom);
            */
        }
    }
}