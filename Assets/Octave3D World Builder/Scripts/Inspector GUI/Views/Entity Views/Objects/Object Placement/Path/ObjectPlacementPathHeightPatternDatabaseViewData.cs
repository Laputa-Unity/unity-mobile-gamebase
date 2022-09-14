#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightPatternDatabaseViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private string _nameForNewHeightPattern = "";
        [SerializeField]
        private Vector2 _patternScrollPosition = Vector2.zero;
        [SerializeField]
        private float _patternScrollViewHeight = 300.0f;
        [SerializeField]
        private Color _colorForActivePatternButton = new Color(0.01f, 1.0f, 0.95f, 1.0f);
        #endregion

        #region Public Static Properties
        public static float MinPatternScrollViewHeight { get { return 150.0f; } }
        public static float MaxPatternScrollViewHeight { get { return 800.0f; } }
        #endregion

        #region Public Properties
        public string NameForNewHeightPattern { get { return _nameForNewHeightPattern; } set { if (value != null) _nameForNewHeightPattern = value; } }
        public Color ColorForActivePatternButton { get { return _colorForActivePatternButton; } set { _colorForActivePatternButton = value; } }
        public Vector2 PatternScrollPosition { get { return _patternScrollPosition; } set { _patternScrollPosition = value; } }
        public float PatternScrollViewHeight { get { return _patternScrollViewHeight; } set { _patternScrollViewHeight = Mathf.Clamp(value, MinPatternScrollViewHeight, MaxPatternScrollViewHeight); } }
        #endregion
    }
}
#endif