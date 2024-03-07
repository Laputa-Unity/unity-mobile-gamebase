using CustomInspector;
using CustomInspector.Drawers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterCustomAttributeDrawer(typeof(LabelWidthDrawer), CustomDrawerOrder.Decorator)]

namespace CustomInspector.Drawers
{
    public class LabelWidthDrawer : CustomAttributeDrawer<LabelWidthAttribute>
    {
        public override void OnGUI(Rect position, CustomProperty property, CustomElement next)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = Attribute.Width;
            next.OnGUI(position);
            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}