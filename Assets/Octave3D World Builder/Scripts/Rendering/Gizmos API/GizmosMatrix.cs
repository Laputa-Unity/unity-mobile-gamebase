#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class GizmosMatrix
    {
        #region Private Static Variables
        private static Matrix4x4 _initialMatrix = Matrix4x4.identity;
        private static Stack<Matrix4x4> _matrixStack = new Stack<Matrix4x4>();
        #endregion

        #region Constructors
        static GizmosMatrix()
        {
            _matrixStack.Push(_initialMatrix);
        }
        #endregion

        #region Public Methods
        public static void Push(Matrix4x4 matrix)
        {
            _matrixStack.Push(matrix);
            Gizmos.matrix = _matrixStack.Peek();
        }

        public static void Pop()
        {
            if (_matrixStack.Count > 1) _matrixStack.Pop();
            Gizmos.matrix = _matrixStack.Peek();
        }
        #endregion
    }
}
#endif