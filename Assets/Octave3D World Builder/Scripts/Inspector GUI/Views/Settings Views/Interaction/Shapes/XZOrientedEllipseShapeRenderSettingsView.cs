#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class XZOrientedEllipseShapeRenderSettingsView : SettingsView
    {
        #region Private Variables
        [SerializeField]
        private XZOrientedEllipseShapeRenderSettings _settings;
        #endregion

        #region Constructors
        public XZOrientedEllipseShapeRenderSettingsView(XZOrientedEllipseShapeRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderBorderLineColorField();
        }
        #endregion

        #region Private Methods
        private void RenderBorderLineColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForBorderLineColorField(), _settings.BorderLineColor);
            if(newColor != _settings.BorderLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.BorderLineColor = newColor;
            }
        }

        private GUIContent GetContentForBorderLineColorField()
        {
            var content = new GUIContent();
            content.text = "Border line color";
            content.tooltip = "Allows you to change the color of the lines which make up the shape.";

            return content;
        }
        #endregion
    }
}
#endif