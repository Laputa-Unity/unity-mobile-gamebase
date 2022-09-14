#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementBlockManualConstructionSessionData
    {
        #region Private Variables
        private ObjectPlacementBlock _block;
        private GameObject _startObject;
        private List<ObjectPlacementBoxStackSegment> _blockSegments;
        private ObjectPlacementExtensionPlane _blockExtensionPlane;
        #endregion

        #region Public Properties
        public ObjectPlacementBlock Block { get { return _block; } set { _block = value; } }
        public GameObject StartObject { get { return _startObject; } set { _startObject = value; } }
        public List<ObjectPlacementBoxStackSegment> BlockSegments { get { return _blockSegments; } set { _blockSegments = value; } }
        public ObjectPlacementExtensionPlane BlockExtensionPlane { get { return _blockExtensionPlane; } set { _blockExtensionPlane = value; } }
        #endregion
    }
}
#endif