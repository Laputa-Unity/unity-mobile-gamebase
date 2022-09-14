#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectLayerObjectMaskViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _layerScrollViewPosition = Vector2.zero;
        [SerializeField]
        private float _layerScrollViewHeight = 200.0f;
        #endregion

        #region Public Properties
        public Vector2 LayerScrollViewPosition { get { return _layerScrollViewPosition; } set { _layerScrollViewPosition = value; } }
        public float LayerScrollViewHeight { get { return _layerScrollViewHeight; } }
        #endregion
    }
}
#endif