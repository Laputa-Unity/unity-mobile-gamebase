#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectVertexSnapSessionRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectVertexSnapSessionRenderSettings _settings;
        #endregion

        #region Constructors
        public ObjectVertexSnapSessionRenderSettingsView(ObjectVertexSnapSessionRenderSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Vertex Snapping";
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderSourceVertexRenderToggle();
            RenderSourceVertexFillColorField();
            RenderSourceVertexBorderColorField();
            RenderSourceVertexRadiusInPixelsField();
        }
        #endregion

        #region Private Methods
        private void RenderSourceVertexRenderToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSourceVertexRenderToggle(), _settings.RenderSourceVertex);
            if(newBool != _settings.RenderSourceVertex)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RenderSourceVertex = newBool;
            }
        }

        private GUIContent GetContentForSourceVertexRenderToggle()
        {
            var content = new GUIContent();
            content.text = "Draw source vertex";
            content.tooltip = "If this is checked, the tool will draw the source vertex which is closest to the mouse cursor position inside the hovered " + 
                              "source triangle. This is the vertex which is snapped to the chosen destination position.";

            return content;
        }

        private void RenderSourceVertexFillColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForSourceVertexFillColorField(), _settings.SourceVertexFillColor);
            if (newColor != _settings.SourceVertexFillColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SourceVertexFillColor = newColor;
            }
        }

        private GUIContent GetContentForSourceVertexFillColorField()
        {
            var content = new GUIContent();
            content.text = "Source vertex fill color";
            content.tooltip = "Allows you to change the source vertex fill color.";

            return content;
        }

        private void RenderSourceVertexBorderColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForSourceVertexBorderColorField(), _settings.SourceVertexBorderColor);
            if (newColor != _settings.SourceVertexBorderColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SourceVertexBorderColor = newColor;
            }
        }

        private GUIContent GetContentForSourceVertexBorderColorField()
        {
            var content = new GUIContent();
            content.text = "Source vertex border color";
            content.tooltip = "Allows you to change the source vertex border color.";

            return content;
        }

        private void RenderSourceVertexRadiusInPixelsField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForSourceVertexRadiusInPixelsField(), _settings.SourceVertexRadiusInPixels);
            if(newFloat != _settings.SourceVertexRadiusInPixels)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SourceVertexRadiusInPixels = newFloat;
            }
        }

        private GUIContent GetContentForSourceVertexRadiusInPixelsField()
        {
            var content = new GUIContent();
            content.text = "Source vertex radius (pixels)";
            content.tooltip = "Allows you to specify the radius of the source vertex in pixels.";

            return content;
        }
        #endregion
    }
}
#endif