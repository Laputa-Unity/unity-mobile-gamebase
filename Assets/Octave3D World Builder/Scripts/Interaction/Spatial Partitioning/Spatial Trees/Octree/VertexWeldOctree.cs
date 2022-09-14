#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class VertexWeldOctree : PointCloudOctree
    {
        #region Constructors
        public VertexWeldOctree(float minLeafSize)
            : base(minLeafSize)
        {
        }
        #endregion

        #region Public Methods
        public List<Vector3> WeldVertices(float weldEpsilon)
        {
            if (_data == null || _data.Count == 0) return new List<Vector3>();

            var processedIndices = new HashSet<int>();

            Vector3 vertexAABBSize = Vector3.one * 2.0f * (weldEpsilon + 1e-2f);        // Add a small value to account for floating point errors
            for(int vertexIndex = 0; vertexIndex < _data.Count; ++vertexIndex)
            {
                EditorUtility.DisplayProgressBar("Welding vertices", "Welding vertex positions. Please wait...", (float)vertexIndex / _data.Count);
                if (processedIndices.Contains(vertexIndex)) continue;
                else processedIndices.Add(vertexIndex);

                Vector3 vertexPos = _data[vertexIndex];
                Box vertexAABB = new Box(vertexPos, vertexAABBSize);

                // Collect all leaves which intersect the current vertex 
                List<OctreeNode> leaves = CollectLeavesAABB(vertexAABB);
                if(leaves.Count != 0)
                {
                    Vector3 posSum = vertexPos;
                    List<int> indicesOfWeldVerts = new List<int>();

                    // Loop through each leaf and identify the vertices which can be welded with the current vertex
                    foreach (OctreeNode leaf in leaves)
                    {
                        HashSet<int> indicesOfPossibleWeldVerts = leaf.DataIndices;
                        if (indicesOfPossibleWeldVerts.Count != 0)
                        {
                            foreach (int possibleWeldIndex in indicesOfPossibleWeldVerts)
                            {
                                Vector3 possibleWeldPos = _data[possibleWeldIndex];
                                if ((possibleWeldPos - vertexPos).magnitude <= weldEpsilon)
                                {
                                    indicesOfWeldVerts.Add(possibleWeldIndex);
                                    processedIndices.Add(possibleWeldIndex);
                                    posSum += possibleWeldPos;
                                }
                            }
                        }
                    }

                    if (indicesOfWeldVerts.Count != 0)
                    {
                        // When welding the vertices, their new position will be set to the average of the positions of all vertices involved
                        Vector3 posAverage = posSum / (indicesOfWeldVerts.Count + 1);

                        // Note: When modifying the position of a vertex, it is possible that the new position will end up in a new leaf, so
                        //       we need to call 'ReclassifyData' to account for this.
                        // Note: Strike that. This is wrong. When a group of vertices are welded, they must be eliminated from further processing.
                        //       Wd don't need to reclasify anything.
                        _data[vertexIndex] = posAverage;
                        //ReclassifyData(vertexIndex, leaves);

                        foreach (int weldVertIndex in indicesOfWeldVerts)
                        {
                            _data[weldVertIndex] = posAverage;
                           // ReclassifyData(weldVertIndex, leaves);
                        }
                    }
                }              
            }
            EditorUtility.ClearProgressBar();

            return new List<Vector3>(_data);
        }
        #endregion
    }
}
#endif