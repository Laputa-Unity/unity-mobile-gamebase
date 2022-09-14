#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private int _minHeight = -5;
        [SerializeField]
        private int _maxHeight = 5;

        [SerializeField]
        private ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsView _view;
        #endregion

        #region Public Properties
        public int MinHeight
        { 
            get { return _minHeight; } 
            set 
            { 
                _minHeight = Mathf.Min(value, _maxHeight - 1);
                ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int MaxHeight
        {
            get { return _maxHeight; } 
            set 
            { 
                _maxHeight = Mathf.Max(value, _minHeight + 1);
                ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings()
        {
            _view = new ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsView(this);
        }
        #endregion
    }
}
#endif