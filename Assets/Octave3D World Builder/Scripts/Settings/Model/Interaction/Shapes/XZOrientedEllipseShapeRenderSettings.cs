#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class XZOrientedEllipseShapeRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _borderLineColor = Color.white;

        [SerializeField]
        private XZOrientedEllipseShapeRenderSettingsView _view;
        #endregion

        #region Public Properties
        public Color BorderLineColor { get { return _borderLineColor; } set { _borderLineColor = value; } }
        public XZOrientedEllipseShapeRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public XZOrientedEllipseShapeRenderSettings()
        {
            _view = new XZOrientedEllipseShapeRenderSettingsView(this);
        }
        #endregion
    }
}
#endif