#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionPaintModeSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private int _selectionShapeWidthInPixels = 55;
        [SerializeField]
        private int _selectionShapeHeightInPixels = 55;
        [SerializeField]
        private int _scrollWheelShapeSizeAdjustmentSpeed = 5;

        [SerializeField]
        private ObjectSelectionPaintModeSettingsView _view;
        #endregion

        #region Public Static Properties
        public static int MinSelectionShapeSizeInPixels { get { return 2; } }
        public static int MinScrollWheelShapeSizeAdjustmentSpeed { get { return 1; } }
        #endregion

        #region Public Properties
        public int SelectionShapeWidthInPixels { get { return _selectionShapeWidthInPixels; } set { _selectionShapeWidthInPixels = Mathf.Max(MinSelectionShapeSizeInPixels, value); } }
        public int SelectionShapeHeightInPixels { get { return _selectionShapeHeightInPixels; } set { _selectionShapeHeightInPixels = Mathf.Max(MinSelectionShapeSizeInPixels, value); } }
        public int ScrollWheelShapeSizeAdjustmentSpeed { get { return _scrollWheelShapeSizeAdjustmentSpeed; } set { _scrollWheelShapeSizeAdjustmentSpeed = Mathf.Max(MinScrollWheelShapeSizeAdjustmentSpeed, value); } }
        public ObjectSelectionPaintModeSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectSelectionPaintModeSettings()
        {
            _view = new ObjectSelectionPaintModeSettingsView(this);
        }
        #endregion
    }
}
#endif