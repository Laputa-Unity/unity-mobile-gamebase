using CustomInspector.Elements;
using CustomInspector;
using CustomInspector.GroupDrawers;
using UnityEngine;

[assembly: RegisterCustomGroupDrawer(typeof(CustomHorizontalGroupDrawer))]

namespace CustomInspector.GroupDrawers
{
    public class CustomHorizontalGroupDrawer : CustomGroupDrawer<DeclareHorizontalGroupAttribute>
    {
        public override CustomPropertyCollectionBaseElement CreateElement(DeclareHorizontalGroupAttribute attribute)
        {
            return new CustomHorizontalGroupElement(attribute.Sizes);
        }
    }
}