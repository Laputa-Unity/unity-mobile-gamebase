#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectMouseMoveAlongDirectionSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _mouseSensitivity = 0.02f;

        [SerializeField]
        private ObjectMouseMoveAlongDirectionSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinMouseSensitivity { get { return 0.001f; } }
        public static float MaxMouseSensitivity { get { return 1.0f; } }
        #endregion

        #region Public Properties
        public float MouseSensitivity { get { return _mouseSensitivity; } set { _mouseSensitivity = Mathf.Clamp(value, MinMouseSensitivity, MaxMouseSensitivity); } }
        public ObjectMouseMoveAlongDirectionSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectMouseMoveAlongDirectionSettings()
        {
            _view = new ObjectMouseMoveAlongDirectionSettingsView(this);
        }
        #endregion
    }
}
#endif