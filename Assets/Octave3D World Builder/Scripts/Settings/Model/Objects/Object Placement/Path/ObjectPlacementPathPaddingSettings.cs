#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathPaddingSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _paddingAlongGrowDirection = 0.0f;
        [SerializeField]
        private float _paddingAlongExtensionPlane = 0.0f;

        [SerializeField]
        private ObjectPlacementPathPaddingSettingsView _view;
        #endregion

        #region Public Properties
        public float PaddingAlongGrowDirection 
        { 
            get { return _paddingAlongGrowDirection; } 
            set 
            {
                _paddingAlongGrowDirection = value;
                ObjectPlacementPathPaddingSettingsWereChangedMessage.SendToInterestedListeners(this);
            }
        }
        public float PaddingAlongExtensionPlane 
        { 
            get { return _paddingAlongExtensionPlane; } 
            set 
            {
                _paddingAlongExtensionPlane = value;
                ObjectPlacementPathPaddingSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public ObjectPlacementPathPaddingSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathPaddingSettings()
        {
            _view = new ObjectPlacementPathPaddingSettingsView(this);
        }
        #endregion
    }
}
#endif