using System;
using System.Collections.Generic;
using System.Reflection;
using CustomInspector;
using CustomInspector.TypeProcessors;
using UnityEngine;

[assembly: RegisterCustomTypeProcessor(typeof(CustomRectOffsetTypeProcessor), 1)]

namespace CustomInspector.TypeProcessors
{
    public class CustomRectOffsetTypeProcessor : CustomTypeProcessor
    {
        private static readonly string[] DrawnProperties = new[]
        {
            "left",
            "right",
            "top",
            "bottom",
        };

        public override void ProcessType(Type type, List<CustomPropertyDefinition> properties)
        {
            if (type != typeof(RectOffset))
            {
                return;
            }

            for (var i = 0; i < DrawnProperties.Length; i++)
            {
                var propertyName = DrawnProperties[i];
                var propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                var propertyDef = CustomPropertyDefinition.CreateForPropertyInfo(i, propertyInfo);

                properties.Add(propertyDef);
            }
        }
    }
}