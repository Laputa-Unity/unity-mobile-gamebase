using System;
using JetBrains.Annotations;

namespace CustomInspector
{
    public abstract class CustomPropertyExtension
    {
        public bool? ApplyOnArrayElement { get; internal set; }

        [PublicAPI]
        public virtual CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            return CustomExtensionInitializationResult.Ok;
        }
    }

    public readonly struct CustomExtensionInitializationResult
    {
        public CustomExtensionInitializationResult(bool shouldApply, string errorMessage)
        {
            ShouldApply = shouldApply;
            ErrorMessage = errorMessage;
        }

        public bool ShouldApply { get; }
        public string ErrorMessage { get; }
        public bool IsError => ErrorMessage != null;

        [PublicAPI]
        public static CustomExtensionInitializationResult Ok => new CustomExtensionInitializationResult(true, null);

        [PublicAPI]
        public static CustomExtensionInitializationResult Skip => new CustomExtensionInitializationResult(false, null);

        [PublicAPI]
        public static CustomExtensionInitializationResult Error([NotNull] string errorMessage)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            return new CustomExtensionInitializationResult(false, errorMessage);
        }

        [PublicAPI]
        public static implicit operator CustomExtensionInitializationResult(string errorMessage)
        {
            return Error(errorMessage);
        }
    }
}