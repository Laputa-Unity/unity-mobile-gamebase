#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionActionsView : ActionsView
    {
        #region Private Variables
        [SerializeField]
        private ObjectSelectionActionsViewData _viewData;
        #endregion

        #region Private Properties
        private ObjectSelectionActionsViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectSelectionActionsViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            EditorGUILayoutEx.BeginVerticalBox();

            EditorGUILayout.BeginHorizontal();
            const float alignButtonWidth = 72.0f;
            var content = new GUIContent();
            content.text = "Align X";
            content.tooltip = "Aligns the positions of the selected objects to the X axis.";
            if (GUILayout.Button(content, GUILayout.Width(alignButtonWidth)))
            {
                ObjectSelectionActions.AlignSelectionToAxis(Axis.X);
            }

            content.text = "Align Y";
            content.tooltip = "Aligns the positions of the selected objects to the Y axis.";
            if (GUILayout.Button(content, GUILayout.Width(alignButtonWidth)))
            {
                ObjectSelectionActions.AlignSelectionToAxis(Axis.Y);
            }

            content.text = "Align Z";
            content.tooltip = "Aligns the positions of the selected objects to the Z axis.";
            if (GUILayout.Button(content, GUILayout.Width(alignButtonWidth)))
            {
                ObjectSelectionActions.AlignSelectionToAxis(Axis.Z);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderMakeSelectionStaticButton();
            RenderMakeSelectionDynamicButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderInvertSelectionButton();
            content.text = "Grab settings...";
            content.tooltip = "Opens up a new window which allows you to modify selection grab settings.";
            if (GUILayout.Button(content, GUILayout.Width(110.0f)))
            {
                Octave3DWorldBuilder.ActiveInstance.EditorWindowPool.SelectionGrabSettingsWindow.ObjectGrabSettings = ObjectSelection.Get().SelectionGrabSettings;
                Octave3DWorldBuilder.ActiveInstance.EditorWindowPool.SelectionGrabSettingsWindow.ShowOctave3DWindow();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderAssignSelectionToLayerButton();
            RenderSelectionAssignmentLayerSelectionPopup();
            EditorGUILayout.EndHorizontal();

            if(ObjectGroupDatabase.Get().NumberOfGroups != 0)
            {
                if (string.IsNullOrEmpty(ViewData.DestObjectGroupName))
                    ViewData.DestObjectGroupName = ObjectGroupDatabase.Get().GetObjectGroupByIndex(0).Name;
                else
                {
                    if(ObjectGroupDatabase.Get().GetObjectGroupByName(ViewData.DestObjectGroupName) == null)
                        ViewData.DestObjectGroupName = ObjectGroupDatabase.Get().GetObjectGroupByIndex(0).Name;
                }

                EditorGUILayout.BeginHorizontal();
                content.text = "Assign to group";
                content.tooltip = "Assigns the object selection to the specified object group.";
                if (GUILayout.Button(content, GUILayout.Width(110.0f)))
                {
                    ObjectGroup destObjectGroup = ObjectGroupDatabase.Get().GetObjectGroupByName(ViewData.DestObjectGroupName);
                    if (destObjectGroup != null) ObjectActions.AssignObjectsToGroup(ObjectSelection.Get().GetAllSelectedGameObjects(), destObjectGroup);
                }

                string newGroupName = EditorGUILayoutEx.Popup(new GUIContent(), ViewData.DestObjectGroupName, ObjectGroupDatabase.Get().GetAllObjectGroupNames());
                if (newGroupName != ViewData.DestObjectGroupName)
                {
                    UndoEx.RecordForToolAction(ViewData);
                    ViewData.DestObjectGroupName = newGroupName;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayoutEx.EndVerticalBox();
        }
        #endregion

        #region Private Methods
        private void RenderAssignSelectionToLayerButton()
        {
            if (GUILayout.Button(GetContentForAssignSelectionToLayerButton(), GUILayout.Width(110.0f)))
            {
                ObjectSelectionActions.AssignSelectedObjectsToLayer(ViewData.SelectionAssignmentLayer);
            }
        }

        private GUIContent GetContentForAssignSelectionToLayerButton()
        {
            var content = new GUIContent();
            content.text = "Assign to layer";
            content.tooltip = "Assigns the current object selection to the layer specified in the adjacent popup.";

            return content;
        }

        private void RenderSelectionAssignmentLayerSelectionPopup()
        {
            int newInt = EditorGUILayout.LayerField(GetContentForSelectionAssignmentLayerSelectionPopup(), ViewData.SelectionAssignmentLayer);
            if(newInt != ViewData.SelectionAssignmentLayer)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.SelectionAssignmentLayer = newInt;
            }
        }

        private GUIContent GetContentForSelectionAssignmentLayerSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "";

            return content;
        }

        private void RenderMakeSelectionStaticButton()
        {
            if (GUILayout.Button(GetContentForMakeSelectionStaticButton(), GUILayout.Width(110.0f)))
            {
                List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
                UndoEx.RecordForToolAction(allSelectedObjects);
                ObjectActions.MakeObjectsStatic(allSelectedObjects);
            }
        }

        private GUIContent GetContentForMakeSelectionStaticButton()
        {
            var content = new GUIContent();
            content.text = "Make static";
            content.tooltip = "Pressing this button will mark all selected objects as static.";

            return content;
        }

        private void RenderMakeSelectionDynamicButton()
        {
            if (GUILayout.Button(GetContentForMakeSelectionDynamicButton(), GUILayout.Width(110.0f)))
            {
                List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
                UndoEx.RecordForToolAction(allSelectedObjects);
                ObjectActions.MakeObjectsDynamic(allSelectedObjects);
            }
        }

        private GUIContent GetContentForMakeSelectionDynamicButton()
        {
            var content = new GUIContent();
            content.text = "Make dynamic";
            content.tooltip = "Pressing this button will mark all selected objects as dynamic.";

            return content;
        }

        private void RenderInvertSelectionButton()
        {
            if (GUILayout.Button(GetContentForInvertSelection(), GUILayout.Width(110.0f)))
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get());
                ObjectSelectionActions.InvertSelection();
            }
        }

        private GUIContent GetContentForInvertSelection()
        {
            var content = new GUIContent();
            content.text = "Invert selection";
            content.tooltip = "Pressing this button will deselect all currently selected objects and select only the ones which are not currently selected.";

            return content;
        }
        #endregion
    }
}
#endif