using System;
using System.Diagnostics;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    [Conditional("UNITY_EDITOR")]
    public class TabAttribute : Attribute
    {
        public TabAttribute(string tab)
        {
            TabName = tab;
        }

        public string TabName { get; }
    }
}