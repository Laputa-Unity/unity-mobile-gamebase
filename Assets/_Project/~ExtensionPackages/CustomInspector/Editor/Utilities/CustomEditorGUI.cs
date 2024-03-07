using UnityEditor;
using UnityEngine;

namespace CustomInspector.Utilities
{
    public static class CustomEditorGUI
    {
        public static void Foldout(Rect rect, CustomProperty property)
        {
            var content = property.DisplayNameContent;
            if (property.TryGetSerializedProperty(out var serializedProperty))
            {
                EditorGUI.BeginProperty(rect, content, serializedProperty);
                property.IsExpanded = EditorGUI.Foldout(rect, property.IsExpanded, content, true);
                EditorGUI.EndProperty();
            }
            else
            {
                property.IsExpanded = EditorGUI.Foldout(rect, property.IsExpanded, content, true);
            }
        }

        public static void DrawBox(Rect position, GUIStyle style,
            bool isHover = false, bool isActive = false, bool on = false, bool hasKeyboardFocus = false)
        {
            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(position, GUIContent.none, isHover, isActive, on, hasKeyboardFocus);
            }
        }
    }
}