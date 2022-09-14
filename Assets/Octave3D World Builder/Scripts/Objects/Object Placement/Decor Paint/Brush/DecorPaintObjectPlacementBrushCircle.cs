#if UNITY_EDITOR
using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushCircle
    {
        #region Private Variables
        [SerializeField]
        private XZOrientedEllipseObjectInteractionShape _circleShape = new XZOrientedEllipseObjectInteractionShape();
        #endregion

        #region Public Properties
        public XZOrientedEllipseShapeRenderSettings CircleShapeRenderSettings { get { return _circleShape.RenderSettings; } }
        public Plane Plane { get { return _circleShape.Plane; } }
        public Vector3 Center { get { return _circleShape.Center; } }
        public MouseCursorRayHit CursorRayHit { get { return _circleShape.CursorRayHit; } }
        public bool IsCursorRayHitAvailable { get { return _circleShape.IsCursorRayHitAvailable(); } }
        public bool IsSittingOnGridCell { get { return _circleShape.HasCursorPickedGridCell(); } }
        public bool IsSittingOnObject { get { return _circleShape.HasCursorPickedObject(); } }
        public bool IsSittingOnMesh { get { return _circleShape.HasCursorPickedMeshObject(); } }
        public bool IsSittingOnTerrain { get { return _circleShape.HasCursorPickedTerrainObject(); } }
        public bool IsSittingOnSprite { get { return _circleShape.HasCursorPickedSprite(); } }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            if (IsVisible())
            {
                OnBeforeRender();
                _circleShape.RenderGizmos();
            }
        }

        public bool IsVisible()
        {
            if (DecorPaintObjectPlacementSettings.Get().DecorPaintMode != DecorPaintMode.Brush ||
                DecorPaintObjectPlacementBrushDatabase.Get().ActiveBrush == null) return false;
            return true;
        }

        public void HandleMouseMoveEvent(Event e)
        {
            if (IsVisible())
            {
                AdjustCircleRadiusIfNecessary();
                AcquireCursorRayHit(false);
                _circleShape.AdjustShapeCenterAndNormalBasedOnCursorRayHit();

                SceneView.RepaintAll();
            }
        }

        public void HandleMouseDragEvent(Event e)
        {
            if (IsVisible())
            {
                AdjustCircleRadiusIfNecessary();
                AcquireCursorRayHit(true);
                _circleShape.AdjustShapeCenterAndNormalBasedOnCursorRayHit();
            }
        }

        public Vector3 GetLocalAxis(CoordinateSystemAxis axis)
        {
            return _circleShape.GetLocalAxis(axis);
        }

        public Vector3 GetRandomPointInside()
        {
            return _circleShape.GetRandomPointInside();
        }

        public bool ContainsPoint(Vector3 point)
        {
            return _circleShape.ContainsPoint(point);
        }
        #endregion

        #region Private Methods
        private void OnBeforeRender()
        {
            AdjustCircleRadiusIfNecessary();
        }

        private void AcquireCursorRayHit(bool dragging)
        {
            if (!dragging)
            {
                _circleShape.AcquireCursorRayHit(DecorPaintObjectPlacement.Get().DecorPaintMask, MouseCursorObjectPickFlags.ObjectBox);
            }
            else
            {
                if (_circleShape.HasCursorPickedTerrainObject())
                {
                    // When we are dragging on the surface of a terrain object, we don't want to allow the brush to
                    // start hovering other objects. That can be very undesirable. So we will always attempt to keep
                    // the brush over the terrain.
                    _circleShape.AcquireCursorRayHitForTerrainObject(_circleShape.GetGameObjectPickedByCursor());
                }
                else _circleShape.AcquireCursorRayHit(DecorPaintObjectPlacement.Get().DecorPaintMask, MouseCursorObjectPickFlags.ObjectBox);
            }
        }

        private void AdjustCircleRadiusIfNecessary()
        {
            float desiredCircleRadius = DecorPaintObjectPlacementBrushDatabase.Get().ActiveBrush.Radius;
            if (_circleShape.MaxModelSpaceRadius != desiredCircleRadius) _circleShape.ModelSpaceRadii = new Vector2(desiredCircleRadius, desiredCircleRadius);
        }
        #endregion
    }
}
#endif