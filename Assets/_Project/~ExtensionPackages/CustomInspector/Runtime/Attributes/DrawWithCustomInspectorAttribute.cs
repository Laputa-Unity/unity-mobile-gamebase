using System;
using System.Diagnostics;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    [Conditional("UNITY_EDITOR")]
    public class DrawWithCustomInspectorAttribute : Attribute
    {
    }
}