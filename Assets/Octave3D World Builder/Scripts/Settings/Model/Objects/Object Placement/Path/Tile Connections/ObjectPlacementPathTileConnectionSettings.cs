#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathTileConnectionSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _useTileConnections = false;
        [SerializeField]
        private ObjectPlacementPathTileConnectionYAxisRotation _commonYAxisRotation;
        [SerializeField]
        private float _commonYOffset = 0.0f;
        [SerializeField]
        private int _commonUpwardsExtrusionAmount = 0;
        [SerializeField]
        private int _commonDownwardsExtrusionAmount = 0;
        [SerializeField]
        private ObjectPlacementPathTileConnectionType _prefabInheritTileConnectionType;

        [SerializeField]
        private ObjectPlacementPathTileConnectionTypeSettings[] _tileConnectionTypeSettings;

        [SerializeField]
        private bool _wasInitialized = false;

        [SerializeField]
        private ObjectPlacementPathTileConnectionSettingsView _view;
        #endregion

        #region Public Properties
        public bool UseTileConnections { get { return _useTileConnections; } set { _useTileConnections = value; } }
        public ObjectPlacementPathTileConnectionYAxisRotation CommonYAxisRotation { get { return _commonYAxisRotation; } set { _commonYAxisRotation = value; } }
        public float CommonYOffset { get { return _commonYOffset; } set { _commonYOffset = value; } }
        public int CommonUpwardsExtrusionAmount { get { return _commonUpwardsExtrusionAmount; } set { _commonUpwardsExtrusionAmount = Math.Max(value, ObjectPlacementPathTileConnectionTypeSettings.MinExtrusionAmount); } }
        public int CommonDownwardsExtrusionAmount { get { return _commonDownwardsExtrusionAmount; } set { _commonDownwardsExtrusionAmount = Math.Max(value, ObjectPlacementPathTileConnectionTypeSettings.MinExtrusionAmount); } }
        public ObjectPlacementPathTileConnectionType PrefabInheritTileConnectionType { get { return _prefabInheritTileConnectionType; } set { _prefabInheritTileConnectionType = value; } }
        public ObjectPlacementPathTileConnectionSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathTileConnectionSettings()
        {
            _view = new ObjectPlacementPathTileConnectionSettingsView(this);
        }
        #endregion

        #region Public Methods
        public ObjectPlacementPathTileConnectionTypeSettings GetSettingsForTileConnectionType(ObjectPlacementPathTileConnectionType tileConnectionType)
        {
            return _tileConnectionTypeSettings[(int)tileConnectionType];
        }

        public bool DoAllTileConnectionsHavePrefabAssociated(bool ignoreAutofill)
        {
            foreach(ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings in _tileConnectionTypeSettings)
            {
                if (tileConnectionTypeSettings.Prefab == null || tileConnectionTypeSettings.Prefab.UnityPrefab == null)
                {
                    if ((tileConnectionTypeSettings.TileConnectionType != ObjectPlacementPathTileConnectionType.Autofill) || !ignoreAutofill) return false;
                }
            }

            return true;
        }

        public bool DoesAutofillTileConnectionHavePrefabAssociated()
        {
            return GetSettingsForTileConnectionType(ObjectPlacementPathTileConnectionType.Autofill).Prefab != null;
        }

        public List<Prefab> GetAllTileConnectionPrefabs(bool ignoreAutofill)
        {
            List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
            List<Prefab> allTileConnectionPrefabs = new List<Prefab>(ObjectPlacementPathTileConnectionTypes.Count);
            foreach(ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
            {
                if (tileConnectionType == ObjectPlacementPathTileConnectionType.Autofill && ignoreAutofill) continue;
                allTileConnectionPrefabs.Add(GetSettingsForTileConnectionType(tileConnectionType).Prefab);
            }

            return allTileConnectionPrefabs;
        }

        public void RecordAllTileConnectionTypeSettingsForUndo()
        {
            List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
            foreach (ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
            {
                UndoEx.RecordForToolAction(GetSettingsForTileConnectionType(tileConnectionType));
            }
        }

        public void RemoveAllPrefabAssociations()
        {
            List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
            foreach (ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
            {
                GetSettingsForTileConnectionType(tileConnectionType).Prefab = null;
            }
        }

        public void RemovePrefabAssociation(Prefab prefab)
        {
            List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
            foreach (ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
            {
                ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings = GetSettingsForTileConnectionType(tileConnectionType);
                if (tileConnectionTypeSettings.Prefab == prefab) tileConnectionTypeSettings.Prefab = null;
            }
        }

        public void RemovePrefabAssociations(List<Prefab> prefabs)
        {
            foreach(Prefab prefab in prefabs)
            {
                RemovePrefabAssociation(prefab);
            }
        }

        public bool UsesSprites()
        {
            List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
            foreach (ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
            {
                ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings = GetSettingsForTileConnectionType(tileConnectionType);
                if (tileConnectionTypeSettings.Prefab != null && tileConnectionTypeSettings.Prefab.UnityPrefab != null && tileConnectionTypeSettings.Prefab.UnityPrefab.IsSprite()) return true;
            }

            return false;
        }

        public void RemoveNullPrefabReferences()
        {
            foreach(var tileCSettings in _tileConnectionTypeSettings)
            {
                if (tileCSettings.Prefab != null && tileCSettings.Prefab.UnityPrefab == null) tileCSettings.Prefab = null;
            }
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            if (!_wasInitialized)
            {
                InitializePathTileConnectionTypeSettingsArray();
                _wasInitialized = true;
            }
        }

        private void InitializePathTileConnectionTypeSettingsArray()
        {
            _tileConnectionTypeSettings = new ObjectPlacementPathTileConnectionTypeSettings[ObjectPlacementPathTileConnectionTypes.Count];
            for (int tileConnectionTypeIndex = 0; tileConnectionTypeIndex < _tileConnectionTypeSettings.Length; ++tileConnectionTypeIndex)
            {
                ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathTileConnectionTypeSettings>();
                tileConnectionTypeSettings.TileConnectionType = (ObjectPlacementPathTileConnectionType)tileConnectionTypeIndex;
                _tileConnectionTypeSettings[tileConnectionTypeIndex] = tileConnectionTypeSettings;
            }
        }
        #endregion
    }
}
#endif