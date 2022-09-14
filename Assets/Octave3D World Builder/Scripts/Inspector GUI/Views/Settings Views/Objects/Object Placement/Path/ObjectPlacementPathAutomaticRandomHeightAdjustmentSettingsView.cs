#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsView(ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderMinHeightField();
            RenderMaxHeightField();
        }
        #endregion

        #region Private Methods
        private void RenderMinHeightField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForMinHeightField(), _settings.MinHeight);
            if(newInt != _settings.MinHeight)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MinHeight = newInt;
            }
        }

        private GUIContent GetContentForMinHeightField()
        {
            var content = new GUIContent();
            content.text = "Min height";
            content.tooltip = "The minimum random height value that can be generated. Height values will always be generated inside the defined [min, max] range.";

            return content;
        }

        private void RenderMaxHeightField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForMaxHeightField(), _settings.MaxHeight);
            if (newInt != _settings.MaxHeight)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MaxHeight = newInt;
            }
        }

        private GUIContent GetContentForMaxHeightField()
        {
            var content = new GUIContent();
            content.text = "Max height";
            content.tooltip = "The maximum random height value that can be generated. Height values will always be generated inside the defined [min, max] range.";

            return content;
        }
        #endregion
    }
}
#endif