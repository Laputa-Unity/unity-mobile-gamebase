using System;
using CustomInspector.Elements;
using JetBrains.Annotations;

namespace CustomInspector
{
    public abstract class CustomGroupDrawer
    {
        public abstract CustomPropertyCollectionBaseElement CreateElementInternal(Attribute attribute);
    }

    public abstract class CustomGroupDrawer<TAttribute> : CustomGroupDrawer
        where TAttribute : Attribute
    {
        public sealed override CustomPropertyCollectionBaseElement CreateElementInternal(Attribute attribute)
        {
            return CreateElement((TAttribute) attribute);
        }

        [PublicAPI]
        public abstract CustomPropertyCollectionBaseElement CreateElement(TAttribute attribute);
    }
}