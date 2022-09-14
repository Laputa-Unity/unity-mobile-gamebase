#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private DecorPaintMode _decorPaintMode = DecorPaintMode.Single;
        [SerializeField]
        private bool _ignoreGrid = true;

        [SerializeField]
        private float _strokeDistance = 1.0f;

        [SerializeField]
        private SingleDecorPaintModeObjectPlacementSettings _singleDecorPaintModeSettings;
        [SerializeField]
        private BrushDecorPaintModeObjectPlacementSettings _brushDecorPaintModeSettings;

        [SerializeField]
        private DecorPaintObjectPlacementSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinStrokeDistance { get { return 1e-5f; } }
        #endregion

        #region Public Properties
        public DecorPaintMode DecorPaintMode { get { return _decorPaintMode; } set { _decorPaintMode = value; } }
        public bool IgnoreGrid { get { return _ignoreGrid; } set { _ignoreGrid = value; } }
        public SingleDecorPaintModeObjectPlacementSettings SingleDecorPaintModeSettings
        {
            get
            {
                if (_singleDecorPaintModeSettings == null) _singleDecorPaintModeSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<SingleDecorPaintModeObjectPlacementSettings>();
                return _singleDecorPaintModeSettings;
            }
        }
        public BrushDecorPaintModeObjectPlacementSettings BrushDecorPaintModeSettings
        {
            get
            {
                if (_brushDecorPaintModeSettings == null) _brushDecorPaintModeSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<BrushDecorPaintModeObjectPlacementSettings>();
                return _brushDecorPaintModeSettings;
            }
        }
        public float StrokeDistance { get { return _strokeDistance; } set { _strokeDistance = Mathf.Max(value, MinStrokeDistance); } }
        public DecorPaintObjectPlacementSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementSettings()
        {
            _view = new DecorPaintObjectPlacementSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static DecorPaintObjectPlacementSettings Get()
        {
            return ObjectPlacementSettings.Get().DecorPaintObjectPlacementSettings;
        }
        #endregion
    }
}
#endif