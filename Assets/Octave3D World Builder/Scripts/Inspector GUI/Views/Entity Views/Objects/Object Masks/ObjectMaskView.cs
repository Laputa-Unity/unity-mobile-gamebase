#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectMaskView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectMask _mask;
        #endregion

        #region Constructors
        public ObjectMaskView(ObjectMask mask)
        {
            _mask = mask;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _mask.ObjectLayerObjectMask.View.Render();
            _mask.ObjectCollectionMask.View.Render();
        }
        #endregion
    }
}
#endif