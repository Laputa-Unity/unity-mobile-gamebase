#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectUniformScaleRandomizationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectUniformScaleRandomizationSettings _settings;
        #endregion

        #region Constructors
        public ObjectUniformScaleRandomizationSettingsView(ObjectUniformScaleRandomizationSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderMinMaxScaleSlider();
        }
        #endregion

        #region Private Methods
        private void RenderMinMaxScaleSlider()
        {
            float minScale = _settings.MinScale;
            float maxScale = _settings.MaxScale;

            EditorGUILayoutEx.MinMaxSliderWithFloatFields(GetContentForMinMaxScaleSlider(), ref minScale, ref maxScale, ObjectUniformScaleRandomizationSettings.MinScaleValue, ObjectUniformScaleRandomizationSettings.MaxScaleValue);

            if (minScale != _settings.MinScale)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MinScale = minScale;
            }
            if (maxScale != _settings.MaxScale)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MaxScale = maxScale;
            }
        }

        private GUIContent GetContentForMinMaxScaleSlider()
        {
            var content = new GUIContent();
            content.text = "Min/Max scale";
            content.tooltip = "Allows you to control the minimum and maximum scale values. The generated value will always reside inside the defined [min, max] range.";

            return content;
        }
        #endregion
    }
}
#endif