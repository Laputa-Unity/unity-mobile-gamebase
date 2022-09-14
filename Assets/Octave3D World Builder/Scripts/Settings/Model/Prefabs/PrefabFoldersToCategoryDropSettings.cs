#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabFoldersToCategoryDropSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _createPrefabCategoriesFromDroppedFolders = true;
        [SerializeField]
        private bool _activateLastCreatedCategory = true;

        [SerializeField]
        private List<string> _tagNamesForDroppedPrefabFolders = new List<string>();

        [SerializeField]
        private bool _createPrefabTagsForEachDroppedFolder = true;

        [SerializeField]
        private bool _processSubfolders = true;

        [SerializeField]
        private PrefabFoldersToCategoryDropSettingsView _view;
        #endregion

        #region Public Properties
        public bool CreatePrefabCategoriesFromDroppedFolders { get { return _createPrefabCategoriesFromDroppedFolders; } set { _createPrefabCategoriesFromDroppedFolders = value; } }
        public bool ActivateLastCreatedCategory { get { return _activateLastCreatedCategory; } set { _activateLastCreatedCategory = value; } }
        public List<string> TagNamesForDroppedFolders { get { return _tagNamesForDroppedPrefabFolders; } set { if (value != null) _tagNamesForDroppedPrefabFolders = value; } }
        public bool CreatePrefabTagsForEachDroppedFolder { get { return _createPrefabTagsForEachDroppedFolder; } set { _createPrefabTagsForEachDroppedFolder = value; } }
        public bool ProcessSubfolders { get { return _processSubfolders; } set { _processSubfolders = value; } }
        public PrefabFoldersToCategoryDropSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabFoldersToCategoryDropSettings()
        {
            _view = new PrefabFoldersToCategoryDropSettingsView(this);
        }
        #endregion
    }
}
#endif