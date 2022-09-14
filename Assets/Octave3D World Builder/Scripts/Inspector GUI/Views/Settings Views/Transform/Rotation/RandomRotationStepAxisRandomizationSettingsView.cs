#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class RandomRotationStepAxisRandomizationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private RandomRotationStepAxisRandomizationSettings _settings;
        #endregion

        #region Constructors
        public RandomRotationStepAxisRandomizationSettingsView(RandomRotationStepAxisRandomizationSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderStepSizeInDegreesSlider();
        }
        #endregion

        #region Private Methods
        private void RenderStepSizeInDegreesSlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForStepSizeInDegreesSlider(), _settings.StepSizeInDegrees, RandomRotationStepAxisRandomizationSettings.MinStepSizeInDegrees, RandomRotationStepAxisRandomizationSettings.MaxStepSizeInDegrees);
            if(newFloat != _settings.StepSizeInDegrees)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.StepSizeInDegrees = newFloat;
            }
        }

        private GUIContent GetContentForStepSizeInDegreesSlider()
        {
            var content = new GUIContent();
            content.text = "Step size (degrees)";
            content.tooltip = "The random rotation will be generated using a random multiple of this step value.";

            return content;
        }
        #endregion
    }
}
#endif