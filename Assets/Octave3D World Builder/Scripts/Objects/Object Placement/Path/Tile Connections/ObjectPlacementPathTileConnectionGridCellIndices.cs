#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public struct ObjectPlacementPathTileConnectionGridCellIndices
    {
        #region Private Variables
        private int _XIndex;
        private int _ZIndex;
        #endregion

        #region Public Properties
        public int XIndex { get { return _XIndex; } }
        public int ZIndex { get { return _ZIndex; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathTileConnectionGridCellIndices(int xIndex, int zIndex)
        {
            _XIndex = xIndex;
            _ZIndex = zIndex;
        }
        #endregion
    }
}
#endif