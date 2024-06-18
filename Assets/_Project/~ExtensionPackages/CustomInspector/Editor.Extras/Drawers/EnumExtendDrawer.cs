using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomInspector;
using CustomInspector.Drawers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterCustomAttributeDrawer(typeof(EnumExtendDrawer), CustomDrawerOrder.Drawer, ApplyOnArrayElement = true)]

namespace CustomInspector.Drawers
{
    public class EnumExtendDrawer : CustomAttributeDrawer<EnumExtendAttribute>
    {
        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            if (!propertyDefinition.FieldType.IsEnum)
            {
                return "EnumExtend attribute can be used only on enums";
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override CustomElement CreateElement(CustomProperty property, CustomElement next)
        {
            return new EnumExtendElement(property);
        }

        private sealed class EnumExtendElement : CustomElement
        {
            private readonly CustomProperty _property;
            private readonly List<KeyValuePair<string, Enum>> _enumValues;

            public EnumExtendElement(CustomProperty property)
            {
                _property = property;
                _enumValues = Enum.GetNames(property.FieldType)
                    .Zip(Enum.GetValues(property.FieldType).OfType<Enum>(),
                        (name, value) => new KeyValuePair<string, Enum>(name, value))
                    .ToList();
            }

            public override float GetHeight(float width)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            public override void OnGUI(Rect position)
            {
                var value = _property.TryGetSerializedProperty(out var serializedProperty)
                    ? (Enum)Enum.ToObject(_property.FieldType, serializedProperty.longValue)
                    : (Enum)_property.Value;

                var currentValue = value.ToString();
                var options = _enumValues.Select(ev => ev.Key).ToList();
                options.Add("...");

                var selectedIndex = options.IndexOf(currentValue);
                if (selectedIndex == -1) selectedIndex = 0;

                var newIndex = EditorGUI.Popup(position, _property.DisplayNameContent.text, selectedIndex, options.ToArray());

                if (newIndex != selectedIndex)
                {
                    if (newIndex == options.Count - 1)
                    {
                        ShowAddEnumPopup();
                    }
                    else
                    {
                        var newValue = _enumValues[newIndex].Value;
                        _property.SetValue(newValue);
                    }
                }
            }

            private void ShowAddEnumPopup()
            {
                var popup = new AddEnumPopup(_property.FieldType);
                popup.OnEnumAdded += AddEnumValue;
                PopupWindow.Show(new Rect(0, 0, 300, 120), popup);
            }

            private void AddEnumValue(string enumName)
            {
                FindClassFile(_property.FieldType.Name, enumName);
            }
        }

        private class AddEnumPopup : PopupWindowContent
        {
            private string _newEnumName = "";
            private readonly Type _enumType;
            public event Action<string> OnEnumAdded;

            public AddEnumPopup(Type enumType)
            {
                _enumType = enumType;
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(220, 60);
            }

            public override void OnGUI(Rect rect)
            {
                EditorGUILayout.LabelField("Add New Enum Value", EditorStyles.boldLabel);
                EditorGUIUtility.labelWidth = 60;
                
                _newEnumName = EditorGUILayout.TextField("Name", _newEnumName,  new GUILayoutOption[]
                {
                    GUILayout.ExpandWidth(true),
                    GUILayout.Height(15)
                });

                if (GUILayout.Button("Add"))
                {
                    if (!string.IsNullOrEmpty(_newEnumName) && Enum.GetNames(_enumType).All(e => e != _newEnumName))
                    {
                        OnEnumAdded?.Invoke(_newEnumName);
                        editorWindow.Close();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Invalid or duplicate enum name.", MessageType.Error);
                    }
                }
            }
        }

        private static void FindClassFile(string enumName, string newEnum)
        {
            var codeFile = FindAllScriptFiles(Application.dataPath, "enum " + enumName);
            if (codeFile.Key != "NOPE")
                AddNewEnum(codeFile.Key, codeFile.Value, enumName, newEnum);
            else
                Debug.LogError("Could not find enum class");
        }

        static void AddNewEnum(string classFile, string path, string enumName, string newEnum)
        {
            string[] originalSplit = classFile.Split(new[] { "enum " + enumName }, StringSplitOptions.RemoveEmptyEntries);
            string newHalf = originalSplit[1];
            string enumSection = newHalf.Split('}')[0];
            string[] commas = enumSection.Split(',');
            if (commas.Length == 0 && enumSection.Split('{')[0].Trim().Length == 0) // They've left the enum empty
            {
                newHalf = newHalf.Replace(enumSection, enumSection + newEnum);
            }
            else
            {
                bool commaAfter = commas[commas.Length - 1].Trim().Length == 0; // Check if there is a trailing comma
                if (commaAfter)
                {
                    newHalf = newHalf.Replace(enumSection, enumSection + newEnum + ", ");
                }
                else
                {
                    while (enumSection.Length > 0 && enumSection[enumSection.Length - 1] == ' ')
                        enumSection = enumSection.Substring(0, enumSection.Length - 1);
                    newHalf = newHalf.Replace(enumSection, enumSection + ", " + newEnum);
                }
            }

            string result = classFile.Replace(originalSplit[1], newHalf);
            using (var file = File.Open(path, FileMode.Create))
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(result);
                }
            }

            AssetDatabase.Refresh();
        }

        private static KeyValuePair<string, string> FindAllScriptFiles(string startDir, string enumToFind)
        {
            try
            {
                foreach (string file in Directory.GetFiles(startDir))
                {
                    if ((file.EndsWith(".cs") || file.EndsWith(".js")) && !file.Contains(".meta"))
                    {
                        var current = File.ReadAllText(file);
                        var currentTrimmed = current.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                        if (currentTrimmed.Contains(enumToFind.Replace(" ", "") + "{"))
                            return new KeyValuePair<string, string>(current, file);
                    }
                }

                foreach (var dir in Directory.GetDirectories(startDir))
                {
                    var result = FindAllScriptFiles(dir, enumToFind);
                    if (result.Key != "NOPE")
                        return result;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            return new KeyValuePair<string, string>("NOPE", "NOPE");
        }
    }
}
