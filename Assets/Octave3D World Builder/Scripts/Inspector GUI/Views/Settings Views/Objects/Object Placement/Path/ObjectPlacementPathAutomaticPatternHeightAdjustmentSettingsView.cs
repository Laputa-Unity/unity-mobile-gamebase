#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsView(ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderApplyPatternsContinuouslyToggle();
            RenderWrapPatternToggle();
        }
        #endregion

        #region Private Methods
        private void RenderApplyPatternsContinuouslyToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForApplyPatternsContinuouslyToggle(), _settings.ApplyPatternsContinuously);
            if(newBool != _settings.ApplyPatternsContinuously)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ApplyPatternsContinuously = newBool;
            }
        }

        private GUIContent GetContentForApplyPatternsContinuouslyToggle()
        {
            var content = new GUIContent();
            content.text = "Apply patterns continuously";
            content.tooltip = "If this is checked, the height patterns will be continuous across segment boundaries. Otherwise, the pattern will be reset when a new segment starts.";

            return content;
        }

        private void RenderWrapPatternToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForWrapPatternToggle(), _settings.WrapPatterns);
            if(newBool != _settings.WrapPatterns)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.WrapPatterns = newBool;
            }
        }

        private GUIContent GetContentForWrapPatternToggle()
        {
            var content = new GUIContent();
            content.text = "Wrap patterns";
            content.tooltip = "If this is checked, the patterns will be wrapped when no more values are available. This is useful when the path is larger than " +
                              "the number of values which exist inside a pattern (probably 99% of the time).";

            return content;
        }
        #endregion
    }
}
#endif