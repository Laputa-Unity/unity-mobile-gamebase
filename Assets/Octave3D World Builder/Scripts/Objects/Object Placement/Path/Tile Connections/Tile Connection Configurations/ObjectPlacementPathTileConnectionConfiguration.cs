#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathTileConnectionConfiguration : ScriptableObject, INamedEntity
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementPathTileConnectionYAxisRotation[] _tileConnectionYAxisRotations = new ObjectPlacementPathTileConnectionYAxisRotation[ObjectPlacementPathTileConnectionTypes.Count];
        [SerializeField]
        private float[] _tileConnectionYOffsets = new float[ObjectPlacementPathTileConnectionTypes.Count];
        [SerializeField]
        private int[] _tileConnectionUpwardsExtrusionAmount = new int[ObjectPlacementPathTileConnectionTypes.Count];
        [SerializeField]
        private int[] _tileConnectionDownwardsExtrusionAmount = new int[ObjectPlacementPathTileConnectionTypes.Count];
        [SerializeField]
        private Prefab[] _tileConnectionPrefabs = new Prefab[ObjectPlacementPathTileConnectionTypes.Count];

        [SerializeField]
        private string _name;
        #endregion

        #region Public Properties
        public string Name { get { return _name; } set { if (!string.IsNullOrEmpty(value)) _name = value; } }
        #endregion

        #region Public Methods
        public void ExtractConfigurationDataFromSettings(ObjectPlacementPathTileConnectionSettings tileConnectionSettings)
        {
            List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
            foreach(ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
            {
                int tileConnectionIndex = (int)tileConnectionType;
                ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings = tileConnectionSettings.GetSettingsForTileConnectionType(tileConnectionType);

                _tileConnectionYAxisRotations[tileConnectionIndex] = tileConnectionTypeSettings.YAxisRotation;
                _tileConnectionYOffsets[tileConnectionIndex] = tileConnectionTypeSettings.YOffset;
                _tileConnectionUpwardsExtrusionAmount[tileConnectionIndex] = tileConnectionTypeSettings.UpwardsExtrusionAmount;
                _tileConnectionDownwardsExtrusionAmount[tileConnectionIndex] = tileConnectionTypeSettings.DownwardsExtrusionAmount;
                _tileConnectionPrefabs[tileConnectionIndex] = tileConnectionTypeSettings.Prefab;
            }
        }

        public void ApplyConfigurationDataToSettings(ObjectPlacementPathTileConnectionSettings tileConnectionSettings)
        {
            List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAll();
            foreach (ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
            {
                int tileConnectionIndex = (int)tileConnectionType;
                ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings = tileConnectionSettings.GetSettingsForTileConnectionType(tileConnectionType);

                tileConnectionTypeSettings.YAxisRotation = _tileConnectionYAxisRotations[tileConnectionIndex];
                tileConnectionTypeSettings.YOffset = _tileConnectionYOffsets[tileConnectionIndex];
                tileConnectionTypeSettings.UpwardsExtrusionAmount = _tileConnectionUpwardsExtrusionAmount[tileConnectionIndex];
                tileConnectionTypeSettings.DownwardsExtrusionAmount = _tileConnectionDownwardsExtrusionAmount[tileConnectionIndex];
                tileConnectionTypeSettings.Prefab = _tileConnectionPrefabs[tileConnectionIndex];
            }
        }

        public void RemovePrefabAssociations(Prefab prefab)
        {
            for(int prefabIndex = 0; prefabIndex < _tileConnectionPrefabs.Length; ++prefabIndex)
            {
                if(_tileConnectionPrefabs[prefabIndex] == prefab) _tileConnectionPrefabs[prefabIndex] = null;
            }
        }

        public void RemovePrefabAssociations(List<Prefab> prefabs)
        {
            for (int prefabIndex = 0; prefabIndex < _tileConnectionPrefabs.Length; ++prefabIndex)
            {
                if (prefabs.Contains(_tileConnectionPrefabs[prefabIndex])) _tileConnectionPrefabs[prefabIndex] = null;
            }
        }
        #endregion
    }
}
#endif