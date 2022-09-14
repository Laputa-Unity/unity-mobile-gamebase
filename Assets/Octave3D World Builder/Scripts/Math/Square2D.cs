#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public struct Square2D
    {
        #region Private Variables
        private Vector2 _center;
        private float _sideLength;
        #endregion

        #region Public Properties
        public Vector2 Center { get { return _center; } set { _center = value; } }
        public float SideLength { get { return _sideLength; } set { _sideLength = Mathf.Max(0.0f, value); } }
        #endregion

        #region Constructors
        public Square2D(float sideLength)
        {
            _center = Vector2.zero;
            _sideLength = 0.0f;

            SideLength = sideLength;
        }

        public Square2D(Vector2 center, float sideLength)
        {
            _center = center;
            _sideLength = 0.0f;

            SideLength = sideLength;
        }
        #endregion

        #region Public Methods
        public Rect ToRectangle()
        {
            Vector2 minPointPosition = new Vector2(_center.x - _sideLength * 0.5f, _center.y - _sideLength * 0.5f);
            return new Rect(minPointPosition.x, minPointPosition.y, _sideLength, _sideLength);
        }
        #endregion
    }
}
#endif