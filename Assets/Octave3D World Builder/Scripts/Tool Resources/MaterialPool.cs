#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class MaterialPool
    {
        [SerializeField]
        private Material _xzGridMaterial;

        public static MaterialPool Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.MaterialPool;
        }

        public Material XZGridMaterial
        {
            get
            {
                if (_xzGridMaterial == null) _xzGridMaterial = new Material(ShaderPool.Get().GridShader);
                return _xzGridMaterial;
            }
        }
    }
}
#endif