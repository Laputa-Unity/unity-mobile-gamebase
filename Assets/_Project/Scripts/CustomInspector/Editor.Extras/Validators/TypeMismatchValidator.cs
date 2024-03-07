using CustomInspector;
using CustomInspector.Validators;
using UnityEditor;

[assembly: RegisterCustomValueValidator(typeof(TypeMismatchValidator<>))]

namespace CustomInspector.Validators
{
    public class TypeMismatchValidator<T> : CustomValueValidator<T>
        where T : UnityEngine.Object
    {
        public override CustomValidationResult Validate(CustomValue<T> propertyValue)
        {
            if (propertyValue.Property.TryGetSerializedProperty(out var serializedProperty) &&
                serializedProperty.propertyType == SerializedPropertyType.ObjectReference &&
                serializedProperty.objectReferenceValue != null &&
                (serializedProperty.objectReferenceValue is T) == false)
            {
                var displayName = propertyValue.Property.DisplayName;
                var actual = serializedProperty.objectReferenceValue.GetType().Name;
                var expected = propertyValue.Property.FieldType.Name;
                var msg = $"{displayName} does not match the type: actual = {actual}, expected = {expected}";
                return CustomValidationResult.Warning(msg);
            }

            return CustomValidationResult.Valid;
        }
    }
}