#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisRotationRandomizationModeSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private AxisRotationRandomizationMode _randomizationMode = AxisRotationRandomizationMode.RandomRotationValue;
        [SerializeField]
        private RandomRotationStepAxisRandomizationSettings _randomRotationStepAxisRandomizationSettings;
        [SerializeField]
        private RandomRotationValueAxisRandomizationSettings _randomRotationValueAxisRandomizationSettings;

        [SerializeField]
        private AxisRotationRandomizationModeSettingsView _view;
        #endregion

        #region Public Properties
        public AxisRotationRandomizationMode RandomizationMode { get { return _randomizationMode; } set { _randomizationMode = value; } }
        public RandomRotationStepAxisRandomizationSettings RandomRotationStepAxisRandomizationSettings
        {
            get
            {
                if (_randomRotationStepAxisRandomizationSettings == null) _randomRotationStepAxisRandomizationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<RandomRotationStepAxisRandomizationSettings>();
                return _randomRotationStepAxisRandomizationSettings;
            }
        }
        public RandomRotationValueAxisRandomizationSettings RandomRotationValueAxisRandomizationSettings
        {
            get
            {
                if (_randomRotationValueAxisRandomizationSettings == null) _randomRotationValueAxisRandomizationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<RandomRotationValueAxisRandomizationSettings>();
                return _randomRotationValueAxisRandomizationSettings;
            }
        }

        public AxisRotationRandomizationModeSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public AxisRotationRandomizationModeSettings()
        {
            _view = new AxisRotationRandomizationModeSettingsView(this);
        }
        #endregion
    }
}
#endif