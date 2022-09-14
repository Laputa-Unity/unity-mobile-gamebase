#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathBorderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathBorderSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementPathBorderSettingsView(ObjectPlacementPathBorderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderUseBordersToggle();
            if(_settings.UseBorders)
            {
                RenderBeginBorderWidthField();
                RenderEndBorderWidthField();
                RenderBottomBorderWidthField();
                RenderTopBorderWidthField();
            }
        }
        #endregion

        #region Private Methods
        private void RenderUseBordersToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseBordersToggle(), _settings.UseBorders);
            if(newBool != _settings.UseBorders)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseBorders = newBool;
            }
        }

        private GUIContent GetContentForUseBordersToggle()
        {
            var content = new GUIContent();
            content.text = "Use borders";
            content.tooltip = "If this is checked, you will have the ability to specify border settings for the path which will allow you to create hollow paths.";

            return content;
        }

        private void RenderBeginBorderWidthField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForBeginBorderWidthField(), _settings.BeginBorderWidth);
            if(newInt != _settings.BeginBorderWidth)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.BeginBorderWidth = newInt;
            }
        }

        private GUIContent GetContentForBeginBorderWidthField()
        {
            var content = new GUIContent();
            content.text = "Begin border width";
            content.tooltip = "Allows you to specify the width of the border at the beginning of the path.";

            return content;
        }

        private void RenderEndBorderWidthField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForEndBorderWidthField(), _settings.EndBorderWidth);
            if (newInt != _settings.EndBorderWidth)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EndBorderWidth = newInt;
            }
        }

        private GUIContent GetContentForEndBorderWidthField()
        {
            var content = new GUIContent();
            content.text = "End border width";
            content.tooltip = "Allows you to specify the width of the border at the end of the path.";

            return content;
        }

        private void RenderBottomBorderWidthField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForBottomBorderWidthField(), _settings.BottomBorderWidth);
            if (newInt != _settings.BottomBorderWidth)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.BottomBorderWidth = newInt;
            }
        }

        private GUIContent GetContentForBottomBorderWidthField()
        {
            var content = new GUIContent();
            content.text = "Bottom border width";
            content.tooltip = "Allows you to specify the width of the border at the bottom of the path.";

            return content;
        }

        private void RenderTopBorderWidthField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForTopBorderWidthField(), _settings.TopBorderWidth);
            if (newInt != _settings.TopBorderWidth)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.TopBorderWidth = newInt;
            }
        }

        private GUIContent GetContentForTopBorderWidthField()
        {
            var content = new GUIContent();
            content.text = "Top border width";
            content.tooltip = "Allows you to specify the width of the border at the top of the path.";

            return content;
        }
        #endregion
    }
}
#endif