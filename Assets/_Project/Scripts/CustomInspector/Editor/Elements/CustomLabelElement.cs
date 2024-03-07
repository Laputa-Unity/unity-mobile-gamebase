using UnityEngine;

namespace CustomInspector.Elements
{
    public class CustomLabelElement : CustomElement
    {
        private readonly GUIContent _label;

        public CustomLabelElement(string label, string tooltip = "")
        {
            _label = new GUIContent(label, tooltip);
        }

        public CustomLabelElement(GUIContent label)
        {
            _label = label;
        }

        public override float GetHeight(float width)
        {
            return GUI.skin.label.CalcHeight(_label, width);
        }

        public override void OnGUI(Rect position)
        {
            GUI.Label(position, _label);
        }
    }
}