#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    public enum PrefabMoveType
    {
        AllPrefabs = 0,
        FilteredPrefabs,
        ActivePrefab
    }

    [Serializable]
    public class ActivePrefabCategoryViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _showPrefabCategoryFolderNames = false;
        [SerializeField]
        private bool _showHints = true;
        [SerializeField]
        private int _maxNumberOfCategoryFolderNames = 10;
        [SerializeField]
        private string _nameForNewPrefabCategory = "";

        [SerializeField]
        private PrefabMoveType _prefabMoveType = PrefabMoveType.AllPrefabs;
        [SerializeField]
        private PrefabCategory _destinationCategoryForPrefabMove;

        [SerializeField]
        private float _prefabOffsetFromGridSurface = 0.0f;
        [SerializeField]
        private float _prefabOffsetFromObjectSurface = 0.0f;

        [SerializeField]
        private string _prefabConfigLoadDir = "";
        [SerializeField]
        private string _prefabConfigSaveDir = "";
        #endregion

        #region Public Properties
        public PrefabMoveType PrefabMoveType { get { return _prefabMoveType; } set { _prefabMoveType = value; } }
        public bool ShowPrefabCategoryFolderNames { get { return _showPrefabCategoryFolderNames; } set { _showPrefabCategoryFolderNames = value; } }
        public bool ShowHints { get { return _showHints; } set { _showHints = value; } }
        public int MaxNumberOfCategoryFolderNames { get { return _maxNumberOfCategoryFolderNames; } set { _maxNumberOfCategoryFolderNames = Mathf.Clamp(value, 1, 10); } }
        public string NameForNewPrefabCategory { get { return _nameForNewPrefabCategory; } set { if (value != null) _nameForNewPrefabCategory = value; } }
        public PrefabCategory DestinationCategoryForPrefabMove 
        {
            get 
            {
                if (_destinationCategoryForPrefabMove == null) _destinationCategoryForPrefabMove = PrefabCategoryDatabase.Get().GetDefaultPrefabCategory();
                return _destinationCategoryForPrefabMove; 
            }
            set { if (value != null) _destinationCategoryForPrefabMove = value; } 
        }
        public float PrefabOffsetFromGridSurface { get { return _prefabOffsetFromGridSurface; } set { _prefabOffsetFromGridSurface = value; } }
        public float PrefabOffsetFromObjectSurface { get { return _prefabOffsetFromObjectSurface; } set { _prefabOffsetFromObjectSurface = value; } }
        public string PrefabConfigLoadDir { get { return _prefabConfigLoadDir; } set { if (value != null) _prefabConfigLoadDir = value; } }
        public string PrefabConfigSaveDir { get { return _prefabConfigSaveDir; } set { if (value != null) _prefabConfigSaveDir = value; } }
        #endregion
    }
}
#endif