#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class XZGridCell
    {
        #region Private Variables
        private int _xIndex;
        private int _zIndex;
        private XZGrid _parentGrid;
        private XZOrientedQuad3D _quad;
        #endregion

        #region Public Properties
        public int XIndex { get { return _xIndex; } }
        public int ZIndex { get { return _zIndex; } }
        public XZOrientedQuad3D Quad { get { return new XZOrientedQuad3D(_quad); } }
        public XZGrid ParentGrid { get { return _parentGrid; } }
        #endregion

        #region Constructors
        public XZGridCell(int xIndex, int zIndex, XZGrid parentGrid, XZOrientedQuad3D orientedQuad)
        {
            _xIndex = xIndex;
            _zIndex = zIndex;

            _parentGrid = parentGrid;
            _quad = orientedQuad;
        }

        public XZGridCell(XZGridCell source)
        {
            _xIndex = source._xIndex;
            _zIndex = source._zIndex;

            _parentGrid = source._parentGrid;
            _quad = source.Quad;
        }
        #endregion
    }
}
#endif