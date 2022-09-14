#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class SingleDecorPaintModeObjectPlacementSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _alignToStroke = false;
        [SerializeField]
        private bool _embedInSurfaceWhenNoAlign = true;
        [SerializeField]
        private bool _randomizePrefabsInActiveCategory = false;
        [SerializeField]
        private bool _useOriginalPivot = false;

        [SerializeField]
        private AxisAlignmentSettings _placementGuideSurfaceAlignmentSettings;
        [SerializeField]
        private ObjectRotationRandomizationSettings _placementGuideRotationRandomizationSettings;
        [SerializeField]
        private ObjectScaleRandomizationSettings _placementGuideScaleRandomizationSettings;

        [SerializeField]
        private SingleDecorPaintModeObjectPlacementSettingsView _view;
        #endregion

        #region Public Properties
        public bool EmbedInSurfaceWhenNoAlign { get { return _embedInSurfaceWhenNoAlign; } set { _embedInSurfaceWhenNoAlign = value; } }
        public bool AlignToStroke { get { return _alignToStroke; } set { _alignToStroke = value; } }
        public bool RandomizePrefabsInActiveCategory { get { return _randomizePrefabsInActiveCategory; } set { _randomizePrefabsInActiveCategory = value; } }
        public bool UseOriginalPivot { get { return _useOriginalPivot; } set { _useOriginalPivot = value; } }
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

        public SingleDecorPaintModeObjectPlacementSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public SingleDecorPaintModeObjectPlacementSettings()
        {
            _view = new SingleDecorPaintModeObjectPlacementSettingsView(this);
        }
        #endregion
    }
}
#endif