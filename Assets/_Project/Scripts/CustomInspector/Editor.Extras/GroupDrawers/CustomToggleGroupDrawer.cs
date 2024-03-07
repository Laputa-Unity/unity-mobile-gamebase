using CustomInspector.Elements;
using CustomInspector;
using CustomInspector.GroupDrawers;

[assembly: RegisterCustomGroupDrawer(typeof(CustomToggleGroupDrawer))]

namespace CustomInspector.GroupDrawers
{
    public class CustomToggleGroupDrawer : CustomGroupDrawer<DeclareToggleGroupAttribute>
    {
        public override CustomPropertyCollectionBaseElement CreateElement(DeclareToggleGroupAttribute attribute)
        {
            return new CustomBoxGroupElement(new CustomBoxGroupElement.Props
            {
                title = attribute.Title,
                titleMode = CustomBoxGroupElement.TitleMode.Toggle,
                expandedByDefault = attribute.Collapsible,
                hideIfChildrenInvisible = true,
            });
        }
    }
}