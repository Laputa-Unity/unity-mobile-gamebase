using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomInspector.Utilities;
using CustomInspector;
using CustomInspector.TypeProcessors;

[assembly: RegisterCustomTypeProcessor(typeof(CustomRegisterUnitySerializedFieldsTypeProcessor), 0)]

namespace CustomInspector.TypeProcessors
{
    public class CustomRegisterUnitySerializedFieldsTypeProcessor : CustomTypeProcessor
    {
        public override void ProcessType(Type type, List<CustomPropertyDefinition> properties)
        {
            const int fieldsOffset = 1;

            properties.AddRange(CustomReflectionUtilities
                .GetAllInstanceFieldsInDeclarationOrder(type)
                .Where(IsSerialized)
                .Select((it, ind) => CustomPropertyDefinition.CreateForFieldInfo(ind + fieldsOffset, it)));
        }

        private static bool IsSerialized(FieldInfo fieldInfo)
        {
            return CustomUnitySerializationUtilities.IsSerializableByUnity(fieldInfo);
        }
    }
}