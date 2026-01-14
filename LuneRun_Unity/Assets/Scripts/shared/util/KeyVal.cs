using System;
using System.Collections.Generic;
using shared.debug;
using shared.math;

namespace shared.util
{
    public class KeyVal
    {
        private object _keyVal;
        private KeyVal _parent;

        public KeyVal(object arg1)
        {
            Debug.Assert(arg1 != null, "Trying to create a KeyVal from a null object.");
            _keyVal = arg1;
        }

        public object GetObject()
        {
            return _keyVal;
        }

        public void SetParent(KeyVal parent)
        {
            _parent = parent;
        }

        public bool Has(string key)
        {
            if (_keyVal is Dictionary<string, object> dict)
            {
                if (dict.ContainsKey(key))
                    return true;
            }
            else if (_keyVal is System.Collections.IDictionary idict)
            {
                if (idict.Contains(key))
                    return true;
            }
            // Fallback: check if property exists via reflection (simplified)
            return false;
        }

        public string GetAsString(string key, string defaultValue = null)
        {
            var val = _GetAsString(key);
            if (val == null)
            {
                Debug.Assert(defaultValue != null, "Trying to get a string key that doesn't exist and has no default value: " + key);
                return defaultValue;
            }
            return val;
        }

        public double GetAsNumber(string key, double defaultValue = double.NaN)
        {
            var val = _GetAsString(key);
            if (val == null)
            {
                Debug.Assert(!double.IsNaN(defaultValue), "Trying to get a number key that doesn't exist and has no default value: " + key);
                return defaultValue;
            }
            if (double.TryParse(val, out double result))
            {
                Debug.Assert(!double.IsNaN(result), "Trying to get an int that is NaN: " + key);
                Debug.Assert(!double.IsInfinity(result), "Trying to get an int that is not finite: " + key);
                return result;
            }
            return defaultValue;
        }

        public double GetAsIntNumber(string key, double defaultValue = double.NaN)
        {
            var val = GetAsNumber(key, defaultValue);
            Debug.Assert(val == Math.Round(val), "Trying to get an integer number (number without decimals) that has decimals: " + key);
            return val;
        }

        public int GetAsInt(string key)
        {
            var val = GetAsNumber(key);
            Debug.Assert(val >= int.MinValue && val <= int.MaxValue, "The expected integer is out of range: " + key);
            int result = (int)val;
            Debug.Assert(result == val, "Trying to get an int that has decimals: " + key);
            return result;
        }

        public int GetAsUint(string key, uint defaultValue = 0)
        {
            var val = GetAsNumber(key, defaultValue);
            Debug.Assert(val >= 0 && val <= uint.MaxValue, "The expected unsigned integer is out of range: " + key);
            uint result = (uint)val;
            Debug.Assert(result == val, "Trying to get an unsigned int that has decimals: " + key);
            return (int)result;
        }

        public int GetAsIntDef(string key, int defaultValue)
        {
            var val = GetAsNumber(key, defaultValue);
            Debug.Assert(val >= int.MinValue && val <= int.MaxValue, "The expected integer is out of range: " + key);
            int result = (int)val;
            Debug.Assert(result == val, "Trying to get an int that has decimals: " + key);
            return result;
        }

        public bool GetAsBool(string key)
        {
            var val = GetAsString(key).ToLower();
            Debug.Assert(val == "0" || val == "1" || val == "false" || val == "true", 
                "Trying to get a boolean that has invalid format (must be 'true', 'false', '0' or '1'): " + key);
            return val == "1" || val == "true";
        }

        public bool GetAsBoolDef(string key, bool defaultValue)
        {
            var val = _GetAsString(key);
            if (val == null)
                return defaultValue;
            return GetAsBool(key);
        }

        public uint GetAsUintFromHex(string key)
        {
            var val = _GetAsString(key);
            return Convert.ToUInt32(val, 16);
        }

        public Array GetAsArray(string key, Array defaultValue = null)
        {
            Debug.Assert(key != null, "Trying to get array value for a null key.");
            Debug.Assert(key.Length > 0, "Trying to get array value for an empty key.");
            // Simplified: assume _keyVal is a Dictionary<string, object>
            if (_keyVal is Dictionary<string, object> dict)
            {
                if (dict.TryGetValue(key, out object val))
                {
                    if (val is Array arr)
                        return arr;
                    // If not array, try to convert (not implemented)
                }
            }
            if (_parent != null)
                return _parent.GetAsArray(key, defaultValue);
            Debug.Assert(defaultValue != null, "Trying to get array but wasn't found and no default value supplied: " + key);
            return defaultValue;
        }

        public List<int> GetAsIntVector(string key)
        {
            var arr = GetAsArray(key);
            if (arr == null) return null;
            var list = new List<int>();
            foreach (var item in arr)
                list.Add(Convert.ToInt32(item));
            return list;
        }

        public List<string> GetAsStringVector(string key)
        {
            var arr = GetAsArray(key);
            if (arr == null) return null;
            var list = new List<string>();
            foreach (var item in arr)
                list.Add(item.ToString());
            return list;
        }

        public object GetAsObject(string key)
        {
            Debug.Assert(key != null, "Trying to get value for a null key.");
            Debug.Assert(key.Length > 0, "Trying to get value for an empty key.");
            if (_keyVal is Dictionary<string, object> dict)
            {
                if (dict.TryGetValue(key, out object val))
                    return val;
            }
            if (_parent != null)
                return _parent.GetAsObject(key);
            Debug.Assert(false, "Trying to get a non existing object from typed key value " + key);
            return null;
        }

        public Vec2 GetAsVec2(string key, Vec2 defaultValue = null)
        {
            if (Has(key))
            {
                var arr = GetAsArray(key);
                Debug.Assert(arr.Length == 2, "Trying to get a vec2 without both x and y.");
                double x = Convert.ToDouble(arr.GetValue(0));
                double y = Convert.ToDouble(arr.GetValue(1));
                return new Vec2(x, y);
            }
            Debug.Assert(defaultValue != null, "Trying to get a vec2 without default value: " + key);
            return defaultValue;
        }

        private string _GetAsString(string key)
        {
            Debug.Assert(key != null, "Trying to get value for a null key.");
            Debug.Assert(key.Length > 0, "Trying to get value for an empty key.");
            if (_keyVal is Dictionary<string, object> dict)
            {
                if (dict.TryGetValue(key, out object val))
                    return val?.ToString();
            }
            if (_parent != null)
                return _parent._GetAsString(key);
            return null;
        }
    }
}