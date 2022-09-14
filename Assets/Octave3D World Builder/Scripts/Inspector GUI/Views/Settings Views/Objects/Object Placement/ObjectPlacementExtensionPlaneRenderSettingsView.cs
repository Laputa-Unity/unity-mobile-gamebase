#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementExtensionPlaneRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementExtensionPlaneRenderSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementExtensionPlaneRenderSettingsView(ObjectPlacementExtensionPlaneRenderSettings settings)
        {
            _settings = settings;

            VisibilityToggleLabel = "Extension Plane";
            ToggleVisibilityBeforeRender = true;
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderPlaneScaleField();
            RenderPlaneColorField();
            RenderPlaneBorderLineColorField();
            RenderPlaneNormalLineLengthField();
            RenderPlaneNormalLineColorField();
        }
        #endregion

        #region Private Methods
        private void RenderPlaneScaleField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForPlaneScaleField(), _settings.PlaneScale);
            if(newFloat != _settings.PlaneScale)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PlaneScale = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForPlaneScaleField()
        {
            var content = new GUIContent();
            content.text = "Plane scale";
            content.tooltip = "Allows you to adjust the scale of the plane.";

            return content;
        }

        private void RenderPlaneColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForPlaneColorField(), _settings.PlaneColor);
            if(newColor != _settings.PlaneColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PlaneColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForPlaneColorField()
        {
            var content = new GUIContent();
            content.text = "Plane color";
            content.tooltip = "Allows you to modify the color of the extension plane.";

            return content;
        }

        private void RenderPlaneBorderLineColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForBorderLineColorField(), _settings.PlaneBorderLineColor);
            if (newColor != _settings.PlaneBorderLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PlaneBorderLineColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForBorderLineColorField()
        {
            var content = new GUIContent();
            content.text = "Border line color";
            content.tooltip = "Allows you to modify the color of the extension plane border lines.";

            return content;
        }

        private void RenderPlaneNormalLineLengthField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForPlaneNormalLineLengthField(), _settings.PlaneNormalLineLength);
            if(newFloat != _settings.PlaneNormalLineLength)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PlaneNormalLineLength = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForPlaneNormalLineLengthField()
        {
            var content = new GUIContent();
            content.text = "Plane normal line length";
            content.tooltip = "Allows you to modiy the length of the lines which represent the plane normals.";

            return content;
        }

        private void RenderPlaneNormalLineColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForPlaneNormalLineColorField(), _settings.PlaneNormalLineColor);
            if(newColor != _settings.PlaneNormalLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PlaneNormalLineColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForPlaneNormalLineColorField()
        {
            var content = new GUIContent();
            content.text = "Plane normal line color";
            content.tooltip = "Allows you to modify the color of the lines which represent the plane normals.";

            return content;
        }
        #endregion
    }
}
#endif