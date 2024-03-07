using System;
using JetBrains.Annotations;

namespace CustomInspector
{
    public abstract class CustomPropertyDisableProcessor : CustomPropertyExtension
    {
        internal Attribute RawAttribute { get; set; }

        [PublicAPI]
        public abstract bool IsDisabled(CustomProperty property);
    }

    public abstract class CustomPropertyDisableProcessor<TAttribute> : CustomPropertyDisableProcessor
        where TAttribute : Attribute
    {
        [PublicAPI]
        public TAttribute Attribute => (TAttribute) RawAttribute;
    }
}