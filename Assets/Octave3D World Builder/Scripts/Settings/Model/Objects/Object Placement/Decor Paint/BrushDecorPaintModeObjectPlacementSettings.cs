#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class BrushDecorPaintModeObjectPlacementSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _scrollWheelCircleRadiusAdjustmentSpeed = MinSrollWheelCircleRadiusAdjustmentSpeed;

        [SerializeField]
        private BrushDecorPaintModeObjectPlacementSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinCircleRadius { get { return 1e-4f; } }
        public static float MinSrollWheelCircleRadiusAdjustmentSpeed { get { return 0.45f; } }
        #endregion

        #region Public Properties
        public float ScrollWheelCircleRadiusAdjustmentSpeed { get { return _scrollWheelCircleRadiusAdjustmentSpeed; } set { _scrollWheelCircleRadiusAdjustmentSpeed = Mathf.Max(MinSrollWheelCircleRadiusAdjustmentSpeed, value); } }
        public BrushDecorPaintModeObjectPlacementSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public BrushDecorPaintModeObjectPlacementSettings()
        {
            _view = new BrushDecorPaintModeObjectPlacementSettingsView(this);
        }
        #endregion
    }
}
#endif