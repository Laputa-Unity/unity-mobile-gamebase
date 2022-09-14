#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class SnapSurface
    {
        #region Private Variables
        [SerializeField]
        private bool _isValid;

        [SerializeField]
        private ObjectColliderSnapSurfaceGrid _objectBoxSnapSurfaceGrid = new ObjectColliderSnapSurfaceGrid();

        [SerializeField]
        private XZOrientedQuad3D _surfaceQuad;
        [SerializeField]
        private Vector3 _mouseCursorPickPoint;
        [SerializeField]
        private SnapSurfaceType _type;

        [SerializeField]
        private GameObject _surfaceObject;
        #endregion

        #region Public Static Properties
        public static Vector3 ModelSpaceRightAxis { get { return XZOrientedQuad3D.ModelSpaceRightAxis; } }
        public static Vector3 ModelSpacePlaneNormal { get { return XZOrientedQuad3D.ModelSpacePlaneNormal; } }
        public static Vector3 ModelSpaceLookAxis { get { return XZOrientedQuad3D.ModelSpaceLookAxis; } }
        #endregion

        #region Public Properties
        public bool IsValid { get { return _isValid; } }
        public Plane Plane { get { return _surfaceQuad != null ? _surfaceQuad.Plane : new Plane(); } }
        public XZOrientedQuad3D SurfaceQuad { get { return new XZOrientedQuad3D(_surfaceQuad); } }
        public XZGridRenderSettings RenderSettingsForColliderSnapSurfaceGrid { get { return _objectBoxSnapSurfaceGrid.RenderSettings; } }
        public GameObject SurfaceObject { get { return _surfaceObject; } }
        public Vector3 Center { get { return _surfaceQuad != null ? _surfaceQuad.Center : Vector3.zero; } }
        public Vector3 CursorPickPoint { get { return _mouseCursorPickPoint; } }
        public SnapSurfaceType SurfaceType { get { return _type; } }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            // Note: We will check if the surface object is null so that we don't render the surface
            //       if the object has somehow been deleted from the scene.
            if (IsValid && _type == SnapSurfaceType.ObjectCollider && _surfaceObject != null && !ObjectSnapping.Get().Settings.EnableObjectToObjectSnap) _objectBoxSnapSurfaceGrid.RenderGizmos();
        }

        public void Refresh()
        {
            if (_isValid && _type == SnapSurfaceType.ObjectCollider)
            {
                _objectBoxSnapSurfaceGrid.FromXZOrientedQuad(_surfaceQuad);
                SceneView.RepaintAll();
            }
        }

        public void FromMouseCursorRayHit(MouseCursorRayHit cursorRayHit)
        {
            _isValid = false;

            if (!cursorRayHit.WasAnythingHit) return;
            else 
            if (cursorRayHit.WasAnObjectHit && ObjectSnapSettings.Get().EnableObjectSurfaceGrid)
            {
                GameObjectRayHit gameObjectRayHit = FindClosestHitObjectWhichCanBeUsedAsSnapSurface(cursorRayHit);
                if (gameObjectRayHit != null)
                {
                    if (!cursorRayHit.WasACellHit) ExtractData(gameObjectRayHit);
                    else
                    {
                        if (gameObjectRayHit.HitEnter <= cursorRayHit.GridCellRayHit.HitEnter) ExtractData(gameObjectRayHit);
                    }
                }            
            }

            // If we didn't manage to build the surface up until now, we will give it one last chance
            // using the cursor grid cell ray hit.
            if (!_isValid && cursorRayHit.WasACellHit) ExtractData(cursorRayHit.GridCellRayHit);
        }

        public Vector3 GetSnapDestinationPointClosestToCursorPickPoint()
        {
            if(IsValid)
            {
                if (_type == SnapSurfaceType.GridCell)
                {
                    List<Vector3> snapDestinationPoints = _surfaceQuad.GetCenterAndCornerPoints();
                    snapDestinationPoints.AddRange(_surfaceQuad.GetMidEdgePoints());

                    return Vector3Extensions.GetClosestPointToPoint(snapDestinationPoints, _mouseCursorPickPoint);
                }
                else
                {
                    XZOrientedQuad3D gridCellQuad = _objectBoxSnapSurfaceGrid.GetCellFromPoint(_mouseCursorPickPoint).Quad;
                    List<Vector3> snapDestinationPoints = gridCellQuad.GetCenterAndCornerPoints();
                    snapDestinationPoints.AddRange(gridCellQuad.GetMidEdgePoints());

                    return Vector3Extensions.GetClosestPointToPoint(snapDestinationPoints, _mouseCursorPickPoint);
                }
            }
            return Vector3.zero;
        }

        public TransformMatrix GetTransformMatrix()
        {
            if (IsValid) return _surfaceQuad.TransformMatrix;
            return TransformMatrix.GetIdentity();
        }

        public Vector3 GetLocalAxis(CoordinateSystemAxis axis)
        {
            if (IsValid) return _surfaceQuad.GetLocalAxis(axis);
            return Vector3.zero;
        }

        public Vector3 GetNormal()
        {
            if (IsValid) return _surfaceQuad.GetLocalAxis(CoordinateSystemAxis.PositiveUp);
            return Vector3.up;
        }
        #endregion

        #region Private Methods
        private GameObjectRayHit FindClosestHitObjectWhichCanBeUsedAsSnapSurface(MouseCursorRayHit cursorRayHit)
        {
            ObjectMask objectSnapMask = ObjectSnapping.Get().ObjectSnapMask;

            List<GameObjectRayHit> gameObjectHits = cursorRayHit.SortedObjectRayHits;
            for(int hitIndex = 0; hitIndex < gameObjectHits.Count; ++hitIndex)
            {
                GameObject hitObject = gameObjectHits[hitIndex].HitObject;

                if (!hitObject.HasMesh()) continue;
                if (objectSnapMask.IsGameObjectMasked(hitObject)) continue;

                return gameObjectHits[hitIndex];
            }

            return null;
        }

        private void ExtractData(GameObjectRayHit objectRayHit)
        {
            if (objectRayHit.WasBoxHit)
            {
                _isValid = true;
                _mouseCursorPickPoint = objectRayHit.HitPoint;
                CalculateSurfaceQuad(objectRayHit.ObjectBoxHit);
                _objectBoxSnapSurfaceGrid.FromXZOrientedQuad(_surfaceQuad);
                _type = SnapSurfaceType.ObjectCollider;
                _surfaceObject = objectRayHit.HitObject;
            }
        }

        private void CalculateSurfaceQuad(OrientedBoxRayHit boxRayHit)
        {
            OrientedBox hitBox = boxRayHit.HitBox;
            CoordinateSystem pickedFaceCoordSystem = hitBox.GetBoxFaceCoordinateSystem(boxRayHit.HitFace);
            _surfaceQuad = new XZOrientedQuad3D(pickedFaceCoordSystem.GetOriginPosition(),
                                                hitBox.GetBoxFaceSizeAlongFaceLocalXZAxes(boxRayHit.HitFace),
                                                pickedFaceCoordSystem.GetRotation());
        }

        private void ExtractData(GridCellRayHit gridCellRayHit)
        {
            _isValid = true;
            _surfaceQuad = gridCellRayHit.HitCell.Quad;
            _mouseCursorPickPoint = gridCellRayHit.HitPoint;
            _type = SnapSurfaceType.GridCell;
            _surfaceObject = null;
        }
        #endregion
    }
}
#endif