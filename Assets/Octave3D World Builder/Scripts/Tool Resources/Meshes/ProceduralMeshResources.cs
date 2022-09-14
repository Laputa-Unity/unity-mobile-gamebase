#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ProceduralMeshResources
    {
        #region Private Variables
        private List<Mesh> _meshes = new List<Mesh>();
        #endregion

        #region Public Methods
        public Mesh CreateXYRectangleMesh(float width, float height, Color color)
        {
            Mesh mesh = ProceduralMeshGeneration.CreateXYRectangleMesh(width, height, color);
            if (mesh != null) _meshes.Add(mesh);

            return mesh;
        }

        public Mesh CreateXZRectangleMesh(float width, float depth, Color color)
        {
            Mesh mesh = ProceduralMeshGeneration.CreateXZRectangleMesh(width, depth, color);
            if (mesh != null) _meshes.Add(mesh);

            return mesh;
        }

        public Mesh CreateXYCircleMesh(float radius, int numberOfSlices, Color color)
        {
            Mesh mesh = ProceduralMeshGeneration.CreateXYCircleMesh(radius, numberOfSlices, color);
            if (mesh != null) _meshes.Add(mesh);

            return mesh;
        }

        public void DisposeMeshes()
        {
            foreach(Mesh mesh in _meshes)
            {
                if (mesh != null) Octave3DWorldBuilder.DestroyImmediate(mesh);
            }
            _meshes.Clear();
        }

        public void DisposeMesh(Mesh mesh)
        {
            int meshIndex = _meshes.FindIndex(item => item == mesh);
            if (meshIndex >= 0)
            {
                Octave3DWorldBuilder.DestroyImmediate(_meshes[meshIndex]);
                _meshes.RemoveAt(meshIndex);
            }
        }
        #endregion
    }
}
#endif