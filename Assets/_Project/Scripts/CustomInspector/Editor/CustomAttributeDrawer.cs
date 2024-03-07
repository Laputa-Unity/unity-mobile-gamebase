using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomInspector
{
    public abstract class CustomAttributeDrawer : CustomDrawer
    {
        internal Attribute RawAttribute { get; set; }
    }

    public abstract class CustomAttributeDrawer<TAttribute> : CustomAttributeDrawer
        where TAttribute : Attribute
    {
        [PublicAPI]
        public TAttribute Attribute => (TAttribute) RawAttribute;

        public sealed override CustomElement CreateElementInternal(CustomProperty property, CustomElement next)
        {
            return CreateElement(property, next);
        }

        [PublicAPI]
        public virtual CustomElement CreateElement(CustomProperty property, CustomElement next)
        {
            return new DefaultAttributeDrawerElement(this, property, next);
        }

        [PublicAPI]
        public virtual float GetHeight(float width, CustomProperty property, CustomElement next)
        {
            return next.GetHeight(width);
        }

        [PublicAPI]
        public virtual void OnGUI(Rect position, CustomProperty property, CustomElement next)
        {
            next.OnGUI(position);
        }

        internal class DefaultAttributeDrawerElement : CustomElement
        {
            private readonly CustomAttributeDrawer<TAttribute> _drawer;
            private readonly CustomElement _next;
            private readonly CustomProperty _property;

            public DefaultAttributeDrawerElement(CustomAttributeDrawer<TAttribute> drawer, CustomProperty property,
                CustomElement next)
            {
                _drawer = drawer;
                _property = property;
                _next = next;

                AddChild(next);
            }

            public override float GetHeight(float width)
            {
                return _drawer.GetHeight(width, _property, _next);
            }

            public override void OnGUI(Rect position)
            {
                _drawer.OnGUI(position, _property, _next);
            }
        }
    }
}