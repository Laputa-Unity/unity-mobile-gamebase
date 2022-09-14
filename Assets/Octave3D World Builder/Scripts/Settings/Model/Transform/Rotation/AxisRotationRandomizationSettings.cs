#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class AxisRotationRandomizationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _randomize = false;

        [SerializeField]
        private TransformAxis _axis = TransformAxis.X;
        [SerializeField]
        private AxisRotationRandomizationModeSettings _randomizationModeSettings;

        [SerializeField]
        private AxisRotationRandomizationSettingsView _view;
        #endregion

        #region Public Properties
        public bool Randomize { get { return _randomize; } set { _randomize = value; } }
        public TransformAxis Axis { get { return _axis; } set { _axis = value; } }
        public AxisRotationRandomizationModeSettings RandomizationModeSettings
        {
            get
            {
                if (_randomizationModeSettings == null) _randomizationModeSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisRotationRandomizationModeSettings>();
                return _randomizationModeSettings;
            }
        }
        public AxisRotationRandomizationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public AxisRotationRandomizationSettings()
        {
            _view = new AxisRotationRandomizationSettingsView(this);
        }
        #endregion
    }
}
#endif