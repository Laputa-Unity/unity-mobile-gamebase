using CustomInspector;
using CustomInspector.Drawers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterCustomAttributeDrawer(typeof(DisplayAsStringDrawer), CustomDrawerOrder.Decorator, ApplyOnArrayElement = true)]

namespace CustomInspector.Drawers
{
    public class DisplayAsStringDrawer : CustomAttributeDrawer<DisplayAsStringAttribute>
    {
        public override float GetHeight(float width, CustomProperty property, CustomElement next)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, CustomProperty property, CustomElement next)
        {
            var value = property.Value;
            var text = value != null ? value.ToString() : "Null";

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            position = EditorGUI.PrefixLabel(position, controlId, property.DisplayNameContent);
            GUI.Label(position, text);
        }
    }
}