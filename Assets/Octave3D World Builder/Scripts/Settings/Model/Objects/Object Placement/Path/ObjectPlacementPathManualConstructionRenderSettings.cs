#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathManualConstructionRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _boxBorderLineColor = Color.white;

        [SerializeField]
        private ObjectPlacementPathManualConstructionRenderSettingsView _view;
        #endregion

        #region Public Properties
        public Color BoxBorderLineColor { get { return _boxBorderLineColor; } set { _boxBorderLineColor = value; } }
        public ObjectPlacementPathManualConstructionRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathManualConstructionRenderSettings()
        {
            _view = new ObjectPlacementPathManualConstructionRenderSettingsView(this);
        }
        #endregion
    }
}
#endif