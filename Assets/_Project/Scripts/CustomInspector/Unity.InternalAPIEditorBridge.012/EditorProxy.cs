using UnityEditor;

namespace CustomInspectorUnityInternalBridge
{
    internal static class EditorProxy
    {
        public static void DoDrawDefaultInspector(SerializedObject obj)
        {
            Editor.DoDrawDefaultInspector(obj);
        }
    }
}