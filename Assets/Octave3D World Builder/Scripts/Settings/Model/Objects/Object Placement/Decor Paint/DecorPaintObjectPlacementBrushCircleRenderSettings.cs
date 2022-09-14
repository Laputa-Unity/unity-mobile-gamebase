#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushCircleRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _circleLineColor = Color.green;

        [SerializeField]
        private DecorPaintObjectPlacementBrushCircleRenderSettingsView _view;
        #endregion

        #region Public Properties
        public Color CircleLineColor { get { return _circleLineColor; } set { _circleLineColor = value; } }
        public DecorPaintObjectPlacementBrushCircleRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrushCircleRenderSettings()
        {
            _view = new DecorPaintObjectPlacementBrushCircleRenderSettingsView(this);
        }
        #endregion
    }
}
#endif