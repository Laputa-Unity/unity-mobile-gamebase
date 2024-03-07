using CustomInspector;
using CustomInspector.Validators;
using UnityEditor;

[assembly: RegisterCustomAttributeValidator(typeof(SceneValidator), ApplyOnArrayElement = true)]

namespace CustomInspector.Validators
{
    public class SceneValidator : CustomAttributeValidator<SceneAttribute>
    {
        public override CustomValidationResult Validate(CustomProperty property)
        {
            if (property.FieldType == typeof(string))
            {
                var value = (string) property.Value;

                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(value) == null)
                {
                    return CustomValidationResult.Error($"{value} not a valid scene");
                }

                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (!property.Comparer.Equals(value, scene.path))
                    {
                        continue;
                    }

                    if (!scene.enabled)
                    {
                        return CustomValidationResult.Error($"{value} disabled in build settings");
                    }

                    return CustomValidationResult.Valid;
                }

                return CustomValidationResult.Error($"{value} not added to build settings");
            }

            return CustomValidationResult.Valid;
        }
    }
}