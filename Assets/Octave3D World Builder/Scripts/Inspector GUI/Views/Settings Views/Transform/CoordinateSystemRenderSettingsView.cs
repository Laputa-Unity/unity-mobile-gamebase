#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class CoordinateSystemRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private CoordinateSystemRenderSettings _settings;
        #endregion

        #region Constructors
        public CoordinateSystemRenderSettingsView(CoordinateSystemRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderIsVisibleToogle();
            _settings.GetAxisRenderSettings(CoordinateSystemAxis.PositiveRight).View.Render();
            _settings.GetAxisRenderSettings(CoordinateSystemAxis.NegativeRight).View.Render();
            _settings.GetAxisRenderSettings(CoordinateSystemAxis.PositiveUp).View.Render();
            _settings.GetAxisRenderSettings(CoordinateSystemAxis.NegativeUp).View.Render();
            _settings.GetAxisRenderSettings(CoordinateSystemAxis.PositiveLook).View.Render();
            _settings.GetAxisRenderSettings(CoordinateSystemAxis.NegativeLook).View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderIsVisibleToogle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIsVisibleToggle(), _settings.IsVisible);
            if(newBool != _settings.IsVisible)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.IsVisible = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForIsVisibleToggle()
        {
            var content = new GUIContent();
            content.text = "Is visible";
            content.tooltip = "Allows you to specify whether or not the coordinate system is visible.";

            return content;
        }
        #endregion
    }
}
#endif