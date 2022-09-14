#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementBlockManualConstructionSettings _manualConstructionSettings;

        [SerializeField]
        private ObjectPlacementBlockProjectionSettings _blockProjectionSettings = new ObjectPlacementBlockProjectionSettings();

        [SerializeField]
        private ObjectPlacementBlockSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectPlacementBlockManualConstructionSettings ManualConstructionSettings
        {
            get
            {
                if (_manualConstructionSettings == null) _manualConstructionSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementBlockManualConstructionSettings>();
                return _manualConstructionSettings;
            }
        }
        public ObjectPlacementBlockProjectionSettings BlockProjectionSettings { get { return _blockProjectionSettings; } }
        public ObjectPlacementBlockSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockSettings()
        {
            _view = new ObjectPlacementBlockSettingsView(this);
        }
        #endregion
    }
}
#endif