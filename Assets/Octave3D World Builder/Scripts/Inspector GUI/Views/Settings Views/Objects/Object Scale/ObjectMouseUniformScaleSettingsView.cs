#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectMouseUniformScaleSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectMouseUniformScaleSettings _settings;
        #endregion

        #region Constructors
        public ObjectMouseUniformScaleSettingsView(ObjectMouseUniformScaleSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Uniform Mouse Scale Settings";
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderMouseSensitivitySlider();
            RenderUseSnappingToggle();
            RenderSnapStepField();
        }
        #endregion

        #region Private Methods
        private void RenderMouseSensitivitySlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForMouseSensitivitySlider(), _settings.MouseSensitivity, ObjectMouseUniformScaleSettings.MinMouseSensitivity, ObjectMouseUniformScaleSettings.MaxMouseSensitivity);
            if (newFloat != _settings.MouseSensitivity)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MouseSensitivity = newFloat;
            }
        }

        private GUIContent GetContentForMouseSensitivitySlider()
        {
            var content = new GUIContent();
            content.text = "Mouse sensitivity";
            content.tooltip = "Allows you to control how sensitive the scale is to mouse movements.";

            return content;
        }

        private void RenderSnapStepField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForSnapStepField(), _settings.SnapStep);
            if(newFloat != _settings.SnapStep)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapStep = newFloat;
            }
        }

        private GUIContent GetContentForSnapStepField()
        {
            var content = new GUIContent();
            content.text = "Snap step";
            content.tooltip = "The amount of scale to be applied during each step when using snapping.";

            return content;
        }

        private void RenderUseSnappingToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseSnappingToggle(), _settings.UseSnapping);
            if (newBool != _settings.UseSnapping)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseSnapping = newBool;
            }
        }

        private GUIContent GetContentForUseSnappingToggle()
        {
            var content = new GUIContent();
            content.text = "Use snapping";
            content.tooltip = "If this is checked, snapping will be used while scaling.";

            return content;
        }
        #endregion
    }
}
#endif