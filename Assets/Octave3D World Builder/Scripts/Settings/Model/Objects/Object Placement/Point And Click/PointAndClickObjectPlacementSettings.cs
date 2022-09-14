#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PointAndClickObjectPlacementSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _randomizePrefabsInActiveCategory = false;
        [SerializeField]
        private AxisAlignmentSettings _placementGuideSurfaceAlignmentSettings;
        [SerializeField]
        private ObjectRotationRandomizationSettings _placementGuideRotationRandomizationSettings;
        [SerializeField]
        private ObjectScaleRandomizationSettings _placementGuideScaleRandomizationSettings;

        [SerializeField]
        private PointAndClickObjectPlacementSettingsView _view;
        #endregion

        #region Public Properties
        public bool RandomizePrefabsInActiveCategory { get { return _randomizePrefabsInActiveCategory; } set { _randomizePrefabsInActiveCategory = value; } }
        public AxisAlignmentSettings PlacementGuideSurfaceAlignmentSettings
        {
            get
            {
                if (_placementGuideSurfaceAlignmentSettings == null) _placementGuideSurfaceAlignmentSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisAlignmentSettings>();
                return _placementGuideSurfaceAlignmentSettings;
            }        
        }
        public ObjectRotationRandomizationSettings PlacementGuideRotationRandomizationSettings
        {
            get
            {
                if (_placementGuideRotationRandomizationSettings == null) _placementGuideRotationRandomizationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectRotationRandomizationSettings>();
                return _placementGuideRotationRandomizationSettings;
            }
        }
        public ObjectScaleRandomizationSettings PlacementGuideScaleRandomizationSettings
        {
            get
            {
                if (_placementGuideScaleRandomizationSettings == null) _placementGuideScaleRandomizationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectScaleRandomizationSettings>();
                return _placementGuideScaleRandomizationSettings;
            }
        }

        public PointAndClickObjectPlacementSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public PointAndClickObjectPlacementSettings()
        {
            _view = new PointAndClickObjectPlacementSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static PointAndClickObjectPlacementSettings Get()
        {
            return ObjectPlacementSettings.Get().PointAndClickPlacementSettings;
        }
        #endregion
    }
}
#endif