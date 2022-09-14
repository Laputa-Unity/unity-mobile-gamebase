#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ProjectedBoxFacePivotPointsRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ProjectedBoxFacePivotPointsRenderSettings _settings;
        #endregion

        #region Constructors
        public ProjectedBoxFacePivotPointsRenderSettingsView(ProjectedBoxFacePivotPointsRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderProjectionLineRenderingToggle();
            RenderProjectionLineColorSelectionField();

            EditorGUILayout.Separator();
            RenderPivotPointConnectionLineRenderToggle();
            RenderPivotPointConnectionLineColorSelectionField();

            EditorGUILayout.Separator();
            _settings.ActivePivotPointRenderSettings.View.Render();
            _settings.InactivePivotPointRenderSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderProjectionLineRenderingToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForProjectionLineRenderingToggle(), _settings.RenderProjectionLines);
            if(newBool != _settings.RenderProjectionLines)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RenderProjectionLines = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForProjectionLineRenderingToggle()
        {
            var content = new GUIContent();
            content.text = "Draw projection lines";
            content.tooltip = "If this is checked, lines will be drawn between the projected pivot points and the corresponding points on the hierarchy box face.";

            return content;
        }

        private void RenderProjectionLineColorSelectionField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForProjectionLineColorSelectionField(), _settings.ProjectionLineColor);
            if(newColor != _settings.ProjectionLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ProjectionLineColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForProjectionLineColorSelectionField()
        {
            var content = new GUIContent();
            content.text = "Projection line color";
            content.tooltip = "Allows you to control the color of the projection lines.";

            return content;
        }

        private void RenderPivotPointConnectionLineRenderToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForPivotPointConnectionLineRenderToggle(), _settings.RenderPivotPointConnectionLines);
            if(newBool != _settings.RenderPivotPointConnectionLines)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RenderPivotPointConnectionLines = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForPivotPointConnectionLineRenderToggle()
        {
            var content = new GUIContent();
            content.text = "Draw pivot point connection lines";
            content.tooltip = "If this is checked, lines will be drawn between the pivot points.";

            return content;
        }

        private void RenderPivotPointConnectionLineColorSelectionField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForPivotPointConnectionLineColorSelectionField(), _settings.PivotPointConnectionLineColor);
            if(newColor != _settings.PivotPointConnectionLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PivotPointConnectionLineColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForPivotPointConnectionLineColorSelectionField()
        {
            var content = new GUIContent();
            content.text = "Color for pivot point connection lines";
            content.tooltip = "This is the color that is used to draw the pivot point connection lines.";

            return content;
        }
        #endregion
    }
}
#endif