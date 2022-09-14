#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintBrushViewData : ScriptableObject
    {
        [SerializeField]
        private Vector2 _elementsScrollPos;
        [SerializeField]
        private int _elementsScrollViewHeight = 300;
        [SerializeField]
        private float _elementPreviewScale = 0.5f;
        [SerializeField]
        private int _numElementsPerRow = 10;
        [SerializeField]
        private Color _activeElementTintColor = ColorExtensions.FromRGBA(255, 165, 0, 255);
        [SerializeField]
        private Color _disabledElementTintColor = Color.red;

        public Vector2 ElementsScrollPos { get { return _elementsScrollPos; } set { _elementsScrollPos = value; } }
        public int ElementsScrollViewHeight { get { return _elementsScrollViewHeight; } set { _elementsScrollViewHeight = Mathf.Max(value, 100); } }
        public float ElementPreviewScale { get { return _elementPreviewScale; } set { _elementPreviewScale = Mathf.Clamp(value, 0.2f, 1.0f); } }
        public int NumElementsPerRow { get { return _numElementsPerRow; } set { _numElementsPerRow = Mathf.Max(value, 1); } }
        public Color ActiveElementTintColor { get { return _activeElementTintColor; } set { _activeElementTintColor = value; } }
        public Color DisabledElementTintColor { get { return _disabledElementTintColor; } set { _disabledElementTintColor = value; } }
    }
}
#endif