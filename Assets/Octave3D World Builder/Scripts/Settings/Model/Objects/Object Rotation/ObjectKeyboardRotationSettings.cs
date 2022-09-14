#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectKeyboardRotationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private AxisKeyboardRotationSettings _xAxisRotationSettings;
        [SerializeField]
        private AxisKeyboardRotationSettings _yAxisRotationSettings;
        [SerializeField]
        private AxisKeyboardRotationSettings _zAxisRotationSettings;
        [SerializeField]
        private CustomAxisKeyboardRotationSettings _customAxisRotationSettings;

        [SerializeField]
        private ObjectKeyboardRotationSettingsView _view;
        #endregion

        #region Public Properties
        public AxisKeyboardRotationSettings XAxisRotationSettings
        {
            get
            {
                if (_xAxisRotationSettings == null)
                {
                    _xAxisRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisKeyboardRotationSettings>();
                    _xAxisRotationSettings.RotationAxis = TransformAxis.X;
                }
                return _xAxisRotationSettings;
            }
        }
        public AxisKeyboardRotationSettings YAxisRotationSettings
        {
            get
            {
                if (_yAxisRotationSettings == null)
                {
                    _yAxisRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisKeyboardRotationSettings>();
                    _yAxisRotationSettings.RotationAxis = TransformAxis.Y;
                }
                return _yAxisRotationSettings;
            }
        }
        public AxisKeyboardRotationSettings ZAxisRotationSettings
        {
            get
            {
                if (_zAxisRotationSettings == null)
                {
                    _zAxisRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<AxisKeyboardRotationSettings>();
                    _zAxisRotationSettings.RotationAxis = TransformAxis.Z;
                }
                return _zAxisRotationSettings;
            }
        }
        public CustomAxisKeyboardRotationSettings CustomAxisRotationSettings
        {
            get
            {
                if (_customAxisRotationSettings == null) _customAxisRotationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<CustomAxisKeyboardRotationSettings>();
                return _customAxisRotationSettings;
            }
        }
        public ObjectKeyboardRotationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectKeyboardRotationSettings()
        {
            _view = new ObjectKeyboardRotationSettingsView(this);
        }
        #endregion
    }
}
#endif