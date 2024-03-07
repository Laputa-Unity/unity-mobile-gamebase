using System;
using System.Diagnostics;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class OnValueChangedAttribute : Attribute
    {
        public OnValueChangedAttribute(string method)
        {
            Method = method;
        }

        public string Method { get; }
    }
}