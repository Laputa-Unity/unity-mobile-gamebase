#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathManualConstructionSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathManualConstructionSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementPathManualConstructionSettingsView(ObjectPlacementPathManualConstructionSettings settings)
        {
            _settings = settings;

            VisibilityToggleIndent = 1;
            SurroundWithBox = true;
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Path Construction Settings";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            if (!ObjectPlacement.Get().UserWantsToPlaceTileConnections) RenderExcludeCornersToggle();
            if (!ObjectPlacement.Get().UserWantsToPlaceTileConnections) RenderRandomizePrefabsInActiveCategoryToggle();
            if (!ObjectPlacement.Get().UserWantsToPlaceTileConnections) RenderRotateObjectsToFollowPathToggle();
            RenderObjectMissChanceSlider();
            RenderOffsetAlongGrowDirectionField();
            if (!ObjectPlacement.Get().UserWantsToPlaceTileConnections) _settings.PaddingSettings.View.Render();

            EditorGUILayout.Separator();
            _settings.BorderSettings.View.Render();

            EditorGUILayout.Separator();
            _settings.HeightAdjustmentSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderExcludeCornersToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForExcludeCornersToggle(), _settings.ExcludeCorners);
            if(newBool != _settings.ExcludeCorners)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ExcludeCorners = newBool;
            }
        }

        private GUIContent GetContentForExcludeCornersToggle()
        {
            var content = new GUIContent();
            content.text = "Exclude corners";
            content.tooltip = "If this is checked, objects which sit at a 90 degree turn (i.e. corner) inside the path will not be placed in the scene. " + 
                              "The corresponding boxes will be hidden during construction to illustrate this fact.";

            return content;
        }

        private void RenderRandomizePrefabsInActiveCategoryToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForRandomizePrefabsInActiveCategoryToggle(), _settings.RandomizePrefabs);
            if (newBool != _settings.RandomizePrefabs)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RandomizePrefabs = newBool;
            }
        }

        private GUIContent GetContentForRandomizePrefabsInActiveCategoryToggle()
        {
            var content = new GUIContent();
            content.text = "Randomize prefabs";
            content.tooltip = "If this is checked, a random prefab will be chosen for each object inside the path. The prefab will be chosen from the active category.";

            return content;
        }

        private void RenderOffsetAlongGrowDirectionField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForOffsetAlongGrowDirectionField(), _settings.OffsetAlongGrowDirection);
            if (newFloat != _settings.OffsetAlongGrowDirection)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.OffsetAlongGrowDirection = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForOffsetAlongGrowDirectionField()
        {
            var content = new GUIContent();
            content.text = "Offset along grow direction";
            content.tooltip = "Allows you to controls the path's offset along the grow direction.";

            return content;
        }

        private void RenderRotateObjectsToFollowPathToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForRotateObjectsToFollowPathToggle(), _settings.RotateObjectsToFollowPath);
            if (newBool != _settings.RotateObjectsToFollowPath)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RotateObjectsToFollowPath = newBool;
            }
        }

        private GUIContent GetContentForRotateObjectsToFollowPathToggle()
        {
            var content = new GUIContent();
            content.text = "Rotate objects to follow path";
            content.tooltip = "If this is checked, each 90 degrees turn inside the path will cause the objects to also rotate 90 degrees.";

            return content;
        }

        private void RenderObjectMissChanceSlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForObjectMissChanceSlider(), _settings.ObjectMissChance, ObjectPlacementPathManualConstructionSettings.MinObjectMissChance, ObjectPlacementPathManualConstructionSettings.MaxObjectMissChance);
            if(newFloat != _settings.ObjectMissChance)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectMissChance = newFloat;
            }
        }

        private GUIContent GetContentForObjectMissChanceSlider()
        {
            var content = new GUIContent();
            content.text = "Object miss chance";
            content.tooltip = "Allows you to control the chance that an object in the path will not be placed in the scene. The bigger the value, the bigger the chance that an object will not be placed.";

            return content;
        }
        #endregion
    }
}
#endif