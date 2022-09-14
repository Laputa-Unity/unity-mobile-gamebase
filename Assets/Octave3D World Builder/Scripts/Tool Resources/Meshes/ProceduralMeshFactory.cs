#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ProceduralMeshFactory
    {
        #region Public Static Functions
        public static Mesh CreateTriangleMesh(Vector3[] vertexPositions, int[] vertexIndices, Color[] vertexColors, Vector3[] vertexNormals)
        {
            var mesh = new Mesh();
            mesh.vertices = vertexPositions;
            mesh.SetIndices(vertexIndices, MeshTopology.Triangles, 0);
            mesh.colors = vertexColors;
            mesh.normals = vertexNormals;

            return mesh;
        }
        #endregion
    }
}
#endif