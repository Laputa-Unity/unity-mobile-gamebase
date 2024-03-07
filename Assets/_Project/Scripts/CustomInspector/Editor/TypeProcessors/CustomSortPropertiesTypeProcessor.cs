using System;
using System.Collections.Generic;
using CustomInspector;
using CustomInspector.TypeProcessors;
using CustomInspector.Utilities;

[assembly: RegisterCustomTypeProcessor(typeof(CustomSortPropertiesTypeProcessor), 10000)]

namespace CustomInspector.TypeProcessors
{
    public class CustomSortPropertiesTypeProcessor : CustomTypeProcessor
    {
        public override void ProcessType(Type type, List<CustomPropertyDefinition> properties)
        {
            foreach (var propertyDefinition in properties)
            {
                if (propertyDefinition.Attributes.TryGet(out PropertyOrderAttribute orderAttribute))
                {
                    propertyDefinition.Order = orderAttribute.Order;
                }
            }

            properties.Sort(PropertyOrderComparer.Instance);
        }

        private class PropertyOrderComparer : IComparer<CustomPropertyDefinition>
        {
            public static readonly PropertyOrderComparer Instance = new PropertyOrderComparer();

            public int Compare(CustomPropertyDefinition x, CustomPropertyDefinition y)
            {
                return x.Order.CompareTo(y.Order);
            }
        }
    }
}