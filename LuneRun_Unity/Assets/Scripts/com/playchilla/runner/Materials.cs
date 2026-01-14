using System.Collections.Generic;

namespace com.playchilla.runner
{
    public class Materials
    {
        private Dictionary<string, object> _materials = new Dictionary<string, object>();
        
        public void RegisterMaterial(string name, object material)
        {
            // Stub implementation
            _materials[name] = material;
        }
        
        public object GetMaterial(string name)
        {
            // Stub implementation
            if (_materials.ContainsKey(name))
                return _materials[name];
            return null;
        }
    }
}