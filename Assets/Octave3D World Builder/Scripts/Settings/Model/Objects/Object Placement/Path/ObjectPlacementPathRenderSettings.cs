#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementPathManualConstructionRenderSettings _manualConstructionRenderSettings;

        [SerializeField]
        private ObjectPlacementPathRenderSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectPlacementPathManualConstructionRenderSettings ManualConstructionRenderSettings
        {
            get
            {
                if (_manualConstructionRenderSettings == null) _manualConstructionRenderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathManualConstructionRenderSettings>();
                return _manualConstructionRenderSettings;
            }
        }
        public ObjectPlacementPathRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathRenderSettings()
        {
            _view = new ObjectPlacementPathRenderSettingsView(this);
        }
        #endregion
    }
}
#endif