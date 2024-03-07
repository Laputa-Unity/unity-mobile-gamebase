using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace CustomInspector.Resolvers
{
    public class DropdownValuesResolver<T>
    {
        [CanBeNull] private ValueResolver<IEnumerable<CustomDropdownItem<T>>> _itemsResolver;
        [CanBeNull] private ValueResolver<IEnumerable<T>> _valuesResolver;

        [PublicAPI]
        public static DropdownValuesResolver<T> Resolve(CustomPropertyDefinition propertyDefinition, string expression)
        {
            var valuesResolver = ValueResolver.Resolve<IEnumerable<T>>(propertyDefinition, expression);
            if (!valuesResolver.TryGetErrorString(out _))
            {
                return new DropdownValuesResolver<T>
                {
                    _valuesResolver = valuesResolver,
                };
            }

            var itemsResolver = ValueResolver.Resolve<IEnumerable<CustomDropdownItem<T>>>(propertyDefinition, expression);

            return new DropdownValuesResolver<T>
            {
                _itemsResolver = itemsResolver,
            };
        }

        [PublicAPI]
        public bool TryGetErrorString(out string error)
        {
            return ValueResolver.TryGetErrorString(_valuesResolver, _itemsResolver, out error);
        }

        [PublicAPI]
        public IEnumerable<ICustomDropdownItem> GetDropdownItems(CustomProperty property)
        {
            if (_valuesResolver != null)
            {
                var values = _valuesResolver.GetValue(property, Enumerable.Empty<T>());

                foreach (var value in values)
                {
                    yield return new CustomDropdownItem {Text = $"{value}", Value = value,};
                }
            }

            if (_itemsResolver != null)
            {
                var values = _itemsResolver.GetValue(property, Enumerable.Empty<CustomDropdownItem<T>>());

                foreach (var value in values)
                {
                    yield return value;
                }
            }
        }
    }
}