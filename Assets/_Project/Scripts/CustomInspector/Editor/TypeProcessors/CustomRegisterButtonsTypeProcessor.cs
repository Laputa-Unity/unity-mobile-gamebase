using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomInspector.Utilities;
using CustomInspector;
using CustomInspector.TypeProcessors;

[assembly: RegisterCustomTypeProcessor(typeof(CustomRegisterButtonsTypeProcessor), 3)]

namespace CustomInspector.TypeProcessors
{
    public class CustomRegisterButtonsTypeProcessor : CustomTypeProcessor
    {
        public override void ProcessType(Type type, List<CustomPropertyDefinition> properties)
        {
            const int methodsOffset = 20001;

            properties.AddRange(CustomReflectionUtilities
                .GetAllInstanceMethodsInDeclarationOrder(type)
                .Where(IsSerialized)
                .Select((it, ind) => CustomPropertyDefinition.CreateForMethodInfo(ind + methodsOffset, it)));
        }

        private static bool IsSerialized(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<ButtonAttribute>(false) != null;
        }
    }
}