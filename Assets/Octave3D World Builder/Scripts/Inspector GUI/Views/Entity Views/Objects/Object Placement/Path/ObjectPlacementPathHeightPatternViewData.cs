#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightPatternViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _isViewVisible = true;
        [SerializeField]
        private bool _allowPatternStringEdit = false;
        [SerializeField]
        private string _currentPathHeightPatternString = "";
        [SerializeField]
        private float _patternTextAreaHeight = MinPatternTextAreaHeight;
        #endregion

        #region Public Static Properties
        public static float MinPatternTextAreaHeight { get { return 50.0f; } }
        public static float MaxPatternTextAreaHeight { get { return 300.0f; } }
        #endregion

        #region Public Properties
        public bool IsViewVisible { get { return _isViewVisible; } set { _isViewVisible = value; } }
        public bool AllowPatternStringEdit { get { return _allowPatternStringEdit; } set { _allowPatternStringEdit = value; } }
        public string CurrentPathHeightPatternString { get { return _currentPathHeightPatternString; } set { if (value != null) _currentPathHeightPatternString = value; } }
        public float PatternTextAreaHeight { get { return _patternTextAreaHeight; } set { _patternTextAreaHeight = Mathf.Clamp(value, MinPatternTextAreaHeight, MaxPatternTextAreaHeight); } }
        #endregion
    }
}
#endif