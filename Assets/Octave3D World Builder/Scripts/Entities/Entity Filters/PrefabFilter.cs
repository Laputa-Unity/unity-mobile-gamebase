#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    public enum PrefabTagFilterMode
    {
        None = 0,
        Any,
        All
    }

    [Serializable]
    public class PrefabFilter : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private EntityNameFilter _nameFilter;

        //[SerializeField]
        //private EntityFilterProperty _filterByActiveTags = new EntityFilterProperty("Filter by active tags");

        [SerializeField]
        private PrefabTagFilterMode _prefabTagFilterMode = PrefabTagFilterMode.All;

        [SerializeField]
        private PrefabFilterView _view;
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
        //public EntityFilterProperty FilterByActiveTags { get { return _filterByActiveTags; } }
        public PrefabTagFilterMode PrefabTagFilterMode { get { return _prefabTagFilterMode; } set { _prefabTagFilterMode = value; } }
        public PrefabFilterView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabFilter()
        {
            _view = new PrefabFilterView(this);
            //_filterByActiveTags.SetActive(true);
        }
        #endregion

        #region Public Methods
        public List<Prefab> GetFilteredPrefabs(List<Prefab> prefabs)
        {
            if (!IsAnyFilteringRequired()) return prefabs;

            return FilterPrefabs(prefabs);
        }

        public bool IsAnyFilteringRequired()
        {
            return NameFilter.IsAnyFilteringRequired() || _prefabTagFilterMode != PrefabTagFilterMode.None; //_filterByActiveTags.IsActive;
        }
        #endregion

        #region Private Methods
        private List<Prefab> FilterPrefabs(List<Prefab> prefabs)
        {
            List<INamedEntity> namedPrefabEntities = (from prefab in prefabs select prefab as INamedEntity).ToList();
            namedPrefabEntities = NameFilter.GetEntitiesFilteredByName(namedPrefabEntities);
            
            return FilterPrefabsByFilterProperties(namedPrefabEntities);
        }

        private List<Prefab> FilterPrefabsByFilterProperties(List<INamedEntity> namedPrefabEntities)
        {
            var filteredPrefabs = new List<Prefab>();
            foreach (Prefab prefab in namedPrefabEntities)
            {
                if (_prefabTagFilterMode != PrefabTagFilterMode.None && prefab.TagAssociations.NumberOfAssociations != 0)
                {
                    if (_prefabTagFilterMode == PrefabTagFilterMode.Any)
                    {
                        if (prefab.TagAssociations.IsAnyAssociationActive()) filteredPrefabs.Add(prefab);
                    }
                    else if (prefab.TagAssociations.AreAllTagAssociationsActive()) filteredPrefabs.Add(prefab);
                }
                else filteredPrefabs.Add(prefab);
            }

            return filteredPrefabs;
        }

        private void OnDestroy()
        {
            if (_nameFilter != null) UndoEx.DestroyObjectImmediate(_nameFilter);
        }
        #endregion
    }
}
#endif