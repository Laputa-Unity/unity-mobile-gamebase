#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [SerializeField]
    public class ObjectUniformScaleRandomizationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _minScale = MinScaleValue;
        [SerializeField]
        private float _maxScale = 1.0f;

        [SerializeField]
        private ObjectUniformScaleRandomizationSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinScaleValue { get { return 0.1f; } }
        public static float MaxScaleValue { get { return 10.0f; } }
        #endregion

        #region Public Properties
        public float MinScale { get { return _minScale; } set { _minScale = Mathf.Clamp(value, MinScaleValue, _maxScale); } }
        public float MaxScale { get { return _maxScale; } set { _maxScale = Mathf.Clamp(value, _minScale, MaxScaleValue); } }
        public ObjectUniformScaleRandomizationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectUniformScaleRandomizationSettings()
        {
            _view = new ObjectUniformScaleRandomizationSettingsView(this);
        }
        #endregion
    }
}
#endif