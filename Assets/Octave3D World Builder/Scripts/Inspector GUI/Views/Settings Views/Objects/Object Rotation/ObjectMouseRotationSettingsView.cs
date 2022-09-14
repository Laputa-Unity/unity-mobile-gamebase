#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectMouseRotationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectMouseRotationSettings _settings;
        #endregion

        #region Constructors
        public ObjectMouseRotationSettingsView(ObjectMouseRotationSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Mouse Rotation Settings";
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _settings.XAxisRotationSettings.View.Render();
            _settings.YAxisRotationSettings.View.Render();
            _settings.ZAxisRotationSettings.View.Render();
            _settings.CustomAxisRotationSettings.View.Render();
            RenderSnappingControls();
        }
        #endregion

        #region Private Variables
        private void RenderSnappingControls()
        {
            RenderUseSnappingToggle();
            RenderSnapStepField();
        }

        private void RenderSnapStepField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForSnapStepField(), _settings.SnapStepInDegrees);
            if (newFloat != _settings.SnapStepInDegrees)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapStepInDegrees = newFloat;
            }
        }

        private GUIContent GetContentForSnapStepField()
        {
            var content = new GUIContent();
            content.text = "Snap step";
            content.tooltip = "The amount of rotation to be applied during each step when using snapping.";

            return content;
        }

        private void RenderUseSnappingToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseSnappingToggle(), _settings.UseSnapping);
            if (newBool != _settings.UseSnapping)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseSnapping = newBool;
            }
        }

        private GUIContent GetContentForUseSnappingToggle()
        {
            var content = new GUIContent();
            content.text = "Use snapping";
            content.tooltip = "If this is checked, snapping will be used while applying rotations.";

            return content;
        }
        #endregion
    }
}
#endif