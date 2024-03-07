using System;
using System.Diagnostics;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    [Conditional("UNITY_EDITOR")]
    public class EnableInPlayModeAttribute : DisableInPlayModeAttribute
    {
        public EnableInPlayModeAttribute()
        {
            Inverse = true;
        }
    }
}