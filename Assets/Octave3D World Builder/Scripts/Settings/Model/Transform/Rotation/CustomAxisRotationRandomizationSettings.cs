#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class CustomAxisRotationRandomizationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _randomize = true;
        private Vector3 _axis = Vector3.up;

        [SerializeField]
        private AxisRotationRandomizationModeSettings _randomizationModeSettings;

        [SerializeField]
        private CustomAxisRotationRandomizationSettingsView _view;
        #endregion

        #region Public Properties
        public bool Randomize { get { return _randomize; } set { _randomize = value; } }
        public Vector3 Axis { get { return _axis; } set { _axis = value; } }
        public AxisRotationRandomizationModeSettings RandomizationModeSettings
        {
            get
            {
                if (_randomizationModeSettings == null) _randomizationModeSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisRotationRandomizationModeSettings>();
                return _randomizationModeSettings;
            }
        }
        public CustomAxisRotationRandomizationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public CustomAxisRotationRandomizationSettings()
        {
            _view = new CustomAxisRotationRandomizationSettingsView(this);
        }
        #endregion
    }
}
#endif