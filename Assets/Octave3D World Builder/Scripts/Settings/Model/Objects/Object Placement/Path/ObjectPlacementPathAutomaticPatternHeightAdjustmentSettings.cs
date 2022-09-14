#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _applyPatternsContinuously = true;
        [SerializeField]
        private bool _wrapPatterns = true;

        [SerializeField]
        private ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsView _view;
        #endregion

        #region Public Properties
        public bool ApplyPatternsContinuously 
        { 
            get { return _applyPatternsContinuously; } 
            set
            { 
                _applyPatternsContinuously = value;
                ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public bool WrapPatterns 
        { 
            get { return _wrapPatterns; } 
            set 
            { 
                _wrapPatterns = value;
                ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings()
        {
            _view = new ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsView(this);
        }
        #endregion
    }
}
#endif