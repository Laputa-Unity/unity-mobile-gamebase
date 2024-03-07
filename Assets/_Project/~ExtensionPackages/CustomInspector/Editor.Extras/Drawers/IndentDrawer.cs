using CustomInspector.Utilities;
using CustomInspector;
using CustomInspector.Drawers;
using UnityEngine;

[assembly: RegisterCustomAttributeDrawer(typeof(IndentDrawer), CustomDrawerOrder.Decorator)]

namespace CustomInspector.Drawers
{
    public class IndentDrawer : CustomAttributeDrawer<IndentAttribute>
    {
        public override void OnGUI(Rect position, CustomProperty property, CustomElement next)
        {
            using (var indentedRectScope = CustomGuiHelper.PushIndentedRect(position, Attribute.Indent))
            {
                next.OnGUI(indentedRectScope.IndentedRect);
            }
        }
    }
}