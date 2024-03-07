using System;
using System.Diagnostics;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class DropdownAttribute : Attribute
    {
        public string Values { get; }

        public CustomMessageType ValidationMessageType { get; set; } = CustomMessageType.Error;

        public DropdownAttribute(string values)
        {
            Values = values;
        }
    }
}