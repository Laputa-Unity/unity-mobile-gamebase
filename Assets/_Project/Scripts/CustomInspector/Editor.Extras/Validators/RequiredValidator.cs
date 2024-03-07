using CustomInspector.Resolvers;
using JetBrains.Annotations;
using CustomInspector.Validators;
using CustomInspector;

[assembly: RegisterCustomAttributeValidator(typeof(RequiredValidator), ApplyOnArrayElement = true)]

namespace CustomInspector.Validators
{
    public class RequiredValidator : CustomAttributeValidator<RequiredAttribute>
    {
        [CanBeNull] private ActionResolver _fixActionResolver;

        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            if (Attribute.FixAction != null)
            {
                _fixActionResolver = ActionResolver.Resolve(propertyDefinition, Attribute.FixAction);
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override CustomValidationResult Validate(CustomProperty property)
        {
            if (property.FieldType == typeof(string))
            {
                var isNull = string.IsNullOrEmpty((string) property.Value);
                if (isNull)
                {
                    var message = Attribute.Message ?? $"{GetName(property)} is required";
                    return MakeError(message, property);
                }
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(property.FieldType))
            {
                var isNull = null == (UnityEngine.Object) property.Value;
                if (isNull)
                {
                    var message = Attribute.Message ?? $"{GetName(property)} is required";
                    return MakeError(message, property);
                }
            }
            else
            {
                return CustomValidationResult.Error("RequiredAttribute only valid on Object and String");
            }

            return CustomValidationResult.Valid;
        }

        private CustomValidationResult MakeError(string error, CustomProperty property)
        {
            var result = CustomValidationResult.Error(error);

            if (_fixActionResolver != null)
            {
                result = AddFix(result, property);
            }

            return result;
        }

        private CustomValidationResult AddFix(CustomValidationResult result, CustomProperty property)
        {
            return result.WithFix(() => _fixActionResolver?.InvokeForAllTargets(property), Attribute.FixActionName);
        }

        private static string GetName(CustomProperty property)
        {
            var name = property.DisplayName;
            if (string.IsNullOrEmpty(name))
            {
                name = property.RawName;
            }

            return name;
        }
    }
}