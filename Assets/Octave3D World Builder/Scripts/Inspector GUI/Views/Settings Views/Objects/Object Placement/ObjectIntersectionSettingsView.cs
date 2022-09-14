#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectIntersectionSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectIntersectionSettings _settings;
        #endregion

        #region Constructors
        public ObjectIntersectionSettingsView(ObjectIntersectionSettings settings)
        {
            _settings = settings;

            VisibilityToggleLabel = "Object Intersection";
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleIndent = 1;
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderAllowIntersectionForDecorPaintSingleModeDragToggle();
            RenderAllowIntersectionForDecorPaintBrushModeDragToggle();
            RenderAllowIntersectionForPathPlacementToggle();
            RenderAllowIntersectionForBlockPlacementToggle();
        }
        #endregion

        #region Private Methods
        private void RenderAllowIntersectionForDecorPaintSingleModeDragToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowIntersectionForDecorPaintSingleModeDragToggle(), _settings.AllowIntersectionForDecorPaintSingleModeDrag);
            if(newBool != _settings.AllowIntersectionForDecorPaintSingleModeDrag)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AllowIntersectionForDecorPaintSingleModeDrag = newBool;
            }
        }

        private GUIContent GetContentForAllowIntersectionForDecorPaintSingleModeDragToggle()
        {
            var content = new GUIContent();
            content.text = "Allow for decor paint (single mode) drag";
            content.tooltip = "If this is checked, object intersections are allowed during a drag operation while operating in decor paint - single mode.";

            return content;
        }

        private void RenderAllowIntersectionForDecorPaintBrushModeDragToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowIntersectionForDecorPaintBrushModeDragToggle(), _settings.AllowIntersectionForDecorPaintBrushModeDrag);
            if (newBool != _settings.AllowIntersectionForDecorPaintBrushModeDrag)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AllowIntersectionForDecorPaintBrushModeDrag = newBool;
            }
        }

        private GUIContent GetContentForAllowIntersectionForDecorPaintBrushModeDragToggle()
        {
            var content = new GUIContent();
            content.text = "Allow for decor paint (brush mode) drag";
            content.tooltip = "If this is checked, object intersections are allowed during a drag operation while operating in decor paint - brush mode.";

            return content;
        }

        private void RenderAllowIntersectionForPathPlacementToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowIntersectionForPathPlacementToggle(), _settings.AllowIntersectionForPathPlacement);
            if (newBool != _settings.AllowIntersectionForPathPlacement)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AllowIntersectionForPathPlacement = newBool;
            }
        }

        private GUIContent GetContentForAllowIntersectionForPathPlacementToggle()
        {
            var content = new GUIContent();
            content.text = "Allow for path";
            content.tooltip = "If this is checked, object intersections are allowed when working in \'Path\' placement mode.";

            return content;
        }

        private void RenderAllowIntersectionForBlockPlacementToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowIntersectionForBlockPlacementToggle(), _settings.AllowIntersectionForBlockPlacement);
            if (newBool != _settings.AllowIntersectionForBlockPlacement)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AllowIntersectionForBlockPlacement = newBool;
            }
        }

        private GUIContent GetContentForAllowIntersectionForBlockPlacementToggle()
        {
            var content = new GUIContent();
            content.text = "Allow for block";
            content.tooltip = "If this is checked, object intersections are allowed when working in \'Block\' placement mode.";

            return content;
        }
        #endregion
    }
}
#endif