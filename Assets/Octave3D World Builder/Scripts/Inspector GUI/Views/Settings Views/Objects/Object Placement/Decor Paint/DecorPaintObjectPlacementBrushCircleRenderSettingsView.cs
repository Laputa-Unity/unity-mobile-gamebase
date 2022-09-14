#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushCircleRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private DecorPaintObjectPlacementBrushCircleRenderSettings _settings;
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrushCircleRenderSettingsView(DecorPaintObjectPlacementBrushCircleRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderCircleLineColorField();
        }
        #endregion

        #region Private Methods
        private void RenderCircleLineColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForCircleLineColorField(), _settings.CircleLineColor);
            if(newColor != _settings.CircleLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CircleLineColor = newColor;
            }
        }

        private GUIContent GetContentForCircleLineColorField()
        {
            var content = new GUIContent();
            content.text = "Circle line color";
            content.tooltip = "Allows you to modify the color that is used to draw the circle lines.";

            return content;
        }
        #endregion
    }
}
#endif