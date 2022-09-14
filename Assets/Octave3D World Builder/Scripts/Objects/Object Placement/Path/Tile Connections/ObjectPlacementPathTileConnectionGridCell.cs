#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathTileConnectionGridCell
    {
        #region Private Variables
        private ObjectPlacementPathTileConnectionGrid _parentGrid;
        private ObjectPlacementPathTileConnectionGridCellIndices _cellIndices;

        private ObjectPlacementPathTileConnectionType _tileConnectionType;
        private ObjectPlacementBoxStack _tileConnectionStack;
        private ObjectPlacementBoxStackSegment _tileConnectionSegment;
        private ObjectPlacementPath _tileConnectionPath;

        private ObjectPlacementPathTileConnectionGridCell _leftNeighbour;
        private ObjectPlacementPathTileConnectionGridCell _rightNeighbour;
        private ObjectPlacementPathTileConnectionGridCell _forwardNeighbour;
        private ObjectPlacementPathTileConnectionGridCell _backNeighbour;
        private int _numberOfNeighbours;
        #endregion

        #region Public Properties
        public ObjectPlacementPathTileConnectionGrid ParentGrid { get { return _parentGrid; } }
        public int XIndex { get { return _cellIndices.XIndex; } }
        public int ZIndex { get { return _cellIndices.ZIndex; } }
        public ObjectPlacementPathTileConnectionGridCellIndices CellIndices { get { return _cellIndices; } }

        public ObjectPlacementBoxStack TileConnectionStack { get { return _tileConnectionStack; } set { _tileConnectionStack = value; } }
        public ObjectPlacementBoxStackSegment TileConnectionSegment { get { return _tileConnectionSegment; } set { _tileConnectionSegment = value; } }
        public ObjectPlacementPath TileConnectionPath { get { return _tileConnectionPath; } set { _tileConnectionPath = value; } }
        public ObjectPlacementPathTileConnectionType TileConnectionType { get { return _tileConnectionType; } set { _tileConnectionType = value; } }

        public ObjectPlacementPathTileConnectionGridCell LeftNeighbour { get { return _leftNeighbour; } }
        public ObjectPlacementPathTileConnectionGridCell RightNeighbour { get { return _rightNeighbour; } }
        public ObjectPlacementPathTileConnectionGridCell ForwardNeighbour { get { return _forwardNeighbour; } }
        public ObjectPlacementPathTileConnectionGridCell BackNeighbour { get { return _backNeighbour; } }
        public int NumberOfNeighbours { get { return _numberOfNeighbours; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathTileConnectionGridCell(ObjectPlacementPathTileConnectionGridCellIndices cellIndices, ObjectPlacementPathTileConnectionGrid parentGrid)
        {
            _cellIndices = cellIndices;
            _parentGrid = parentGrid;
        }
        #endregion

        #region Public Methods
        public void AddNeighbourIfPossible(ObjectPlacementPathTileConnectionGridCell neighbour)
        {
            if (neighbour.ZIndex == ZIndex)
            {
                if (neighbour.XIndex == XIndex + 1)
                {
                    _rightNeighbour = neighbour;
                    CalculateNumberOfNeighbours();
                }
                else
                if (neighbour.XIndex == XIndex - 1)
                {
                    _leftNeighbour = neighbour;
                    CalculateNumberOfNeighbours();
                }
            }
            else
                if (neighbour.XIndex == XIndex)
            {
                if (neighbour.ZIndex == ZIndex + 1)
                {
                    _forwardNeighbour = neighbour;
                    CalculateNumberOfNeighbours();
                }
                else
                if (neighbour.ZIndex == ZIndex - 1)
                {
                    _backNeighbour = neighbour;
                    CalculateNumberOfNeighbours();
                }
            }
        }

        public bool HasLeftAndRightNeighbours()
        {
            return _leftNeighbour != null && _rightNeighbour != null;
        }

        public bool HasForwardAndBackNeightbours()
        {
            return _forwardNeighbour != null && _backNeighbour != null;
        }

        public ObjectPlacementPathTileConnectionGridCell GetFirstNeighbour()
        {
            if (_leftNeighbour != null) return _leftNeighbour;
            if (_rightNeighbour != null) return _rightNeighbour;
            if (_forwardNeighbour != null) return _forwardNeighbour;
            if (_backNeighbour != null) return _backNeighbour;

            return null;
        }
        #endregion

        #region Private Methods
        private void CalculateNumberOfNeighbours()
        {
            _numberOfNeighbours = 0;
            if (_leftNeighbour != null) ++_numberOfNeighbours;
            if (_rightNeighbour != null) ++_numberOfNeighbours;
            if (_forwardNeighbour != null) ++_numberOfNeighbours;
            if (_backNeighbour != null) ++_numberOfNeighbours;
        }
        #endregion
    }
}
#endif