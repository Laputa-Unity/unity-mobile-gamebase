#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockHeightAdjustmentSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementBlockHeightAdjustmentSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementBlockHeightAdjustmentSettingsView(ObjectPlacementBlockHeightAdjustmentSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderHeightAdjustmentModeSelectionPopup();
            if (_settings.HeightAdjustmentMode == ObjectPlacementBlockHeightAdjustmentMode.Manual) _settings.ManualHeightAdjustmentSettings.View.Render();
            else _settings.AutomaticRandomHeightAdjustmentSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderHeightAdjustmentModeSelectionPopup()
        {
            ObjectPlacementBlockHeightAdjustmentMode newHeightAdjustmentMode = (ObjectPlacementBlockHeightAdjustmentMode)EditorGUILayout.EnumPopup(GetContentForHeightAdjustmentModeSelectionPopup(), _settings.HeightAdjustmentMode);
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
            content.tooltip = "Allows you to specify the way in which the height of the block is adjusted.";

            return content;
        }
        #endregion
    }
}
#endif