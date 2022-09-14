#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class Object3DMassEraseSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _circleShapeRadius = 5.5f;
        [SerializeField]
        private float _scrollWheelCircleRadiusAdjustmentSpeed = MinSrollWheelCircleRadiusAdjustmentSpeed;
        [SerializeField]
        private bool _allowPartialOverlap = true;

        [SerializeField]
        private Object3DMassEraseSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinCircleRadius { get { return 1e-4f; } }
        public static float MinSrollWheelCircleRadiusAdjustmentSpeed { get { return 0.45f; } }
        #endregion

        #region Public Properties
        public float CircleShapeRadius { get { return _circleShapeRadius; } set { _circleShapeRadius = Mathf.Max(MinCircleRadius, value); } }
        public bool AllowPartialOverlap { get { return _allowPartialOverlap; } set { _allowPartialOverlap = value; } }
        public float ScrollWheelCircleRadiusAdjustmentSpeed { get { return _scrollWheelCircleRadiusAdjustmentSpeed; } set { _scrollWheelCircleRadiusAdjustmentSpeed = Mathf.Max(MinSrollWheelCircleRadiusAdjustmentSpeed, value); } }
        public Object3DMassEraseSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public Object3DMassEraseSettings()
        {
            _view = new Object3DMassEraseSettingsView(this);
        }
        #endregion
    }
}
#endif