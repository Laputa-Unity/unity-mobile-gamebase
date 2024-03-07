using UnityEditor;
using UnityEngine;

namespace CustomInspectorUnityInternalBridge
{
    internal static class EditorGUIUtilityProxy
    {
        public static Texture2D GetHelpIcon(MessageType type)
        {
            return EditorGUIUtility.GetHelpIcon(type);
        }
    }
}