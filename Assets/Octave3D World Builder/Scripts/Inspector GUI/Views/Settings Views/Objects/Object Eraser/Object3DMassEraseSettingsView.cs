#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class Object3DMassEraseSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private Object3DMassEraseSettings _settings;
        #endregion

        #region Constructors
        public Object3DMassEraseSettingsView(Object3DMassEraseSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderCircleShapeRadiusField();
            RenderScrollWheelCircleRadiusAdjustmentSpeedField();
            RenderAllowPartialOverlapToggle();
        }
        #endregion

        #region Private Methods
        private void RenderCircleShapeRadiusField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForCircleShapeRadiusField(), _settings.CircleShapeRadius);
            if(newFloat != _settings.CircleShapeRadius)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CircleShapeRadius = newFloat;
            }
        }

        private GUIContent GetContentForCircleShapeRadiusField()
        {
            var content = new GUIContent();
            content.text = "Circle radius";
            content.tooltip = "Allows you to control the radius of the mass erase circle.";

            return content;
        }

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
            content.tooltip = "Allows you to specify how fast the radius of the mass erase circle can be adjusted using " +
                              "the mouse scroll wheel.";

            return content;
        }

        private void RenderAllowPartialOverlapToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowPartialOverlapToggle(), _settings.AllowPartialOverlap);
            if (newBool != _settings.AllowPartialOverlap)
            {
                UndoEx.RecordForToolAction(ObjectEraser.Get());
                _settings.AllowPartialOverlap = newBool;
            }
        }

        private GUIContent GetContentForAllowPartialOverlapToggle()
        {
            var content = new GUIContent();
            content.text = "Allow partial overlap";
            content.tooltip = "When this is NOT checked, objects will be erased ONLY if they are fully contained by the erase circle.";

            return content;
        }
        #endregion
    }
}
#endif