using CustomInspector.Editors;
using CustomInspector.Elements;
using CustomInspector.Utilities;
using CustomInspector;
using CustomInspector.Drawers;
using CustomInspectorUnityInternalBridge;

[assembly: RegisterCustomValueDrawer(typeof(CustomBuiltInDrawer), CustomDrawerOrder.Fallback - 999)]

namespace CustomInspector.Drawers
{
    public class CustomBuiltInDrawer : ValueDrawer<object>
    {
        public override CustomElement CreateElement(CustomValue<object> propertyValue, CustomElement next)
        {
            var property = propertyValue.Property;

            if (property.TryGetSerializedProperty(out var serializedProperty))
            {
                var handler = ScriptAttributeUtilityProxy.GetHandler(serializedProperty);

                var drawWithHandler = handler.hasPropertyDrawer ||
                                      property.PropertyType == CustomPropertyType.Primitive ||
                                      CustomUnityInspectorUtilities.MustDrawWithUnity(property);

                if (drawWithHandler)
                {
                    if (property.TryGetAttribute(out DrawWithUnityAttribute withUnityAttribute) &&
                        withUnityAttribute.WithUiToolkit)
                    {
                        handler.SetPreferredLabel(property.DisplayName);

                        var visualElement = handler.CreatePropertyGUI(serializedProperty);

                        if (visualElement != null &&
                            CustomEditorCore.UiElementsRoots.TryGetValue(property.PropertyTree, out var rootElement))
                        {
                            return new CustomUiToolkitPropertyElement(property, serializedProperty,
                                visualElement, rootElement);
                        }
                    }

                    return new CustomBuiltInPropertyElement(property, serializedProperty, handler);
                }
            }

            return base.CreateElement(propertyValue, next);
        }
    }
}