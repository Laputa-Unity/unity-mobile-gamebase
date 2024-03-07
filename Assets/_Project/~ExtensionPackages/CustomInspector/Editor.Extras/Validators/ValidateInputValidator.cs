using CustomInspector.Resolvers;
using CustomInspector;
using CustomInspector.Validators;

[assembly: RegisterCustomAttributeValidator(typeof(ValidateInputValidator))]

namespace CustomInspector.Validators
{
    public class ValidateInputValidator : CustomAttributeValidator<ValidateInputAttribute>
    {
        private ValueResolver<CustomValidationResult> _resolver;

        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            base.Initialize(propertyDefinition);

            _resolver = ValueResolver.Resolve<CustomValidationResult>(propertyDefinition, Attribute.Method);

            if (_resolver.TryGetErrorString(out var error))
            {
                return error;
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override CustomValidationResult Validate(CustomProperty property)
        {
            if (_resolver.TryGetErrorString(out var error))
            {
                return CustomValidationResult.Error(error);
            }

            return _resolver.GetValue(property);
        }
    }
}