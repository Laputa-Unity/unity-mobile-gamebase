using UnityEditor;
using UnityEngine;

namespace CustomInspector.Drawers
{
    public abstract class BuiltinDrawerBase<T> : ValueDrawer<T>
    {
        public sealed override CustomElement CreateElement(CustomValue<T> propertyValue, CustomElement next)
        {
            if (propertyValue.Property.TryGetSerializedProperty(out _))
            {
                return next;
            }

            return base.CreateElement(propertyValue, next);
        }

        public virtual int CompactModeLines => 1;
        public virtual int WideModeLines => 1;

        public sealed override float GetHeight(float width, CustomValue<T> propertyValue, CustomElement next)
        {
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            var lines = EditorGUIUtility.wideMode ? WideModeLines : CompactModeLines;
            return lineHeight * lines + spacing * (lines - 1);
        }

        public sealed override void OnGUI(Rect position, CustomValue<T> propertyValue, CustomElement next)
        {
            var value = propertyValue.SmartValue;

            EditorGUI.BeginChangeCheck();

            value = OnValueGUI(position, propertyValue.Property.DisplayNameContent, value);

            if (EditorGUI.EndChangeCheck())
            {
                propertyValue.SetValue(value);
            }
        }

        protected abstract T OnValueGUI(Rect position, GUIContent label, T value);
    }
}