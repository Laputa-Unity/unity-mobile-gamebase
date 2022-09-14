#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagAssociationsView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabTagAssociations _tagAssociations;

        [SerializeField]
        private PrefabTagSelectionView _prefabTagSelectionView = new PrefabTagSelectionView();
        #endregion

        #region Constructors
        public PrefabTagAssociationsView(PrefabTagAssociations tagAssociation)
        {
            _tagAssociations = tagAssociation;

            VisibilityToggleLabel = "Associated Tags";
            ToggleVisibilityBeforeRender = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _prefabTagSelectionView.PrefabTagFilter = _tagAssociations.PrefabTagFilter;
            _prefabTagSelectionView.ListOfSelectedTagNames = _tagAssociations.GetAllAssociatedTagNames();
            _prefabTagSelectionView.Render();

            AssociatePrefabWithSelectedTags();
        }
        #endregion

        #region Private Methods
        private void AssociatePrefabWithSelectedTags()
        {
            if (_prefabTagSelectionView.HasSelectionChanged)
            {
                UndoEx.RecordForToolAction(_tagAssociations);
                _tagAssociations.RemoveAllAssociations();
                _tagAssociations.Associate(_prefabTagSelectionView.ListOfSelectedTagNames);
            }
        }
        #endregion
    }
}
#endif