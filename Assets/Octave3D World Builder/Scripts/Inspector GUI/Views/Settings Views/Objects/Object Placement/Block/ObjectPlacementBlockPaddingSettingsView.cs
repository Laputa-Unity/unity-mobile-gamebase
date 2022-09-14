#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockPaddingSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementBlockPaddingSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementBlockPaddingSettingsView(ObjectPlacementBlockPaddingSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderPaddingAlongGrowDirectionField();
            RenderPaddingAlongExtensionPlaneField();
        }
        #endregion

        #region Private Methods
        private void RenderPaddingAlongGrowDirectionField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForPaddingAlongGrowDirectionField(), _settings.PaddingAlongGrowDirection);
            if (newFloat != _settings.PaddingAlongGrowDirection)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PaddingAlongGrowDirection = newFloat;
            }
        }

        private GUIContent GetContentForPaddingAlongGrowDirectionField()
        {
            var content = new GUIContent();
            content.text = "Padding along grow direction";
            content.tooltip = "This is the padding between objects along the block's grow direction.";

            return content;
        }

        private void RenderPaddingAlongExtensionPlaneField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForPaddingAlongRightExtensionAxisField(), _settings.PaddingAlongExtensionPlane);
            if (newFloat != _settings.PaddingAlongExtensionPlane)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PaddingAlongExtensionPlane = newFloat;
            }
        }

        private GUIContent GetContentForPaddingAlongRightExtensionAxisField()
        {
            var content = new GUIContent();
            content.text = "Padding along extension plane";
            content.tooltip = "This is the padding between objects along the extension plane.";

            return content;
        }
        #endregion
    }
}
#endif