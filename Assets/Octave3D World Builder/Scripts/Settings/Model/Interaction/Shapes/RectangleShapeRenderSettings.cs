#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class RectangleShapeRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _borderLineColor = Color.white;
        [SerializeField]
        private Color _fillColor = Color.white;

        [SerializeField]
        private RectangleShapeRenderSettingsView _view;
        #endregion

        #region Public Properties
        public Color BorderLineColor { get { return _borderLineColor; } set { _borderLineColor = value; } }
        public Color FillColor { get { return _fillColor; } set { _fillColor = value; } }
        public RectangleShapeRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public RectangleShapeRenderSettings()
        {
            _view = new RectangleShapeRenderSettingsView(this);
        }
        #endregion
    }
}
#endif