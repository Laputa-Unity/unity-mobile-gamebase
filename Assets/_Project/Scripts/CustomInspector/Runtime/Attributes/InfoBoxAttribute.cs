using System;
using System.Diagnostics;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class InfoBoxAttribute : Attribute
    {
        public string Text { get; }
        public CustomMessageType MessageType { get; }
        public string VisibleIf { get; }

        public InfoBoxAttribute(string text, CustomMessageType messageType = CustomMessageType.Info, string visibleIf = null)
        {
            Text = text;
            MessageType = messageType;
            VisibleIf = visibleIf;
        }
    }
}