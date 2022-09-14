#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public abstract class SceneRenderPath
    {
        #region Public Abstract Methods
        public abstract void RenderGizmos();
        public abstract void RenderHandles();
        #endregion
    }
}
#endif
