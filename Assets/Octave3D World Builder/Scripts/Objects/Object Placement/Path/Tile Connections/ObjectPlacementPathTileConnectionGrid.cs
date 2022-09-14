#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathTileConnectionGrid
    {
        #region Private Variables
        private Dictionary<ObjectPlacementPathTileConnectionGridCellIndices, ObjectPlacementPathTileConnectionGridCell> _occupiedCells = new Dictionary<ObjectPlacementPathTileConnectionGridCellIndices, ObjectPlacementPathTileConnectionGridCell>();
        private bool _cellLimitsAreDirty = true;

        private int _minCellIndexX = int.MaxValue;
        private int _maxCellIndexX = int.MinValue;
        private int _minCellIndexZ = int.MaxValue;
        private int _maxCellIndexZ = int.MinValue;

        private Vector3 _origin;
        private ObjectPlacementPath _path;
        private float _cellXZSize;
        #endregion

        #region Public Properties
        public int NumberOfCellsOnX { get { return MaxCellIndexX - MinCellIndexX + 1; } }
        public int NumberOfCellsOnZ { get { return MaxCellIndexZ - MinCellIndexZ + 1; } }

        public int MinCellIndexX
        {
            get
            {
                if (_cellLimitsAreDirty) CalculateMinMaxIndices();
                return _minCellIndexX;
            }
        }

        public int MaxCellIndexX
        {
            get
            {
                if (_cellLimitsAreDirty) CalculateMinMaxIndices();
                return _maxCellIndexX;
            }
        }

        public int MinCellIndexZ
        {
            get
            {
                if (_cellLimitsAreDirty) CalculateMinMaxIndices();
                return _minCellIndexZ;
            }
        }

        public int MaxCellIndexZ
        {
            get
            {
                if (_cellLimitsAreDirty) CalculateMinMaxIndices();
                return _maxCellIndexZ;
            }
        }

        public int NumberOfOccupiedCells { get { return _occupiedCells.Count; } }
        public float CellXZSize { get { return _cellXZSize; } }
        public List<ObjectPlacementPathTileConnectionGridCell> OccupiedTileConnectionCells { get { return new List<ObjectPlacementPathTileConnectionGridCell>(_occupiedCells.Values); } }
        #endregion

        #region Constructors
        public ObjectPlacementPathTileConnectionGrid(float cellXZSize, Vector3 gridOrigin, ObjectPlacementPath path)
        {
            _cellXZSize = cellXZSize;
            _origin = gridOrigin;
            _path = path;
        }
        #endregion

        #region Public Methods
        public void AddCell(ObjectPlacementPathTileConnectionGridCell cell)
        {
            _occupiedCells.Add(cell.CellIndices, cell);
            _cellLimitsAreDirty = true;
        }

        public bool IsCellOccupied(ObjectPlacementPathTileConnectionGridCellIndices cellIndices)
        {
            return _occupiedCells.ContainsKey(cellIndices);
        }

        public bool IsCellOccupied(int cellIndeX, int cellIndexZ)
        {
            return IsCellOccupied(new ObjectPlacementPathTileConnectionGridCellIndices(cellIndeX, cellIndexZ));
        }

        public ObjectPlacementPathTileConnectionGridCell GetCellAtIndicesIfOccupied(ObjectPlacementPathTileConnectionGridCellIndices cellIndices)
        {
            if (IsCellOccupied(cellIndices)) return _occupiedCells[cellIndices];
            else return null;
        }

        public Vector3 CalculateCellPosition(int cellIndexX, int cellIndexZ)
        {
            Vector3 planeRight = _path.ExtensionPlaneRightAxis;
            Vector3 planeLook = _path.ExtensionPlaneLookAxis;

            return _origin + planeRight * cellIndexX * _cellXZSize + planeLook * cellIndexZ * _cellXZSize;
        }

        public Vector3 CalculateCellPosition(ObjectPlacementPathTileConnectionGridCellIndices cellIndices) 
        {
            return CalculateCellPosition(cellIndices.XIndex, cellIndices.ZIndex);
        }

        public ObjectPlacementPathTileConnectionGridCellIndices CalculateCellIndicesFromPosition(Vector3 position)
        {
            Vector3 projectedPosition = _path.ExtensionPlane.ProjectPoint(position);
            Vector3 pathExtensionPlaneRight = _path.ExtensionPlaneRightAxis;
            Vector3 pathExtensionPlaneLook = _path.ExtensionPlaneLookAxis;

            Vector3 fromOriginToPoint = projectedPosition - _origin;
            float xOffset = Vector3.Dot(pathExtensionPlaneRight, fromOriginToPoint);
            float zOffset = Vector3.Dot(pathExtensionPlaneLook, fromOriginToPoint);

            float divX = xOffset / _cellXZSize;
            float divZ = zOffset / _cellXZSize;

            int cellIndexX, cellIndexZ;
            if (divX > 0.0f) cellIndexX = (int)(xOffset / _cellXZSize + 0.5f);
            else cellIndexX = (int)(xOffset / _cellXZSize - 0.5f);
            if (divZ > 0.0f) cellIndexZ = (int)(zOffset / _cellXZSize + 0.5f);
            else cellIndexZ = (int)(zOffset / _cellXZSize - 0.5f);

            return new ObjectPlacementPathTileConnectionGridCellIndices(cellIndexX, cellIndexZ);
        }

        public void EstablishCellNeighbours()
        {
            foreach (var pair in _occupiedCells)
            {
                ObjectPlacementPathTileConnectionGridCell tileConnectionGridCell = pair.Value;

                var potentialNeighbour = new ObjectPlacementPathTileConnectionGridCellIndices(tileConnectionGridCell.XIndex + 1, tileConnectionGridCell.ZIndex);
                if (IsCellOccupied(potentialNeighbour)) tileConnectionGridCell.AddNeighbourIfPossible(GetCellAtIndicesIfOccupied(potentialNeighbour));

                potentialNeighbour = new ObjectPlacementPathTileConnectionGridCellIndices(tileConnectionGridCell.XIndex - 1, tileConnectionGridCell.ZIndex);
                if (IsCellOccupied(potentialNeighbour)) tileConnectionGridCell.AddNeighbourIfPossible(GetCellAtIndicesIfOccupied(potentialNeighbour));

                potentialNeighbour = new ObjectPlacementPathTileConnectionGridCellIndices(tileConnectionGridCell.XIndex, tileConnectionGridCell.ZIndex + 1);
                if (IsCellOccupied(potentialNeighbour)) tileConnectionGridCell.AddNeighbourIfPossible(GetCellAtIndicesIfOccupied(potentialNeighbour));

                potentialNeighbour = new ObjectPlacementPathTileConnectionGridCellIndices(tileConnectionGridCell.XIndex, tileConnectionGridCell.ZIndex - 1);
                if (IsCellOccupied(potentialNeighbour)) tileConnectionGridCell.AddNeighbourIfPossible(GetCellAtIndicesIfOccupied(potentialNeighbour));
            }
        }

        public void DetectTileConnectionTypesForAllOccupiedCells()
        {
            ObjectPlacementBoxStack firstStackInFirstPathSegment = _path.GetSegmentByIndex(0).GetStackByIndex(0);
            foreach (var pair in _occupiedCells)
            {
                ObjectPlacementPathTileConnectionGridCell tileConnectionGridCell = pair.Value;

                int numberOfNeighbours = tileConnectionGridCell.NumberOfNeighbours;
                if (numberOfNeighbours == 0) tileConnectionGridCell.TileConnectionType = ObjectPlacementPathTileConnectionType.Begin;
                else if (numberOfNeighbours == 1)
                {
                    if (tileConnectionGridCell.TileConnectionStack == firstStackInFirstPathSegment) tileConnectionGridCell.TileConnectionType = ObjectPlacementPathTileConnectionType.Begin;
                    else tileConnectionGridCell.TileConnectionType = ObjectPlacementPathTileConnectionType.End;
                }
                else if (numberOfNeighbours == 2)
                {
                    if (tileConnectionGridCell.HasLeftAndRightNeighbours() || tileConnectionGridCell.HasForwardAndBackNeightbours()) tileConnectionGridCell.TileConnectionType = ObjectPlacementPathTileConnectionType.Forward;
                    else tileConnectionGridCell.TileConnectionType = ObjectPlacementPathTileConnectionType.Turn;
                }
                else if (numberOfNeighbours == 3) tileConnectionGridCell.TileConnectionType = ObjectPlacementPathTileConnectionType.TJunction;
                else tileConnectionGridCell.TileConnectionType = ObjectPlacementPathTileConnectionType.Cross;
            }
        }

        public List<ObjectPlacementPathTileConnectionGridCell> CreateAndReturnAutofillTileConnectionCells()
        {
            ObjectPlacementPathTileConnectionSettings tileConnectionSettings = _path.Settings.TileConnectionSettings;
            if (!tileConnectionSettings.DoesAutofillTileConnectionHavePrefabAssociated()) return new List<ObjectPlacementPathTileConnectionGridCell>();

            Plane pathExtensionPlane = _path.ExtensionPlane;
            OrientedBox autofillPrefabOrientedBox = tileConnectionSettings.GetSettingsForTileConnectionType(ObjectPlacementPathTileConnectionType.Autofill).Prefab.UnityPrefab.GetHierarchyWorldOrientedBox();
            Vector3 autofillStackBoxSize = new Vector3(_cellXZSize, autofillPrefabOrientedBox.ScaledSize.y, _cellXZSize);
            Quaternion autofillStackRotation = Quaternion.LookRotation(_path.ExtensionPlaneLookAxis, pathExtensionPlane.normal);

            bool[,] cellAutofillFlags = new bool[NumberOfCellsOnX, NumberOfCellsOnZ];

            // We will first set all occupied cells to true
            foreach (var pair in _occupiedCells)
            {
                cellAutofillFlags[pair.Key.XIndex - MinCellIndexX, pair.Key.ZIndex - MinCellIndexZ] = true;
            }

            var autofillGridCells = new List<ObjectPlacementPathTileConnectionGridCell>(NumberOfCellsOnX * NumberOfCellsOnZ);
            for (int z = MinCellIndexZ; z <= MaxCellIndexZ; ++z)
            {
                for (int x = MinCellIndexX; x <= MaxCellIndexX; ++x)
                {
                    if (cellAutofillFlags[x - MinCellIndexX, z - MinCellIndexZ]) continue;

                    var traversedIndices = new HashSet<ObjectPlacementPathTileConnectionGridCellIndices>();
                    if (DetectCellsWhichLeadOutsideOfGridRecurse(cellAutofillFlags, x, z, traversedIndices))
                    {
                        foreach (ObjectPlacementPathTileConnectionGridCellIndices indices in traversedIndices)
                        {
                            cellAutofillFlags[indices.XIndex - MinCellIndexX, indices.ZIndex - MinCellIndexZ] = true;
                        }
                    }

                    if(!cellAutofillFlags[x - MinCellIndexX, z - MinCellIndexZ])
                    {
                        var autofillGridCell = new ObjectPlacementPathTileConnectionGridCell(new ObjectPlacementPathTileConnectionGridCellIndices(x, z), this);
                        autofillGridCells.Add(autofillGridCell);

                        AddCell(autofillGridCell);

                        var tileConnectionStack = new ObjectPlacementBoxStack();
                        tileConnectionStack.SetBoxSize(autofillStackBoxSize);
                        tileConnectionStack.SetRotation(autofillStackRotation);
                        tileConnectionStack.SetBasePosition(CalculateCellPosition(x, z));
                        tileConnectionStack.PlaceOnPlane(pathExtensionPlane);
                        tileConnectionStack.GrowUpwards(1);

                        autofillGridCell.TileConnectionStack = tileConnectionStack;
                        autofillGridCell.TileConnectionPath = _path;
                        autofillGridCell.TileConnectionType = ObjectPlacementPathTileConnectionType.Autofill;
                    }
                }
            }

            return autofillGridCells;
        }
        #endregion

        #region Private Methods
        private void CalculateMinMaxIndices()
        {
            _minCellIndexX = int.MaxValue;
            _maxCellIndexX = int.MinValue;
            _minCellIndexZ = int.MaxValue;
            _maxCellIndexZ = int.MinValue;

            foreach (var pair in _occupiedCells)
            {
                int cellIndexX = pair.Key.XIndex;
                int cellIndexZ = pair.Key.ZIndex;

                if (cellIndexX < _minCellIndexX) _minCellIndexX = cellIndexX;
                if (cellIndexX > _maxCellIndexX) _maxCellIndexX = cellIndexX;
                if (cellIndexZ < _minCellIndexZ) _minCellIndexZ = cellIndexZ;
                if (cellIndexZ > _maxCellIndexZ) _maxCellIndexZ = cellIndexZ;
            }

            _cellLimitsAreDirty = false;
        }

        private bool DetectCellsWhichLeadOutsideOfGridRecurse(bool[,] cellAutofillFlags, int x, int z, HashSet<ObjectPlacementPathTileConnectionGridCellIndices> traversedIndices)
        {
            int left = x - 1;
            int right = x + 1;
            int back = z - 1;
            int forward = z + 1;

            traversedIndices.Add(new ObjectPlacementPathTileConnectionGridCellIndices(x, z));

            if (left < MinCellIndexX || right > MaxCellIndexX ||
                back < MinCellIndexZ || forward > MaxCellIndexZ)
            {
                cellAutofillFlags[x - MinCellIndexX, z - MinCellIndexZ] = true;
                return true;
            }

            if (!IsCellOccupied(left, z) && !traversedIndices.Contains(new ObjectPlacementPathTileConnectionGridCellIndices(left, z)))
            {
                if (DetectCellsWhichLeadOutsideOfGridRecurse(cellAutofillFlags, left, z, traversedIndices))
                {
                    cellAutofillFlags[x - MinCellIndexX, z - MinCellIndexZ] = true;
                    return true;
                }
            }

            if (!IsCellOccupied(right, z) && !traversedIndices.Contains(new ObjectPlacementPathTileConnectionGridCellIndices(right, z)))
            {
                if (DetectCellsWhichLeadOutsideOfGridRecurse(cellAutofillFlags, right, z, traversedIndices))
                {
                    cellAutofillFlags[x - MinCellIndexX, z - MinCellIndexZ] = true;
                    return true;
                }
            }

            if (!IsCellOccupied(x, back) && !traversedIndices.Contains(new ObjectPlacementPathTileConnectionGridCellIndices(x, back)))
            {
                if (DetectCellsWhichLeadOutsideOfGridRecurse(cellAutofillFlags, x, back, traversedIndices))
                {
                    cellAutofillFlags[x - MinCellIndexX, z - MinCellIndexZ] = true;
                    return true;
                }
            }

            if (!IsCellOccupied(x, forward) && !traversedIndices.Contains(new ObjectPlacementPathTileConnectionGridCellIndices(x, forward)))
            {
                if (DetectCellsWhichLeadOutsideOfGridRecurse(cellAutofillFlags, x, forward, traversedIndices))
                {
                    cellAutofillFlags[x - MinCellIndexX, z - MinCellIndexZ] = true;
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
#endif