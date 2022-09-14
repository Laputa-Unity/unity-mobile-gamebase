#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class SingleModeDecorPaintStroke : DecorPaintStroke
    {
        #region Private Variables
        private List<GameObject> _objectsPlacedWhileDragging = new List<GameObject>();
        private ProjectedBoxFacePivotPoints _pivotPointsOfLastPlacedHierarchy = new ProjectedBoxFacePivotPoints();
        private OrientedBox _orientedBoxOfLastPlacedHierarchy;
        private int _indexOfStrokeAlignmentSegmentPlane = -1;
        #endregion

        #region Public Methods
        public override void RenderGizmos()
        {
        }

        public void UpdateGuidePivotPoints()
        {
            if (GuideExistsInSceneAndStrokeSurfaceIsValid()) AdjustGuidePivotPoints();
        }

        public Vector3 CalculatePlacementGuidePosition()
        {
            if (_strokeSurface.IsValid) return CalculateNewPlacementGuidePosition();
            return Vector3.zero;
        }
        #endregion

        #region Protected Methods
        protected override void OnMouseMoved(Event e)
        {
            if (CanAdjustPlacementGuideTransformAndPivotPoints())
            {
                AdjustPlacementGuideRotation();
                AdjustPlacementGuidePositionAndPivotPoints();
            }
        }

        protected override void OnStrokeStarted(Event e)
        {
            _objectsPlacedWhileDragging.Clear();
            if (CanPlaceObjectOnStrokeStart())
            {
                // Note: We won't perform any intersection tests when the first object is palced because this
                //       ocurs when the user clicks a mouse button and in that case they may not necessarily
                //       want to start a drag. It's more of a PointAndClick type of placement.
                PlaceObjectFromPlacementGuide(false);
                RegisterPlacementPoint();
                ApplyRotationAndScaleRandimizationsForPlacementGuideIfNecessary();
            }
        }

        protected override void OnStrokeEnded(Event e)
        {
            DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectCollectionMask.Unmask(_objectsPlacedWhileDragging);
            _objectsPlacedWhileDragging.Clear();
        }

        protected override void OnStrokeDragged(Event e)
        {
            if (!WasNecessaryStrokeAlignmentDataCalculated() &&
                CanCalculateNecessaryStrokeAlignmentData()) CalculateAllNecessaryStrokeAlignmentData();

            if (CanAdjustPlacementGuideTransformAndPivotPoints())
            {
                AdjustPlacementGuideRotation();
                AdjustPlacementGuidePositionAndPivotPoints();
            }

            if (CanPlaceObjectOnStrokeDrag())
            {
                PlaceObjectFromPlacementGuide(true);
                RegisterPlacementPoint();
                ApplyRotationAndScaleRandimizationsForPlacementGuideIfNecessary();
            }
        }

        protected override Vector3 CalculateRotationAxisForGuideStrokeAlignment()
        {
            Vector3 rotationAxis = Vector3.Cross(PenultimateStrokeTravelDirection, LastStrokeTravelDirection);
            rotationAxis.Normalize();

            return rotationAxis;
        }

        protected override float CalculateRotationDegreeAngleUsedForStrokeAlignment()
        {
            float angle = PenultimateStrokeTravelDirection.AngleWith(LastStrokeTravelDirection);
            return !float.IsNaN(angle) ? angle : 0.0f;
        }
        #endregion

        #region Private Methods
        private bool WasNecessaryStrokeAlignmentDataCalculated()
        {
            return _initialStrokeTravelDirection.magnitude != 0.0f;
        }

        private bool CanCalculateNecessaryStrokeAlignmentData()
        {
            return GuideExistsInSceneAndStrokeSurfaceIsValid() && 
                   IsOnlyOneStrokePlacementPointAvailable() &&
                   WasCursorMovedFromCenterPivotPointOfLastPlacedHierarchy();
        }

        private bool WasCursorMovedFromCenterPivotPointOfLastPlacedHierarchy()
        {
            return (_pivotPointsOfLastPlacedHierarchy.CenterPoint - _strokeSurface.MouseCursorPickPoint).magnitude != 0.0f;
        }

        private bool CanAdjustPlacementGuideTransformAndPivotPoints()
        {
            return GuideExistsInSceneAndStrokeSurfaceIsValid();
        }

        private bool CanPlaceObjectOnStrokeStart()
        {
            if (DecorPaintObjectPlacementSettings.Get().IgnoreGrid && HasCursorPickedGridCellButNoObject()) return false;
            return GuideExistsInSceneAndStrokeSurfaceIsValid();
        }

        private bool GuideExistsInSceneAndStrokeSurfaceIsValid()
        {
            return ObjectPlacementGuide.ExistsInSceneAndIsActive && _strokeSurface.IsValid;
        }

        private bool CanPlaceObjectOnStrokeDrag()
        {
            if (DecorPaintObjectPlacementSettings.Get().IgnoreGrid && HasCursorPickedGridCellButNoObject()) return false;

            bool canPlace = ObjectPlacementGuide.ExistsInSceneAndIsActive && _strokeSurface.IsValid;
            if (!canPlace) return false;

            if (MustAlignGuideToStroke()) return !DoesGuideIntersectPreviuslyPlacedObjectHierarchy();
            return IsStrokeDistanceConditionSatisfied();
        }

        private bool DoesGuideIntersectPreviuslyPlacedObjectHierarchy()
        {
            OrientedBox guideWorldOrientedBox = ObjectPlacementGuide.SceneObject.GetHierarchyWorldOrientedBox();
            return guideWorldOrientedBox.Intersects(_orientedBoxOfLastPlacedHierarchy);
        }

        private bool MustAlignGuideToStroke()
        {
            return DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.AlignToStroke;
        }

        private bool CanAlignGuideToStroke()
        {
            return ObjectPlacementGuide.ExistsInSceneAndIsActive && NumberOfStrokePlacementPoints != 0;
        }

        private void PlaceObjectFromPlacementGuide(bool checkForIntersection)
        {
            ObjectPlacementGuide placementGuide = ObjectPlacementGuide.Instance;
            GameObject placedHierarchyRoot = null;

            OrientedBox guideHierarchyWorldOrientedBox = ObjectPlacementGuide.SceneObject.GetHierarchyWorldOrientedBox();
            List<GameObject> intersectingObjects = ObjectQueries.GetIntersectingObjects(guideHierarchyWorldOrientedBox, new List<GameObject> { _strokeSurface.SurfaceObject }, true);

            if (!checkForIntersection || ObjectPlacementSettings.Get().ObjectIntersectionSettings.AllowIntersectionForDecorPaintSingleModeDrag ||
               intersectingObjects.Count == 0)
            {
                placedHierarchyRoot = Octave3DScene.Get().InstantiateObjectHierarchyFromPrefab(placementGuide.SourcePrefab, placementGuide.gameObject.transform);
                ObjectHierarchyRootsWerePlacedInSceneMessage.SendToInterestedListeners(placedHierarchyRoot, ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.ObjectPlacement);

                List<GameObject> allPlacedObjects = placedHierarchyRoot.GetAllChildrenIncludingSelf();
                _objectsPlacedWhileDragging.AddRange(allPlacedObjects);
                DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectCollectionMask.Mask(allPlacedObjects);
            }

            _orientedBoxOfLastPlacedHierarchy = ObjectPlacementGuide.SceneObject.GetHierarchyWorldOrientedBox();
            _pivotPointsOfLastPlacedHierarchy.FromOrientedBoxAndDecorStrokeSurface(_orientedBoxOfLastPlacedHierarchy, _strokeSurface);

            if(DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.RandomizePrefabsInActiveCategory)
            {
                PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                if (activePrefabCategory != null) activePrefabCategory.RandomizeActivePrefab();
            }
        }

        private void CalculateAllNecessaryStrokeAlignmentData()
        {
            CalculateIndexOfStrokeAlignmentSegmentPlane();
            CalculateInitialStrokeTravelDirection();
        }

        private void CalculateIndexOfStrokeAlignmentSegmentPlane()
        {
            int indexOfPointClosestToStrokeSurfacePickPoint = _pivotPointsOfLastPlacedHierarchy.GetIndexOfPointClosestToPoint(_strokeSurface.MouseCursorPickPoint);
            if (indexOfPointClosestToStrokeSurfacePickPoint != ProjectedBoxFacePivotPoints.IndexOfPointInCenter)
            {
                Vector3 pivotPointClosestToStrokeSurfacePickPoint = _pivotPointsOfLastPlacedHierarchy.GetPointByIndex(indexOfPointClosestToStrokeSurfacePickPoint);
               
                // Now identify the segment whose center point is closest to the calculated pivot point
                List<Segment3D> segments = _pivotPointsOfLastPlacedHierarchy.GetAllBoundarySegments();
                int indexOfBestSegment = -1;
                float minDistance = float.MaxValue;
                for(int segmentIndex = 0; segmentIndex < segments.Count; ++segmentIndex)
                {
                    Segment3D segment = segments[segmentIndex];
                    Vector3 segmentMidPoint = segment.GetPoint(0.5f);

                    float distanceFromPivotPoint = (segmentMidPoint - pivotPointClosestToStrokeSurfacePickPoint).magnitude;
                    if(distanceFromPivotPoint < minDistance)
                    {
                        minDistance = distanceFromPivotPoint;
                        indexOfBestSegment = segmentIndex;
                    }
                }

                _indexOfStrokeAlignmentSegmentPlane = indexOfBestSegment;
            }
            else
            {
                Vector3 fromCenterToStrokeSurfacePickPoint = _strokeSurface.MouseCursorPickPoint - _pivotPointsOfLastPlacedHierarchy.CenterPoint;
                List<Plane> pivotPointBoundarySegmentPlanes = _pivotPointsOfLastPlacedHierarchy.GetAllBoundarySegmentPlanes();
                _indexOfStrokeAlignmentSegmentPlane = PlaneExtensions.GetIndexOfPlaneWhoseNormalIsMostAlignedWithDir(pivotPointBoundarySegmentPlanes, fromCenterToStrokeSurfacePickPoint);
            }

            /* Left here for reference. This is the way in which it was initially done.
            if(indexOfPointClosestToStrokeSurfacePickPoint != ProjectedBoxFacePivotPoints.IndexOfPointInCenter)
            {
                Vector3 pivotPointClosestToStrokeSurfacePickPoint = _pivotPointsOfLastPlacedHierarchy.GetPointByIndex(indexOfPointClosestToStrokeSurfacePickPoint);
                Vector3 fromCenterPivotToCalculatedClosestPoint = pivotPointClosestToStrokeSurfacePickPoint - _pivotPointsOfLastPlacedHierarchy.CenterPoint;

                pivotPointBoundarySegmentPlanes = _pivotPointsOfLastPlacedHierarchy.GetAllBoundarySegmentPlanes();
                _indexOfStrokeAlignmentSegmentPlane = PlaneExtensions.GetIndexOfPlaneWhoseNormalIsMostAlignedWithDir(pivotPointBoundarySegmentPlanes, fromCenterPivotToCalculatedClosestPoint);
            }
            else
            {
                fromClosestPointToStrokeSurfacePickPoint = _strokeSurface.MouseCursorPickPoint - _pivotPointsOfLastPlacedHierarchy.CenterPoint;
                pivotPointBoundarySegmentPlanes = _pivotPointsOfLastPlacedHierarchy.GetAllBoundarySegmentPlanes();
                _indexOfStrokeAlignmentSegmentPlane = PlaneExtensions.GetIndexOfPlaneWhoseNormalIsMostAlignedWithDir(pivotPointBoundarySegmentPlanes, fromCenterPivotToStrokeSurfacePickPoint);
            }
            */
        }

        private void CalculateInitialStrokeTravelDirection()
        {
            List<Plane> pivotPointBoundarySegmentPlanes = _pivotPointsOfLastPlacedHierarchy.GetAllBoundarySegmentPlanes();
            _initialStrokeTravelDirection = pivotPointBoundarySegmentPlanes[_indexOfStrokeAlignmentSegmentPlane].normal;
        }

        private void AdjustPlacementGuidePositionAndPivotPoints()
        {
            AdjustGuidePivotPoints();
            ObjectPlacementGuide.Instance.SetWorldPosition(CalculateNewPlacementGuidePosition());
            if (!DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.PlacementGuideSurfaceAlignmentSettings.IsEnabled &&
                 DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.EmbedInSurfaceWhenNoAlign) 
            {
                if(_strokeSurface.Type == DecorPaintStrokeSurfaceType.Terrain) ObjectPlacementGuide.SceneObject.EmbedInSurfaceByVertex(-Vector3.up, _strokeSurface.SurfaceObject);
                else if (_strokeSurface.Type == DecorPaintStrokeSurfaceType.Mesh) ObjectPlacementGuide.SceneObject.EmbedInSurfaceByVertex(-_strokeSurface.Plane.normal, _strokeSurface.SurfaceObject);
                ObjectPlacementGuide.Instance.RegisterCurrentPosition();
            }

            // Note: After the position of the guide has changed, the pivot points must be recalculated
            //       for the next repaint event. Otherwise there will be a discrepancy between the way
            //       in which the pivot points are rendered and what is actually happening.
            AdjustGuidePivotPoints();
        }

        private void AdjustGuidePivotPoints()
        {
            OrientedBox guideHierarchyWorldOrientedBox = ObjectPlacementGuide.SceneObject.GetHierarchyWorldOrientedBox();
            ObjectPlacement.Get().ProjectedGuidePivotPoints.FromOrientedBoxAndDecorStrokeSurface(guideHierarchyWorldOrientedBox, _strokeSurface);
        }

        private Vector3 CalculateNewPlacementGuidePosition()
        {
            float offsetFromSurface = ObjectPlacementGuide.Instance.SourcePrefab.OffsetFromGridSurface;
            if (SurfaceType != DecorPaintStrokeSurfaceType.GridCell) offsetFromSurface = ObjectPlacementGuide.Instance.SourcePrefab.OffsetFromObjectSurface;

            if (AllShortcutCombos.Instance.PlaceGuideBehindSurfacePlane.IsActive()) offsetFromSurface *= -1.0f;

            if(ObjectPlacement.Get().Settings.DecorPaintObjectPlacementSettings.SingleDecorPaintModeSettings.UseOriginalPivot)
            {
                return _strokeSurface.MouseCursorPickPoint + StrokeSurfacePlane.normal * offsetFromSurface;
            }
            else
            {
                Vector3 fromActivePivotPointToGuidePosition = ObjectPlacementGuide.Instance.WorldPosition - ObjectPlacement.Get().ActiveGuidePivotPoint;
                return _strokeSurface.MouseCursorPickPoint + fromActivePivotPointToGuidePosition + StrokeSurfacePlane.normal * offsetFromSurface;
            }
        }

        private void ApplyRotationAndScaleRandimizationsForPlacementGuideIfNecessary()
        {
            DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.PlacementGuideRotationRandomizationSettings.CustomAxisRandomizationSettings.Axis = StrokeSurfacePlane.normal;
            if (!MustAlignGuideToStroke()) ObjectRotationRandomization.Randomize(ObjectPlacementGuide.SceneObject, DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.PlacementGuideRotationRandomizationSettings);
            ObjectScaleRandomization.Randomize(ObjectPlacementGuide.SceneObject, DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.PlacementGuideScaleRandomizationSettings);
        }

        private void AdjustPlacementGuideRotation()
        {
            if (_isBeingPerformed && MustAlignGuideToStroke() && CanAlignGuideToStroke()) 
                ObjectPlacementGuide.SceneObject.SetHierarchyWorldRotationAndPreserveHierarchyCenter(RotationToApplyForStrokeAlignment * _orientedBoxOfLastPlacedHierarchy.Rotation);
            AlignPlacementGuideAxisWithStrokeSurfaceNormalIfNecessary();
        }

        private void AlignPlacementGuideAxisWithStrokeSurfaceNormalIfNecessary()
        {
            AxisAlignmentSettings surfaceAlignmentSettings = DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.PlacementGuideSurfaceAlignmentSettings;
            AxisAlignment.AlignObjectAxis(ObjectPlacementGuide.SceneObject, surfaceAlignmentSettings, StrokeSurfacePlane.normal);
        }
        #endregion
    }
}
#endif