#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace O3DWB
{
    public class ObjectPlacementSceneRenderPath : SceneRenderPath
    {
        #region Public Methods
        public override void RenderGizmos()
        {
            ObjectPlacement.Get().RenderGizmos();
        }

        public override void RenderHandles()
        {
            Handles.BeginGUI();
            ObjectPlacement.Get().RenderHandles();
            Handles.EndGUI();
        }
        #endregion
    }
}
#endif