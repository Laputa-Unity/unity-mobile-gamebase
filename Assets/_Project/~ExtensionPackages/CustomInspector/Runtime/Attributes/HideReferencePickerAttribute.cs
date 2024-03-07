using System;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HideReferencePickerAttribute : Attribute
    {
    }
}