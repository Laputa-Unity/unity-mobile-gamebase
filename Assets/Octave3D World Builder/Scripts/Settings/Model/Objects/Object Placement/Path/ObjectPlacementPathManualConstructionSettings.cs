#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathManualConstructionSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _offsetAlongGrowDirection = 0.0f;
        [SerializeField]
        private bool _excludeCorners = false;
        [SerializeField]
        private bool _randomizePrefabs = false;
        [SerializeField]
        private bool _rotateObjectsToFollowPath = false;
        [SerializeField]
        private float _objectMissChance = 0.0f;

        [SerializeField]
        private ObjectPlacementPathPaddingSettings _paddingSettings;
        [SerializeField]
        private ObjectPlacementPathHeightAdjustmentSettings _heightAdjustmentSettings;
        [SerializeField]
        private ObjectPlacementPathBorderSettings _borderSettings;

        [SerializeField]
        private ObjectPlacementPathManualConstructionSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinObjectMissChance { get { return 0.0f; } }
        public static float MaxObjectMissChance { get { return 1.0f; } }
        #endregion

        #region Public Properties
        public float OffsetAlongGrowDirection { get { return _offsetAlongGrowDirection; } set { _offsetAlongGrowDirection = value; } }
        public bool ExcludeCorners 
        { 
            get { return _excludeCorners; } 
            set 
            { 
                _excludeCorners = value;
                ObjectPlacementPathExcludeCornersWasChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public bool RandomizePrefabs { get { return _randomizePrefabs; } set { _randomizePrefabs = value; } }
        public bool RotateObjectsToFollowPath 
        {
            get { return _rotateObjectsToFollowPath; } 
            set 
            { 
                _rotateObjectsToFollowPath = value;
                ObjectPlacementPathRotateObjectsToFollowPathWasChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public float ObjectMissChance { get { return _objectMissChance; } set { _objectMissChance = Mathf.Clamp(value, MinObjectMissChance, MaxObjectMissChance); } }
        public ObjectPlacementPathPaddingSettings PaddingSettings
        {
            get
            {
                if (_paddingSettings == null) _paddingSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathPaddingSettings>();
                return _paddingSettings;
            }
        }
        public ObjectPlacementPathHeightAdjustmentSettings HeightAdjustmentSettings
        {
            get
            {
                if (_heightAdjustmentSettings == null) _heightAdjustmentSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathHeightAdjustmentSettings>();
                return _heightAdjustmentSettings;
            }
        }
        public ObjectPlacementPathBorderSettings BorderSettings
        {
            get
            {
                if (_borderSettings == null) _borderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathBorderSettings>();
                return _borderSettings;
            }
        }
        public ObjectPlacementPathManualConstructionSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathManualConstructionSettings()
        {
            _view = new ObjectPlacementPathManualConstructionSettingsView(this);
        }
        #endregion
    }
}
#endif