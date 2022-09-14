#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionPrefabCreationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private PrefabCategory _destinationCategory;
        [SerializeField]
        private string _prefabName = "";
        [SerializeField]
        private string _destinationFolder = "";
        [SerializeField]
        private Pivot _pivot = Pivot.Center;

        [SerializeField]
        private ObjectSelectionPrefabCreationSettingsView _view;
        #endregion

        #region Public Properties
        public PrefabCategory DestinationCategory
        {
            get
            {
                if (_destinationCategory == null) _destinationCategory = PrefabCategoryDatabase.Get().GetDefaultPrefabCategory();
                return _destinationCategory;
            }
            set
            {
                if (value != null) _destinationCategory = value;
            }
        }
        public string PrefabName { get { return _prefabName; } set { if (value != null) _prefabName = value; } }
        public string DestinationFolder { get { return _destinationFolder; } set { if (value != null) _destinationFolder = value; } }
        public Pivot Pivot { get { return _pivot; } set { _pivot = value; } }
        public ObjectSelectionPrefabCreationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectSelectionPrefabCreationSettings()
        {
            _view = new ObjectSelectionPrefabCreationSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectSelectionPrefabCreationSettings Get()
        {
            return ObjectSelection.Get().PrefabCreationSettings;
        }
        #endregion
    }
}
#endif