#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathManualConstructionSessionData
    {
        #region Private Variables
        private ObjectPlacementPath _path;
        private List<ObjectPlacementBoxStackSegment> _pathSegments;
        private List<ObjectPlacementPathTileConnectionGridCell> _tileConnectionGridCells;
        private ObjectPlacementExtensionPlane _pathExtensionPlane;
        private GameObject _startObject;
        #endregion

        #region Public Properties
        public ObjectPlacementPath Path { get { return _path; } set { _path = value; } }
        public List<ObjectPlacementBoxStackSegment> PathSegments { get { return _pathSegments; } set { _pathSegments = value; } }
        public List<ObjectPlacementPathTileConnectionGridCell> TileConnectionGridCells { get { return _tileConnectionGridCells; } set { _tileConnectionGridCells = value; } }
        public ObjectPlacementExtensionPlane PathExtensionPlane { get { return _pathExtensionPlane; } set { _pathExtensionPlane = value; } }
        public GameObject StartObject { get { return _startObject; } set { _startObject = value; } }
        #endregion
    }
}
#endif