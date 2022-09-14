#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathTileConnectionTypeSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementPathTileConnectionType _tileConnectionType;
        [SerializeField]
        private Prefab _prefab;
        [SerializeField]
        private ObjectPlacementPathTileConnectionYAxisRotation _yAxisRotation = ObjectPlacementPathTileConnectionYAxisRotation._0;

        [SerializeField]
        private float _yOffset = 0.0f;
        [SerializeField]
        private int _upwardsExtrusionAmount = 0;
        [SerializeField]
        private int _downwardsExtrusionAmount = 0;

        [SerializeField]
        private ObjectPlacementPathTileConnectionTypeSettingsView _view;
        #endregion

        #region Public Static Properties
        public static int MinExtrusionAmount { get { return 0; } }
        #endregion

        #region Public Properties
        public ObjectPlacementPathTileConnectionType TileConnectionType { get { return _tileConnectionType; } set { _tileConnectionType = value; } }
        public Prefab Prefab 
        { 
            get { return _prefab; } 
            set 
            {
                if (value == null)
                {
                    _prefab = null;
                    return;
                }

                OrientedBox prefabOrientedBox = value.UnityPrefab.GetHierarchyWorldOrientedBox();
                Vector3 scaledSize = prefabOrientedBox.ScaledSize;
                if (value.UnityPrefab.IsSprite())
                {
                    if (Mathf.Abs(scaledSize.x) < 1e-5f ||
                        Mathf.Abs(scaledSize.y) < 1e-5f) Debug.LogWarning("3D tile connections must have a non-zero XZ size and sprite tiles must have a non-zer XY size. This prefab does not meet these requirements.");
                    else _prefab = value; 
                }
                else
                {
                    if(Mathf.Abs(scaledSize.x) < 1e-5f ||
                       Mathf.Abs(scaledSize.z) < 1e-5f) Debug.LogWarning("3D tile connections must have a non-zero XZ size and sprite tiles must have a non-zer XY size. This prefab does not meet these requirements.");
                    else _prefab = value; 
                }
            } 
        }
        public ObjectPlacementPathTileConnectionYAxisRotation YAxisRotation { get { return _yAxisRotation; } set { _yAxisRotation = value; } }
        public float YOffset { get { return _yOffset; } set { _yOffset = value; } }
        public int UpwardsExtrusionAmount { get { return _upwardsExtrusionAmount; } set { _upwardsExtrusionAmount = Mathf.Max(MinExtrusionAmount, value); } }
        public int DownwardsExtrusionAmount { get { return _downwardsExtrusionAmount; } set { _downwardsExtrusionAmount = Mathf.Max(MinExtrusionAmount, value); } }
        public ObjectPlacementPathTileConnectionTypeSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathTileConnectionTypeSettings()
        {
            _view = new ObjectPlacementPathTileConnectionTypeSettingsView(this);
        }
        #endregion

        #region Public Methods
        public bool IsAnyExtrusionNecessary()
        {
            return UpwardsExtrusionAmount != 0 || DownwardsExtrusionAmount != 0;
        }
        #endregion
    }
}
#endif