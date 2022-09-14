#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisKeyboardRotationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private TransformAxis _rotationAxis;
        [SerializeField]
        private float _rotationAmountInDegrees = 90.0f;

        [SerializeField]
        private AxisKeyboardRotationSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinRotationAmountInDegrees { get { return 0.0f; } }
        public static float MaxRotationAmountInDegrees { get { return 360.0f; } }
        #endregion

        #region Public Properties
        public TransformAxis RotationAxis { get { return _rotationAxis; } set { _rotationAxis = value; } }
        public float RotationAmountInDegrees { get { return _rotationAmountInDegrees; } set { _rotationAmountInDegrees = Mathf.Clamp(value, MinRotationAmountInDegrees, MaxRotationAmountInDegrees); } }
        public AxisKeyboardRotationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public AxisKeyboardRotationSettings()
        {
            _view = new AxisKeyboardRotationSettingsView(this);
        }
        #endregion
    }
}
#endif