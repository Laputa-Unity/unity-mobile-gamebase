#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class CustomAxisKeyboardRotationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _rotationAmountInDegrees = 90.0f;

        [SerializeField]
        private CustomAxisKeyboardRotationSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinRotationAmountInDegrees { get { return 0.0f; } }
        public static float MaxRotationAmountInDegrees { get { return 360.0f; } }
        #endregion

        #region Public Properties
        public float RotationAmountInDegrees { get { return _rotationAmountInDegrees; } set { _rotationAmountInDegrees = Mathf.Clamp(value, MinRotationAmountInDegrees, MaxRotationAmountInDegrees); } }
        public CustomAxisKeyboardRotationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public CustomAxisKeyboardRotationSettings()
        {
            _view = new CustomAxisKeyboardRotationSettingsView(this);
        }
        #endregion
    }
}
#endif