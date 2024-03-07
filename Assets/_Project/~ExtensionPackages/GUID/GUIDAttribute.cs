using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class GUIDAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(GUIDAttribute))]
public class GuidAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        if (string.IsNullOrEmpty(property.stringValue))
        {
            property.stringValue = System.Guid.NewGuid().ToString();
        }

        var w = position.width * 0.3f;

        position.width = position.width * 0.7f;
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, GUIContent.none);
        GUI.enabled = true;

        position.position += new Vector2(position.width, 0);
        position.width = w;
        if (GUI.Button(position, new GUIContent("Change")))
        {
            if (!property.serializedObject.isEditingMultipleObjects)
                property.stringValue = System.Guid.NewGuid().ToString();
        }

        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }
}
#endif