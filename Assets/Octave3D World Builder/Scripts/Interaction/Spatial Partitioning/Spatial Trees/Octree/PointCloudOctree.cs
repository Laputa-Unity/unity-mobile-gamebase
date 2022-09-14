#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class PointCloudOctree : Octree<Vector3>
    {
        #region Constructors
        public PointCloudOctree(float minLeafSize)
            : base(minLeafSize) 
        {
        }
        #endregion

        #region Protected Methods
        protected override Box CreateRootNodeAABB(List<Vector3> points)
        {
            // Note: We will scale the root node size by a small factor in order to account for
            //       floating point rounding errors. Otherwise, it is possible that the tree
            //       build algorithm will ignore certain positions because they fall outside
            //       of the root when in fact they are not.
            return Box.FromPoints(points, 1.1f);
        }

        protected override List<int> GatherIndicesOfDataWhichFallsWithinNodeAABB(List<int> dataIndices, OctreeNode node)
        {
            if (dataIndices.Count == 0) return new List<int>();

            var indicesOfPointsInsideAABB = new List<int>(dataIndices.Count);
            foreach(int index in dataIndices)
            {
                Vector3 point = _data[index];
                if (DoesDataFallWithinNodeAABB(point, node)) indicesOfPointsInsideAABB.Add(index);
            }

            return indicesOfPointsInsideAABB;
        }

        protected override bool DoesDataFallWithinNodeAABB(Vector3 data, OctreeNode node)
        {
            return node.AABB.ContainsPoint(data);
        }
        #endregion
    }
}
#endif