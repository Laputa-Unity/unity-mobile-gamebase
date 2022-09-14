#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionBoxRenderModeSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSelectionBoxRenderModeSettings _settings;
        #endregion

        #region Constructors
        public ObjectSelectionBoxRenderModeSettingsView(ObjectSelectionBoxRenderModeSettings settings)
        {
            _settings = settings;
            ToggleVisibilityBeforeRender = false;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderEdgeRenderModeSelectionPopup();
            if (_settings.EdgeRenderMode == ObjectSelectionBoxEdgeRenderMode.CornerEdges) _settings.CornerEdgesRenderModeSettings.View.Render();
            RenderEdgeColorField();
            RenderBoxColorField();
            RenderBoxScaleField();
        }
        #endregion

        #region Private Methods
        private void RenderEdgeRenderModeSelectionPopup()
        {
            ObjectSelectionBoxEdgeRenderMode newEdgeRenderMode = (ObjectSelectionBoxEdgeRenderMode)EditorGUILayout.EnumPopup(GetContentForEdgeRenderModeSelectionPopup(), _settings.EdgeRenderMode);
            if (newEdgeRenderMode != _settings.EdgeRenderMode)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EdgeRenderMode = newEdgeRenderMode;
            }
        }

        private GUIContent GetContentForEdgeRenderModeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Edge draw mode";
            content.tooltip = "Allows you to specify how the edges of the object selection boxes are drawn.";

            return content;
        }

        private void RenderEdgeColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForEdgeColorField(), _settings.EdgeColor);
            if (newColor != _settings.EdgeColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EdgeColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForEdgeColorField()
        {
            var content = new GUIContent();
            content.text = "Edge color";
            content.tooltip = "Allows you to specify the color that is used to draw the object selection box edges.";

            return content;
        }

        private void RenderBoxColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForBoxColorField(), _settings.BoxColor);
            if (newColor != _settings.BoxColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.BoxColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForBoxColorField()
        {
            var content = new GUIContent();
            content.text = "Box color";
            content.tooltip = "Allows you to specify the color that is used to draw the object selection boxes.";

            return content;
        }

        private void RenderBoxScaleField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForBoxScaleField(), _settings.BoxScale);
            if (newFloat != _settings.BoxScale)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.BoxScale = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForBoxScaleField()
        {
            var content = new GUIContent();
            content.text = "Box scale";
            content.tooltip = "Allows you to adjust the scale of the selection boxes.";

            return content;
        }
        #endregion
    }
}
#endif