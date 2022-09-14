#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementBlockManualConstructionRenderSettings _manualConstructionRenderSettings;

        [SerializeField]
        private ObjectPlacementBlockRenderSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectPlacementBlockManualConstructionRenderSettings ManualConstructionRenderSettings
        {
            get
            {
                if (_manualConstructionRenderSettings == null) _manualConstructionRenderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementBlockManualConstructionRenderSettings>();
                return _manualConstructionRenderSettings;
            }
        }
        public ObjectPlacementBlockRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockRenderSettings()
        {
            _view = new ObjectPlacementBlockRenderSettingsView(this);
        }
        #endregion
    }
}
#endif