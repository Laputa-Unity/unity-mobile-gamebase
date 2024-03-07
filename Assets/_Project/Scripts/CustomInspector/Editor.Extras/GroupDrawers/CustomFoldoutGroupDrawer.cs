using CustomInspector.Elements;
using CustomInspector;
using CustomInspector.GroupDrawers;

[assembly: RegisterCustomGroupDrawer(typeof(CustomFoldoutGroupDrawer))]

namespace CustomInspector.GroupDrawers
{
    public class CustomFoldoutGroupDrawer : CustomGroupDrawer<DeclareFoldoutGroupAttribute>
    {
        public override CustomPropertyCollectionBaseElement CreateElement(DeclareFoldoutGroupAttribute attribute)
        {
            return new CustomBoxGroupElement(new CustomBoxGroupElement.Props
            {
                title = attribute.Title,
                titleMode = CustomBoxGroupElement.TitleMode.Foldout,
                expandedByDefault = attribute.Expanded,
                hideIfChildrenInvisible = true,
            });
        }
    }
}