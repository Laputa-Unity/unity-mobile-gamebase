#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectMouseMoveAlongDirectionSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectMouseMoveAlongDirectionSettings _settings;
        #endregion

        #region Constructors
        public ObjectMouseMoveAlongDirectionSettingsView(ObjectMouseMoveAlongDirectionSettings settings)
        {
            _settings = settings;
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
            float newFloat = EditorGUILayout.Slider(GetContentForMouseSensitivitySlider(), _settings.MouseSensitivity, ObjectMouseMoveAlongDirectionSettings.MinMouseSensitivity, ObjectMouseMoveAlongDirectionSettings.MaxMouseSensitivity);
            if (newFloat != _settings.MouseSensitivity)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MouseSensitivity = newFloat;
            }
        }

        private GUIContent GetContentForMouseSensitivitySlider()
        {
            var content = new GUIContent();
            content.text = "Offset mouse sensitivity";
            content.tooltip = "The sensitivity of the mouse which applies when moving the entity with the mouse.";

            return content;
        }
        #endregion
    }
}
#endif