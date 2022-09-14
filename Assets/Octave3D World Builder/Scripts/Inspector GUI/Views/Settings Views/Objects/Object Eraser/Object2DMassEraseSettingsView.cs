#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class Object2DMassEraseSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private Object2DMassEraseSettings _settings;
        #endregion

        #region Constructors
        public Object2DMassEraseSettingsView(Object2DMassEraseSettings settings)
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
            int newInt = EditorGUILayout.IntField(GetContentForCircleShapeRadiusField(), _settings.CircleShapeRadius);
            if (newInt != _settings.CircleShapeRadius)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CircleShapeRadius = newInt;
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
            int newInt = EditorGUILayout.IntField(GetContentForScrollWheelCircleRadiusAdjustmentSpeedField(), _settings.ScrollWheelCircleRadiusAdjustmentSpeed);
            if (newInt != _settings.ScrollWheelCircleRadiusAdjustmentSpeed)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ScrollWheelCircleRadiusAdjustmentSpeed = newInt;
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
                UndoEx.RecordForToolAction(_settings);
                _settings.AllowPartialOverlap = newBool;
            }
        }

        private GUIContent GetContentForAllowPartialOverlapToggle()
        {
            var content = new GUIContent();
            content.text = "Allow partial overlap";
            content.tooltip = "When this is NOT checked, objects will be erased ONLY if their screen rectangle is totally contained by the mass erase circle.";

            return content;
        }
        #endregion
    }
}
#endif