#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectSnapping : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectSnapSettings _settings;
        [SerializeField]
        private ObjectMask _objectSnapMask = new ObjectMask();
        [SerializeField]
        private XZGrid _xzSnapGrid;
        [SerializeField]
        private SnapSurface _objectSnapSurface = new SnapSurface();

        [SerializeField]
        private bool _wasInitialized = false;
        #endregion
        
        #region Public Properties
        public ObjectMask ObjectSnapMask { get { return _objectSnapMask; } }
        public ObjectSnapSettings Settings
        {
            get
            {
                if (_settings == null) _settings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectSnapSettings>();
                return _settings;
            }
        }
        public XZGrid XZSnapGrid
        {
            get
            {
                if (_xzSnapGrid == null) _xzSnapGrid = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<XZGrid>();
                return _xzSnapGrid;
            }
        }
        public XZGridRenderSettings RenderSettingsForColliderSnapSurfaceGrid { get { return _objectSnapSurface.RenderSettingsForColliderSnapSurfaceGrid; } }
        public Plane ObjectSnapSurfacePlane { get { return _objectSnapSurface.Plane; } }
        public GameObject ObjectSnapSurfaceObject { get { return _objectSnapSurface.SurfaceObject; } }
        public SnapSurfaceType SnapSurfaceType { get { return _objectSnapSurface.SurfaceType; } }
        #endregion

        #region Public Static Functions
        public static ObjectSnapping Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ObjectSnapping;
        }
        #endregion

        #region Public Methods
        public void RefreshSnapSurface()
        {
            _objectSnapSurface.Refresh();
        }

        public void RenderGizmos()
        {
            XZSnapGrid.RenderGizmos();
            if (!Settings.SnapToCursorHitPoint) _objectSnapSurface.RenderGizmos();
        }

        public void SnapXZGridToCursorPickPoint(bool snapToClosestTopOrBottom)
        {
            MouseCursor.Instance.PushObjectMask(null);
            MouseCursor.Instance.PushObjectPickMaskFlags(MouseCursorObjectPickFlags.ObjectBox | MouseCursorObjectPickFlags.ObjectTerrain);
            MouseCursorRayHit cursorRayHit = MouseCursor.Instance.GetRayHit();
            MouseCursor.Instance.PopObjectPickMaskFlags();
            MouseCursor.Instance.PopObjectMask();

            if (cursorRayHit.WasAnythingHit)
            {
                Vector3 snapDestPoint;
                if (cursorRayHit.WasAnObjectHit)
                {
                    snapDestPoint = cursorRayHit.ClosestObjectRayHit.HitPoint;
                    UndoEx.RecordForToolAction(XZSnapGrid);

                    if(snapToClosestTopOrBottom)
                    {
                        Box objectWorldBox = cursorRayHit.ClosestObjectRayHit.HitObject.GetWorldBox();
                        Vector3 fromCenterToHitPoint = snapDestPoint - objectWorldBox.Center;
                        if (Vector3.Dot(fromCenterToHitPoint, Vector3.up) > 0.0f) snapDestPoint = objectWorldBox.Center + Vector3.up * objectWorldBox.Extents.y;
                        else snapDestPoint = objectWorldBox.Center - Vector3.up * objectWorldBox.Extents.y;
                    }

                    XZSnapGrid.SnapToPoint(snapDestPoint);
                }
            }
        }

        public List<XZGrid> GetAllSnapGrids()
        {
            var allSnapGrids = new List<XZGrid>();
            allSnapGrids.Add(XZSnapGrid);

            return allSnapGrids;
        }

        public void UpdateProjectedBoxFacePivotPoints(GameObject hierarchyRoot, ProjectedBoxFacePivotPoints projectedBoxFacePivotPoints, bool keepCurrentSnapSurface)
        {
            OrientedBox hierarchyWorldOrientedBox = hierarchyRoot.GetHierarchyWorldOrientedBox();
            if (!hierarchyWorldOrientedBox.IsValid()) return;

            if(keepCurrentSnapSurface)
            {
                if (!_objectSnapSurface.IsValid) return;
                projectedBoxFacePivotPoints.FromOrientedBoxAndSnapSurface(hierarchyWorldOrientedBox, _objectSnapSurface);
            }
            else
            {
                _objectSnapSurface.FromMouseCursorRayHit(GetCursorRayHit());
                projectedBoxFacePivotPoints.FromOrientedBoxAndSnapSurface(hierarchyWorldOrientedBox, _objectSnapSurface);
            }
        }

        public void SnapObjectHierarchy(GameObject hierarchyRoot, ProjectedBoxFacePivotPoints projectedBoxFacePivotPoints, float offsetFromSnapSurface)
        {
            _objectSnapSurface.FromMouseCursorRayHit(GetCursorRayHit());
            if (_objectSnapSurface.IsValid)
            {
                OrientedBox hierarchyWorldOrientedBox = hierarchyRoot.GetHierarchyWorldOrientedBox();
                if (!hierarchyWorldOrientedBox.IsValid()) return;

                Vector3 pivotPoint = projectedBoxFacePivotPoints.ActivePoint;
                if (Settings.UseOriginalPivot) pivotPoint = hierarchyRoot.transform.position;

                if (Settings.SnapToCursorHitPoint || Settings.EnableObjectToObjectSnap)
                {
                    SnapObjectHierarchyPosition(hierarchyRoot, pivotPoint, _objectSnapSurface.CursorPickPoint, projectedBoxFacePivotPoints, offsetFromSnapSurface);
                }
                else
                {
                    if (_objectSnapSurface.SurfaceType == SnapSurfaceType.GridCell && Settings.SnapCenterToCenterForXZGrid && !Settings.UseOriginalPivot) SnapObjectHierarchyToCenterOfSnapSurface(hierarchyRoot, projectedBoxFacePivotPoints.CenterPoint, projectedBoxFacePivotPoints, offsetFromSnapSurface);
                    else
                    if (_objectSnapSurface.SurfaceType == SnapSurfaceType.ObjectCollider && Settings.SnapCenterToCenterForObjectSurface && !Settings.UseOriginalPivot) SnapObjectHierarchyToCenterOfSnapSurface(hierarchyRoot, projectedBoxFacePivotPoints.CenterPoint, projectedBoxFacePivotPoints, offsetFromSnapSurface);
                    else SnapObjectHierarchyPosition(hierarchyRoot, pivotPoint, _objectSnapSurface.GetSnapDestinationPointClosestToCursorPickPoint(), projectedBoxFacePivotPoints, offsetFromSnapSurface);
                    if (AllShortcutCombos.Instance.KeepSnappedHierarchyInSnapSurfaceArea.IsActive()) KeepSnappedHierarchyInSnapSurfaceArea(hierarchyRoot, projectedBoxFacePivotPoints);
                }

                if (Settings.EnableObjectToObjectSnap) SnapHierarchyToNearbyObjects(hierarchyRoot, projectedBoxFacePivotPoints);
            }
        }

        public void SnapHierarchyToNearbyObjects(GameObject hierarchyRoot, ProjectedBoxFacePivotPoints projectedBoxFacePivotPoints)
        {
            Object2ObjectSnap.SnapResult snapResult = Object2ObjectSnap.Snap(hierarchyRoot, Settings.ObjectToObjectSnapEpsilon, ObjectSnapping.Get().ObjectSnapMask.ObjectCollectionMask.GetAllMaskedGameObjects());
            if (snapResult.WasSnapped) projectedBoxFacePivotPoints.MovePoints(snapResult.SnapDestination - snapResult.SnapPivot);
        }

        public void SnapObjectHierarchyToCenterOfSnapSurface(GameObject hierarchyRoot, Vector3 snapPivotPoint, ProjectedBoxFacePivotPoints projectedBoxFacePivotPoints, float offsetFromSnapSurface)
        {
            _objectSnapSurface.FromMouseCursorRayHit(GetCursorRayHit());
            if (_objectSnapSurface.IsValid)
            {
                OrientedBox hierarchyWorldOrientedBox = hierarchyRoot.GetHierarchyWorldOrientedBox();
                if (!hierarchyWorldOrientedBox.IsValid()) return;

                SnapObjectHierarchyPosition(hierarchyRoot, snapPivotPoint, _objectSnapSurface.Center, projectedBoxFacePivotPoints, offsetFromSnapSurface);
            }
        }

        public void SnapObjectPosition(GameObject gameObject)
        {
            _objectSnapSurface.FromMouseCursorRayHit(GetCursorRayHit());
            if(_objectSnapSurface.IsValid)
            {
                Transform objectTransform = gameObject.transform;
                objectTransform.position = _objectSnapSurface.GetSnapDestinationPointClosestToCursorPickPoint();
            }
        }

        public void SnapObjectPositionToSnapSurfaceCenter(GameObject gameObject)
        {
            _objectSnapSurface.FromMouseCursorRayHit(GetCursorRayHit());
            if (_objectSnapSurface.IsValid)
            {
                Transform objectTransform = gameObject.transform;
                objectTransform.position = _objectSnapSurface.Center;
            }
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            if(!_wasInitialized)
            {
                CoordinateSystemRenderSettings coordSystemRenderSettings = XZSnapGrid.RenderableCoordinateSystem.RenderSettings;
                coordSystemRenderSettings.SetAxisRenderInfinite(CoordinateSystemAxis.PositiveRight, true);
                coordSystemRenderSettings.SetAxisRenderInfinite(CoordinateSystemAxis.NegativeRight, true);
                coordSystemRenderSettings.SetAxisRenderInfinite(CoordinateSystemAxis.PositiveLook, true);
                coordSystemRenderSettings.SetAxisRenderInfinite(CoordinateSystemAxis.NegativeLook, true);

                _wasInitialized = true;
            }
        }

        private MouseCursorRayHit GetCursorRayHit()
        {
            MouseCursor.Instance.PushObjectPickMaskFlags(MouseCursorObjectPickFlags.ObjectTerrain | MouseCursorObjectPickFlags.ObjectMesh);
            MouseCursor.Instance.PushObjectMask(ObjectSnapMask);

            MouseCursorRayHit cursorRayHit = MouseCursor.Instance.GetRayHit();

            MouseCursor.Instance.PopObjectPickMaskFlags();
            MouseCursor.Instance.PopObjectMask();

            return cursorRayHit;
        }

        private void SnapObjectHierarchyPosition(GameObject hierarchyRoot, Vector3 snapPivotPoint, Vector3 snapDestinationPoint, ProjectedBoxFacePivotPoints projectedBoxFacePivotPoints, float offsetFromSnapSurface)
        {
            snapDestinationPoint += _objectSnapSurface.Plane.normal * offsetFromSnapSurface;

            Transform hierarchyRootTransform = hierarchyRoot.transform;
            Vector3 snapVector = hierarchyRootTransform.position - snapPivotPoint;
            hierarchyRootTransform.position = snapDestinationPoint + snapVector;
            projectedBoxFacePivotPoints.MovePoints(snapDestinationPoint - snapPivotPoint);
        }

        private void KeepSnappedHierarchyInSnapSurfaceArea(GameObject hierarchyRoot, ProjectedBoxFacePivotPoints projectedHierarchyBoxFacePivotPoints)
        {
            OrientedBox hierarchyWorldOrientedBox = hierarchyRoot.GetHierarchyWorldOrientedBox();
            List<Vector3> worldBoxPoints = hierarchyWorldOrientedBox.GetCenterAndCornerPoints();
            XZOrientedQuad3D snapSurfaceQuad = _objectSnapSurface.SurfaceQuad;

            List<Plane> quadSegmentPlanes = snapSurfaceQuad.GetBoundarySegmentPlanesFacingOutward();
            List<Vector3> pushVectors = new List<Vector3>(quadSegmentPlanes.Count);

            // All box points which are in front of the surface quad's plane are outside
            // the surface so we will have to push them back.
            for(int segmentPlaneIndex = 0; segmentPlaneIndex < quadSegmentPlanes.Count; ++segmentPlaneIndex)
            {
                Plane segmentPlane = quadSegmentPlanes[segmentPlaneIndex];
                Vector3 furthestPointInFront;
                if(segmentPlane.GetFurthestPointInFront(worldBoxPoints, out furthestPointInFront))
                {
                    Vector3 projectedPoint = segmentPlane.ProjectPoint(furthestPointInFront);
                    pushVectors.Add(projectedPoint - furthestPointInFront);
                }
            }

            Transform hierarchyRootTransform = hierarchyRoot.transform;
            foreach(Vector3 pushVector in pushVectors)
            {
                hierarchyRootTransform.position += pushVector;
                projectedHierarchyBoxFacePivotPoints.MovePoints(pushVector);
            }
        }        
        #endregion
    }
}
#endif