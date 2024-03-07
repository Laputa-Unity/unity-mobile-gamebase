using System;
using System.Collections.Generic;

namespace CustomInspector
{
    public abstract class CustomTypeProcessor
    {
        public abstract void ProcessType(Type type, List<CustomPropertyDefinition> properties);
    }
}