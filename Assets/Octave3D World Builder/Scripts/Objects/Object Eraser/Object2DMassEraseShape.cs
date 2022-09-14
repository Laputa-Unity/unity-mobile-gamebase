#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class Object2DMassEraseShape
    {
        #region Private Variables
        [SerializeField]
        private EllipseObjectInteractionShape _circleShape = new EllipseObjectInteractionShape();
        #endregion

        #region Public Properties
        public EllipseShapeRenderSettings CircleShapeRenderSettings { get { return _circleShape.RenderSettings; } }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            if (IsVisible())
            {
                EnsureWidthAndHeightAreUpToDate();
                GetShape().RenderGizmos();
            }
        }

        public List<GameObject> GetOverlappedGameObjectsForEraseOperation()
        {
            return GetShape().GetOverlappedGameObjects(ObjectEraserSettings.Get().Mass2DEraseSettings.AllowPartialOverlap);
        }

        public bool IsVisible()
        {
            if (ObjectEraserSettings.Get().EraseMode != ObjectEraseMode.ObjectMass2D) return false;
            if (!SceneViewCamera.Camera.pixelRect.Contains(Event.current.mousePosition)) return false;

            return true;
        }

        public void HandleMouseMoveEvent(Event e)
        {
            if (IsVisible())
            {
                EnsureWidthAndHeightAreUpToDate();
                EnsureCenterIsUpToDate();
            }
        }

        public void HandleMouseDragEvent(Event e)
        {
            if(IsVisible())
            {
                EnsureWidthAndHeightAreUpToDate();
                EnsureCenterIsUpToDate();
            }
        }
        #endregion

        #region Private Methods
        private ObjectInteraction2DShape GetShape()
        {
            return _circleShape;
        }

        private void EnsureWidthAndHeightAreUpToDate()
        {
            Object2DMassEraseSettings objectMassEraseSettings = ObjectEraserSettings.Get().Mass2DEraseSettings;
            SetWidthHeight(objectMassEraseSettings.CircleShapeRadius, objectMassEraseSettings.CircleShapeRadius);
        }

        private void EnsureCenterIsUpToDate()
        {
            GetShape().EnclosingRectCenter = Event.current.InvMousePos(SceneViewCamera.Camera);
        }

        private void SetWidthHeight(float width, float height)
        {
            Vector2 center = _circleShape.EnclosingRectCenter;
            _circleShape.EnclosingRectWidth = width;
            _circleShape.EnclosingRectHeight = height;
            _circleShape.EnclosingRectCenter = center;
        }
        #endregion
    }
}
#endif