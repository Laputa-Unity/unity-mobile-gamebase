#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectMouseUniformScaleSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _mouseSensitivity = 0.02f;
        [SerializeField]
        private float _snapStep = 0.2f;
        [SerializeField]
        private bool _useSnapping = false;

        [SerializeField]
        private ObjectMouseUniformScaleSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinMouseSensitivity { get { return 0.01f; } }
        public static float MaxMouseSensitivity { get { return 1.0f; } }
        public static float MinSnapStep { get { return 1e-5f; } }
        #endregion

        #region Public Properties
        public float MouseSensitivity { get { return _mouseSensitivity; } set { _mouseSensitivity = Mathf.Clamp(value, MinMouseSensitivity, MaxMouseSensitivity); } }
        public float SnapStep { get { return _snapStep; } set { _snapStep = Mathf.Max(value, MinSnapStep); } }
        public bool UseSnapping { get { return _useSnapping; } set { _useSnapping = value; } }
        public ObjectMouseUniformScaleSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectMouseUniformScaleSettings()
        {
            _view = new ObjectMouseUniformScaleSettingsView(this);
        }
        #endregion
    }
}
#endif