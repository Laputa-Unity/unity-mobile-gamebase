#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabsToPathTileConectionButtonDropSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private PrefabCategory _destinationCategoryForDroppedPrefabs;

        [SerializeField]
        private PrefabsToPathTileConectionButtonDropSettingsView _view;
        #endregion

        #region Public Properties
        public PrefabCategory DestinationCategoryForDroppedPrefabs
        {
            get
            {
                if (_destinationCategoryForDroppedPrefabs == null) _destinationCategoryForDroppedPrefabs = PrefabCategoryDatabase.Get().GetDefaultPrefabCategory();
                return _destinationCategoryForDroppedPrefabs;
            }
            set
            {
                if (value != null) _destinationCategoryForDroppedPrefabs = value;
            }
        }
        public PrefabsToPathTileConectionButtonDropSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabsToPathTileConectionButtonDropSettings()
        {
            _view = new PrefabsToPathTileConectionButtonDropSettingsView(this);
        }
        #endregion
    }
}
#endif