#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisAlignmentSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private AxisAlignmentSettings _settings;
        #endregion

        #region Constructors
        public AxisAlignmentSettingsView(AxisAlignmentSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Axis Alignment";
            IndentContent = true;
        }
        #endregion 

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderIsEnabledToggle();
            RenderAlignmentAxisSelectionPopup();
        }
        #endregion

        #region Private Methods
        private void RenderIsEnabledToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIsEnabledToggle(), _settings.IsEnabled);
            if(newBool != _settings.IsEnabled)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.IsEnabled = newBool;
            }
        }

        private GUIContent GetContentForIsEnabledToggle()
        {
            var content = new GUIContent();
            content.text = "Align axis";
            content.tooltip = "If this is checked, axis alignment is turned on.";

            return content;
        }

        private void RenderAlignmentAxisSelectionPopup()
        {
            CoordinateSystemAxis newCoordSystemAxis = (CoordinateSystemAxis)EditorGUILayout.EnumPopup(GetContentForAxisAlignmentSelectionPopup(), _settings.AlignmentAxis);
            if(newCoordSystemAxis != _settings.AlignmentAxis)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AlignmentAxis = newCoordSystemAxis;
            }
        }

        private GUIContent GetContentForAxisAlignmentSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Alignment axis";
            content.tooltip = "This is the axis that will be used for alignment.";

            return content;
        }
        #endregion
    }
}
#endif