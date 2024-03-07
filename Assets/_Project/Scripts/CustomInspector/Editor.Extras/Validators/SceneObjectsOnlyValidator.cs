using CustomInspector;
using CustomInspector.Validators;
using UnityEditor;
using UnityEngine;

[assembly: RegisterCustomAttributeValidator(typeof(SceneObjectsOnlyValidator))]

namespace CustomInspector.Validators
{
    public class SceneObjectsOnlyValidator : CustomAttributeValidator<SceneObjectsOnlyAttribute>
    {
        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            if (!typeof(Object).IsAssignableFrom(propertyDefinition.FieldType))
            {
                return "AssetsOnly attribute can be used only on Object fields";
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override CustomValidationResult Validate(CustomProperty property)
        {
            var obj = property.TryGetSerializedProperty(out var serializedProperty)
                ? serializedProperty.objectReferenceValue
                : (Object) property.Value;

            if (obj == null || !AssetDatabase.Contains(obj))
            {
                return CustomValidationResult.Valid;
            }

            return CustomValidationResult.Error($"{obj} cannot be an asset.");
        }
    }
}