#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class RenderableCoordinateSystem : CoordinateSystem
    {
        #region Private Variables
        [SerializeField]
        private CoordinateSystemRenderSettings _renderSettings;
        private CoordinateSystemRenderer _renderer = new CoordinateSystemRenderer();
        #endregion

        #region Public Properties
        public CoordinateSystemRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<CoordinateSystemRenderSettings>();
                return _renderSettings;
            }
        }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            _renderer.RenderGizmos(this);
        }
        #endregion
    }
}
#endif