#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class XZGridRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private XZGridRenderSettings _settings;

        private bool _modifyLineThickness = true;
        #endregion

        public bool ModifyLineThickness { get { return _modifyLineThickness; } set { _modifyLineThickness = value; } }

        #region Constructors
        public XZGridRenderSettingsView(XZGridRenderSettings settings)
        {
            _settings = settings;
            VisibilityToggleLabel = "MySettings";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderIsVisibleToggle();
            RenderGridCellLineColorField();
            if (ModifyLineThickness) RenderCellLineThicknessField();
            RenderPlaneColorField();
        }
        #endregion

        #region Private Methods
        private void RenderIsVisibleToggle()
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
            content.tooltip = "If this is checked, the grid will be drawn in the scene.";

            return content;
        }

        private void RenderGridCellLineColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForGridCellLineColorField(), _settings.CellLineColor);
            if(newColor != _settings.CellLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CellLineColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForGridCellLineColorField()
        {
            var content = new GUIContent();
            content.text = "Cell line color";
            content.tooltip = "Allows you to modify the color of the grid cell lines.";

            return content;
        }

        private void RenderCellLineThicknessField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForCellLineThicknessField(), _settings.CellLineThickness);
            if (newFloat != _settings.CellLineThickness)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CellLineThickness = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForCellLineThicknessField()
        {
            var content = new GUIContent();
            content.text = "Cell line thickness";
            content.tooltip = "Allows you to modify thickness of the cell lines.";

            return content;
        }

        private void RenderPlaneColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForPlaneColorField(), _settings.PlaneColor);
            if (newColor != _settings.PlaneColor)
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
            content.tooltip = "Allows you to specify the color of the grid plane.";

            return content;
        }
        #endregion
    }
}
#endif