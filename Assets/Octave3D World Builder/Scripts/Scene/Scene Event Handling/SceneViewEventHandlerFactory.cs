#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class SceneViewEventHandlerFactory
    {
        #region Public Static Functions
        public static SceneViewEventHandler Create(InspectorGUIIdentifier activeGUIIdentifier)
        {
            switch(activeGUIIdentifier)
            {
                case InspectorGUIIdentifier.ObjectSnapping:
                case InspectorGUIIdentifier.ObjectPlacement:

                    return new ObjectPlacementSceneViewEventHandler();

                case InspectorGUIIdentifier.ObjectErase:

                    return new ObjectEraserSceneViewEventHandler();

                case InspectorGUIIdentifier.ObjectSelection:

                    return new ObjectSelectionSceneViewEventHandler();

                default:

                    return null;
            }
        }
        #endregion
    }
}
#endif