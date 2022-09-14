#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectLayerDatabaseView : EntityView
    {
        #region Private Constant Variables
        private const float _actionButtonScale = 0.5f;
        #endregion

        #region Private Variables
        [NonSerialized]
        private ObjectLayerDatabase _database;
        #endregion

        #region Constructors
        public ObjectLayerDatabaseView(ObjectLayerDatabase objectLayerDatabase)
        {
            _database = objectLayerDatabase;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            EditorGUILayout.BeginVertical("Box");
            RenderActiveLayerSelectionPopup();
            RenderActionControlsForActiveLayer();
            EditorGUILayout.EndHorizontal();

            RenderAllLayersActionControls();
        }
        #endregion

        #region Private Methods
        private void RenderActiveLayerSelectionPopup()
        {
            int newInt = EditorGUILayout.LayerField(GetContentForActiveLayerSelectionPopup(), _database.ActiveLayer);
            if(newInt != _database.ActiveLayer)
            {
                UndoEx.RecordForToolAction(_database);
                _database.ActiveLayer = newInt;
            }
        }

        private GUIContent GetContentForActiveLayerSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Active layer";
            content.tooltip = "Allows you to choose the active layer. Any actions performed using the buttons below apply to this layer.";

            return content;
        }

        private void RenderActionControlsForActiveLayer()
        {
            EditorGUILayout.BeginHorizontal();
            RenderSelectActiveLayerButton();
            RenderDeselectActiveLayerButton();
            RenderShowActiveLayerButton();
            RenderHideActiveLayerButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderMakeActiveLayerStatic();
            RenderMakeActiveLayerDynamic();
            RenderEraseActiveLayerButton();
            RenderAssignSelectedObjectsToActiveLayerButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderSelectActiveLayerButton()
        {
            if (GUILayout.Button(GetContentForSelectActiveLayerButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get());
                ObjectSelectionActions.SelectAllObjectsInLayer(_database.ActiveLayer);
            }
        }

        private GUIContent GetContentForSelectActiveLayerButton()
        {
            var content = new GUIContent();
            content.text = "Select";
            content.tooltip = "Clears the current selection and selects only the objects which reside in the active layer.";

            return content;
        }

        private void RenderDeselectActiveLayerButton()
        {
            if (GUILayout.Button(GetContentForDeselectActiveLayerButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get());
                ObjectSelectionActions.DeselectAllObjectsInLayer(_database.ActiveLayer);
            }
        }

        private GUIContent GetContentForDeselectActiveLayerButton()
        {
            var content = new GUIContent();
            content.text = "Deselect";
            content.tooltip = "Deselects the objects which resisde in the active layer.";

            return content;
        }

        private void RenderShowActiveLayerButton()
        {
            if (GUILayout.Button(GetContentForShowActiveLayerButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                ObjectLayerDatabase.Get().ShowLayer(_database.ActiveLayer);
            }
        }

        private GUIContent GetContentForShowActiveLayerButton()
        {
            var content = new GUIContent();
            content.text = "Show";
            content.tooltip = "Shows all objects which reside in the active layer.";

            return content;
        }

        private void RenderHideActiveLayerButton()
        {
            if (GUILayout.Button(GetContentForHideActiveLayerButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                ObjectLayerDatabase.Get().HideLayer(_database.ActiveLayer);
            }
        }

        private GUIContent GetContentForHideActiveLayerButton()
        {
            var content = new GUIContent();
            content.text = "Hide";
            content.tooltip = "Hides all objects which reside in the active layer.";

            return content;
        }

        private void RenderMakeActiveLayerStatic()
        {
            if (GUILayout.Button(GetContentForMakeActiveLayerStatic(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                ObjectLayerDatabase.Get().MakeLayerStatic(_database.ActiveLayer);
            }
        }

        private GUIContent GetContentForMakeActiveLayerStatic()
        {
            var content = new GUIContent();
            content.text = "Make static";
            content.tooltip = "Marks all objects in the active layer as static.";

            return content;
        }

        private void RenderMakeActiveLayerDynamic()
        {
            if (GUILayout.Button(GetContentForMakeActiveLayerDynamic(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                ObjectLayerDatabase.Get().MakeLayerDynamic(_database.ActiveLayer);
            }
        }

        private GUIContent GetContentForMakeActiveLayerDynamic()
        {
            var content = new GUIContent();
            content.text = "Make dynamic";
            content.tooltip = "Marks all objects in the active layer as dynamic.";

            return content;
        }

        private void RenderEraseActiveLayerButton()
        {
            if (GUILayout.Button(GetContentForEraseActiveLayerButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get());
                ObjectActions.EraseAllGameObjectsInLayer(_database.ActiveLayer);
            }
        }

        private GUIContent GetContentForEraseActiveLayerButton()
        {
            var content = new GUIContent();
            content.text = "Erase";
            content.tooltip = "Erases all objects which reside in the active layer.";

            return content;
        }

        private void RenderAssignSelectedObjectsToActiveLayerButton()
        {
            if (GUILayout.Button(GetContentForAssignSelectedObjectsToActiveLayerButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                ObjectLayerDatabase.Get().AssignObjectsToLayer(ObjectSelection.Get().GetAllSelectedGameObjects(), _database.ActiveLayer);
            }
        }

        private GUIContent GetContentForAssignSelectedObjectsToActiveLayerButton()
        {
            var content = new GUIContent();
            content.text = "Assign selection";
            content.tooltip = "Assigns the currently selected objects to active layer.";

            return content;
        }

        private void RenderAllLayersActionControls()
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();
            RenderSelectAllLayersButton();
            RenderDeselectAllLayersButton();
            RenderShowAllLayersButton();
            RenderHideAllLayersButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderMakeAllLayersStaticButton();
            RenderMakeAllLayersDynamicButton();
            RenderEraseAllLayersButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }


        private void RenderSelectAllLayersButton()
        {
            if (GUILayout.Button(GetContentForSelectAllLayersButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get());
                ObjectSelectionActions.SelectAllObjectsInAllLayers();
            }
        }

        private GUIContent GetContentForSelectAllLayersButton()
        {
            var content = new GUIContent();
            content.text = "Select all";
            content.tooltip = "Clears the current selection and selects all objects in all layers.";

            return content;
        }

        private void RenderDeselectAllLayersButton()
        {
            if (GUILayout.Button(GetContentForDeselectAllLayersButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get());
                ObjectSelectionActions.DeselectAllObjectsInAllLayers();
            }
        }

        private GUIContent GetContentForDeselectAllLayersButton()
        {
            var content = new GUIContent();
            content.text = "Deselect all";
            content.tooltip = "Deselects all objects in all layers.";

            return content;
        }

        private void RenderEraseAllLayersButton()
        {
            if (GUILayout.Button(GetContentForEraseAllLayersButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get());
                ObjectActions.EraseGameObjectsInAllLayers();
            }
        }

        private GUIContent GetContentForEraseAllLayersButton()
        {
            var content = new GUIContent();
            content.text = "Erase all";
            content.tooltip = "Erases all objects which reside in all layers.";

            return content;
        }

        private void RenderShowAllLayersButton()
        {
            if (GUILayout.Button(GetContentForShowAllLayersButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                ObjectLayerDatabase.Get().ShowAllLayers();
            }
        }

        private GUIContent GetContentForShowAllLayersButton()
        {
            var content = new GUIContent();
            content.text = "Show all";
            content.tooltip = "Shows all objects in all layers.";

            return content;
        }

        private void RenderHideAllLayersButton()
        {
            if (GUILayout.Button(GetContentForHideAllLayersButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                ObjectLayerDatabase.Get().HideAllLayers();
            }
        }

        private GUIContent GetContentForHideAllLayersButton()
        {
            var content = new GUIContent();
            content.text = "Hide all";
            content.tooltip = "Hide all objects in all layers.";

            return content;
        }

        private void RenderMakeAllLayersStaticButton()
        {
            if (GUILayout.Button(GetContentForMakeAllLayersStaticButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                ObjectLayerDatabase.Get().MakeAllLayersStatic();
            }
        }

        private GUIContent GetContentForMakeAllLayersStaticButton()
        {
            var content = new GUIContent();
            content.text = "Make all static";
            content.tooltip = "Marks all objects in all layers as static.";

            return content;
        }

        private void RenderMakeAllLayersDynamicButton()
        {
            if (GUILayout.Button(GetContentForMakeAllLayersDynamicButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                ObjectLayerDatabase.Get().MakeAllLayersDynamic();
            }
        }

        private GUIContent GetContentForMakeAllLayersDynamicButton()
        {
            var content = new GUIContent();
            content.text = "Make all dynamic";
            content.tooltip = "Marks all objects in all layers as dynamic.";

            return content;
        }       
        #endregion
    }
}
#endif