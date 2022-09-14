#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class EntityNameFilter : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private string _nameFilter = "";

        [SerializeField]
        private EntityNameFilterView _view;
        #endregion

        #region Public Properties
        public string NameFilter { get { return _nameFilter; } set { if (value != null) _nameFilter = value; } }
        public EntityNameFilterView View { get { return _view; } }
        #endregion

        #region Constructors
        public EntityNameFilter()
        {
            _view = new EntityNameFilterView(this);
        }
        #endregion

        #region Public Methods
        public List<INamedEntity> GetEntitiesFilteredByName(List<INamedEntity> namedEntities)
        {
            if (IsAnyFilteringRequired()) return EntityNameMatching.GetEntitiesWithMatchingNames(namedEntities, _nameFilter, false);
            else return namedEntities;
        }

        public bool IsAnyFilteringRequired()
        {
            return !string.IsNullOrEmpty(_nameFilter);
        }
        #endregion
    }
}
#endif