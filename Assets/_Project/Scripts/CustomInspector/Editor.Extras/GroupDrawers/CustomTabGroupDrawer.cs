using CustomInspector.Elements;
using CustomInspector;
using CustomInspector.GroupDrawers;

[assembly: RegisterCustomGroupDrawer(typeof(CustomTabGroupDrawer))]

namespace CustomInspector.GroupDrawers
{
    public class CustomTabGroupDrawer : CustomGroupDrawer<DeclareTabGroupAttribute>
    {
        public override CustomPropertyCollectionBaseElement CreateElement(DeclareTabGroupAttribute attribute)
        {
            return new CustomTabGroupElement();
        }
    }
}