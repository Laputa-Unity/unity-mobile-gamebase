#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class RandomRotationStepAxisRandomizationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _stepSizeInDegrees = 90.0f;

        [SerializeField]
        private RandomRotationStepAxisRandomizationSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinStepSizeInDegrees { get { return 0.0001f; } }
        public static float MaxStepSizeInDegrees { get { return 360.0f; } }
        #endregion

        #region Public Properties
        public float StepSizeInDegrees { get { return _stepSizeInDegrees; } set { _stepSizeInDegrees = Mathf.Clamp(value, MinStepSizeInDegrees, MaxStepSizeInDegrees); } }
        public RandomRotationStepAxisRandomizationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public RandomRotationStepAxisRandomizationSettings()
        {
            _view = new RandomRotationStepAxisRandomizationSettingsView(this);
        }
        #endregion
    }
}
#endif