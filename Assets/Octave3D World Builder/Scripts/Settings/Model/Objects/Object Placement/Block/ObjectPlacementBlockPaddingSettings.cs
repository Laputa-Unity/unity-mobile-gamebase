#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockPaddingSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _paddingAlongGrowDirection = 0.0f;
        [SerializeField]
        private float _paddingAlongExtensionPlane = 0.0f;

        [SerializeField]
        private ObjectPlacementBlockPaddingSettingsView _view;
        #endregion

        #region Public Properties
        public float PaddingAlongGrowDirection 
        { 
            get { return _paddingAlongGrowDirection; } 
            set 
            {
                _paddingAlongGrowDirection = value;
                ObjectPlacementBlockPaddingSettingsWereChangedMessage.SendToInterestedListeners(this);
            }
        }
        public float PaddingAlongExtensionPlane 
        { 
            get { return _paddingAlongExtensionPlane; } 
            set 
            {
                _paddingAlongExtensionPlane = value;
                ObjectPlacementBlockPaddingSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public ObjectPlacementBlockPaddingSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockPaddingSettings()
        {
            _view = new ObjectPlacementBlockPaddingSettingsView(this);
        }
        #endregion
    }
}
#endif