using shared.util;

namespace com.playchilla.gameapi.api
{
    public class ErrorResponse
    {
        internal string _msg;

        public ErrorResponse(string message)
        {
            _msg = message;
        }

        public string GetMessage()
        {
            return _msg;
        }

        public static ErrorResponse Create(object data)
        {
            var kv = new KeyVal(data);
            return new ErrorResponse(kv.GetAsString("message"));
        }
    }
}