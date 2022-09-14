#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectMouseRotationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private AxisMouseRotationSettings _xAxisRotationSettings;
        [SerializeField]
        private AxisMouseRotationSettings _yAxisRotationSettings;
        [SerializeField]
        private AxisMouseRotationSettings _zAxisRotationSettings;
        [SerializeField]
        private CustomAxisMouseRotationSettings _customAxisRotationSettings;

        [SerializeField]
        private float _snapStepInDegrees = 45.0f;
        [SerializeField]
        private bool _useSnapping = false;

        [SerializeField]
        private ObjectMouseRotationSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinSnapStepInDegrees { get { return 1e-5f; } }
        #endregion

        #region Public Properties
        public AxisMouseRotationSettings XAxisRotationSettings
        {
            get
            {
                if (_xAxisRotationSettings == null)
                {
                    _xAxisRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisMouseRotationSettings>();
                    _xAxisRotationSettings.RotationAxis = TransformAxis.X;
                }
                return _xAxisRotationSettings;
            }
        }
        public AxisMouseRotationSettings YAxisRotationSettings
        {
            get
            {
                if (_yAxisRotationSettings == null)
                {
                    _yAxisRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisMouseRotationSettings>();
                    _yAxisRotationSettings.RotationAxis = TransformAxis.Y;
                }
                return _yAxisRotationSettings;
            }
        }
        public AxisMouseRotationSettings ZAxisRotationSettings
        {
            get
            {
                if (_zAxisRotationSettings == null)
                {
                    _zAxisRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisMouseRotationSettings>();
                    _zAxisRotationSettings.RotationAxis = TransformAxis.Z;
                }
                return _zAxisRotationSettings;
            }
        }
        public CustomAxisMouseRotationSettings CustomAxisRotationSettings
        {
            get
            {
                if (_customAxisRotationSettings == null) _customAxisRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<CustomAxisMouseRotationSettings>();
                return _customAxisRotationSettings;
            }
        }
        public float SnapStepInDegrees { get { return _snapStepInDegrees; } set { _snapStepInDegrees = Mathf.Max(value, MinSnapStepInDegrees); } }
        public bool UseSnapping { get { return _useSnapping; } set { _useSnapping = value; } }
        public ObjectMouseRotationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectMouseRotationSettings()
        {
            _view = new ObjectMouseRotationSettingsView(this);
        }
        #endregion
    }
}
#endif