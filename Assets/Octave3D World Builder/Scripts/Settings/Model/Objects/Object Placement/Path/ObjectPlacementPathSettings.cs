#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementPathManualConstructionSettings _manualConstructionSettings;

        [SerializeField]
        private ObjectPlacementPathTileConnectionSettings _tileConnectionSettings;

        [SerializeField]
        private ObjectPlacementPathSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectPlacementPathManualConstructionSettings ManualConstructionSettings
        {
            get
            {
                if (_manualConstructionSettings == null) _manualConstructionSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathManualConstructionSettings>();
                return _manualConstructionSettings;
            }
        }
        public ObjectPlacementPathTileConnectionSettings TileConnectionSettings
        {
            get
            {
                if (_tileConnectionSettings == null) _tileConnectionSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathTileConnectionSettings>();
                return _tileConnectionSettings;
            }
        }

        public ObjectPlacementPathSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathSettings()
        {
            _view = new ObjectPlacementPathSettingsView(this);
        }
        #endregion
    }
}
#endif