#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectColliderSnapSurfaceGridSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectColliderSnapSurfaceGridSettings _settings;
        #endregion

        #region Constructors
        public ObjectColliderSnapSurfaceGridSettingsView(ObjectColliderSnapSurfaceGridSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Surface Grid Settings";
            IndentContent = true;
            VisibilityToggleIndent = 1;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderDesiredCellSizeField();
        }
        #endregion

        #region Private Methods
        private void RenderDesiredCellSizeField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForDesiredCellSizeField(), _settings.DesiredCellSize);
            if(newFloat != _settings.DesiredCellSize)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.DesiredCellSize = newFloat;
            }
        }

        private GUIContent GetContentForDesiredCellSizeField()
        {
            var content = new GUIContent();
            content.text = "Desired cell size";
            content.tooltip = "This is the desired cell size for the grid which allows you to snap objects long the collider surfaces of other objects. " + 
                              "Note: The actual cell size may differ from the value that you specify here but it will be as close as possible.";

            return content;
        }
        #endregion
    }
}
#endif