#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabToCategoryDropSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabsToCategoryDropSettings _settings;

        [SerializeField]
        private PrefabTagSelectionView _tagSelectionForDroppedPrefabs = new PrefabTagSelectionView();
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
        public PrefabToCategoryDropSettingsView(PrefabsToCategoryDropSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Prefabs Drop Settings";
            SurroundWithBox = true;

            _tagSelectionForDroppedPrefabs.VisibilityToggleLabel = "Tags For Dropped Prefabs";
            _tagSelectionForDroppedPrefabs.ToggleVisibilityBeforeRender = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderDroppedPrefabsTagSelectionControls();
        }
        #endregion

        #region Private Methods   
        private void RenderDroppedPrefabsTagSelectionControls()
        {
            _tagSelectionForDroppedPrefabs.PrefabTagFilter = PrefabTagFilterForTagSelection;
            _tagSelectionForDroppedPrefabs.Render();

            if (_tagSelectionForDroppedPrefabs.HasSelectionChanged)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.TagNamesForDroppedPrefabs = _tagSelectionForDroppedPrefabs.ListOfSelectedTagNames;
            }
        }
        #endregion
    }
}
#endif