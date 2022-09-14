#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class SceneRenderPathFactory
    {
        #region Public Static Functions
        public static SceneRenderPath Create(SceneRenderPathType sceneRenderPathType)
        {
            switch(sceneRenderPathType)
            {
                case SceneRenderPathType.ObjectPlacement:

                    return new ObjectPlacementSceneRenderPath();

                case SceneRenderPathType.ObjectErase:

                    return new ObjectEraseSceneRenderPath();

                case SceneRenderPathType.ObjectSelection:
                case SceneRenderPathType.ScenePreparation:

                    return new ObjectSelectionSceneRenderPath();

                case SceneRenderPathType.ObjectLayers:

                    return new ObjectLayersSceneRenderPath();

                default:

                    return null;
            }
        }
        #endregion
    }
}
#endif