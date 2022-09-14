#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisMouseRotationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private AxisMouseRotationSettings _settings;
        #endregion

        #region Constructors
        public AxisMouseRotationSettingsView(AxisMouseRotationSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderMouseSensitivitySlider();
        }
        #endregion

        #region Private Methods
        private void RenderMouseSensitivitySlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForMouseSensitivitySlider(), _settings.MouseSensitivity, AxisMouseRotationSettings.MinMouseSensitivity, AxisMouseRotationSettings.MaxMouseSensitivity);
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
            content.tooltip = "Allows you to control how sensitive the rotation is to mouse movements.";

            return content;
        }
        #endregion
    }
}
#endif