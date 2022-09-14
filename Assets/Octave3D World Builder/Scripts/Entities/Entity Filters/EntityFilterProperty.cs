#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class EntityFilterProperty
    {
        #region Private Variables
        [SerializeField]
        private bool _isActive;
        [SerializeField]
        private string _name;

        [NonSerialized]
        private List<EntityFilterProperty> _mutualExclusionList = new List<EntityFilterProperty>();
        #endregion

        #region Public Properties
        public bool IsActive { get { return _isActive; } }
        public string Name { get { return _name; } set { if (!string.IsNullOrEmpty(value)) _name = value; } }
        #endregion

        #region Constructors
        public EntityFilterProperty(string name)
        {
            _name = name;
        }
        #endregion

        #region Public Methods
        public void SetActive(bool active)
        {
            _isActive = active;
            if (_isActive) DeacivateMutualExclusions();
        }

        public void AddMutualExclusion(EntityFilterProperty entityFilterProperty)
        {
            if (entityFilterProperty == this || entityFilterProperty == null) return;
            _mutualExclusionList.Add(entityFilterProperty);
        }
        #endregion

        #region Private Methods
        private void DeacivateMutualExclusions()
        {
            foreach (EntityFilterProperty filterProperty in _mutualExclusionList)
            {
                filterProperty.SetActive(false);
            }
        }
        #endregion
    }
}
#endif