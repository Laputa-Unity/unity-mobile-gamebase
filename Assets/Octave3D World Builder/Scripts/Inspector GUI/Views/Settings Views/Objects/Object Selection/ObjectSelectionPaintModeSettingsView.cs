#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionPaintModeSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSelectionPaintModeSettings _settings;
        #endregion

        #region Constructors
        public ObjectSelectionPaintModeSettingsView(ObjectSelectionPaintModeSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderSelectionShapeWidthField();
            RenderSelectionShapeHeightField();
            RenderScrollWheelShapeSizeAdjustmentSpeedField();
        }
        #endregion

        #region Private Methods
        private void RenderSelectionShapeWidthField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForSelectionShapeWidthField(), _settings.SelectionShapeWidthInPixels);
            if (newInt != _settings.SelectionShapeWidthInPixels)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SelectionShapeWidthInPixels = newInt;
            }
        }

        private GUIContent GetContentForSelectionShapeWidthField()
        {
            var content = new GUIContent();
            content.text = "Shape width (pixels)";
            content.tooltip = "Allows you to modify the width of the selection shape when operating in \'Paint\' mode.";

            return content;
        }

        private void RenderSelectionShapeHeightField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForSelectionShapeHeightField(), _settings.SelectionShapeHeightInPixels);
            if (newInt != _settings.SelectionShapeHeightInPixels)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SelectionShapeHeightInPixels = newInt;
            }
        }

        private GUIContent GetContentForSelectionShapeHeightField()
        {
            var content = new GUIContent();
            content.text = "Shape height (pixels)";
            content.tooltip = "Allows you to modify the height of the selection shape when operating in \'Paint\' mode.";

            return content;
        }

        private void RenderScrollWheelShapeSizeAdjustmentSpeedField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForScrollWheelShapeSizeAdjustmentSpeedField(), _settings.ScrollWheelShapeSizeAdjustmentSpeed);
            if (newInt != _settings.ScrollWheelShapeSizeAdjustmentSpeed)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ScrollWheelShapeSizeAdjustmentSpeed = newInt;
            }
        }

        private GUIContent GetContentForScrollWheelShapeSizeAdjustmentSpeedField()
        {
            var content = new GUIContent();
            content.text = "Size adjustment speed (scroll wheel)";
            content.tooltip = "Allows you to specify how fast the width and height of the selection shape can be adjusted using " +
                              "the mouse scroll wheel. This is only available in \'Paint\' mode.";

            return content;
        }
        #endregion
    }
}
#endif