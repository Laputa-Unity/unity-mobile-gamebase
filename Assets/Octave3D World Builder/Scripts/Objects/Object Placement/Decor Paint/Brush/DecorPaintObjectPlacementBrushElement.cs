#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    public enum BrushElementRotationRandomizationMode
    {
        None = 0,
        SurfaceNormal,
        X,
        Y,
        Z
    }
    
    [Serializable]
    public class DecorPaintObjectPlacementBrushElement : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private DecorPaintObjectPlacementBrush _parentBrush;

        [SerializeField]
        private bool _isEnabled = true;
        [SerializeField]
        private Prefab _prefab;
        [SerializeField]
        private bool _alignToSurface = true;
        [SerializeField]
        private float _offsetFromSurface = 0.0f;
        [SerializeField]
        private CoordinateSystemAxis _alignmentAxis = CoordinateSystemAxis.PositiveUp;
        [SerializeField]
        private float _rotationOffsetInDegrees = 0.0f;
        [SerializeField]
        private bool _alignToStroke = false;
        [SerializeField]
        private float _spawnChance = 1.0f;
        [SerializeField]
        private BrushElementRotationRandomizationMode _rotationRandomizationMode = BrushElementRotationRandomizationMode.SurfaceNormal;
        [SerializeField]
        private float _scale = 1.0f;
        [SerializeField]
        private ObjectScaleRandomizationSettings _scaleRandomizationSettings;
        [SerializeField]
        private DecorPaintSlopeSettings _slopeSettings;

        [SerializeField]
        private bool _wasInitialized = false;

        [SerializeField]
        private DecorPaintObjectPlacementBrushElementView _view;
        #endregion

        #region Public Static Properties
        public static float MinSpawnChance { get { return 0.0f; } }
        public static float MaxSpawnChance { get { return 1.0f; } }
        public static float MinScale { get { return 0.02f; } }
        public static float MinRotationOffsetInDegrees { get { return 0.0f; } }
        public static float MaxRotationOffsetInDegrees { get { return 360.0f; } }
        #endregion

        #region Public Properties
        public DecorPaintObjectPlacementBrush ParentBrush { get { return _parentBrush; } set { if (value != null) _parentBrush = value; } }
        public bool IsEnabled { get { return _isEnabled; } set { _isEnabled = value; } }
        public Prefab Prefab { get { return _prefab; } set { _prefab = value; } }
        public bool AlignToSurface { get { return _alignToSurface; } set { _alignToSurface = value; } }
        public float OffsetFromSurface { get { return _offsetFromSurface; } set { _offsetFromSurface = value; } }
        public CoordinateSystemAxis AlignmentAxis { get { return _alignmentAxis; } set { _alignmentAxis = value; } }
        public float RotationOffsetInDegrees { get { return _rotationOffsetInDegrees; } set { _rotationOffsetInDegrees = Mathf.Clamp(value, MinRotationOffsetInDegrees, MaxRotationOffsetInDegrees); } }
        public bool AlignToStroke { get { return _alignToStroke; } set { _alignToStroke = value; } }
        public float SpawnChance 
        { 
            get { return _spawnChance; } 
            set 
            { 
                _spawnChance = Mathf.Clamp(value, MinSpawnChance, MaxSpawnChance);
            } 
        }
        public BrushElementRotationRandomizationMode RotationRandomizationMode { get { return _rotationRandomizationMode; } set { _rotationRandomizationMode = value; } }
        public float Scale { get { return _scale; } set { _scale = Mathf.Max(value, MinScale); } }
        public ObjectScaleRandomizationSettings ScaleRandomizationSettings
        {
            get
            {
                if (_scaleRandomizationSettings == null) _scaleRandomizationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectScaleRandomizationSettings>();
                return _scaleRandomizationSettings;
            }
        }
        public DecorPaintSlopeSettings SlopeSettings
        {
            get
            {
                if (_slopeSettings == null) _slopeSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<DecorPaintSlopeSettings>();
                return _slopeSettings;
            }
        }
        public DecorPaintObjectPlacementBrushElementView View { get { return _view; } }
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrushElement()
        {
            _view = new DecorPaintObjectPlacementBrushElementView(this);
        }
        #endregion

        #region Public Methods
        public bool IsValid()
        {
            return _prefab != null && _prefab.UnityPrefab != null;
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            ScaleRandomizationSettings.View.ToggleVisibilityBeforeRender = false;
            ScaleRandomizationSettings.View.SurroundWithBox = false;

            if(!_wasInitialized)
            {
                ScaleRandomizationSettings.RandomizeScale = true;
                ScaleRandomizationSettings.UniformScaleRandomizationSettings.MinScale = 0.5f;
                ScaleRandomizationSettings.UniformScaleRandomizationSettings.MaxScale = 1.0f;
                _wasInitialized = true;
            }
        }

        private void OnDestroy()
        {
            if (_slopeSettings != null) UndoEx.DestroyObjectImmediate(_slopeSettings);
            if (_scaleRandomizationSettings != null) UndoEx.DestroyObjectImmediate(_scaleRandomizationSettings);
        }
        #endregion
    }
}
#endif