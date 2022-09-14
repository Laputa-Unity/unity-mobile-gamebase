#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabsToCategoryDropSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private List<string> _tagNamesForDroppedPrefabs = new List<string>();

        [SerializeField]
        private PrefabToCategoryDropSettingsView _view;
        #endregion

        #region Public Properties
        public List<string> TagNamesForDroppedPrefabs { get { return _tagNamesForDroppedPrefabs; } set { if (value != null)_tagNamesForDroppedPrefabs = value; } }
        public PrefabToCategoryDropSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabsToCategoryDropSettings()
        {
            _view = new PrefabToCategoryDropSettingsView(this);
        }
        #endregion
    }
}
#endif