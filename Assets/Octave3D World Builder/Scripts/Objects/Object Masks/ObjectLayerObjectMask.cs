#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectLayerObjectMask : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private int _layerMask = 0;

        [SerializeField]
        private ObjectLayerObjectMaskView _view;
        #endregion

        #region Public Properties
        public ObjectLayerObjectMaskView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectLayerObjectMask()
        {
            _view = new ObjectLayerObjectMaskView(this);
        }
        #endregion

        #region Public Methods
        public bool IsMasked(int objectLayer)
        {
            return LayerExtensions.IsLayerNumberValid(objectLayer) && (_layerMask & (1 << objectLayer)) != 0;
        }

        public void Mask(int objectLayer)
        {
            if (LayerExtensions.IsLayerNumberValid(objectLayer)) _layerMask |= (1 << objectLayer);
        }

        public void MaskAll()
        {
            _layerMask = ~0;
        }

        public void Unmask(int objectLayer)
        {
            if (LayerExtensions.IsLayerNumberValid(objectLayer)) _layerMask &= ~(1 << objectLayer);
        }

        public void UnmaskAll()
        {
            _layerMask = 0;
        }

        public void RemoveInvalidLayerNumbers()
        {
            _layerMask = LayerExtensions.ClearMaskOfInvalidLayers(_layerMask);
        }
        #endregion
    }
}
#endif