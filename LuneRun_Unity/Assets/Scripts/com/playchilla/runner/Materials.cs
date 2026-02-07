using System.Collections.Generic;
using UnityEngine;

namespace com.playchilla.runner
{
    public class Materials
    {
        private Dictionary<string, Material> _materials = new ();
        
        public void RegisterMaterial(string name, Material material)
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

        public List<Material> GetMaterialVector(string str)
        {
            return new List<Material>(); // TOdo;

        }
    }

}