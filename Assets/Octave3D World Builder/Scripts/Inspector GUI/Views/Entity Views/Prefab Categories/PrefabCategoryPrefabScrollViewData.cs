#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabCategoryPrefabScrollViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _prefabScrollPosition = Vector2.zero;
        [SerializeField]
        private int _numberOfPrefabsPerRow = 6;
        [SerializeField]
        private float _prefabPreviewScale = 1.0f;
        [SerializeField]
        private float _prefabScrollViewHeight = 200.0f;
        [SerializeField]
        private Color _activePrefabTint = new Color(0.639f, 0.909f, 1.0f, 1.0f);
        [SerializeField]
        private bool _showPrefabNames = false;
        [SerializeField]
        private Color _prefabNameLabelColor = Color.black;

        [SerializeField]
        private PrefabCategoryScrollViewLookAndFeelWindow _lookAndFeelWindow;
        #endregion

        #region Public Static Properties
        public static int MinNumberOfPrefabPreviewsPerRow = 1;
        public static float MinPrefabPreviewScale { get { return 0.5f; } }
        public static float MaxPrefabPreviewScale { get { return 1.0f; } }
        public static float MinPrefabScrollViewHeight { get { return 150.0f; } }
        public static float MaxPrefabScrollViewHeight { get { return 800.0f; } }
        #endregion

        #region Public Properties
        public Vector2 PrefabScrollPosition { get { return _prefabScrollPosition; } set { _prefabScrollPosition = value; } }
        public float PrefabPreviewScale { get { return _prefabPreviewScale; } set { _prefabPreviewScale = Mathf.Clamp(value, MinPrefabPreviewScale, MaxPrefabPreviewScale); } }
        public int NumberOfPrefabsPerRow { get { return _numberOfPrefabsPerRow; } set { _numberOfPrefabsPerRow = Mathf.Max(MinNumberOfPrefabPreviewsPerRow, value); } }
        public float PrefabScrollViewHeight { get { return _prefabScrollViewHeight; } set { _prefabScrollViewHeight = Mathf.Clamp(value, MinPrefabScrollViewHeight, MaxPrefabScrollViewHeight); } }
        public Color ActivePrefabTint { get { return _activePrefabTint; } set { _activePrefabTint = value; } }
        public bool ShowPrefabNames { get { return _showPrefabNames; } set { _showPrefabNames = value; } }
        public Color PrefabNameLabelColor { get { return _prefabNameLabelColor; } set { _prefabNameLabelColor = value; } }
        public PrefabCategoryScrollViewLookAndFeelWindow LookAndFeelWindow
        {
            get
            {
                if (_lookAndFeelWindow == null) _lookAndFeelWindow = Octave3DWorldBuilder.ActiveInstance.EditorWindowPool.CreateWindow<PrefabCategoryScrollViewLookAndFeelWindow>();
                if (_lookAndFeelWindow != null) _lookAndFeelWindow.LookAndFeelData = this;

                return _lookAndFeelWindow;
            }
        }
        #endregion
    }
}
#endif