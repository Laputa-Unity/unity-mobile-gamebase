using CustomInspector.Resolvers;
using CustomInspector;
using CustomInspector.Validators;

[assembly: RegisterCustomAttributeValidator(typeof(DropdownValidator<>))]

namespace CustomInspector.Validators
{
    public class DropdownValidator<T> : CustomAttributeValidator<DropdownAttribute>
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

        public override CustomValidationResult Validate(CustomProperty property)
        {
            foreach (var item in _valuesResolver.GetDropdownItems(property))
            {
                if (property.Comparer.Equals(item.Value, property.Value))
                {
                    return CustomValidationResult.Valid;
                }
            }

            var msg = $"Dropdown value '{property.Value}' not valid";

            switch (Attribute.ValidationMessageType)
            {
                case CustomMessageType.Info:
                    return CustomValidationResult.Info(msg);

                case CustomMessageType.Warning:
                    return CustomValidationResult.Warning(msg);

                case CustomMessageType.Error:
                    return CustomValidationResult.Error(msg);
            }

            return CustomValidationResult.Valid;
        }
    }
}