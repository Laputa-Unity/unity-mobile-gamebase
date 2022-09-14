#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public abstract class Octree<T>
    {
        #region Protected Variables
        protected float _minLeafSize = 1.0f;
        protected OctreeNode _root;
        protected bool _wasBuilt = false;

        protected List<T> _data;
        #endregion

        #region Constructors
        public Octree(float minLeafSize)
        {
            _minLeafSize = minLeafSize;
        }
        #endregion

        #region Public Methods
        public void Build(List<T> data)
        {
            if (data == null || data.Count == 0) return;

            _data = new List<T>(data);
            _root = new OctreeNode(CreateRootNodeAABB(data), false);

            List<int> dataIndices = new List<int>(_data.Count);
            for (int index = 0; index < _data.Count; ++index) dataIndices.Add(index);

            EditorUtility.DisplayProgressBar("Octree Build", "Building tree. Please wait...", 0.5f);
            BuildRecurse(_root, dataIndices);
            _wasBuilt = true;
            EditorUtility.ClearProgressBar();
        }

        public void RenderGizmosDebug(Matrix4x4 transformMatrix, Color aabbLineColor)
        {
            if (!_wasBuilt) return;

            GizmosMatrix.Push(transformMatrix);
            GizmosColor.Push(aabbLineColor);

            RenderGizmosDebugRecurse(_root);

            GizmosColor.Pop();
            GizmosMatrix.Pop();
        }

        public List<int> CollectDataIndicesInLeavesAABB(Box aabb)
        {
            if (!_wasBuilt) return new List<int>();

            var dataIndices = new List<int>(100);
            CollectDataIndicesInLeavesAABBrecurse(aabb, dataIndices, _root);

            return dataIndices;
        }

        public List<OctreeNode> CollectLeavesAABB(Box aabb)
        {
            if (!_wasBuilt) return new List<OctreeNode>();

            var leaves = new List<OctreeNode>(50);
            CollectLeavesAABBRecurse(aabb, leaves, _root);

            return leaves;
        }

        public void ReclassifyData(int dataIndex, List<OctreeNode> parentLeaves)
        {
            if (!_wasBuilt) return;

            foreach (var leaf in parentLeaves) leaf.RemoveDataIndex(dataIndex);
            ReclassifyDataRecurse(dataIndex, _root);
        }
        #endregion

        #region Protected Abstract Methods
        protected abstract Box CreateRootNodeAABB(List<T> data);
        protected abstract List<int> GatherIndicesOfDataWhichFallsWithinNodeAABB(List<int> dataIndices, OctreeNode node);
        protected abstract bool DoesDataFallWithinNodeAABB(T data, OctreeNode node);
        #endregion

        #region Private Methods
        private void BuildRecurse(OctreeNode root, List<int> dataIndices)
        {
            Box rootAABB = root.AABB;

            // If we reached the minimum size, we will create a leaf node. We will
            // also create a leaf node if no more data exists.
            if (rootAABB.Size.sqrMagnitude <= (_minLeafSize * _minLeafSize) || dataIndices.Count == 0)
            {
                root.IsLeaf = true;
                root.DataIndices = new HashSet<int>(dataIndices);
                return;
            }

            // Divide the node into 8 subnodes
            Vector3 newNodeSize = root.AABB.Extents;
            Vector3 newNodeExtents = newNodeSize * 0.5f;
            Vector3[] aabbCenters = new Vector3[]
            {
                rootAABB.Center - Vector3.right * newNodeExtents.x - Vector3.up * newNodeExtents.y - Vector3.forward * newNodeExtents.z,
                rootAABB.Center - Vector3.right * newNodeExtents.x + Vector3.up * newNodeExtents.y - Vector3.forward * newNodeExtents.z,

                rootAABB.Center + Vector3.right * newNodeExtents.x - Vector3.up * newNodeExtents.y - Vector3.forward * newNodeExtents.z,
                rootAABB.Center + Vector3.right * newNodeExtents.x + Vector3.up * newNodeExtents.y - Vector3.forward * newNodeExtents.z,

                rootAABB.Center - Vector3.right * newNodeExtents.x - Vector3.up * newNodeExtents.y + Vector3.forward * newNodeExtents.z,
                rootAABB.Center - Vector3.right * newNodeExtents.x + Vector3.up * newNodeExtents.y + Vector3.forward * newNodeExtents.z,

                rootAABB.Center + Vector3.right * newNodeExtents.x - Vector3.up * newNodeExtents.y + Vector3.forward * newNodeExtents.z,
                rootAABB.Center + Vector3.right * newNodeExtents.x + Vector3.up * newNodeExtents.y + Vector3.forward * newNodeExtents.z
            };
          
            for(int subnodeIndex = 0; subnodeIndex < 8; ++subnodeIndex)
            {
                OctreeNode subnode = new OctreeNode(new Box(aabbCenters[subnodeIndex], newNodeSize), false);
                root.AddChild(subnode);
          
                BuildRecurse(subnode, GatherIndicesOfDataWhichFallsWithinNodeAABB(dataIndices, subnode));
            }
        }

        private void RenderGizmosDebugRecurse(OctreeNode node)
        {
            Box nodeAABB = node.AABB;
            Gizmos.DrawWireCube(nodeAABB.Center, nodeAABB.Size);
           
            List<OctreeNode> children = node.Children;
            foreach (var child in children) RenderGizmosDebugRecurse(child);
        }

        private void CollectDataIndicesInLeavesAABBrecurse(Box aabb, List<int> dataIndices, OctreeNode root)
        {
            if (!root.AABB.IntersectsBox(aabb)) return;

            if (root.IsLeaf) dataIndices.AddRange(root.DataIndices);
            else
            {
                List<OctreeNode> children = root.Children;
                foreach (var child in children) CollectDataIndicesInLeavesAABBrecurse(aabb, dataIndices, child);
            }
        }

        private void CollectLeavesAABBRecurse(Box aabb, List<OctreeNode> leaves, OctreeNode root)
        {
            if (!root.AABB.IntersectsBox(aabb)) return;

            if (root.IsLeaf) leaves.Add(root);
            else
            {
                List<OctreeNode> children = root.Children;
                foreach (var child in children) CollectLeavesAABBRecurse(aabb, leaves, child);
            }
        }

        private void ReclassifyDataRecurse(int dataIndex, OctreeNode root)
        {
            if (!DoesDataFallWithinNodeAABB(_data[dataIndex], root)) return;

            if (root.IsLeaf) root.AddDataIndex(dataIndex);
            else
            {
                List<OctreeNode> children = root.Children;
                foreach (var child in children) ReclassifyDataRecurse(dataIndex, child);
            }
        }
        #endregion
    }
}
#endif