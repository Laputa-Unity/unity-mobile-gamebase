using System.Collections.Generic;
using System.Text;
using CustomInspector.Elements;
using CustomInspector;
using CustomInspector.Drawers;

[assembly: RegisterCustomAttributeDrawer(typeof(ShowDrawerChainDrawer), CustomDrawerOrder.System)]

namespace CustomInspector.Drawers
{
    public class ShowDrawerChainDrawer : CustomAttributeDrawer<ShowDrawerChainAttribute>
    {
        public override CustomElement CreateElement(CustomProperty property, CustomElement next)
        {
            return new CustomDrawerChainInfoElement(property.AllDrawers, next);
        }
    }

    public class CustomDrawerChainInfoElement : CustomElement
    {
        public CustomDrawerChainInfoElement(IReadOnlyList<CustomDrawer> drawers, CustomElement next)
        {
            var info = new StringBuilder();

            info.Append("Drawer Chain:");

            for (var i = 0; i < drawers.Count; i++)
            {
                var drawer = drawers[i];
                info.AppendLine();
                info.Append(i).Append(": ").Append(drawer.GetType().Name);
            }

            AddChild(new CustomInfoBoxElement(info.ToString()));
            AddChild(next);
        }
    }
}