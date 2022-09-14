#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PathObjectPlacementSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private AxisAlignmentSettings _placementGuideSurfaceAlignmentSettings;

        [SerializeField]
        private PathObjectPlacementSettingsView _view;
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

        public PathObjectPlacementSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public PathObjectPlacementSettings()
        {
            _view = new PathObjectPlacementSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static PathObjectPlacementSettings Get()
        {
            return ObjectPlacementSettings.Get().PathPlacementSettings;
        }
        #endregion
    }
}
#endif