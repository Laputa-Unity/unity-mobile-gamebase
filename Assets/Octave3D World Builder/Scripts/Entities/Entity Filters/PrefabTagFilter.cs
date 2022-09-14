#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagFilter : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private EntityNameFilter _nameFilter;

        [SerializeField]
        private PrefabTagFilterView _view;
        #endregion

        #region Public Properties
        public EntityNameFilter NameFilter
        {
            get
            {
                if (_nameFilter == null) _nameFilter = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<EntityNameFilter>();
                return _nameFilter;
            }
        }
        public PrefabTagFilterView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabTagFilter()
        {
            _view = new PrefabTagFilterView(this);
        }
        #endregion

        #region Public Methods
        public List<PrefabTag> GetFilteredPrefabTags(List<PrefabTag> prefabTags)
        {
            if (!IsAnyFilteringRequired()) return prefabTags;

            return FilterTags(prefabTags);
        }

        public bool IsAnyFilteringRequired()
        {
            return NameFilter.IsAnyFilteringRequired();
        }
        #endregion

        #region Private Methods
        private List<PrefabTag> FilterTags(List<PrefabTag> prefabTags)
        {
            List<INamedEntity> namedTagEntities = (from prefabTag in prefabTags select prefabTag as INamedEntity).ToList();
            namedTagEntities = NameFilter.GetEntitiesFilteredByName(namedTagEntities);

            return FilterTagsByFilterProperties(namedTagEntities);
        }

        private List<PrefabTag> FilterTagsByFilterProperties(List<INamedEntity> namedTagEntities)
        {
            var filteredTags = new List<PrefabTag>();
            foreach (PrefabTag prefabTag in namedTagEntities)
            {
                filteredTags.Add(prefabTag);
            }

            return filteredTags;
        }

        private void OnDestroy()
        {
            if (_nameFilter != null) UndoEx.DestroyObjectImmediate(_nameFilter);
        }
        #endregion
    }
}
#endif