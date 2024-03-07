using CustomInspector.Elements;
using CustomInspector;
using CustomInspector.GroupDrawers;

[assembly: RegisterCustomGroupDrawer(typeof(CustomBoxGroupDrawer))]

namespace CustomInspector.GroupDrawers
{
    public class CustomBoxGroupDrawer : CustomGroupDrawer<DeclareBoxGroupAttribute>
    {
        public override CustomPropertyCollectionBaseElement CreateElement(DeclareBoxGroupAttribute attribute)
        {
            return new CustomBoxGroupElement(new CustomBoxGroupElement.Props
            {
                title = attribute.Title,
                titleMode = attribute.HideTitle
                    ? CustomBoxGroupElement.TitleMode.Hidden
                    : CustomBoxGroupElement.TitleMode.Normal,
                hideIfChildrenInvisible = true,
            });
        }
    }
}