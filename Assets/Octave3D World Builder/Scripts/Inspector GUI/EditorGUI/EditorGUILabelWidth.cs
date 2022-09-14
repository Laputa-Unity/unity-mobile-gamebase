#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public static class EditorGUILabelWidth
    {
        #region Private Static Variables
        private static float _initialWidth = EditorGUIUtility.labelWidth;
        private static Stack<float> _labelWidths = new Stack<float>();
        #endregion

        #region Constructors
        static EditorGUILabelWidth()
        {
            _labelWidths.Push(_initialWidth);
        }
        #endregion

        #region Public Methods
        public static void Push(float labelWidth)
        {
            _labelWidths.Push(labelWidth);
            EditorGUIUtility.labelWidth = _labelWidths.Peek();
        }

        public static void Pop()
        {
            if (_labelWidths.Count > 1) _labelWidths.Pop();
            EditorGUIUtility.labelWidth = _labelWidths.Peek();
        }
        #endregion
    }
}
#endif