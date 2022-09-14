#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionBoxCornerEdgesRenderModeSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSelectionBoxCornerEdgesRenderModeSettings _settings;
        #endregion

        #region Constructors
        public ObjectSelectionBoxCornerEdgesRenderModeSettingsView(ObjectSelectionBoxCornerEdgesRenderModeSettings settings)
        {
            _settings = settings;
            VisibilityToggleLabel = "Custom Edge Length Settings";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderCornerEdgeLengthPercentageField();
        }
        #endregion

        #region Private Methods
        private void RenderCornerEdgeLengthPercentageField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForCornerEdgeLengthPercentageField(), _settings.CornerEdgeLengthPercentage);
            if (newFloat != _settings.CornerEdgeLengthPercentage)
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get());
                _settings.CornerEdgeLengthPercentage = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForCornerEdgeLengthPercentageField()
        {
            var content = new GUIContent();
            content.text = "Corner edge length percentage";
            content.tooltip = "The length of a corner edge = half box edge * percentage value.";

            return content;
        }
        #endregion
    }
}
#endif