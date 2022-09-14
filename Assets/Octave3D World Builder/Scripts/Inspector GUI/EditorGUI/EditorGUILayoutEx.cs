#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public static class EditorGUILayoutEx
    {
        #region Public Static Properties
        public static float ToggleButtonWidth { get { return 12.0f; } }
        public static float PreferedActionButtonWidth { get { return 220.0f; } }
        public static float PreferedEditorWindowLabelWidth { get { return 220.0f; } }
        public static float PreferedMinMaxSliderLabelWidth { get { return 150.0f; } }
        public static float PreferedMinMaxSliderFloatFieldWidth { get { return 100.0f; } }
        public static float DefaultPrefabPreviewSize { get { return 128.0f; } }
        #endregion

        #region Public Static Functions
        public static string DelayedTextField(GUIContent content, string text)
        {
            #if !UNITY_5_3_3 && !UNITY_5_3_OR_NEWER
            return EditorGUILayout.TextField(content, text);
            #else
            return EditorGUILayout.DelayedTextField(content, text);
            #endif
        }

        public static Vector2 CalculateTextSize(string text)
        {
            return GUI.skin.label.CalcSize(new GUIContent(text));
        }

        public static bool PrefabPreview(Prefab prefab, float previewScale, bool useBoxStyle)
        {
            var previewButtonRenderData = new PrefabPreviewButtonRenderData();
            previewButtonRenderData.ExtractFromPrefab(prefab, previewScale);

            string previewStyleName = useBoxStyle ? "Box" : "";
            var previewStyle = new GUIStyle(previewStyleName);

            return GUILayout.Button(previewButtonRenderData.ButtonContent, previewStyle, GUILayout.Width(previewButtonRenderData.ButtonWidth), GUILayout.Height(previewButtonRenderData.ButtonHeight));
        }

        public static bool PrefabPreview(Prefab prefab, bool useBoxStyle, PrefabPreviewButtonRenderData previewButtonRenderData)
        {
            string previewStyleName = useBoxStyle ? "Box" : "";
            var previewStyle = new GUIStyle(previewStyleName);

            return GUILayout.Button(previewButtonRenderData.ButtonContent, previewStyle, GUILayout.Width(previewButtonRenderData.ButtonWidth), GUILayout.Height(previewButtonRenderData.ButtonHeight));
        }

        public static void BeginVerticalBox(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical("Box", options);
        }

        public static void BeginHorizontalBox(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal("Box");
        }

        public static void EndHorizontalBox()
        {
            EditorGUILayout.EndHorizontal();
        }

        public static void BeginVerticalBox()
        {
            EditorGUILayout.BeginVertical("Box");
        }

        public static void EndVerticalBox()
        {
            EditorGUILayout.EndVertical();
        }

        public static void BeginVertical(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
        }

        public static void EndVertical()
        {
            EditorGUILayout.EndVertical();
        }

        public static void BeginHorizontal(params GUILayoutOption[] options)
        {
            EditorGUILayout.EndHorizontal();
        }

        public static string Popup(GUIContent content, string selectedChoice, List<string> selectionChoices)
        {
            var selectionChoicesContent = new GUIContent[selectionChoices.Count];
            for (int choiceIndex = 0; choiceIndex < selectionChoices.Count; ++choiceIndex)
            {
                selectionChoicesContent[choiceIndex] = new GUIContent();
                selectionChoicesContent[choiceIndex].text = selectionChoices[choiceIndex];
            }

            int indexOfSelectedChoice = selectionChoices.FindIndex(item => item == selectedChoice);
            return selectionChoices[EditorGUILayout.Popup(content, indexOfSelectedChoice, selectionChoicesContent)];
        }

        public static int Popup(GUIContent content, int indexOfSelectedChoice, List<string> selectionChoices)
        {
            var selectionChoicesContent = new GUIContent[selectionChoices.Count];
            for (int choiceIndex = 0; choiceIndex < selectionChoices.Count; ++choiceIndex)
            {
                selectionChoicesContent[choiceIndex] = new GUIContent();
                selectionChoicesContent[choiceIndex].text = selectionChoices[choiceIndex];
            }

            return EditorGUILayout.Popup(content, indexOfSelectedChoice, selectionChoicesContent);
        }

        public static string SortedPopup(GUIContent content, int indexOfSelectedChoice, List<string> selectionChoices)
        {
            var choices = new List<string>(selectionChoices);
            string selectedChoice = choices[indexOfSelectedChoice];
            choices.Sort(delegate(string s0, string s1)
            {
                return s0.CompareTo(s1);
            });
            indexOfSelectedChoice = choices.FindIndex(item => item == selectedChoice);

            var selectionChoicesContent = new GUIContent[choices.Count];
            for (int choiceIndex = 0; choiceIndex < choices.Count; ++choiceIndex)
            {
                selectionChoicesContent[choiceIndex] = new GUIContent();
                selectionChoicesContent[choiceIndex].text = choices[choiceIndex];
            }

            int newSelectedIndex = EditorGUILayout.Popup(content, indexOfSelectedChoice, selectionChoicesContent);
            return choices[newSelectedIndex];
        }

        public static string TextArea(string text, bool requireWordWrap, params GUILayoutOption[] options)
        {
            if (!requireWordWrap) return EditorGUILayout.TextArea(text);
            else
            {
                bool oldState = EditorStyles.textField.wordWrap;
                EditorStyles.textField.wordWrap = true;
                string newText = EditorGUILayout.TextArea(text, options);
                EditorStyles.textField.wordWrap = oldState;

                return newText;
            }
        }

        public static void MinMaxSliderWithFloatFields(GUIContent content, ref float minValue, ref float maxValue, float minLimit, float maxLimit, params GUILayoutOption[] options)
        {
            // Can't seem to be able to format those controls to sit nicely right next to each other no matter what I try. 
            // Especially if the indent level is different than 0...
            float space = 55.0f * EditorGUI.indentLevel / 5.0f;
            EditorGUILayout.BeginHorizontal();
            minValue = EditorGUILayout.FloatField(content, minValue, GUILayout.ExpandWidth(false));
            GUILayout.Space(-space);
            EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, minLimit, maxLimit);
            GUILayout.Space(-space);
            maxValue = EditorGUILayout.FloatField(maxValue, GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();
        }

        public static void LabelInMiddleOfControlRect(Rect controlRectangle, string labelText, float controlHeight, GUIStyle style)
        {
            Rect labelRectangle = controlRectangle;
            labelRectangle.yMin += controlHeight * 0.05f;
            labelRectangle.xMin = controlRectangle.center.x - EditorGUILayoutEx.CalculateTextSize(labelText).x * 0.5f;

            GUI.Label(labelRectangle, labelText, style);
        }
        #endregion
    }
}
#endif