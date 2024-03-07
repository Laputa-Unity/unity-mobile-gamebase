using UnityEditor;
using UnityEngine;

namespace CustomInspector.Elements
{
    public class CustomMultiEditNotSupportedElement : CustomElement
    {
        private readonly CustomProperty _property;
        private readonly GUIContent _message;

        public CustomMultiEditNotSupportedElement(CustomProperty property)
        {
            _property = property;
            _message = new GUIContent("Multi edit not supported");
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