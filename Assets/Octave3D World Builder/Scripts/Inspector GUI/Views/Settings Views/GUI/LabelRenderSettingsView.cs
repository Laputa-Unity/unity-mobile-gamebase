#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    public class LabelRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private LabelRenderSettings _settings;
        #endregion

        #region Constructors
        public LabelRenderSettingsView(LabelRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderTextColorField();
            RenderFontSizeField();
            RenderBoldToggle();
        }
        #endregion

        #region Private Methods
        private void RenderTextColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForTextColorField(), _settings.TextColor);
            if(newColor != _settings.TextColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.TextColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForTextColorField()
        {
            var content = new GUIContent();
            content.text = "Text color";
            content.tooltip = "Allows you to change the text color.";

            return content;
        }

        private void RenderFontSizeField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForFontSizeField(), _settings.FontSize);
            if(newInt != _settings.FontSize)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.FontSize = newInt;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForFontSizeField()
        {
            var content = new GUIContent();
            content.text = "Font size";
            content.tooltip = "Allows you to change the font size.";

            return content;
        }

        private void RenderBoldToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForBoldToggle(), _settings.Bold);
            if(newBool != _settings.Bold)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.Bold = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForBoldToggle()
        {
            var content = new GUIContent();
            content.text = "Bold";
            content.tooltip = "Allows you to specify if a bold font must be used.";

            return content;
        }
        #endregion
    }
}
#endif