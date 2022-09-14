#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagDatabaseView : EntityView
    {
        private static readonly float _buttonWidth = 100.0f;

        #region Private Variables
        [NonSerialized]
        private PrefabTagDatabase _prefabTagDatabase;
        [NonSerialized]
        private List<PrefabTag> _filteredTags;

        [SerializeField]
        private PrefabTagDatabaseViewData _viewData;
        #endregion

        #region Private Properties
        private PrefabTagDatabaseViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabTagDatabaseViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Constructors
        public PrefabTagDatabaseView(PrefabTagDatabase prefabTagDatabase)
        {
            _prefabTagDatabase = prefabTagDatabase;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Prefab Tags";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            if (!PrefabTagDatabase.Get().IsEmpty)
            {
                _prefabTagDatabase.PrefabTagFilter.View.Render();
                RenderPrefabTagScrollView();
            }
            else EditorGUILayout.HelpBox("There are no prefab tags to display.", UnityEditor.MessageType.None);

            RenderActionControls();
        }
        #endregion

        #region Private Methods
        private void RenderPrefabTagScrollView()
        {
            ViewData.TagScrollPosition = EditorGUILayout.BeginScrollView(ViewData.TagScrollPosition, GetStyleForTagScrollView());
            RenderViewsForFilteredTags();
            EditorGUILayout.EndScrollView();
        }

        private GUIStyle GetStyleForTagScrollView()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private void RenderViewsForFilteredTags()
        {
            _filteredTags = _prefabTagDatabase.GetFilteredPrefabTags();
            foreach (PrefabTag prefabTag in _filteredTags)
            {
                prefabTag.View.AllowTagNameChange = ViewData.AllowTagNameChange;
                prefabTag.View.Render();
            }
        }

        private void RenderActionControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderCreateNewTagButton();
            RenderCreateNewTagNameField();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderActivateAllTagsButton();
            RenderDeactivateAllTagsButton();
            RenderActivateOnlyFilteredTagsButton();
            RenderDeactivateFilteredTagsButtons();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderRemoveAllTagsButton();
            RenderAllowTagNameChangeToggle();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderCreateNewTagButton()
        {
            if (GUILayout.Button(GetContentForCreateNewTagButton(), GUILayout.Width(_buttonWidth)))
            {
                UndoEx.RecordForToolAction(_prefabTagDatabase);
                _prefabTagDatabase.CreatePrefabTag(ViewData.NameForNewTag);
            }
        }

        private GUIContent GetContentForCreateNewTagButton()
        {
            var content = new GUIContent();
            content.text = "Create tag";
            content.tooltip = "Creates a new prefab tag using the name specified in the adjacent text field. " +
                              "Note: Names are automatically adjusted such that each tag name is unique.";

            return content;
        }

        private void RenderCreateNewTagNameField()
        {
            string newString = EditorGUILayout.TextField(GetContentForNewTagNameField(), ViewData.NameForNewTag);
            if (newString != ViewData.NameForNewTag)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.NameForNewTag = newString;
            }
        }

        private GUIContent GetContentForNewTagNameField()
        {
            var content = new GUIContent();
            content.tooltip = "Allows you to specify a name that must be used when creating a new prefab tag.";

            return content;
        }

        private void RenderActivateAllTagsButton()
        {
            if (GUILayout.Button(GetContentForActivateAllTagsButton(), GUILayout.Width(_buttonWidth)))
            {
                UndoEx.RecordForToolAction(_prefabTagDatabase.GetAllPrefabTags());
                _prefabTagDatabase.ActivateAllTags();
            }
        }

        private GUIContent GetContentForActivateAllTagsButton()
        {
            var content = new GUIContent();
            content.text = "Activate all";
            content.tooltip = "Activates all tags.";

            return content;
        }

        private void RenderDeactivateAllTagsButton()
        {
            if (GUILayout.Button(GetContentForDeactivateAllTagsButton(), GUILayout.Width(_buttonWidth)))
            {
                UndoEx.RecordForToolAction(_prefabTagDatabase.GetAllPrefabTags());
                _prefabTagDatabase.DeactivateAllTags();
            }
        }

        private GUIContent GetContentForDeactivateAllTagsButton()
        {
            var content = new GUIContent();
            content.text = "Deactivate all";
            content.tooltip = "Deactivates all tags.";

            return content;
        }

        private void RenderActivateOnlyFilteredTagsButton()
        {
            if (GUILayout.Button(GetContentForActivateOnlyFilteredTagsButton(), GUILayout.Width(_buttonWidth + 40.0f)))
            {
                UndoEx.RecordForToolAction(_prefabTagDatabase);
                ActivateOnlyFilteredTags();
            }
        }

        private void ActivateOnlyFilteredTags()
        {
            if (_filteredTags.Count != 0)
            {
                _prefabTagDatabase.DeactivateAllTags();
                PrefabTagActions.ActivatePrefabTags(_filteredTags);
            }
        }

        private GUIContent GetContentForActivateOnlyFilteredTagsButton()
        {
            var content = new GUIContent();
            content.text = "Activate only filtered";
            content.tooltip = "Activates only the filtered tags.";

            return content;
        }

        private void RenderDeactivateFilteredTagsButtons()
        {
            if (GUILayout.Button(GetContentForDeactivateOnlyFilteredTagsButton(), GUILayout.Width(_buttonWidth + 40.0f)))
            {
                UndoEx.RecordForToolAction(_prefabTagDatabase);
                PrefabTagActions.DeactivatePrefabTags(_filteredTags);
            }
        }

        private GUIContent GetContentForDeactivateOnlyFilteredTagsButton()
        {
            var content = new GUIContent();
            content.text = "Deactivate filtered";
            content.tooltip = "Deactivates the filtered tags.";

            return content;
        }

        private void RenderRemoveAllTagsButton()
        {
            if (GUILayout.Button(GetContentForRemoveAllTagsButton(), GUILayout.Width(_buttonWidth)))
            {
                UndoEx.RecordForToolAction(_prefabTagDatabase);
                _prefabTagDatabase.RemoveAndDestroyAllPrefabTags();
            }
        }

        private GUIContent GetContentForRemoveAllTagsButton()
        {
            var content = new GUIContent();
            content.text = "Remove all";
            content.tooltip = "Removes all tags.";

            return content;
        }

        private void RenderAllowTagNameChangeToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowTagNameChangeToggle(), ViewData.AllowTagNameChange);
            if (newBool != ViewData.AllowTagNameChange)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.AllowTagNameChange = newBool;
            }
        }

        private GUIContent GetContentForAllowTagNameChangeToggle()
        {
            var content = new GUIContent();
            content.text = "Allow tag name change";
            content.tooltip = "You can check this if you wish to change the names of the tags.";

            return content;
        }
        #endregion
    }
}
#endif