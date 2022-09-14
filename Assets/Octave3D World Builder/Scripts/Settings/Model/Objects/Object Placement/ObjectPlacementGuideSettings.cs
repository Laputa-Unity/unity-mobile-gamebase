#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementGuideSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectMouseMoveAlongDirectionSettings _mouseOffsetFromPlacementSurfaceSettings;
        [SerializeField]
        private ObjectKeyboardRotationSettings _keyboardRotationSettings;
        [SerializeField]
        private ObjectMouseRotationSettings _mouseRotationSettings;
        [SerializeField]
        private ObjectMouseUniformScaleSettings _mouseUniformScaleSettings;

        [SerializeField]
        private ObjectPlacementGuideSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectMouseMoveAlongDirectionSettings MouseOffsetFromPlacementSurfaceSettings
        {
            get
            {
                if (_mouseOffsetFromPlacementSurfaceSettings == null) _mouseOffsetFromPlacementSurfaceSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectMouseMoveAlongDirectionSettings>();
                return _mouseOffsetFromPlacementSurfaceSettings;
            }
        }
        public ObjectKeyboardRotationSettings KeyboardRotationSettings
        {
            get
            {
                if (_keyboardRotationSettings == null) _keyboardRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectKeyboardRotationSettings>();
                return _keyboardRotationSettings;
            }
        }
        public ObjectMouseRotationSettings MouseRotationSettings
        {
            get
            {
                if (_mouseRotationSettings == null) _mouseRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectMouseRotationSettings>();
                return _mouseRotationSettings;
            }
        }
        public ObjectMouseUniformScaleSettings MouseUniformScaleSettings
        {
            get
            {
                if (_mouseUniformScaleSettings == null) _mouseUniformScaleSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectMouseUniformScaleSettings>();
                return _mouseUniformScaleSettings;
            }
        }

        public ObjectPlacementGuideSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementGuideSettings()
        {
            _view = new ObjectPlacementGuideSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectPlacementGuideSettings Get()
        {
            return ObjectPlacementSettings.Get().ObjectPlacementGuideSettings;
        }
        #endregion
    }
}
#endif