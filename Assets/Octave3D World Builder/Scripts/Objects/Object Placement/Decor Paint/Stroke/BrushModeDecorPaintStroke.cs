#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class BrushModeDecorPaintStroke : DecorPaintStroke
    {
        #region Private Variables
        [SerializeField]
        private DecorPaintObjectPlacementBrushCircle _brushCircle = new DecorPaintObjectPlacementBrushCircle();
        private List<GameObject> _objectsPlacedWhileDragging = new List<GameObject>();

        private BrushModeDecorPaintObjectPlacementDataCalculator _objectPlacementDataCalculator = new BrushModeDecorPaintObjectPlacementDataCalculator();
        #endregion

        #region Public Properties
        public XZOrientedEllipseShapeRenderSettings BrushCircleRenderSettings { get { return _brushCircle.CircleShapeRenderSettings; } }
        #endregion

        #region Public Methods
        public override void RenderGizmos()
        {
            _brushCircle.RenderGizmos();
        }
        #endregion

        #region Protected Methods
        protected override void OnMouseMoved(Event e)
        {
            _brushCircle.HandleMouseMoveEvent(e);
        }

        protected override void OnStrokeStarted(Event e)
        {
            _objectsPlacedWhileDragging.Clear();
            _objectPlacementDataCalculator.BeginSession(_brushCircle, DecorPaintObjectPlacementBrushDatabase.Get().ActiveBrush);
            PlaceObjects();
        }

        protected override void OnStrokeEnded(Event e)
        {
            DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectCollectionMask.Unmask(_objectsPlacedWhileDragging);
            _objectsPlacedWhileDragging.Clear();
            _objectPlacementDataCalculator.EndSession();
        }

        protected override void OnStrokeDragged(Event e)
        {
            _brushCircle.HandleMouseDragEvent(e);
            if (IsStrokeDistanceConditionSatisfied() && CanPlaceObjectsOnHoveredSurface()) PlaceObjects();

            if (MustCalculateInitialStrokeTravelDirectionOnDrag() &&
                CanCalculateInitialStrokeTravelDirecion()) CalculateInitialStrokeTravelDirection();
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
        private void PlaceObjects()
        {
            if(CanPlaceObjectsOnHoveredSurface())
            {
                var placementDataInstances = _objectPlacementDataCalculator.Calculate(RotationToApplyForStrokeAlignment);
                List<GameObject> placedHierarchyRoots = Octave3DScene.Get().InstantiateObjectHirarchiesFromPlacementDataCollection(placementDataInstances);

                List<GameObject> allPlacedObjects = GameObjectExtensions.GetAllObjectsInHierarchyCollection(placedHierarchyRoots);
                _objectsPlacedWhileDragging.AddRange(allPlacedObjects);
                DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectCollectionMask.Mask(allPlacedObjects);

                ObjectHierarchyRootsWerePlacedInSceneMessage.SendToInterestedListeners(placedHierarchyRoots, ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.ObjectPlacement);
            }

            RegisterPlacementPoint();
        }

        private bool MustCalculateInitialStrokeTravelDirectionOnDrag()
        {
            return NumberOfStrokePlacementPoints == 1 || _initialStrokeTravelDirection.magnitude == 0.0f;
        }

        private bool CanCalculateInitialStrokeTravelDirecion()
        {
            return _strokeSurface.IsValid;
        }

        private void CalculateInitialStrokeTravelDirection()
        {
            _initialStrokeTravelDirection = _strokeSurface.MouseCursorPickPoint - FirstStrokePlacementPoint;
            _initialStrokeTravelDirection.Normalize();
        }

        private bool CanPlaceObjectsOnHoveredSurface()
        {
            if (DecorPaintObjectPlacementSettings.Get().IgnoreGrid && HasCursorPickedGridCellButNoObject()) return false;
            return true;
        }
        #endregion
    }
}
#endif