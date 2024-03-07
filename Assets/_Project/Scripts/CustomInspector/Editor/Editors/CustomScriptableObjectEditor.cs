using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editors
{
    [CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(ScriptableObject), editorForChildClasses: true, isFallback = true)]
    internal sealed class CustomScriptableObjectEditor : CustomEditor
    {
    }
}