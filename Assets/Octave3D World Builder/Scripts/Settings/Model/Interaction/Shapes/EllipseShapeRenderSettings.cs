#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class EllipseShapeRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _borderLineColor = Color.white;
        [SerializeField]
        private Color _fillColor = Color.white;

        [SerializeField]
        private EllipseShapeRenderSettingsView _view;
        #endregion

        #region Public Properties
        public Color BorderLineColor { get { return _borderLineColor; } set { _borderLineColor = value; } }
        public Color FillColor { get { return _fillColor; } set { _fillColor = value; } }
        public EllipseShapeRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public EllipseShapeRenderSettings()
        {
            _view = new EllipseShapeRenderSettingsView(this);
        }
        #endregion
    }
}
#endif