#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisAlignmentSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _isEnabled = false;
        [SerializeField]
        private CoordinateSystemAxis _alignmentAxis = CoordinateSystemAxis.PositiveUp;

        [SerializeField]
        private AxisAlignmentSettingsView _view;
        #endregion

        #region Public Properties
        public bool IsEnabled { get { return _isEnabled; } set { _isEnabled = value; } }
        public CoordinateSystemAxis AlignmentAxis { get { return _alignmentAxis; } set { _alignmentAxis = value; } }
        public AxisAlignmentSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public AxisAlignmentSettings()
        {
            _view = new AxisAlignmentSettingsView(this);
        }
        #endregion
    }
}
#endif