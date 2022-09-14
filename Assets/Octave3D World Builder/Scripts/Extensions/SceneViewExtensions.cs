#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace O3DWB
{
    public static class SceneViewExtensions
    {
        #region Utilities
        public static bool IsSceneViewWindowFocused()
        {
            // Note: The focused window can be null if we just switched from play mode to edit mode.
            if (EditorWindow.focusedWindow == null) return false;
            return EditorWindow.focusedWindow.GetType() == typeof(SceneView);
        }
        #endregion
    }
}
#endif