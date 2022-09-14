#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private DecorPaintObjectPlacementSettings _settings;
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementSettingsView(DecorPaintObjectPlacementSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderDecorPaintModeSelectionPopup();
            RenderIgnoreGridToggle();
            RenderStrokeDistanceField();

            if (_settings.DecorPaintMode == DecorPaintMode.Single) _settings.SingleDecorPaintModeSettings.View.Render();
            else if (_settings.DecorPaintMode == DecorPaintMode.Brush) _settings.BrushDecorPaintModeSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderDecorPaintModeSelectionPopup()
        {
            DecorPaintMode newDecorPaintMode = (DecorPaintMode)EditorGUILayout.EnumPopup(GetContentForDecorPaintModeSelectionPopup(), _settings.DecorPaintMode);
            if(newDecorPaintMode != _settings.DecorPaintMode)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.DecorPaintMode = newDecorPaintMode;
            }
        }

        private GUIContent GetContentForDecorPaintModeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Decor paint mode";
            content.tooltip = "Allows you to choose the mode in which decorations can be painted.";

            return content;
        }

        private void RenderIgnoreGridToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIgnoreGridToggle(), _settings.IgnoreGrid);
            if(newBool != _settings.IgnoreGrid)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.IgnoreGrid = newBool;
            }

        }

        private GUIContent GetContentForIgnoreGridToggle()
        {
            var content = new GUIContent();
            content.text = "Ignore grid";
            content.tooltip = "When this is checked, you can not paint objects on the grid.";

            return content;
        }

        private void RenderStrokeDistanceField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForStrokeDistanceField(), _settings.StrokeDistance);
            if(newFloat != _settings.StrokeDistance)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.StrokeDistance = newFloat;
            }
        }

        private GUIContent GetContentForStrokeDistanceField()
        {
            var content = new GUIContent();
            content.text = "Stroke distance";
            content.tooltip = "This represents the minimum drag distance which must happen before " + 
                              "a new object can be placed. Note: The distance is expressed in world units.";

            return content;
        }
        #endregion
    }
}
#endif