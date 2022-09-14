#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockManualConstructionSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _constrainSize = false;
        [SerializeField]
        private int _maxSize = 30;
        [SerializeField]
        private float _offsetAlongGrowDirection = 0.0f;
        [SerializeField]
        private bool _excludeCorners;
        [SerializeField]
        private bool _randomizePrefabs = false;
        [SerializeField]
        private float _objectMissChance = 0.0f;

        [SerializeField]
        private ObjectPlacementBlockPaddingSettings _paddingSettings;
        [SerializeField]
        private ObjectRotationRandomizationSettings _objectRotationRandomizationSettings;
        [SerializeField]
        private ObjectPlacementBlockHeightAdjustmentSettings _heightAdjustmentSettings;
        [SerializeField]
        private ObjectPlacementBlockSubdivisionSettings _subdivisionSettings;

        [SerializeField]
        private ObjectPlacementBlockManualConstructionSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinObjectMissChance { get { return 0.0f; } }
        public static float MaxObjectMissChance { get { return 1.0f; } }
        #endregion

        #region Public Properties
        public bool ContrainSize { get { return _constrainSize; } set { _constrainSize = value; } }
        public int MaxSize { get { return _maxSize; } set { _maxSize = Mathf.Max(1, value); } }
        public float OffsetAlongGrowDirection { get { return _offsetAlongGrowDirection; } set { _offsetAlongGrowDirection = value; } }
        public bool ExcludeCorners
        { 
            get { return _excludeCorners; } 
            set 
            { 
                _excludeCorners = value;
                ObjectPlacementBlockExcludeCornersWasChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public bool RandomizePrefabs { get { return _randomizePrefabs; } set { _randomizePrefabs = value; } }
        public float ObjectMissChance { get { return _objectMissChance; } set { _objectMissChance = Mathf.Clamp(value, MinObjectMissChance, MaxObjectMissChance); } }
        public ObjectPlacementBlockPaddingSettings PaddingSettings
        {
            get
            {
                if (_paddingSettings == null) _paddingSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementBlockPaddingSettings>();
                return _paddingSettings;
            }
        }
        public ObjectRotationRandomizationSettings ObjectRotationRandomizationSettings
        {
            get
            {
                if (_objectRotationRandomizationSettings == null) _objectRotationRandomizationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectRotationRandomizationSettings>();
                return _objectRotationRandomizationSettings;
            }
        }
        public ObjectPlacementBlockHeightAdjustmentSettings HeightAdjustmentSettings
        {
            get
            {
                if (_heightAdjustmentSettings == null) _heightAdjustmentSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementBlockHeightAdjustmentSettings>();
                return _heightAdjustmentSettings;
            }
        }
        public ObjectPlacementBlockSubdivisionSettings SubdivisionSettings
        {
            get
            {
                if (_subdivisionSettings == null) _subdivisionSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementBlockSubdivisionSettings>();
                return _subdivisionSettings;
            }
        }
        public ObjectPlacementBlockManualConstructionSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockManualConstructionSettings()
        {
            _view = new ObjectPlacementBlockManualConstructionSettingsView(this);
        }
        #endregion
    }
}
#endif