#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabFoldersToCategoryDropSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabFoldersToCategoryDropSettings _settings;

        [SerializeField]
        private PrefabTagSelectionView _tagSelectionForDroppedPrefabFolders = new PrefabTagSelectionView();
        [SerializeField]
        private PrefabTagFilter _prefabTagFilterForTagSelection;
        #endregion

        #region Private Properties
        private PrefabTagFilter PrefabTagFilterForTagSelection
        {
            get
            {
                if (_prefabTagFilterForTagSelection == null) _prefabTagFilterForTagSelection = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabTagFilter>();
                return _prefabTagFilterForTagSelection;
            }
        }
        #endregion

        #region Constructors
        public PrefabFoldersToCategoryDropSettingsView(PrefabFoldersToCategoryDropSettings settings)
        {
            _settings = settings;

            SurroundWithBox = true;
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Prefab Folders Drop Settings";

            _tagSelectionForDroppedPrefabFolders.VisibilityToggleLabel = "Tags For Dropped Prefab Folders";
            _tagSelectionForDroppedPrefabFolders.ToggleVisibilityBeforeRender = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderCreateCategoriesFromDroppedFoldersToggle();
            if (_settings.CreatePrefabCategoriesFromDroppedFolders) RenderActivateLastCreatedCategoryToggle();

            RenderProcessSubfoldersToggle();
            RenderCreatePrefabTagsForEachDroppedFolderToggle();

            if (!_settings.CreatePrefabTagsForEachDroppedFolder) RenderDroppedPrefabFoldersTagSelectionControls();
        }
        #endregion

        #region Private Methods
        private void RenderCreateCategoriesFromDroppedFoldersToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForCreateCategoriesFromDroppedFoldersToggle(), _settings.CreatePrefabCategoriesFromDroppedFolders);
            if (newBool != _settings.CreatePrefabCategoriesFromDroppedFolders)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CreatePrefabCategoriesFromDroppedFolders = newBool;
            }
        }

        private GUIContent GetContentForCreateCategoriesFromDroppedFoldersToggle()
        {
            var content = new GUIContent();
            content.text = "Create categories from dropped folders";
            content.tooltip = "If this button is checked, the tool will automatically create prefab categories from any dropped prefab folders.";

            return content;
        }

        private void RenderActivateLastCreatedCategoryToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForActivateLastCreatedCategoryToggle(), _settings.ActivateLastCreatedCategory);
            if(newBool != _settings.ActivateLastCreatedCategory)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ActivateLastCreatedCategory = newBool;
            }
        }

        private GUIContent GetContentForActivateLastCreatedCategoryToggle()
        {
            var content = new GUIContent();
            content.text = "Activate last created category";
            content.tooltip = "This is only taken into account when \'Create categories from dropped folders\' is checked. If this toggle is checked, " + 
                              "the last created category will be set as the active prefab category.";

            return content;
        }

        private void RenderProcessSubfoldersToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForProcessSubfoldersToggle(), _settings.ProcessSubfolders);
            if(newBool != _settings.ProcessSubfolders)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ProcessSubfolders = newBool;
            }
        }

        private GUIContent GetContentForProcessSubfoldersToggle()
        {
            var content = new GUIContent();
            content.text = "Process subfolders";
            content.tooltip = "If this is checked, the tool will take into account any subfolders which exist inside the dropped prefab folders. When \'Create categories from dropped folders\' is checked, " + 
                              "a category will be created for each subfolder that contains prefabs. If subfolders are not processed, a category will be created for the top-most folder. The same rules apply for prefab tags when " +
                              "\'Create prefab tags for each dropped folder\' is checked.";

            return content;
        }

        private void RenderCreatePrefabTagsForEachDroppedFolderToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForCreatePrefabTagsForEachDroppedFolderToggle(), _settings.CreatePrefabTagsForEachDroppedFolder);
            if(newBool != _settings.CreatePrefabTagsForEachDroppedFolder)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CreatePrefabTagsForEachDroppedFolder = newBool;
            }
        }

        private GUIContent GetContentForCreatePrefabTagsForEachDroppedFolderToggle()
        {
            var content = new GUIContent();
            content.text = "Create prefab tags for each dropped folder";
            content.tooltip = "If this is checked, a tag will be created for each dropped folder and the prefabs which reside in those " +
                              "folders will be associated with those tags.";

            return content;
        }

        private void RenderDroppedPrefabFoldersTagSelectionControls()
        {
            _tagSelectionForDroppedPrefabFolders.PrefabTagFilter = PrefabTagFilterForTagSelection;
            _tagSelectionForDroppedPrefabFolders.Render();

            if (_tagSelectionForDroppedPrefabFolders.HasSelectionChanged)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.TagNamesForDroppedFolders = _tagSelectionForDroppedPrefabFolders.ListOfSelectedTagNames;
            }
        }
        #endregion
    }
}
#endif