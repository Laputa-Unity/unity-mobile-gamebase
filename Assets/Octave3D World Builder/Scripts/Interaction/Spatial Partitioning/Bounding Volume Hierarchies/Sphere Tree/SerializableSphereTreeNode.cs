#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class SerializableSphereTreeNode<T>
    {
        #region Private Variables
        [SerializeField]
        private T _nodeData;
        [SerializeField]
        private int _parentNodeIndex;
        [SerializeField]
        private Vector3 _sphereCenter;
        [SerializeField]
        private float _sphereRadius;
        [SerializeField]
        private SphereTreeNodeFlags _flags;
        #endregion

        #region Public Properties
        public T NodeData { get { return _nodeData; } set { _nodeData = value; } }
        public int ParentNodeIndex { get { return _parentNodeIndex; } set { _parentNodeIndex = value; } }
        public Vector3 SphereCenter { get { return _sphereCenter; } set { _sphereCenter = value; } }
        public float SphereRadius { get { return _sphereRadius; } set { _sphereRadius = value; } }
        public SphereTreeNodeFlags Flags { get { return _flags; } set { _flags = value; } }
        #endregion
    }
}
#endif