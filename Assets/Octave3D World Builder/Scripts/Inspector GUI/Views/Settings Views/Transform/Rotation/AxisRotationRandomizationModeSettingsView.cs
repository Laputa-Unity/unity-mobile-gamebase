#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisRotationRandomizationModeSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private AxisRotationRandomizationModeSettings _settings;
        #endregion

        #region Constructors
        public AxisRotationRandomizationModeSettingsView(AxisRotationRandomizationModeSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderRotationRandomizationModeSelectionPopup();

            if (_settings.RandomizationMode == AxisRotationRandomizationMode.RandomRotationStep) _settings.RandomRotationStepAxisRandomizationSettings.View.Render();
            else if (_settings.RandomizationMode == AxisRotationRandomizationMode.RandomRotationValue) _settings.RandomRotationValueAxisRandomizationSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderRotationRandomizationModeSelectionPopup()
        {
            AxisRotationRandomizationMode newRandomizationMode = (AxisRotationRandomizationMode)EditorGUILayout.EnumPopup(GetContentForRotationRandomizationModeSelectionPopup(), _settings.RandomizationMode);
            if (newRandomizationMode != _settings.RandomizationMode)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RandomizationMode = newRandomizationMode;
            }
        }

        private GUIContent GetContentForRotationRandomizationModeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Randomization mode";
            content.tooltip = "Allows you to control the way in which the random rotation is calculated.";

            return content;
        }
        #endregion
    }
}
#endif