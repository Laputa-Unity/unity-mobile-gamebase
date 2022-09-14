#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class Object2DMassEraseSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _allowPartialOverlap = true;
        [SerializeField]
        private int _circleShapeRadius = 45;
        [SerializeField]
        private int _scrollWheelCircleRadiusAdjustmentSpeed = 5;

        [SerializeField]
        private Object2DMassEraseSettingsView _view;
        #endregion

        #region Public Static Properties
        public static int MinCircleRadiusInPixels { get { return 2; } }
        public static int MinScrollWheelCircleRadiusAdjustmentSpeed { get { return 1; } }
        #endregion

        #region Public Properties
        public int CircleShapeRadius { get { return _circleShapeRadius; } set { _circleShapeRadius = Mathf.Max(MinCircleRadiusInPixels, value); } }
        public bool AllowPartialOverlap { get { return _allowPartialOverlap; } set { _allowPartialOverlap = value; } }
        public int ScrollWheelCircleRadiusAdjustmentSpeed { get { return _scrollWheelCircleRadiusAdjustmentSpeed; } set { _scrollWheelCircleRadiusAdjustmentSpeed = Mathf.Max(MinScrollWheelCircleRadiusAdjustmentSpeed, value); } }
        public Object2DMassEraseSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public Object2DMassEraseSettings()
        {
            _view = new Object2DMassEraseSettingsView(this);
        }
        #endregion
    }
}
#endif