#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class RandomRotationValueAxisRandomizationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _minRotationInDegrees = 0.0f;
        [SerializeField]
        private float _maxRotationInDegrees = 360.0f;

        [SerializeField]
        private RandomRotationValueAxisRandomizationSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinRotationValueInDegrees { get { return 0.0f; } }
        public static float MaxRotationValueInDegrees { get { return 360.0f; } }
        #endregion

        #region Public Properties
        public float MinRotationInDegrees { get { return _minRotationInDegrees; } set { _minRotationInDegrees = Mathf.Clamp(value, MinRotationValueInDegrees, _maxRotationInDegrees); } }
        public float MaxRotationInDegrees { get { return _maxRotationInDegrees; } set { _maxRotationInDegrees = Mathf.Clamp(value, _minRotationInDegrees, MaxRotationValueInDegrees); } }
        public RandomRotationValueAxisRandomizationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public RandomRotationValueAxisRandomizationSettings()
        {
            _view = new RandomRotationValueAxisRandomizationSettingsView(this);
        }
        #endregion
    }
}
#endif