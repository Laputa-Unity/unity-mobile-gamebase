#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class InteractableMirrorSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectMouseMoveAlongDirectionSettings _mouseOffsetFromHoverSurfaceSettings;
        [SerializeField]
        private ObjectMouseRotationSettings _mouseRotationSettings;
        [SerializeField]
        private ObjectKeyboardRotationSettings _keyboardRotationSettings;

        [SerializeField]
        private InteractableMirrorSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectMouseMoveAlongDirectionSettings MouseOffsetFromHoverSurfaceSettings
        {
            get
            {
                if (_mouseOffsetFromHoverSurfaceSettings == null) _mouseOffsetFromHoverSurfaceSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectMouseMoveAlongDirectionSettings>();
                return _mouseOffsetFromHoverSurfaceSettings;
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

        public ObjectKeyboardRotationSettings KeyboardRotationSettings
        {
            get
            {
                if (_keyboardRotationSettings == null) _keyboardRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectKeyboardRotationSettings>();
                return _keyboardRotationSettings;
            }
        }

        public InteractableMirrorSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        private InteractableMirrorSettings()
        {
            _view = new InteractableMirrorSettingsView(this);
        }
        #endregion
    }
}
#endif