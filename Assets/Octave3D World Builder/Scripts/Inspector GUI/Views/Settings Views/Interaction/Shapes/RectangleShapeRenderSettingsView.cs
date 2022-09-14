#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class RectangleShapeRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private RectangleShapeRenderSettings _settings;
        #endregion

        #region Constructors
        public RectangleShapeRenderSettingsView(RectangleShapeRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderFillColorField();
            RenderBorderLineColorField();
        }
        #endregion

        #region Private Methods
        private void RenderFillColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForFillColorField(), _settings.FillColor);
            if (newColor != _settings.FillColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.FillColor = newColor;
            }
        }

        private GUIContent GetContentForFillColorField()
        {
            var content = new GUIContent();
            content.text = "Fill color";
            content.tooltip = "Allows you to specify the color that is used to draw the interior of the rectangle shape.";

            return content;
        }

        private void RenderBorderLineColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForBorderLineColorField(), _settings.BorderLineColor);
            if (newColor != _settings.BorderLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.BorderLineColor = newColor;
            }
        }

        private GUIContent GetContentForBorderLineColorField()
        {
            var content = new GUIContent();
            content.text = "Border line color";
            content.tooltip = "Allows you to specify the color that is used to draw the rectangle border lines.";

            return content;
        }
        #endregion
    }
}
#endif