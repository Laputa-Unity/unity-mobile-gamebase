#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public struct Ray2D
    {
        #region Private Variables
        private Vector2 _origin;
        private Vector2 _direction;
        #endregion

        #region Public Properties
        public Vector2 Origin { get { return _origin; } set { _origin = value; } }
        public Vector2 Direction { get { return _direction; } set { _direction = value; } }
        #endregion

        #region Constructors
        public Ray2D(Vector2 origin, Vector2 direction)
        {
            _origin = origin;
            _direction = direction;
        }
        #endregion
    }
}
#endif