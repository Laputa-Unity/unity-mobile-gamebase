using System;
using System.Collections.Generic;

namespace CustomInspector.Utilities
{
    internal static class CustomAttributeUtilities
    {
        public static bool TryGet<T>(this IReadOnlyList<Attribute> attributes, out T it)
            where T : Attribute
        {
            foreach (var attribute in attributes)
            {
                if (attribute is T typeAttribute)
                {
                    it = typeAttribute;
                    return true;
                }
            }

            it = null;
            return false;
        }
    }
}