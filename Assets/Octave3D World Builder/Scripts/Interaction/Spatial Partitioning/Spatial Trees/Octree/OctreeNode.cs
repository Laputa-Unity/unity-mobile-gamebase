#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class OctreeNode
    {
        #region Private Variables
        private Box _aabb;
        private bool _isLeaf = false;
        private List<OctreeNode> _children = new List<OctreeNode>(8);
        private HashSet<int> _dataIndices = new HashSet<int>();
        #endregion

        #region Public Properties
        public Box AABB { get { return _aabb; } }
        public bool IsLeaf { get { return _isLeaf; } set { _isLeaf = value; } }
        public List<OctreeNode> Children { get { return new List<OctreeNode>(_children); } }
        public HashSet<int> DataIndices { get { return new HashSet<int>(_dataIndices); } set { if (value != null) _dataIndices = value; } }
        #endregion

        #region Constructors
        public OctreeNode(Box aabb, bool isLeaf)
        {
            _aabb = aabb;
            _isLeaf = isLeaf;
        }
        #endregion

        #region Public Methods
        public void AddChild(OctreeNode child)
        {
            if (_children.Count == 8) return;
            _children.Add(child);
        }

        public void RemoveDataIndex(int dataIndex)
        {
            _dataIndices.Remove(dataIndex);
        }

        public bool ContainsDataIndex(int dataIndex)
        {
            return _dataIndices.Contains(dataIndex);
        }

        public void AddDataIndex(int dataIndex)
        {
            if (!ContainsDataIndex(dataIndex)) _dataIndices.Add(dataIndex);
        }
        #endregion
    }
}
#endif