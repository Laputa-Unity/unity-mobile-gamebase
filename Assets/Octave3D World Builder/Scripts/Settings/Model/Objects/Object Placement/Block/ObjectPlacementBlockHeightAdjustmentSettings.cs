#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockHeightAdjustmentSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementBlockHeightAdjustmentMode _heightAdjustmentMode = ObjectPlacementBlockHeightAdjustmentMode.Manual;
        [SerializeField]
        private ObjectPlacementBlockManualHeightAdjustmentSettings _manualHeightAdjustmentSettings;
        [SerializeField]
        private ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings _automaticRandomHeightAdjustmentSettings;

        [SerializeField]
        private ObjectPlacementBlockHeightAdjustmentSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectPlacementBlockHeightAdjustmentMode HeightAdjustmentMode 
        { 
            get { return _heightAdjustmentMode; } 
            set 
            { 
                _heightAdjustmentMode = value;
                ObjectPlacementBlockHeightAdjustmentModeWasChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public ObjectPlacementBlockManualHeightAdjustmentSettings ManualHeightAdjustmentSettings
        {
            get
            {
                if (_manualHeightAdjustmentSettings == null) _manualHeightAdjustmentSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementBlockManualHeightAdjustmentSettings>();
                return _manualHeightAdjustmentSettings;
            }
        }
        public ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings AutomaticRandomHeightAdjustmentSettings
        {
            get
            {
                if (_automaticRandomHeightAdjustmentSettings == null) _automaticRandomHeightAdjustmentSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings>();
                return _automaticRandomHeightAdjustmentSettings;
            }
        }
        public ObjectPlacementBlockHeightAdjustmentSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockHeightAdjustmentSettings()
        {
            _view = new ObjectPlacementBlockHeightAdjustmentSettingsView(this);
        }
        #endregion
    }
}
#endif