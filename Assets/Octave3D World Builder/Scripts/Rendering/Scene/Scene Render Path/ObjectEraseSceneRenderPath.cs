#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectEraseSceneRenderPath : SceneRenderPath
    {
        #region Public Methods
        public override void RenderGizmos()
        {
            ObjectSnapping.Get().XZSnapGrid.RenderGizmos();
            ObjectEraser.Get().RenderGizmos();
        }

        public override void RenderHandles()
        {
        }
        #endregion
    }
}
#endif