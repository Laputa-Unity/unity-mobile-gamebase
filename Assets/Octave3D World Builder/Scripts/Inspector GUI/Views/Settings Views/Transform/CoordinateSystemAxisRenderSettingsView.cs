#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class CoordinateSystemAxisRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private CoordinateSystemAxisRenderSettings _settings;
        #endregion

        #region Constructors
        public CoordinateSystemAxisRenderSettingsView(CoordinateSystemAxisRenderSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            IndentContent = true;
            VisibilityToggleLabel = "Axis Render Settings";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderIsAxisVisibleToggle();
            RenderIsInfiniteToggle();
            RenderFiniteSizeField();
            RenderAxisColorSelectionField();
        }
        #endregion

        #region Private Methods
        private void RenderIsAxisVisibleToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIsAxisVisibleToggle(), _settings.IsVisible);
            if(newBool != _settings.IsVisible)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.IsVisible = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForIsAxisVisibleToggle()
        {
            var content = new GUIContent();
            content.text = "Is visible";
            content.tooltip = "Allows you to specify whether or not the axis is visible.";

            return content;
        }

        private void RenderAxisColorSelectionField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForAxisColorSelectionField(), _settings.Color);
            if(newColor != _settings.Color)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.Color = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForAxisColorSelectionField()
        {
            var content = new GUIContent();
            content.text = "Color";
            content.tooltip = "Allows you to control the color of the axis.";

            return content;
        }

        private void RenderIsInfiniteToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIsInfiniteToggle(), _settings.IsInfinite);
            if(newBool != _settings.IsInfinite)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.IsInfinite = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForIsInfiniteToggle()
        {
            var content = new GUIContent();
            content.text = "Is infinite";
            content.tooltip = "If this is checked, the axis will be rendered as infinite.";

            return content;
        }

        private void RenderFiniteSizeField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForFiniteSize(), _settings.FiniteSize);
            if(newFloat != _settings.FiniteSize)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.FiniteSize = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForFiniteSize()
        {
            var content = new GUIContent();
            content.text = "Finite size";
            content.tooltip = "Allows you to specify the size of the axis. This value is used only when \'Is infinite\' is NOT checked.";

            return content;
        }
        #endregion
    }
}
#endif