#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class XZVisibleGridCellRange
    {
        #region Private Variables
        Range<int> _xAxisVisibleCellRange = new Range<int>();
        Range<int> _zAxisVisibleCellRange = new Range<int>();
        #endregion

        #region Public Properties
        public Range<int> XAxisVisibleCellRange { get { return _xAxisVisibleCellRange; } }
        public Range<int> ZAxisVisibleCellRange { get { return _zAxisVisibleCellRange; } }
        #endregion
    }
}
#endif