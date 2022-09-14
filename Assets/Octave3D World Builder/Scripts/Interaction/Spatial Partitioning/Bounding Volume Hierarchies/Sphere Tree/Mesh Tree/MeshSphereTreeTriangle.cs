#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class MeshSphereTreeTriangle
    {
        #region Private Variables
        [SerializeField]
        private int _triangleIndex;
        #endregion

        #region Public Properties
        public int TriangleIndex { get { return _triangleIndex; } }
        #endregion

        #region Constructors
        public MeshSphereTreeTriangle(int triangleIndex)
        {
            _triangleIndex = triangleIndex;
        }
        #endregion
    }
}
#endif