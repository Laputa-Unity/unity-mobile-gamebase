#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class SingleDecorPaintModeObjectPlacementSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private SingleDecorPaintModeObjectPlacementSettings _settings;
        #endregion

        #region Constructors
        public SingleDecorPaintModeObjectPlacementSettingsView(SingleDecorPaintModeObjectPlacementSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            var content = new GUIContent();
            content.text = "Embed in surface (no align)";
            content.tooltip = "This is only used if axis alignment is turned off and if it is checked it will ensure that the objects are " + 
                              "embedded inside the surface as much as it is needed so that they don't float above it.";
            bool newBool = EditorGUILayout.ToggleLeft(content, _settings.EmbedInSurfaceWhenNoAlign);
            if(newBool != _settings.EmbedInSurfaceWhenNoAlign)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EmbedInSurfaceWhenNoAlign = newBool;
            }

            RenderAlignToStrokeToggle();
            RenderRandomizePrefabsInActiveCategoryToggle();

            content.text = "Use original pivot";
            content.tooltip = "If this is checked, surface snapping will be done using the pivot point defined in the modelling package.";
            newBool = EditorGUILayout.ToggleLeft(content, _settings.UseOriginalPivot);
            if (newBool != _settings.UseOriginalPivot)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseOriginalPivot = newBool;
            }

            EditorGUILayout.Separator();
            _settings.PlacementGuideSurfaceAlignmentSettings.View.Render();
            if(!_settings.AlignToStroke) _settings.PlacementGuideRotationRandomizationSettings.View.Render();
            _settings.PlacementGuideScaleRandomizationSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderAlignToStrokeToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAlignToStrokeToggle(), _settings.AlignToStroke);
            if(newBool != _settings.AlignToStroke)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AlignToStroke = newBool;
            }
        }

        private GUIContent GetContentForAlignToStrokeToggle()
        {
            var content = new GUIContent();
            content.text = "Align to stroke";
            content.tooltip = "If this is checked, the rotation of the objects will be adjusted such that they follow the stroke travel direction.";

            return content;
        }

        private void RenderRandomizePrefabsInActiveCategoryToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForRandomizePrefabsInActiveCategoryToggle(), _settings.RandomizePrefabsInActiveCategory);
            if(newBool != _settings.RandomizePrefabsInActiveCategory)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RandomizePrefabsInActiveCategory = newBool;
            }
        }

        private GUIContent GetContentForRandomizePrefabsInActiveCategoryToggle()
        {
            var content = new GUIContent();
            content.text = "Randomize prefabs in active category";
            content.tooltip = "If this is checked, a random prefab will be chosen from the active category each time an object is placed in the scene.";

            return content;
        }
        #endregion
    }
}
#endif