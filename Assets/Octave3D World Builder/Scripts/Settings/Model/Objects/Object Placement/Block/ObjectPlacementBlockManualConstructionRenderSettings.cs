#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockManualConstructionRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _boxBorderLineColor = Color.white;

        [SerializeField]
        private LabelRenderSettings _dimensionsLabelRenderSettings;

        [SerializeField]
        private ObjectPlacementBlockManualConstructionRenderSettingsView _view;
        #endregion

        #region Public Properties
        public Color BoxBorderLineColor { get { return _boxBorderLineColor; } set { _boxBorderLineColor = value; } }
        public LabelRenderSettings DimensionsLabelRenderSettings
        {
            get
            {
                if (_dimensionsLabelRenderSettings == null) _dimensionsLabelRenderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<LabelRenderSettings>();
                return _dimensionsLabelRenderSettings;
            }
        }
        public ObjectPlacementBlockManualConstructionRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockManualConstructionRenderSettings()
        {
            _view = new ObjectPlacementBlockManualConstructionRenderSettingsView(this);
        }
        #endregion
    }
}
#endif