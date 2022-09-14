#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class BlockObjectPlacementSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private AxisAlignmentSettings _placementGuideSurfaceAlignmentSettings;
        
        [SerializeField]
        private BlockObjectPlacementSettingsView _view;
        #endregion

        #region Public Properties
        public AxisAlignmentSettings PlacementGuideSurfaceAlignmentSettings
        {
            get
            {
                if (_placementGuideSurfaceAlignmentSettings == null) _placementGuideSurfaceAlignmentSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisAlignmentSettings>();
                return _placementGuideSurfaceAlignmentSettings;
            }
        }
        public BlockObjectPlacementSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public BlockObjectPlacementSettings()
        {
            _view = new BlockObjectPlacementSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static BlockObjectPlacementSettings Get()
        {
            return ObjectPlacementSettings.Get().BlockObjectPlacementSettings;
        }
        #endregion
    }
}
#endif