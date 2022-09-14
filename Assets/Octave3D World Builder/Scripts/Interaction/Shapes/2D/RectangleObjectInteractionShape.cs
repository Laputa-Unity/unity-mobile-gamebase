#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class RectangleObjectInteractionShape : ObjectInteraction2DShape
    {
        #region Private Variables
        [SerializeField]
        private RectangleShapeRenderSettings _renderSettings;
        #endregion

        #region Public Properties
        public RectangleShapeRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<RectangleShapeRenderSettings>();
                return _renderSettings;
            }
        }
        public Rect Rectangle { get { return EnclosingRect; } }
        #endregion

        #region Public Methods
        public override void RenderGizmos()
        {
            Rect rectangle = Rectangle;

            GizmosEx.Render2DFilledRectangle(rectangle, _renderSettings.FillColor);
            GizmosEx.Render2DRectangleBorderLines(rectangle, _renderSettings.BorderLineColor);
        }

        public override List<GameObject> GetOverlappedGameObjects(bool allowPartialOverlap)
        {
            return GetGameObjectsOverlappedByEnclosingRect(allowPartialOverlap);
        }
        #endregion
    }
}
#endif