using CustomInspector;
using CustomInspector.Drawers;
using UnityEngine;

[assembly: RegisterCustomAttributeDrawer(typeof(PropertySpaceDrawer), CustomDrawerOrder.Inspector)]

namespace CustomInspector.Drawers
{
    public class PropertySpaceDrawer : CustomAttributeDrawer<PropertySpaceAttribute>
    {
        public override float GetHeight(float width, CustomProperty property, CustomElement next)
        {
            var totalSpace = Attribute.SpaceBefore + Attribute.SpaceAfter;

            return next.GetHeight(width) + totalSpace;
        }

        public override void OnGUI(Rect position, CustomProperty property, CustomElement next)
        {
            var contentPosition = new Rect(position)
            {
                yMin = position.yMin + Attribute.SpaceBefore,
                yMax = position.yMax - Attribute.SpaceAfter,
            };

            next.OnGUI(contentPosition);
        }
    }
}