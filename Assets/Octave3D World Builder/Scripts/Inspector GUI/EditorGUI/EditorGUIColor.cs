#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public static class EditorGUIColor
    {
        #region Private Static Variables
        private static Color _baseColor = Color.white;
        private static Stack<Color> _colorStack = new Stack<Color>();
        #endregion

        #region Constructors
        static EditorGUIColor()
        {
            _colorStack.Push(_baseColor);
        }
        #endregion

        #region Public Methods
        public static void Push(Color color)
        {
            _colorStack.Push(color);
            GUI.color = _colorStack.Peek();
        }

        public static void Pop()
        {
            if (_colorStack.Count > 1) _colorStack.Pop();
            GUI.color = _colorStack.Peek();
        }
        #endregion
    }
}
#endif