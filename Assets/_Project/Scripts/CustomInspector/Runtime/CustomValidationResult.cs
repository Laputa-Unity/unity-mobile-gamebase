using System;
using UnityEngine;

namespace CustomInspector
{
    public readonly struct CustomValidationResult
    {
        public static CustomValidationResult Valid => new CustomValidationResult(true, null, CustomMessageType.None);

        public CustomValidationResult(bool valid, string message, CustomMessageType messageType,
            Action fixAction = null, GUIContent fixActionContent = null)
        {
            IsValid = valid;
            Message = message;
            MessageType = messageType;
            FixAction = fixAction;
            FixActionContent = fixActionContent;
        }

        public bool IsValid { get; }
        public string Message { get; }
        public CustomMessageType MessageType { get; }
        public Action FixAction { get; }
        public GUIContent FixActionContent { get; }

        public CustomValidationResult WithFix(Action action, string name = null)
        {
            return new CustomValidationResult(IsValid, Message, MessageType, action, new GUIContent(name ?? "Fix"));
        }

        public static CustomValidationResult Info(string error)
        {
            return new CustomValidationResult(false, error, CustomMessageType.Info);
        }

        public static CustomValidationResult Error(string error)
        {
            return new CustomValidationResult(false, error, CustomMessageType.Error);
        }

        public static CustomValidationResult Warning(string error)
        {
            return new CustomValidationResult(false, error, CustomMessageType.Warning);
        }
    }

    public enum CustomMessageType
    {
        None,
        Info,
        Warning,
        Error,
    }
}