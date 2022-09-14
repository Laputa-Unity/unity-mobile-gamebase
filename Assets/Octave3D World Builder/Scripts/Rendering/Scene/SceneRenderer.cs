#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class SceneRenderer
    {
        #region Public Methods
        public void RenderGizmos()
        {
            if (Octave3DWorldBuilder.ActiveInstance == null) return;
            SceneRenderPathType sceneRenderPathType = InspectorGUIIdentifiers.GetSceneRenderPathTypeFromIdentifier(Octave3DWorldBuilder.ActiveInstance.Inspector.ActiveInspectorGUIIdentifier);
            SceneRenderPathFactory.Create(sceneRenderPathType).RenderGizmos();
        }

        public void RenderHandles()
        {
            if (Octave3DWorldBuilder.ActiveInstance == null) return;
            SceneRenderPathType sceneRenderPathType = InspectorGUIIdentifiers.GetSceneRenderPathTypeFromIdentifier(Octave3DWorldBuilder.ActiveInstance.Inspector.ActiveInspectorGUIIdentifier);
            SceneRenderPathFactory.Create(sceneRenderPathType).RenderHandles();
        }
        #endregion
    }
}
#endif