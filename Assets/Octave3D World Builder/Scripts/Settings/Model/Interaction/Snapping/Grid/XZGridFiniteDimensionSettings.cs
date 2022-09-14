#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class XZGridFiniteDimensionSettings
    {
        #region Private Variables
        [SerializeField]
        private Range<int> _xAxisCellIndexRange = new Range<int>(10, 10);
        [SerializeField]
        private Range<int> _zAxisCellIndexRange = new Range<int>(10, 10);
        #endregion

        #region Public Properties
        public Range<int> XAxisCellIndexRange { get { return _xAxisCellIndexRange; } }
        public Range<int> ZAxisCellIndexRange { get { return _zAxisCellIndexRange; } }
        #endregion
    }
}
#endif