#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisKeyboardRotationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private AxisKeyboardRotationSettings _settings;
        #endregion

        #region Constructors
        public AxisKeyboardRotationSettingsView(AxisKeyboardRotationSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
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
            if (newFloat != _settings.RotationAmountInDegrees)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RotationAmountInDegrees = newFloat;
            }
        }

        private GUIContent GetContentForRotationAmountField()
        {
            var content = new GUIContent();
            content.text = "Rotation amount (degrees)";
            content.tooltip = "Allows you to specify the amount of rotation which must be applied when rotating around this axis. The value is expressed in degrees.";

            return content;
        }
        #endregion
    }
}
#endif