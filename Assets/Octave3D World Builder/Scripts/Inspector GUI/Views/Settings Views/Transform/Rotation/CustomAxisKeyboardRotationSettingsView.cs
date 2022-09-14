#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class CustomAxisKeyboardRotationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private CustomAxisKeyboardRotationSettings _settings;
        #endregion

        #region Constructors
        public CustomAxisKeyboardRotationSettingsView(CustomAxisKeyboardRotationSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Custom Axis Settings";
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderRotationAmountField();
        }
        #endregion

        #region Private Methods
        private void RenderRotationAmountField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForRotationAmountField(), _settings.RotationAmountInDegrees);
            if(newFloat != _settings.RotationAmountInDegrees)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RotationAmountInDegrees = newFloat;
            }
        }

        private GUIContent GetContentForRotationAmountField()
        {
            var content = new GUIContent();
            content.text = "Rotation amount (degrees)";
            content.tooltip = "The amount of rotation which must be applied. The value is expressed in degrees.";

            return content;
        }
        #endregion
    }
}
#endif