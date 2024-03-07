using System.Collections.Generic;
using CustomInspector.Resolvers;
using UnityEngine;

namespace CustomInspector.Elements
{
    public class CustomTabGroupElement : CustomHeaderGroupBaseElement
    {
        private const string DefaultTabName = "Main";

        private readonly List<TabInfo> _tabs;
        private readonly Dictionary<string, CustomElement> _tabElements;

        private string _activeTabName;

        private struct TabInfo
        {
            public string name;
            public ValueResolver<string> titleResolver;
            public CustomProperty property;
        }

        public CustomTabGroupElement()
        {
            _tabs = new List<TabInfo>();
            _tabElements = new Dictionary<string, CustomElement>();
            _activeTabName = null;
        }

        protected override void DrawHeader(Rect position)
        {
            if (_tabs.Count == 0)
            {
                return;
            }

            var tabRect = new Rect(position)
            {
                width = position.width / _tabs.Count,
            };

            if (_tabs.Count == 1)
            {
                var tab = _tabs[0];
                var content = tab.titleResolver.GetValue(tab.property);
                GUI.Toggle(tabRect, true, content, CustomEditorStyles.TabOnlyOne);
            }
            else
            {
                for (int index = 0, tabCount = _tabs.Count; index < tabCount; index++)
                {
                    var tab = _tabs[index];
                    var content = tab.titleResolver.GetValue(tab.property);
                    var tabStyle = index == 0 ? CustomEditorStyles.TabFirst
                        : index == tabCount - 1 ? CustomEditorStyles.TabLast
                        : CustomEditorStyles.TabMiddle;

                    var isTabActive = GUI.Toggle(tabRect, _activeTabName == tab.name, content, tabStyle);
                    if (isTabActive && _activeTabName != tab.name)
                    {
                        SetActiveTab(tab.name);
                    }

                    tabRect.x += tabRect.width;
                }
            }
        }

        protected override void AddPropertyChild(CustomElement element, CustomProperty property)
        {
            var tabName = DefaultTabName;

            if (property.TryGetAttribute(out TabAttribute tab))
            {
                tabName = tab.TabName ?? tabName;
            }

            if (!_tabElements.TryGetValue(tabName, out var tabElement))
            {
                tabElement = new CustomElement();

                var info = new TabInfo
                {
                    name = tabName,
                    titleResolver = ValueResolver.ResolveString(property.Definition, tabName),
                    property = property,
                };

                _tabElements[tabName] = tabElement;
                _tabs.Add(info);

                if (info.titleResolver.TryGetErrorString(out var error))
                {
                    tabElement.AddChild(new CustomInfoBoxElement(error, CustomMessageType.Error));
                }

                if (_activeTabName == null)
                {
                    SetActiveTab(tabName);
                }
            }

            tabElement.AddChild(element);
        }

        private void SetActiveTab(string tabName)
        {
            _activeTabName = tabName;

            RemoveAllChildren();

            AddChild(_tabElements[_activeTabName]);
        }
    }
}