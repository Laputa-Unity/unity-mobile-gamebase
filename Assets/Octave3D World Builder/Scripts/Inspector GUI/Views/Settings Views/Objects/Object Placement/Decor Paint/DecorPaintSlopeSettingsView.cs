#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintSlopeSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private DecorPaintSlopeSettings _settings;
        #endregion

        #region Constructors
        public DecorPaintSlopeSettingsView(DecorPaintSlopeSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderMinMaxSlopeSlider();
            RenderUseSlopeOnlyForTerrainObjectsField();
        }
        #endregion

        #region Private Methods
        private void RenderMinMaxSlopeSlider()
        {
            float minSlope = _settings.MinSlopeInDegrees;
            float maxSlope = _settings.MaxSlopeInDegrees;

            EditorGUILayoutEx.MinMaxSliderWithFloatFields(GetContentForMinMaxSlopeSlider(), ref minSlope, ref maxSlope, DecorPaintSlopeSettings.MinSlopeValueInDegrees, DecorPaintSlopeSettings.MaxSlopeValueInDegrees);

            if (minSlope != _settings.MinSlopeInDegrees)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MinSlopeInDegrees = minSlope;
            }
            if (maxSlope != _settings.MaxSlopeInDegrees)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MaxSlopeInDegrees = maxSlope;
            }
        }

        private GUIContent GetContentForMinMaxSlopeSlider()
        {
            var content = new GUIContent();
            content.text = "Min/Max slope";
            content.tooltip = "Allows you to control the minimum and maximum slope values. Objects will only be placed on a paint surface whose normal resides inside the defined [min, max] range.";

            return content;
        }

        private void RenderUseSlopeOnlyForTerrainObjectsField()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseSlopeOnlyForTerrainObjectsField(), _settings.UseSlopeOnlyForTerrainObjects);
            if(newBool != _settings.UseSlopeOnlyForTerrainObjects)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseSlopeOnlyForTerrainObjects = newBool;
            }
        }

        private GUIContent GetContentForUseSlopeOnlyForTerrainObjectsField()
        {
            var content = new GUIContent();
            content.text = "Use slope only for terrain objects";
            content.tooltip = "If this is checked, the slope values are only taken into consideration when placing objects on a terrain.";

            return content;
        }
        #endregion
    }
}
#endif