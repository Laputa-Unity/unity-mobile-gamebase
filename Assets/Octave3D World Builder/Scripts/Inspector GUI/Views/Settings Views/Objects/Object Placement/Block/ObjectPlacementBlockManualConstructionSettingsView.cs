#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockManualConstructionSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementBlockManualConstructionSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementBlockManualConstructionSettingsView(ObjectPlacementBlockManualConstructionSettings settings)
        {
            _settings = settings;

            VisibilityToggleIndent = 1;
            SurroundWithBox = true;
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Block Construction Settings";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            bool newBool; int newInt;

            var content = new GUIContent();
            content.text = "Contrain size";
            content.tooltip = "If this is checked, you will be able to constrain the block to a specified size limit.";
            newBool = EditorGUILayout.ToggleLeft(content, _settings.ContrainSize);
            if(newBool != _settings.ContrainSize)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ContrainSize = newBool;
            }

            if(_settings.ContrainSize)
            {
                content.text = "Max size";
                content.tooltip = "Allows you to specify a maximum block size. The same value is used for both width and depth.";
                newInt = EditorGUILayout.IntField(content, _settings.MaxSize);
                if(newInt != _settings.MaxSize)
                {
                    UndoEx.RecordForToolAction(_settings);
                    _settings.MaxSize = newInt;
                }
            }

            RenderExcludeCornersToggle();
            RenderRandomizePrefabsInActiveCategoryToggle();
            RenderOffsetAlongGrowDirectionField();
            RenderObjectMissChanceSlider();
            _settings.PaddingSettings.View.Render();

            EditorGUILayout.Separator();
            _settings.SubdivisionSettings.View.Render();

            EditorGUILayout.Separator();
            _settings.ObjectRotationRandomizationSettings.View.Render();

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

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForExcludeCornersToggle()
        {
            var content = new GUIContent();
            content.text = "Exclude corners";
            content.tooltip = "If this is checked, objects which sit inside the block's corners will not be placed in the scene. " +
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
            content.tooltip = "If this is checked, a random prefab will be chosen for each object inside the block. The prefab will be chosen from the active category.";

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
            content.tooltip = "Allows you to controls the block's offset along the grow direction.";

            return content;
        }

        private void RenderObjectMissChanceSlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForObjectMissChanceSlider(), _settings.ObjectMissChance, ObjectPlacementBlockManualConstructionSettings.MinObjectMissChance, ObjectPlacementBlockManualConstructionSettings.MaxObjectMissChance);
            if (newFloat != _settings.ObjectMissChance)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectMissChance = newFloat;
            }
        }

        private GUIContent GetContentForObjectMissChanceSlider()
        {
            var content = new GUIContent();
            content.text = "Object miss chance";
            content.tooltip = "Allows you to control the chance that an object in the block will not be placed in the scene. The bigger the value, the bigger the chance that an object will not be placed.";

            return content;
        }
        #endregion
    }
}
#endif