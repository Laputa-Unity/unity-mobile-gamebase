#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class XZGridCellSizeSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _cellSizeX = 1.0f;
        [SerializeField]
        private float _cellSizeZ = 1.0f;

        [SerializeField]
        private XZGridCellSizeSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinCellSize { get { return 0.1f; } }
        #endregion

        #region Public Properties
        public float CellSizeX { get { return _cellSizeX; } set { _cellSizeX = Mathf.Max(MinCellSize, value); } }
        public float HalfCellSizeX { get { return _cellSizeX * 0.5f; } }
        public float CellSizeZ { get { return _cellSizeZ; } set { _cellSizeZ = Mathf.Max(MinCellSize, value); } }
        public float HalfCellSizeZ { get { return _cellSizeZ * 0.5f; } }
        public XZGridCellSizeSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public XZGridCellSizeSettings()
        {
            _view = new XZGridCellSizeSettingsView(this);
        }
        #endregion
    }
}
#endif