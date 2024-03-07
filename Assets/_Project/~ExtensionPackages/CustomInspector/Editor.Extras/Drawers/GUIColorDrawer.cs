using CustomInspector.Resolvers;
using JetBrains.Annotations;
using CustomInspector;
using CustomInspector.Drawers;
using UnityEngine;

[assembly: RegisterCustomAttributeDrawer(typeof(GUIColorDrawer), CustomDrawerOrder.Decorator)]

namespace CustomInspector.Drawers
{
    public class GUIColorDrawer : CustomAttributeDrawer<GUIColorAttribute>
    {
        [CanBeNull] private ValueResolver<Color> _colorResolver;

        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            if (!string.IsNullOrEmpty(Attribute.GetColor))
            {
                _colorResolver = ValueResolver.Resolve<Color>(propertyDefinition, Attribute.GetColor);
            }

            if (_colorResolver != null && _colorResolver.TryGetErrorString(out var error))
            {
                return error;
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override void OnGUI(Rect position, CustomProperty property, CustomElement next)
        {
            var oldColor = GUI.color;
            var newColor = _colorResolver?.GetValue(property, Color.white) ?? Attribute.Color;

            GUI.color = newColor;
            GUI.contentColor = newColor;

            next.OnGUI(position);

            GUI.color = oldColor;
            GUI.contentColor = oldColor;
        }
    }
}