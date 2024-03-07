using System;
using System.Diagnostics;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class ShowInInspectorAttribute : Attribute
    {
    }
}