#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightPatternFilter : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private EntityNameFilter _nameFilter;

        [SerializeField]
        private EntityFilterProperty _onlyActive = new EntityFilterProperty("Only active");

        [SerializeField]
        private ObjectPlacementPathHeightPatternFilterView _view;
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
        public EntityFilterProperty OnlyActive { get { return _onlyActive; } }
        public ObjectPlacementPathHeightPatternFilterView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathHeightPatternFilter()
        {
            _view = new ObjectPlacementPathHeightPatternFilterView(this);
        }
        #endregion

        #region Public Methods
        public List<ObjectPlacementPathHeightPattern> GetFilteredPatterns(List<ObjectPlacementPathHeightPattern> heightPatterns)
        {
            if (!IsAnyFilteringRequired()) return heightPatterns;

            return FilterPatterns(heightPatterns);
        }

        public bool IsAnyFilteringRequired()
        {
            if (!_onlyActive.IsActive && !NameFilter.IsAnyFilteringRequired()) return false;
            return true;
        }
        #endregion

        #region Private Methods
        private List<ObjectPlacementPathHeightPattern> FilterPatterns(List<ObjectPlacementPathHeightPattern> heightPatterns)
        {
            List<INamedEntity> namedPatternEntities = (from pattern in heightPatterns select pattern as INamedEntity).ToList();
            namedPatternEntities = NameFilter.GetEntitiesFilteredByName(namedPatternEntities);

            return FilterPatternsByFilterProperties(namedPatternEntities);
        }

        private List<ObjectPlacementPathHeightPattern> FilterPatternsByFilterProperties(List<INamedEntity> namedPatternEntities)
        {
            var filteredPatterns = new List<ObjectPlacementPathHeightPattern>();
            ObjectPlacementPathHeightPattern activePattern = ObjectPlacementPathHeightPatternDatabase.Get().ActivePattern;

            foreach (ObjectPlacementPathHeightPattern pattern in namedPatternEntities)
            {
                if (_onlyActive.IsActive)
                {
                    if (pattern == activePattern)
                    {
                        filteredPatterns.Add(pattern);
                        return filteredPatterns;
                    }
                }
                else filteredPatterns.Add(pattern);
            }

            return filteredPatterns;
        }
        #endregion
    }
}
#endif