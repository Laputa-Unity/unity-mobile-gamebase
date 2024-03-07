using System;
using System.Collections.Generic;
using CustomInspector.Utilities;

namespace CustomInspector
{
    public class CustomTypeDefinition
    {
        private static readonly Dictionary<Type, CustomTypeDefinition> Cache =
            new Dictionary<Type, CustomTypeDefinition>();

        private CustomTypeDefinition(IReadOnlyList<CustomPropertyDefinition> properties)
        {
            Properties = properties;
        }

        public IReadOnlyList<CustomPropertyDefinition> Properties { get; }

        public static CustomTypeDefinition GetCached(Type type)
        {
            if (Cache.TryGetValue(type, out var definition))
            {
                return definition;
            }

            var processors = CustomDrawersUtilities.AllTypeProcessors;
            var properties = new List<CustomPropertyDefinition>();

            foreach (var processor in processors)
            {
                processor.ProcessType(type, properties);
            }

            return Cache[type] = new CustomTypeDefinition(properties);
        }
    }
}