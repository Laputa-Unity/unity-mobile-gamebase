#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightAdjustmentSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathHeightAdjustmentSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementPathHeightAdjustmentSettingsView(ObjectPlacementPathHeightAdjustmentSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderHeightAdjustmentModeSelectionPopup();
            if (_settings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.Manual) _settings.ManualHeightAdjustmentSettings.View.Render();
            else if (_settings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticRandom) _settings.AutomaticRandomHeightAdjustmentSettings.View.Render();
            else _settings.AutomaticPatternHeightAdjustmentSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderHeightAdjustmentModeSelectionPopup()
        {
            ObjectPlacementPathHeightAdjustmentMode newHeightAdjustmentMode = (ObjectPlacementPathHeightAdjustmentMode)EditorGUILayout.EnumPopup(GetContentForHeightAdjustmentModeSelectionPopup(), _settings.HeightAdjustmentMode);
            if(newHeightAdjustmentMode != _settings.HeightAdjustmentMode)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.HeightAdjustmentMode = newHeightAdjustmentMode;
            }
        }

        private GUIContent GetContentForHeightAdjustmentModeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Height adjustment mode";
            content.tooltip = "Allows you to specify the way in which the height of the path is adjusted.";

            return content;
        }
        #endregion
    }
}
#endif