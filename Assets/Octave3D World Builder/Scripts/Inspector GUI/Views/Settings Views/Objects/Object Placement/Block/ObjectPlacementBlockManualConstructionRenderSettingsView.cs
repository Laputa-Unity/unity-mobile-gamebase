#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockManualConstructionRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementBlockManualConstructionRenderSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementBlockManualConstructionRenderSettingsView(ObjectPlacementBlockManualConstructionRenderSettings settings)
        {
            _settings = settings;

            VisibilityToggleLabel = "Block Construction";
            ToggleVisibilityBeforeRender = true;
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderBoxBorderLineColorField();
            _settings.DimensionsLabelRenderSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderBoxBorderLineColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForBoxBorderLineColorField(), _settings.BoxBorderLineColor);
            if(newColor != _settings.BoxBorderLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.BoxBorderLineColor = newColor;
            }
        }

        private GUIContent GetContentForBoxBorderLineColorField()
        {
            var content = new GUIContent();
            content.text = "Box border line color";
            content.tooltip = "Allows you to modify the color of the box border lines which are rendered when a block is under construction.";

            return content;
        }
        #endregion
    }
}
#endif