#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class GizmosColor
    {
        #region Private Static Variables
        private static Color _initialColor = GUI.color;
        private static Stack<Color> _colorStack = new Stack<Color>();
        #endregion

        #region Constructors
        static GizmosColor()
        {
            _colorStack.Push(_initialColor);
        }
        #endregion

        #region Public Methods
        public static void Push(Color color)
        {
            _colorStack.Push(color);
            Gizmos.color = _colorStack.Peek();
        }

        public static void Pop()
        {
            if (_colorStack.Count > 1) _colorStack.Pop();
            Gizmos.color = _colorStack.Peek();
        }
        #endregion
    }
}
#endif