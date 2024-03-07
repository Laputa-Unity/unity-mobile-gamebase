using System;
using JetBrains.Annotations;

namespace CustomInspector
{
    public abstract class CustomValidator : CustomPropertyExtension
    {
        [PublicAPI]
        public abstract CustomValidationResult Validate(CustomProperty property);
    }

    public abstract class CustomAttributeValidator : CustomValidator
    {
        internal Attribute RawAttribute { get; set; }
    }

    public abstract class CustomAttributeValidator<TAttribute> : CustomAttributeValidator
        where TAttribute : Attribute
    {
        [PublicAPI]
        public TAttribute Attribute => (TAttribute) RawAttribute;
    }

    public abstract class CustomValueValidator : CustomValidator
    {
    }

    public abstract class CustomValueValidator<T> : CustomValueValidator
    {
        public sealed override CustomValidationResult Validate(CustomProperty property)
        {
            return Validate(new CustomValue<T>(property));
        }

        [PublicAPI]
        public abstract CustomValidationResult Validate(CustomValue<T> propertyValue);
    }
}