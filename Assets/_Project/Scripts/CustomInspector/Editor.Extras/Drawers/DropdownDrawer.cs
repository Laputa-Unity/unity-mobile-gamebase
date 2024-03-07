using CustomInspector.Elements;
using CustomInspector.Resolvers;
using CustomInspector;
using CustomInspector.Drawers;

[assembly: RegisterCustomAttributeDrawer(typeof(DropdownDrawer<>), CustomDrawerOrder.Decorator)]

namespace CustomInspector.Drawers
{
    public class DropdownDrawer<T> : CustomAttributeDrawer<DropdownAttribute>
    {
        private DropdownValuesResolver<T> _valuesResolver;

        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            _valuesResolver = DropdownValuesResolver<T>.Resolve(propertyDefinition, Attribute.Values);

            if (_valuesResolver.TryGetErrorString(out var error))
            {
                return error;
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override CustomElement CreateElement(CustomProperty property, CustomElement next)
        {
            return new CustomDropdownElement(property, _valuesResolver.GetDropdownItems);
        }
    }
}