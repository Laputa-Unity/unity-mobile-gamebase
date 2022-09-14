#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectMask
    {
        #region Private Variables
        [SerializeField]
        private ObjectLayerObjectMask _objectLayerObjectMask;
        [SerializeField]
        private ObjectCollectionMask _objectCollectionMask;

        [SerializeField]
        private ObjectMaskView _view;
        #endregion

        #region Public Properties
        public ObjectLayerObjectMask ObjectLayerObjectMask
        {
            get
            {
                if (_objectLayerObjectMask == null) _objectLayerObjectMask = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectLayerObjectMask>();
                return _objectLayerObjectMask;
            }
        }
        public ObjectCollectionMask ObjectCollectionMask
        {
            get
            {
                if (_objectCollectionMask == null) _objectCollectionMask = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectCollectionMask>();
                return _objectCollectionMask;
            }
        }

        public ObjectMaskView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectMask()
        {
            _view = new ObjectMaskView(this);
        }
        #endregion

        #region Public Methods
        public bool IsGameObjectMasked(GameObject gameObject)
        {
            if (ObjectLayerObjectMask.IsMasked(gameObject.layer)) return true;
            if (ObjectCollectionMask.IsMasked(gameObject)) return true;

            return false;
        }

        public void RemoveInvalidEntries()
        {
            ObjectCollectionMask.RemoveNullEntries();
            ObjectLayerObjectMask.RemoveInvalidLayerNumbers();
        }
        #endregion
    }
}
#endif