using CustomInspector.Resolvers;
using CustomInspector;
using CustomInspector.Validators;

[assembly: RegisterCustomAttributeValidator(typeof(InfoBoxValidator))]

namespace CustomInspector.Validators
{
    public class InfoBoxValidator : CustomAttributeValidator<InfoBoxAttribute>
    {
        private ValueResolver<string> _resolver;
        private ValueResolver<bool> _visibleIfResolver;

        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            _resolver = ValueResolver.ResolveString(propertyDefinition, Attribute.Text);
            _visibleIfResolver = Attribute.VisibleIf != null
                ? ValueResolver.Resolve<bool>(propertyDefinition, Attribute.VisibleIf)
                : null;

            if (ValueResolver.TryGetErrorString(_resolver, _visibleIfResolver, out var error))
            {
                return error;
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override CustomValidationResult Validate(CustomProperty property)
        {
            if (_visibleIfResolver != null && !_visibleIfResolver.GetValue(property))
            {
                return CustomValidationResult.Valid;
            }

            var message = _resolver.GetValue(property, "");
            return new CustomValidationResult(false, message, Attribute.MessageType);
        }
    }
}