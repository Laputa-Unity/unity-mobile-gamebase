#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectScaleRandomizationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectScaleRandomizationSettings _settings;
        #endregion

        #region Constructors
        public ObjectScaleRandomizationSettingsView(ObjectScaleRandomizationSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Scale Randomization Settings";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderRandomizeToggle();
            if (_settings.RandomizeScale) _settings.UniformScaleRandomizationSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderRandomizeToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForRandomizeToggle(), _settings.RandomizeScale);
            if(newBool != _settings.RandomizeScale)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RandomizeScale = newBool;
            }
        }

        private GUIContent GetContentForRandomizeToggle()
        {
            var content = new GUIContent();
            content.text = "Randomize scale";
            content.tooltip = "If this is checked, the scale will be randomized using the specified randomization settings.";

            return content;
        }
        #endregion
    }
}
#endif