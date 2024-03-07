using CustomInspector;
using CustomInspector.Validators;
using UnityEditor;

[assembly: RegisterCustomValueValidator(typeof(MissingReferenceValidator))]

namespace CustomInspector.Validators
{
    public class MissingReferenceValidator : CustomValueValidator<UnityEngine.Object>
    {
        public override CustomValidationResult Validate(CustomValue<UnityEngine.Object> propertyValue)
        {
            if (propertyValue.Property.TryGetSerializedProperty(out var serializedProperty) &&
                serializedProperty.propertyType == SerializedPropertyType.ObjectReference &&
                serializedProperty.objectReferenceValue == null &&
                serializedProperty.objectReferenceInstanceIDValue != 0)
            {
                return CustomValidationResult.Warning($"{GetName(propertyValue.Property)} is missing");
            }

            return CustomValidationResult.Valid;
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