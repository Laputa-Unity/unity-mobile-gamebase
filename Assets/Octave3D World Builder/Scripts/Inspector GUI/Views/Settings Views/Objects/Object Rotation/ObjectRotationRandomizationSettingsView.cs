#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectRotationRandomizationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectRotationRandomizationSettings _settings;
        #endregion

        #region Constructors
        public ObjectRotationRandomizationSettingsView(ObjectRotationRandomizationSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Rotation Randomization Settings";
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderRandomizeToggle();

            if(_settings.RandomizeRotation)
            {
                _settings.CustomAxisRandomizationSettings.View.Render();
                _settings.XAxisRandomizationSettings.View.Render();
                _settings.YAxisRandomizationSettings.View.Render();
                _settings.ZAxisRandomizationSettings.View.Render();
            }
        }
        #endregion

        #region Private Methods
        private void RenderRandomizeToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForRandomizeToggle(), _settings.RandomizeRotation);
            if(newBool != _settings.RandomizeRotation)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RandomizeRotation = newBool;
            }
        }

        private GUIContent GetContentForRandomizeToggle()
        {
            var content = new GUIContent();
            content.text = "Randomize rotation";
            content.tooltip = "If this is checked, the rotation will be randomized using the specified randomization settings.";

            return content;
        }
        #endregion
    }
}
#endif