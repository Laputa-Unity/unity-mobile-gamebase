#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisRotationRandomizationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private AxisRotationRandomizationSettings _settings;
        #endregion

        #region Constructors
        public AxisRotationRandomizationSettingsView(AxisRotationRandomizationSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            IndentContent = true;
            VisibilityToggleIndent = 1;
            VisibilityToggleLabel = "Axis Rotation Settings";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderRandomizeRotationToggle();
            _settings.RandomizationModeSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderRandomizeRotationToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForRandomizeRotationToggle(), _settings.Randomize);
            if(newBool != _settings.Randomize)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.Randomize = newBool;
            }
        }

        private GUIContent GetContentForRandomizeRotationToggle()
        {
            var content = new GUIContent();
            content.text = "Randomize";
            content.tooltip = "Allows you to toggle rotation randomization for this axis.";

            return content;
        }
        #endregion
    }
}
#endif