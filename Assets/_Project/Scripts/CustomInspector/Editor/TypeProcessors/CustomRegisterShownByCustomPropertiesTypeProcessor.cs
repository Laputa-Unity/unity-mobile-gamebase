using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomInspector.Utilities;
using CustomInspector;
using CustomInspector.TypeProcessors;

[assembly: RegisterCustomTypeProcessor(typeof(CustomRegisterShownByCustomPropertiesTypeProcessor), 1)]

namespace CustomInspector.TypeProcessors
{
    public class CustomRegisterShownByCustomPropertiesTypeProcessor : CustomTypeProcessor
    {
        public override void ProcessType(Type type, List<CustomPropertyDefinition> properties)
        {
            const int propertiesOffset = 10001;

            properties.AddRange(CustomReflectionUtilities
                .GetAllInstancePropertiesInDeclarationOrder(type)
                .Where(IsSerialized)
                .Select((it, ind) => CustomPropertyDefinition.CreateForPropertyInfo(ind + propertiesOffset, it)));
        }

        private static bool IsSerialized(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<ShowInInspectorAttribute>(false) != null;
        }
    }
}