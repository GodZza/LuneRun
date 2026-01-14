using shared.util;

namespace com.playchilla.gameapi.api
{
    public class ApiUser
    {
        internal double _id;
        internal string _name;

        public ApiUser(double id, string name)
        {
            _id = id;
            _name = name;
        }

        public double GetId()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }

        public object ToObject()
        {
            return new { id = _id, name = _name };
        }

        public static ApiUser Create(object data)
        {
            var kv = new KeyVal(data);
            var userKv = new KeyVal(kv.GetAsObject("user"));
            return new ApiUser(userKv.GetAsIntNumber("id"), userKv.GetAsString("name"));
        }
    }
}