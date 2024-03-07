using JetBrains.Annotations;
using UnityEngine;

namespace CustomInspector
{
    public abstract class ValueDrawer : CustomDrawer
    {
    }

    public abstract class ValueDrawer<TValue> : ValueDrawer
    {
        public sealed override CustomElement CreateElementInternal(CustomProperty property, CustomElement next)
        {
            return CreateElement(new CustomValue<TValue>(property), next);
        }

        [PublicAPI]
        public virtual CustomElement CreateElement(CustomValue<TValue> propertyValue, CustomElement next)
        {
            return new DefaultValueDrawerElement<TValue>(this, propertyValue, next);
        }

        [PublicAPI]
        public virtual float GetHeight(float width, CustomValue<TValue> propertyValue, CustomElement next)
        {
            return next.GetHeight(width);
        }

        [PublicAPI]
        public virtual void OnGUI(Rect position, CustomValue<TValue> propertyValue, CustomElement next)
        {
            next.OnGUI(position);
        }

        internal class DefaultValueDrawerElement<T> : CustomElement
        {
            private readonly ValueDrawer<T> _drawer;
            private readonly CustomElement _next;
            private readonly CustomValue<T> _propertyValue;

            public DefaultValueDrawerElement(ValueDrawer<T> drawer, CustomValue<T> propertyValue, CustomElement next)
            {
                _drawer = drawer;
                _propertyValue = propertyValue;
                _next = next;

                AddChild(next);
            }

            public override float GetHeight(float width)
            {
                return _drawer.GetHeight(width, _propertyValue, _next);
            }

            public override void OnGUI(Rect position)
            {
                _drawer.OnGUI(position, _propertyValue, _next);
            }
        }
    }
}