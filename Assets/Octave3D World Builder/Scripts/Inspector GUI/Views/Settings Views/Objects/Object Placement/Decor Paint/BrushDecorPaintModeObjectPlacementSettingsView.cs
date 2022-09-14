#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class BrushDecorPaintModeObjectPlacementSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private BrushDecorPaintModeObjectPlacementSettings _settings;
        #endregion

        #region Constructors
        public BrushDecorPaintModeObjectPlacementSettingsView(BrushDecorPaintModeObjectPlacementSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderScrollWheelCircleRadiusAdjustmentSpeedField();
        }
        #endregion

        #region Private Methods
        private void RenderScrollWheelCircleRadiusAdjustmentSpeedField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForScrollWheelCircleRadiusAdjustmentSpeedField(), _settings.ScrollWheelCircleRadiusAdjustmentSpeed);
            if (newFloat != _settings.ScrollWheelCircleRadiusAdjustmentSpeed)
            {
                UndoEx.RecordForToolAction(ObjectEraser.Get());
                _settings.ScrollWheelCircleRadiusAdjustmentSpeed = newFloat;
            }
        }

        private GUIContent GetContentForScrollWheelCircleRadiusAdjustmentSpeedField()
        {
            var content = new GUIContent();
            content.text = "Radius adjustment speed (scroll wheel)";
            content.tooltip = "Allows you to specify how fast the radius of the brush circle can be adjusted using " +
                              "the mouse scroll wheel.";

            return content;
        }
        #endregion
    }
}
#endif