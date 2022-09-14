#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class CustomAxisMouseRotationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _mouseSensitivity = 1.0f;

        [SerializeField]
        private CustomAxisMouseRotationSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinMouseSensitivity { get { return 0.01f; } }
        public static float MaxMouseSensitivity { get { return 1.0f; } }
        #endregion

        #region Public Properties
        public float MouseSensitivity { get { return _mouseSensitivity; } set { _mouseSensitivity = Mathf.Clamp(value, MinMouseSensitivity, MaxMouseSensitivity); } }
        public CustomAxisMouseRotationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public CustomAxisMouseRotationSettings()
        {
            _view = new CustomAxisMouseRotationSettingsView(this);
        }
        #endregion
    }
}
#endif