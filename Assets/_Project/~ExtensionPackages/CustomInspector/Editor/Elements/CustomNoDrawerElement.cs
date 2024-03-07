using UnityEditor;
using UnityEngine;

namespace CustomInspector.Elements
{
    public class CustomNoDrawerElement : CustomElement
    {
        private readonly GUIContent _message;
        private readonly CustomProperty _property;

        public CustomNoDrawerElement(CustomProperty property)
        {
            _property = property;
            _message = new GUIContent($"No drawer for {property.FieldType}");
        }

        public override float GetHeight(float width)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position)
        {
            EditorGUI.LabelField(position, _property.DisplayNameContent, _message);
        }
    }
}