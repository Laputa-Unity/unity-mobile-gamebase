﻿using System;
using JetBrains.Annotations;

namespace CustomInspector
{
    public struct CustomValue<T>
    {
        internal CustomValue(CustomProperty property)
        {
            Property = property;
        }

        public CustomProperty Property { get; }

        [Obsolete("Use SmartValue instead", true)]
        public T Value
        {
            get => (T) Property.Value;
            set => Property.SetValue(value);
        }

        [PublicAPI]
        public T SmartValue
        {
            get => (T) Property.Value;
            set
            {
                if (Property.Comparer.Equals(Property.Value, value))
                {
                    return;
                }

                Property.SetValue(value);
            }
        }

        [PublicAPI]
        public void SetValue(T value)
        {
            Property.SetValue(value);
        }
    }
}