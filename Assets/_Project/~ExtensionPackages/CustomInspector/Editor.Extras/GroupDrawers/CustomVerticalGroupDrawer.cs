using CustomInspector.Elements;
using CustomInspector;
using CustomInspector.GroupDrawers;

[assembly: RegisterCustomGroupDrawer(typeof(CustomVerticalGroupDrawer))]

namespace CustomInspector.GroupDrawers
{
    public class CustomVerticalGroupDrawer : CustomGroupDrawer<DeclareVerticalGroupAttribute>
    {
        public override CustomPropertyCollectionBaseElement CreateElement(DeclareVerticalGroupAttribute attribute)
        {
            return new CustomVerticalGroupElement();
        }
    }
}