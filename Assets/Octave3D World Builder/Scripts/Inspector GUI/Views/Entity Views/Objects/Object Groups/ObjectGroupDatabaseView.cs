#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectGroupDatabaseView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectGroupDatabase _database;

        [SerializeField]
        private ObjectGroupDatabaseViewData _viewData;
        #endregion

        #region Private Properties
        private ObjectGroupDatabaseViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectGroupDatabaseViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Constructors
        public ObjectGroupDatabaseView(ObjectGroupDatabase database)
        {
            _database = database;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            if (_database.IsEmpty) EditorGUILayout.HelpBox("No object groups are currently available", UnityEditor.MessageType.None);
            else
            {
                EditorGUILayout.BeginHorizontal();
                var content = new GUIContent();
                content.text = "Replace";
                content.tooltip = "Replaces the current active group object with the one specified in the field to the right.";
                if (GUILayout.Button(content, GUILayout.Width(100.0f)))
                {
                    if (_database.ActiveGroup.GroupObject != ViewData.Replacement)
                    {
                        UndoEx.RecordForToolAction(_database);
                        _database.ActiveGroup.GroupObject = ViewData.Replacement;
                    }
                }

                GameObject newObject = EditorGUILayout.ObjectField("", ViewData.Replacement, typeof(GameObject), true) as GameObject;
                if(newObject != ViewData.Replacement)
                {
                    if (newObject.IsSceneObject())
                    {
                        UndoEx.RecordForToolAction(ViewData);
                        ViewData.Replacement = newObject;
                    }
                    else Debug.LogWarning("Only scene objects can act as object groups. Prefabs are not allowed.");
                }
                EditorGUILayout.EndHorizontal();

                RenderActiveGroupSelectionPopup();
                _database.ActiveGroup.View.Render();
            }

            RenderActionControls();
            RenderPreserveGroupChildrenToggle();
        }
        #endregion

        #region Private Methods
        private void RenderActiveGroupSelectionPopup()
        {
            int newInt = EditorGUILayoutEx.Popup(GetContentForActiveGroupSelectionPopup(), _database.IndexOfActiveGroup, _database.GetAllObjectGroupNames());
            if(newInt != _database.IndexOfActiveGroup)
            {
                UndoEx.RecordForToolAction(_database);
                _database.SetActiveObjectGroup(_database.GetObjectGroupByIndex(newInt));
            }
        }

        private GUIContent GetContentForActiveGroupSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Active group";
            content.tooltip = "Allows you to change the active object group.";

            return content;
        }

        private void RenderActionControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderCreateNewGroupButton();
            RenderCreateNewGroupNameChangeTextField();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            var content = new GUIContent();
            content.text = "Remove";
            content.tooltip = "Removes the active group. The associated object will remain in the scene but it will no longer be regarded as a group.";
            if (GUILayout.Button(content, GUILayout.Width(100.0f)))
            {
                ObjectGroup objectGroup = Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase.ActiveGroup;
                if (objectGroup != null && objectGroup.GroupObject != null)
                {
                    Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase.MakeNoLongerGroup(objectGroup.GroupObject);
                }
            }

            content.text = "Remove all";
            content.tooltip = "Removes all groups. The associated objects will remain in the scene but they will no longer be regarded as groups.";
            if (GUILayout.Button(content, GUILayout.Width(100.0f)))
            {
                ObjectGroup objectGroup = Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase.ActiveGroup;
                if (objectGroup != null && objectGroup.GroupObject != null)
                {
                    Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase.MakeAllNoLongerGroup();
                }
            }

            RenderDestroyActiveGroupButton();
            RenderDestroyAllGroupsButton();
            EditorGUILayout.EndHorizontal();

            if(_database.ActiveGroup != null)
            {
                EditorGUILayout.BeginHorizontal();
                content.text = "Select";
                content.tooltip = "Selects the active group.";
                if (GUILayout.Button(content, GUILayout.Width(100.0f)))
                {
                    ObjectGroup objectGroup = Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase.ActiveGroup;
                    UndoEx.RecordForToolAction(ObjectSelection.Get());
                    ObjectSelection.Get().Clear();
                    ObjectSelection.Get().AddGameObjectCollectionToSelection(objectGroup.GroupObject.GetAllChildren());
                    ObjectSelection.Get().ObjectSelectionGizmos.OnObjectSelectionUpdated();

                    SceneView.RepaintAll();
                }
                RenderMakeActiveGroupStaticButton();
                RenderMakeActiveGroupDynamicButton();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void RenderCreateNewGroupButton()
        {
            if (GUILayout.Button(GetContentForCreateNewGroupButton(), GUILayout.Width(100.0f)))
            {
                UndoEx.RecordForToolAction(_database);
                _database.CreateObjectGroup(ViewData.NameForNewGroup);
            }
        }

        private GUIContent GetContentForCreateNewGroupButton()
        {
            var content = new GUIContent();
            content.text = "Create group";
            content.tooltip = "Creates new object groups using the name specified in the adjacent text field.";

            return content;
        }

        private void RenderCreateNewGroupNameChangeTextField()
        {
            string newString = EditorGUILayout.TextField(ViewData.NameForNewGroup);
            if (newString != ViewData.NameForNewGroup)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.NameForNewGroup = newString;
            }
        }

        private void RenderDestroyActiveGroupButton()
        {
            if(GUILayout.Button(GetContentForDestroyActiveGroupButton(), GUILayout.Width(100.0f)))
            {
                UndoEx.RecordForToolAction(_database);
                _database.RemoveAndDestroyObjectGroup(_database.ActiveGroup);
            }
        }

        private GUIContent GetContentForDestroyActiveGroupButton()
        {
            var content = new GUIContent();
            content.text = "Destroy";
            content.tooltip = "Destroys the active group. Note: This will delete the group object from the scene.";

            return content;
        }

        private void RenderDestroyAllGroupsButton()
        {
            if(GUILayout.Button(GetContentForDestroyAllGroupsButton()))
            {
                UndoEx.RecordForToolAction(_database);
                _database.RemoveAndDestroyAllObjectGroups();
            }
        }

        private GUIContent GetContentForDestroyAllGroupsButton()
        {
            var content = new GUIContent();
            content.text = "Destroy all";
            content.tooltip = "Destroys all object groups. Note: This will delete the all group objects from the scene.";

            return content;
        }

        private void RenderPreserveGroupChildrenToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForPreserveGroupChildrenToggle(), _database.PreserveGroupChildren);
            if(newBool != _database.PreserveGroupChildren)
            {
                UndoEx.RecordForToolAction(_database);
                _database.PreserveGroupChildren = newBool;
            }
        }

        private GUIContent GetContentForPreserveGroupChildrenToggle()
        {
            var content = new GUIContent();
            content.text = "Preserve group children";
            content.tooltip = "If this is checked, when a group is deleted, its children will be moved one level up the hierarchy so that they don't get deleted together with the group's parent.";

            return content;
        }

        private void RenderMakeActiveGroupStaticButton()
        {
            if (GUILayout.Button(GetContentForMakeActiveGroupStaticButton(), GUILayout.Width(100.0f)))
            {
                List<GameObject> allObjectsInActivegroup = _database.ActiveGroup.GroupObject.GetAllChildrenIncludingSelf();
                UndoEx.RecordForToolAction(allObjectsInActivegroup);
                ObjectActions.MakeObjectsStatic(allObjectsInActivegroup);
            }
        }

        private GUIContent GetContentForMakeActiveGroupStaticButton()
        {
            var content = new GUIContent();
            content.text = "Make static";
            content.tooltip = "Marks the active group (and all its child objects) as static.";

            return content;
        }

        private void RenderMakeActiveGroupDynamicButton()
        {
            if (GUILayout.Button(GetContentForMakeActiveGroupDynamicButton()))
            {
                List<GameObject> allObjectsInActivegroup = _database.ActiveGroup.GroupObject.GetAllChildrenIncludingSelf();
                UndoEx.RecordForToolAction(allObjectsInActivegroup);
                ObjectActions.MakeObjectsDynamic(allObjectsInActivegroup);
            }
        }

        private GUIContent GetContentForMakeActiveGroupDynamicButton()
        {
            var content = new GUIContent();
            content.text = "Make dynamic";
            content.tooltip = "Marks the active group (and all its child objects) as dynamic.";

            return content;
        }
        #endregion
    }
}
#endif