#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class XZGridCellSizeSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private XZGridCellSizeSettings _settings;

        [SerializeField]
        private string _xAxisName = XZGrid.ModelSpaceRightAxisName;
        [SerializeField]
        private string _zAxisName = XZGrid.ModelSpaceLookAxisName;
        #endregion

        #region Public Properties
        public string XAxisName { get { return _xAxisName; } set { if (!string.IsNullOrEmpty(value)) _xAxisName = value; } }
        public string ZAxisName { get { return _zAxisName; } set { if (!string.IsNullOrEmpty(value)) _zAxisName = value; } }
        #endregion

        #region Constructors
        public XZGridCellSizeSettingsView(XZGridCellSizeSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderCellSizeFieldForXAxis();
            RenderCellSizeFieldForZAxis();
        }
        #endregion

        #region Private Methods
        private void RenderCellSizeFieldForXAxis()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForCellSizeField(_xAxisName), _settings.CellSizeX);
            if (newFloat != _settings.CellSizeX)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CellSizeX = newFloat;

                SceneView.RepaintAll();
            }
        }

        private void RenderCellSizeFieldForZAxis()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForCellSizeField(_zAxisName), _settings.CellSizeZ);
            if (newFloat != _settings.CellSizeZ)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CellSizeZ = newFloat;

                SceneView.RepaintAll();
            }
        }
       
        private GUIContent GetContentForCellSizeField(string sizeAxisLabel)
        {
            var content = new GUIContent();
            content.text = "Cell size " + sizeAxisLabel;
            content.tooltip = "Allows you to change the cell size along the " + sizeAxisLabel + " axis.";

            return content;
        }
        #endregion
    }
}
#endif
