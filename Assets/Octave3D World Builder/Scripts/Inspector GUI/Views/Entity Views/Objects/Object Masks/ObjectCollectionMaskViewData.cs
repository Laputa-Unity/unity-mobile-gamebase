#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectCollectionMaskViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _objectCollectionViewPosition = Vector2.zero;
        [SerializeField]
        private float _objectCollectionScrollViewHeight = 200.0f;
        #endregion

        #region Public Properties
        public Vector2 ObjectCollectionViewPosition { get { return _objectCollectionViewPosition; } set { _objectCollectionViewPosition = value; } }
        public float ObjectCollectionScrollViewHeight { get { return _objectCollectionScrollViewHeight; } }
        #endregion

    }
}
#endif