#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightAdjustmentSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementPathHeightAdjustmentMode _heightAdjustmentMode = ObjectPlacementPathHeightAdjustmentMode.Manual;
        [SerializeField]
        private ObjectPlacementPathManualHeightAdjustmentSettings _manualHeightAdjustmentSettings;
        [SerializeField]
        private ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings _automaticRandomHeightAdjustmentSettings;
        [SerializeField]
        private ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings _automaticPatternHeightAdjustmentSettings;

        [SerializeField]
        private ObjectPlacementPathHeightAdjustmentSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectPlacementPathHeightAdjustmentMode HeightAdjustmentMode 
        { 
            get { return _heightAdjustmentMode; } 
            set 
            { 
                _heightAdjustmentMode = value;
                ObjectPlacementPathHeightAdjustmentModeWasChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public ObjectPlacementPathManualHeightAdjustmentSettings ManualHeightAdjustmentSettings
        {
            get
            {
                if (_manualHeightAdjustmentSettings == null) _manualHeightAdjustmentSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathManualHeightAdjustmentSettings>();
                return _manualHeightAdjustmentSettings;
            }
        }
        public ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings AutomaticRandomHeightAdjustmentSettings
        {
            get
            {
                if (_automaticRandomHeightAdjustmentSettings == null) _automaticRandomHeightAdjustmentSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings>();
                return _automaticRandomHeightAdjustmentSettings;
            }
        }
        public ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings AutomaticPatternHeightAdjustmentSettings
        {
            get
            {
                if (_automaticPatternHeightAdjustmentSettings == null) _automaticPatternHeightAdjustmentSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings>();
                return _automaticPatternHeightAdjustmentSettings;
            }
        }
        public ObjectPlacementPathHeightAdjustmentSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathHeightAdjustmentSettings()
        {
            _view = new ObjectPlacementPathHeightAdjustmentSettingsView(this);
        }
        #endregion
    }
}
#endif