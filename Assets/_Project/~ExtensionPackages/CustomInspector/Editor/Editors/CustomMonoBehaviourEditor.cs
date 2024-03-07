using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editors
{
    [CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(MonoBehaviour), editorForChildClasses: true, isFallback = true)]
    internal sealed class CustomMonoBehaviourEditor : CustomEditor
    {
    }
}