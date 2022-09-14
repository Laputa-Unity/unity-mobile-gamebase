#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisMouseRotationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private TransformAxis _rotationAxis;
        [SerializeField]
        private float _mouseSensitivity = 1.0f;

        [SerializeField]
        private AxisMouseRotationSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinMouseSensitivity { get { return 0.01f; } }
        public static float MaxMouseSensitivity { get { return 1.0f; } }
        #endregion

        #region Public Properties
        public TransformAxis RotationAxis { get { return _rotationAxis; } set { _rotationAxis = value; } }
        public float MouseSensitivity { get { return _mouseSensitivity; } set { _mouseSensitivity = Mathf.Clamp(value, MinMouseSensitivity, MaxMouseSensitivity); } }
        public AxisMouseRotationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public AxisMouseRotationSettings()
        {
            _view = new AxisMouseRotationSettingsView(this);
        }
        #endregion
    }
}
#endif