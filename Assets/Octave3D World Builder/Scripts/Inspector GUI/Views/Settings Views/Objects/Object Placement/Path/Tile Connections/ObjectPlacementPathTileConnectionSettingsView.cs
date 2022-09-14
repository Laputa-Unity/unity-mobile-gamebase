#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathTileConnectionSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathTileConnectionSettings _settings;
        [NonSerialized]
        private float _setCommonTilePropertyButtonWidth = EditorGUILayoutEx.PreferedActionButtonWidth * 1.23f;
        #endregion

        #region Constructors
        public ObjectPlacementPathTileConnectionSettingsView(ObjectPlacementPathTileConnectionSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderUseTileConnectionsToggle();
            if(_settings.UseTileConnections)
            {
                Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("When using tile connections, the extension plane will always reside at the bottom of the placement guide in its local space.");
                RenderSetCommonPropertiesControls();
                RenderRemoveAllPrefabAssociationsButton();

                EditorGUILayout.Separator();
                PrefabsToPathTileConectionDropEventHandler.Get().DropSettings.View.Render();
                RenderViewForEachTileConnectionType();
            }
        }
        #endregion

        #region Private Methods
        private void RenderUseTileConnectionsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseTileConnectionsToggle(), _settings.UseTileConnections);
            if(newBool != _settings.UseTileConnections)
            {
                if (!ObjectPlacementPathTileConnectionSettingsChangeValidation.Validate(true)) return;

                UndoEx.RecordForToolAction(_settings);
                _settings.UseTileConnections = newBool;
            }
        }

        private GUIContent GetContentForUseTileConnectionsToggle()
        {
            var content = new GUIContent();
            content.text = "Use tile connections";
            content.tooltip = "Check this when you would like to construct paths that use tile connections.";

            return content;
        }

        private void RenderSetCommonPropertiesControls()
        {
            RenderCommonYAxisRotationControls();
            RenderCommonYOffsetControls();
            RenderSetCommonExtrusionAmountControls();
            RenderInheritPrefabControls();
        }

        private void RenderCommonYAxisRotationControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderSetCommonYAxisRotationButton();
            RenderCommonYAxisRotationSelectionPopup();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderSetCommonYAxisRotationButton()
        {
            if (GUILayout.Button(GetContentForSetCommonYAxisRotationButton(), GUILayout.Width(_setCommonTilePropertyButtonWidth)))
            {
                List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
                foreach(ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
                {
                    UndoEx.RecordForToolAction(_settings.GetSettingsForTileConnectionType(tileConnectionType));
                    _settings.GetSettingsForTileConnectionType(tileConnectionType).YAxisRotation = _settings.CommonYAxisRotation;
                }
            }
        }

        private GUIContent GetContentForSetCommonYAxisRotationButton()
        {
            var content = new GUIContent();
            content.text = "Set Y rotation for all tiles";
            content.tooltip = "Pressing this button will set the Y axis rotation for all tile connections to the value specified in the adjacent popup.";

            return content;
        }

        private void RenderCommonYAxisRotationSelectionPopup()
        {
            ObjectPlacementPathTileConnectionYAxisRotation newYAxisRotation = (ObjectPlacementPathTileConnectionYAxisRotation)EditorGUILayout.EnumPopup(GetContentForCommonYAxisRotationSelectionPopup(), _settings.CommonYAxisRotation);
            if(newYAxisRotation != _settings.CommonYAxisRotation)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CommonYAxisRotation = newYAxisRotation;
            }
        }

        private GUIContent GetContentForCommonYAxisRotationSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "This is the common Y axis rotation that can be set for all tile connections by pressing the adjacent button.";

            return content;
        }

        private void RenderCommonYOffsetControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderSetCommonYOffsetButton();
            RenderCommonYOffsetField();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderSetCommonYOffsetButton()
        {
            if (GUILayout.Button(GetContentForSetCommonYOffsetButton(), GUILayout.Width(_setCommonTilePropertyButtonWidth)))
            {
                List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
                foreach (ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
                {
                    UndoEx.RecordForToolAction(_settings.GetSettingsForTileConnectionType(tileConnectionType));
                    _settings.GetSettingsForTileConnectionType(tileConnectionType).YOffset = _settings.CommonYOffset;
                }

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForSetCommonYOffsetButton()
        {
            var content = new GUIContent();
            content.text = "Set Y offset for all tiles";
            content.tooltip = "Pressing this button will set the Y offset for all tile connections to the value specified in the adjacent popup.";

            return content;
        }

        private void RenderCommonYOffsetField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForCommonYOffsetField(), _settings.CommonYOffset);
            if(newFloat != _settings.CommonYOffset)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CommonYOffset = newFloat;
            }
        }

        private GUIContent GetContentForCommonYOffsetField()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "This is the common Y offset that can be set for all tile connections by pressing the adjacent button.";

            return content;
        }

        private void RenderViewForEachTileConnectionType()
        {
            List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
            for(int tileConnectionIndex = 0; tileConnectionIndex < allTileConnectionTypes.Count; ++tileConnectionIndex)
            {
                _settings.GetSettingsForTileConnectionType(allTileConnectionTypes[tileConnectionIndex]).View.Render();
            }
        }

        private void RenderSetCommonExtrusionAmountControls()
        {
            RenderSetCommonUpwardsExtrusionAmountControls();
            RenderSetCommonDownwardsExtrusionAmountControls();
        }

        private void RenderSetCommonUpwardsExtrusionAmountControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderSetCommonUpwardsExtrusionAmountButton();
            RenderCommonUpwardsExtrusionAmountField();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderSetCommonUpwardsExtrusionAmountButton()
        {
            if (GUILayout.Button(GetContentForSetCommonUpwardsExtrusionAmountButton(), GUILayout.Width(_setCommonTilePropertyButtonWidth)))
            {
                List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
                foreach (ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
                {
                    UndoEx.RecordForToolAction(_settings.GetSettingsForTileConnectionType(tileConnectionType));
                    _settings.GetSettingsForTileConnectionType(tileConnectionType).UpwardsExtrusionAmount = _settings.CommonUpwardsExtrusionAmount;
                }

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForSetCommonUpwardsExtrusionAmountButton()
        {
            var content = new GUIContent();
            content.text = "Set upwards extrusion amount for all tiles";
            content.tooltip = "Pressing this button will set the upwards extrusion amount for all tile connections to the value specified in the adjacent field.";

            return content;
        }

        private void RenderCommonUpwardsExtrusionAmountField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForCommonUpwardsExtrusionAmountField(), _settings.CommonUpwardsExtrusionAmount);
            if(newInt != _settings.CommonUpwardsExtrusionAmount)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CommonUpwardsExtrusionAmount = newInt;
            }
        }

        private GUIContent GetContentForCommonUpwardsExtrusionAmountField()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "This is the common upwards extrusion amount that can be set for all tile connections by pressing the adjacent button.";

            return content;
        }

        private void RenderSetCommonDownwardsExtrusionAmountControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderSetCommonDownwardsExtrusionAmountButton();
            RenderCommonDownwardsExtrusionAmountField();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderSetCommonDownwardsExtrusionAmountButton()
        {
            if (GUILayout.Button(GetContentForSetCommonDownwardsExtrusionAmountButton(), GUILayout.Width(_setCommonTilePropertyButtonWidth)))
            {
                List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
                foreach (ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
                {
                    UndoEx.RecordForToolAction(_settings.GetSettingsForTileConnectionType(tileConnectionType));
                    _settings.GetSettingsForTileConnectionType(tileConnectionType).DownwardsExtrusionAmount = _settings.CommonDownwardsExtrusionAmount;
                }

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForSetCommonDownwardsExtrusionAmountButton()
        {
            var content = new GUIContent();
            content.text = "Set downwards extrusion amount for all tiles";
            content.tooltip = "Pressing this button will set the downwards extrusion amount for all tile connections to the value specified in the adjacent field.";

            return content;
        }

        private void RenderCommonDownwardsExtrusionAmountField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForCommonDownwardsExtrusionAmountField(), _settings.CommonDownwardsExtrusionAmount);
            if (newInt != _settings.CommonDownwardsExtrusionAmount)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CommonDownwardsExtrusionAmount = newInt;
            }
        }

        private GUIContent GetContentForCommonDownwardsExtrusionAmountField()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "";

            return content;
        }

        private void RenderInheritPrefabControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderInheritPrefabButton();
            RenderPrefabInheritTileConnectionType();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderInheritPrefabButton()
        {
            if (GUILayout.Button(GetContentForInheritPrefabButton(), GUILayout.Width(_setCommonTilePropertyButtonWidth)))
            {
                if (!ObjectPlacementPathTileConnectionSettingsChangeValidation.Validate(true)) return;

                Prefab prefabToInherit = _settings.GetSettingsForTileConnectionType(_settings.PrefabInheritTileConnectionType).Prefab;
                List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
                foreach (ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
                {
                    UndoEx.RecordForToolAction(_settings.GetSettingsForTileConnectionType(tileConnectionType));
                    _settings.GetSettingsForTileConnectionType(tileConnectionType).Prefab = prefabToInherit;
                }
            }
        }

        private GUIContent GetContentForInheritPrefabButton()
        {
            var content = new GUIContent();
            content.text = "Inherit prefab from tile connection";
            content.tooltip = "Pressing this button will adjust the prefab of all tile connections to be the same as the one associated with the tile specified in the adjacent popup.";

            return content;
        }

        private void RenderPrefabInheritTileConnectionType()
        {
            ObjectPlacementPathTileConnectionType newTileConnectionType = (ObjectPlacementPathTileConnectionType)EditorGUILayout.EnumPopup(GetContentForPrefabInheritTileConnectionTypeSelectionPopup(), _settings.PrefabInheritTileConnectionType);
            if(newTileConnectionType != _settings.PrefabInheritTileConnectionType)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PrefabInheritTileConnectionType = newTileConnectionType;
            }
        }

        private GUIContent GetContentForPrefabInheritTileConnectionTypeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "";

            return content;
        }

        private void RenderRemoveAllPrefabAssociationsButton()
        {
            if (GUILayout.Button(GetContentForRemoveAllPrefabAssociationsButton(), GUILayout.Width(_setCommonTilePropertyButtonWidth)))
            {
                if (!ObjectPlacementPathTileConnectionSettingsChangeValidation.Validate(true)) return;

                _settings.RecordAllTileConnectionTypeSettingsForUndo();
                _settings.RemoveAllPrefabAssociations();
            }
        }

        private GUIContent GetContentForRemoveAllPrefabAssociationsButton()
        {
            var content = new GUIContent();
            content.text = "Remove all prefab associations";
            content.tooltip = "Pressing this button will remove the prefab associations for all tile connections.";

            return content;
        }
        #endregion
    }
}
#endif