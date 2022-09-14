#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ShaderPool
    {
        [SerializeField]
        private Shader _gridShader;

        public static ShaderPool Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ShaderPool;
        }

        public Shader GridShader
        {
            get
            {
                if (_gridShader == null) _gridShader = Shader.Find("Octave3D/XZGrid");
                return _gridShader;
            }
        }
    }
}
#endif