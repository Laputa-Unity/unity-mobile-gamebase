#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class RandomRotationValueAxisRandomizationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private RandomRotationValueAxisRandomizationSettings _settings;
        #endregion

        #region Constructors
        public RandomRotationValueAxisRandomizationSettingsView(RandomRotationValueAxisRandomizationSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderMinMaxRotationSlider();
        }
        #endregion

        #region Private Methods
        private void RenderMinMaxRotationSlider()
        {
            float minRotation = _settings.MinRotationInDegrees;
            float maxRotation = _settings.MaxRotationInDegrees;

            EditorGUILayoutEx.MinMaxSliderWithFloatFields(GetContentForMinMaxRotationSlider(), ref minRotation, ref maxRotation, RandomRotationValueAxisRandomizationSettings.MinRotationValueInDegrees, RandomRotationValueAxisRandomizationSettings.MaxRotationValueInDegrees);

            if (minRotation != _settings.MinRotationInDegrees)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MinRotationInDegrees = minRotation;
            }
            if (maxRotation != _settings.MaxRotationInDegrees)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MaxRotationInDegrees = maxRotation;
            }
        }

        private GUIContent GetContentForMinMaxRotationSlider()
        {
            var content = new GUIContent();
            content.text = "Min/Max rotation";
            content.tooltip = "Allows you to control the minimum and maximum rotation values. The generated value will always reside inside the defined [min, max] range.";

            return content;
        }
        #endregion
    }
}
#endif