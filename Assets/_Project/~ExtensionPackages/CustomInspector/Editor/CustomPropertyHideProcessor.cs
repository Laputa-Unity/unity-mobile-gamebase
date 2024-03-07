using System;
using JetBrains.Annotations;

namespace CustomInspector
{
    public abstract class CustomPropertyHideProcessor : CustomPropertyExtension
    {
        internal Attribute RawAttribute { get; set; }

        [PublicAPI]
        public abstract bool IsHidden(CustomProperty property);
    }

    public abstract class CustomPropertyHideProcessor<TAttribute> : CustomPropertyHideProcessor
        where TAttribute : Attribute
    {
        [PublicAPI]
        public TAttribute Attribute => (TAttribute) RawAttribute;
    }
}