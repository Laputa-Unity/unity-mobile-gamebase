#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class CoordinateSystemAxisRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private CoordinateSystemAxis _axis;

        [SerializeField]
        private bool _isVisible = true;
        [SerializeField]
        private Color _color;
        [SerializeField]
        private bool _isInfinite = false;
        [SerializeField]
        private float _finiteSize = 5.0f;

        [SerializeField]
        private CoordinateSystemAxisRenderSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinFiniteAxisSize { get { return 0.1f; } }
        public static float InfniteAxisSize { get { return 999999999.999999999f; } }
        #endregion

        #region Public Properties
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        public Color Color { get { return _color; } set { _color = value; } }
        public bool IsInfinite { get { return _isInfinite; } set { _isInfinite = value; } }
        public float FiniteSize { get { return _finiteSize; } set { _finiteSize = Mathf.Max(MinFiniteAxisSize, value); } }
        public CoordinateSystemAxis Axis { get { return _axis; } set { _axis = value; } }
        public CoordinateSystemAxisRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public CoordinateSystemAxisRenderSettings()
        {
            _view = new CoordinateSystemAxisRenderSettingsView(this);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This is a convenience method which allows the client code to retrieve the
        /// size of the axis without having to worry about whether or not the axis is
        /// rendered infinite. If the axis is rendered infinite, the method will return
        /// the value of the 'InfniteAxisSize' property.
        /// </summary>
        public float GetSize()
        {
            if (IsInfinite) return InfniteAxisSize;
            else return FiniteSize;
        }
        #endregion
    }
}
#endif