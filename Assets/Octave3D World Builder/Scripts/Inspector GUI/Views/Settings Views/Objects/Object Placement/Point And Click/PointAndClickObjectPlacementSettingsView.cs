#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class PointAndClickObjectPlacementSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private PointAndClickObjectPlacementSettings _settings;
        #endregion

        #region Constructors
        public PointAndClickObjectPlacementSettingsView(PointAndClickObjectPlacementSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderRandomizePrefabsInActiveCategoryToggle();
            _settings.PlacementGuideSurfaceAlignmentSettings.View.Render();
            _settings.PlacementGuideRotationRandomizationSettings.View.Render();
            _settings.PlacementGuideScaleRandomizationSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderRandomizePrefabsInActiveCategoryToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForRandomizePrefabsInActiveCategoryToggle(), _settings.RandomizePrefabsInActiveCategory);
            if (newBool != _settings.RandomizePrefabsInActiveCategory)
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