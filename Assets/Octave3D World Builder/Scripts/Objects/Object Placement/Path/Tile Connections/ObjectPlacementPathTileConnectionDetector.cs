#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathTileConnectionDetector
    {
        #region Private Variables
        private float _tileXZSize;
        #endregion

        #region Public Methods
        public List<ObjectPlacementPathTileConnectionGridCell> Detect(ObjectPlacementPath path, float tileXZSize)
        {
            _tileXZSize = tileXZSize;

            List<ObjectPlacementBoxStackSegment> allPathSegments = path.GetAllSegments();
            if (allPathSegments.Count == 0) return new List<ObjectPlacementPathTileConnectionGridCell>();

            // Create the grid cell instances which are occupied by tiles
            Vector3 gridOrigin = path.ExtensionPlane.ProjectPoint(allPathSegments[0].GetStackByIndex(0).BasePosition);
            var tileConnectionGrid = new ObjectPlacementPathTileConnectionGrid(_tileXZSize, gridOrigin, path);
            foreach(ObjectPlacementBoxStackSegment segment in allPathSegments)
            {
                for(int stackIndex = 0; stackIndex < segment.NumberOfStacks; ++stackIndex)
                {
                    ObjectPlacementBoxStack stack = segment.GetStackByIndex(stackIndex);
                    if (stack.IsOverlappedByAnotherStack) continue;

                    var tileConnectionGridCell = new ObjectPlacementPathTileConnectionGridCell(tileConnectionGrid.CalculateCellIndicesFromPosition(stack.BasePosition), tileConnectionGrid);
                    tileConnectionGridCell.TileConnectionSegment = segment;
                    tileConnectionGridCell.TileConnectionStack = stack;
                    tileConnectionGridCell.TileConnectionPath = path;

                    if (!tileConnectionGrid.IsCellOccupied(tileConnectionGridCell.CellIndices)) tileConnectionGrid.AddCell(tileConnectionGridCell);
                }
            }

            tileConnectionGrid.EstablishCellNeighbours();
            tileConnectionGrid.DetectTileConnectionTypesForAllOccupiedCells();

            return tileConnectionGrid.OccupiedTileConnectionCells;
        }
        #endregion
    }
}
#endif