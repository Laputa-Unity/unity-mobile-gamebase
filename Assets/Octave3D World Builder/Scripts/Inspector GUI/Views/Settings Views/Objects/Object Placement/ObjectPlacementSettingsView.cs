#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementSettings _settings;

        [SerializeField]
        private ObjectPlacementModeSelectionToolbar _objectPlacementModeSelectionToolbar = new ObjectPlacementModeSelectionToolbar();
        #endregion

        #region Public Properties
        public ObjectPlacementModeSelectionToolbar ObjectPlacementModeSelectionToolbar { get { return _objectPlacementModeSelectionToolbar; } }
        #endregion

        #region Constructors
        public ObjectPlacementSettingsView(ObjectPlacementSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Placement Settings";
            SurroundWithBox = true;

            _objectPlacementModeSelectionToolbar.ButtonScale = 0.25f;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            bool newBool;

            RenderHideWireframeWhenPlacingObjectsToggle();
            RenderInheritRotationOnPrefabScrollToggle();

            EditorGUILayout.BeginHorizontal();
            RenderOpenMoreSettingsWindowButton();
            RenderOpenPrefabManagementWindowButton();
            RenderOpenPrefabTagsWindowButton();
            RenderOpenObjectLayersWindowButton();
            EditorGUILayout.EndHorizontal();

            _objectPlacementModeSelectionToolbar.Render();
            _settings.ObjectIntersectionSettings.View.Render();

            var content = new GUIContent();
            content.text = "Spawn in prefab layer";
            content.tooltip = "If this is checked, spawned objects will be assigned to the same layer as the active prefab.";
            newBool = EditorGUILayout.ToggleLeft(content, _settings.SpawnInPrefabLayer);
            if(newBool != _settings.SpawnInPrefabLayer)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SpawnInPrefabLayer = newBool;
            }

            if (!_settings.SpawnInPrefabLayer)
            {
                content.text = "Custom spawn layer";
                content.tooltip = "Allows you to choose a custom spawn layer.";

                var layerNames = LayerExtensions.GetAllAvailableLayerNames();
                string activeLayerName = LayerMask.LayerToName(_settings.CustomSpawnLayer);
                var layerNameContents = new GUIContent[layerNames.Count];
                for(int layerIndex = 0; layerIndex < layerNameContents.Length; ++layerIndex)
                {
                    layerNameContents[layerIndex] = new GUIContent(layerNames[layerIndex]);
                }

                int selectedIndex = layerNames.IndexOf(activeLayerName);
                if (selectedIndex >= 0)
                {
                    int newSelectedIndex = EditorGUILayout.Popup(content, selectedIndex, layerNameContents);
                    if (newSelectedIndex != selectedIndex)
                    {
                        UndoEx.RecordForToolAction(_settings);
                        _settings.CustomSpawnLayer = LayerMask.NameToLayer(layerNames[newSelectedIndex]);
                    }
                }
            }

            RenderMakePlacedObjectsChildrenOfHoveredObject();
            RenderAttachPlacedObjectsToActiveGroupToggle();
            if (_settings.AttachPlacedObjectsToObjectGroup) RenderUseActivePrefabCategoryGroupToggle();

            EditorGUILayout.Separator();
            if (_settings.ObjectPlacementMode == ObjectPlacementMode.DecorPaint) RenderDecorPaintPlacementControls();
            else if (_settings.ObjectPlacementMode == ObjectPlacementMode.PointAndClick) RenderPointAndClickPlacementControls();
            else if (_settings.ObjectPlacementMode == ObjectPlacementMode.Path) RenderPathPlacementControls();
            else if (_settings.ObjectPlacementMode == ObjectPlacementMode.Block) RenderBlockPlacementControls();
        }
        #endregion

        #region Private Methods
        private void RenderHideWireframeWhenPlacingObjectsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForHideWireframeWhenPlacingObjectsToggle(), _settings.HideWireframeWhenPlacingObjects);
            if(newBool != _settings.HideWireframeWhenPlacingObjects)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.HideWireframeWhenPlacingObjects = newBool;
            }
        }

        private GUIContent GetContentForHideWireframeWhenPlacingObjectsToggle()
        {
            var content = new GUIContent();
            content.text = "Hide wireframe when placing objects";
            content.tooltip = "If this is checked, the tool will hide the object wireframe when objects are placed in the scene.";

            return content;
        }

        private void RenderInheritRotationOnPrefabScrollToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForInheritRotationOnPrefabScrollToggle(), _settings.InheritRotationOnPrefabScroll);
            if(newBool != _settings.InheritRotationOnPrefabScroll)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.InheritRotationOnPrefabScroll = newBool;
            }
        }

        private GUIContent GetContentForInheritRotationOnPrefabScrollToggle()
        {
            var content = new GUIContent();
            content.text = "Inherit rotation on prefab scroll";
            content.tooltip = "If this is checked, prefabs will inheirt the rotation of the previous active prefab when cycling thorugh different " +
                              "prefabs in the active category using the mouse scroll wheel.";

            return content;
        }

        private void RenderOpenMoreSettingsWindowButton()
        {
            if(GUILayout.Button(GetContentForOpenMoreSettingsWindowButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.54f)))
            {
                ObjectPlacementSettingsWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForOpenMoreSettingsWindowButton()
        {
            var content = new GUIContent();
            content.text = "More settings...";
            content.tooltip = "Opens up a window which allows you to modify additional settings.";

            return content;
        }

        private void RenderOpenPrefabManagementWindowButton()
        {
            if (GUILayout.Button(GetContentForOpenPrefabManagementWindowButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.4f)))
            {
                PrefabManagementWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForOpenPrefabManagementWindowButton()
        {
            var content = new GUIContent();
            content.text = "Prefabs...";
            content.tooltip = "Opens up a window which allows you to manage prefabs and prefab categories.";

            return content;
        }

        private void RenderOpenPrefabTagsWindowButton()
        {
            if (GUILayout.Button(GetContentForOpenPrefabTagsWindowButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.45f)))
            {
                PrefabTagsWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForOpenPrefabTagsWindowButton()
        {
            var content = new GUIContent();
            content.text = "Prefab tags...";
            content.tooltip = "Opens up a window which allows you to manage prefab tags.";

            return content;
        }

        private void RenderOpenObjectLayersWindowButton()
        {
            if (GUILayout.Button(GetContentForOpenObjectLayersWindowButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.48f)))
            {
                ObjectLayersWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForOpenObjectLayersWindowButton()
        {
            var content = new GUIContent();
            content.text = "Object layers...";
            content.tooltip = "Opens up a window which allows you to perform object layer actions.";

            return content;
        }

        private void RenderMakePlacedObjectsChildrenOfHoveredObject()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForMakePlacedObjectsChildrenOfHoveredObject(), _settings.MakePlacedObjectsChildrenOfHoveredObject);
            if(newBool != _settings.MakePlacedObjectsChildrenOfHoveredObject)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MakePlacedObjectsChildrenOfHoveredObject = newBool;
            }
        }

        private GUIContent GetContentForMakePlacedObjectsChildrenOfHoveredObject()
        {
            var content = new GUIContent();
            content.text = "Attach to hovered object";
            content.tooltip = "If this is checked, any object that is placed in the scene will be made a child of the object which was hovered " +
                              "when the object was instantiated. Note: This option is ignored when \'Attach to object group\' is checked.";

            return content;
        }

        private void RenderAttachPlacedObjectsToActiveGroupToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAttachPlacedObjectsToActiveGroupToggle(), _settings.AttachPlacedObjectsToObjectGroup);
            if(newBool != _settings.AttachPlacedObjectsToObjectGroup)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AttachPlacedObjectsToObjectGroup = newBool;
            }
        }

        private GUIContent GetContentForAttachPlacedObjectsToActiveGroupToggle()
        {
            var content = new GUIContent();
            content.text = "Attach to object group";
            content.tooltip = "If this is checked, all placed objects will be attached to the currently active object group (if any). Note: If 'Use active " + 
                              "prefab category group' is checked, the group associated with the active prefab category will be used instead.";

            return content;
        }

        private void RenderUseActivePrefabCategoryGroupToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseActiveCategoryGroupToggle(), _settings.UseActivePrefabCategoryGroup);
            if (newBool != _settings.UseActivePrefabCategoryGroup)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseActivePrefabCategoryGroup = newBool;
            }
        }

        private GUIContent GetContentForUseActiveCategoryGroupToggle()
        {
            var content = new GUIContent();
            content.text = "Use active prefab category group";
            content.tooltip = "If this is checked, the tool will attach placed objects to the group that is associated with the active prefab category.";

            return content;
        }

        private void RenderDecorPaintPlacementControls()
        {
            _settings.DecorPaintObjectPlacementSettings.View.Render();
        }

        private void RenderPointAndClickPlacementControls()
        {
            _settings.PointAndClickPlacementSettings.View.Render();
        }

        private void RenderPathPlacementControls()
        {
            _settings.PathPlacementSettings.View.Render();

            EditorGUILayout.Separator();
            PathObjectPlacement.Get().PathSettings.View.Render();

            EditorGUILayout.Separator();
            if (PathObjectPlacement.Get().PathSettings.ManualConstructionSettings.HeightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticPattern)
                ObjectPlacementPathHeightPatternDatabase.Get().View.Render();
        }

        private void RenderBlockPlacementControls()
        {
            _settings.BlockObjectPlacementSettings.View.Render();

            EditorGUILayout.Separator();
            BlockObjectPlacement.Get().BlockSettings.View.Render();
        }
        #endregion
    }
}
#endif