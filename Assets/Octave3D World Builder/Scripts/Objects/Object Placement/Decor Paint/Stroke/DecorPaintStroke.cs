#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public abstract class DecorPaintStroke
    {
        #region Protected Variables
        protected MouseCursorRayHit _cursorRayHit;
        protected GameObjectRayHit _strokeGameObjectRayHit;
        protected GridCellRayHit _strokeGridCellRayHit;

        protected List<Vector3> _strokePlacementPoints = new List<Vector3>();
        protected Vector3 _initialStrokeTravelDirection;
        protected bool _isBeingPerformed;

        [SerializeField]
        protected DecorPaintStrokeSurface _strokeSurface = new DecorPaintStrokeSurface();
        #endregion

        #region Protected Properties
        protected int NumberOfStrokePlacementPoints { get { return _strokePlacementPoints.Count; } }
        protected Vector3 PenultimateStrokePlacementPoint { get { return _strokePlacementPoints[NumberOfStrokePlacementPoints - 2]; } }
        protected Vector3 FirstStrokePlacementPoint { get { return _strokePlacementPoints[0]; } }
        protected Vector3 LastStrokePlacementPoint { get { return _strokePlacementPoints[NumberOfStrokePlacementPoints - 1]; } }
        protected Vector3 LastStrokeTravelDirection { get { return _strokeSurface.MouseCursorPickPoint - LastStrokePlacementPoint; } }
        protected Vector3 PenultimateStrokeTravelDirection { get { return IsOnlyOneStrokePlacementPointAvailable() ? _initialStrokeTravelDirection : LastStrokePlacementPoint - PenultimateStrokePlacementPoint; } }
        protected Vector3 FromFirstPlacementPointToCursorPickPoint { get { return _strokeSurface.MouseCursorPickPoint - FirstStrokePlacementPoint; } }
        #endregion

        #region Public Properties
        public Plane StrokeSurfacePlane { get { return _strokeSurface.Plane; } }
        public GameObject StrokeSurfaceObject { get { return _strokeSurface.SurfaceObject; } }
        public bool IsBeingPerformed { get { return _isBeingPerformed; } }
        public Quaternion RotationToApplyForStrokeAlignment { get { return NumberOfStrokePlacementPoints != 0 ? CalculateRotationWhichMustBeAppliedForStrokeAlignment() : Quaternion.identity; } }
        public DecorPaintStrokeSurfaceType SurfaceType { get { return _strokeSurface.Type; } }
        #endregion

        #region Public Methods
        public void HandleMouseMoveEvent(Event e)
        {
            // Note: Normally, mouse movements can not happen while a stroke is being performed.
            //       Only mouse drags can happen in that case. However, if the left mouse button
            //       is released outside the area of the scene view window, the left mouse button
            //       up event will not be fired and the '_isBeingPerformed' variable will remain
            //       set to true.
            if (_isBeingPerformed) EndStroke(e);

            AcquireCurorRayHit();
            UpdateStrokeSurfaceFromCursorPickInfo();
            OnMouseMoved(e);
        }

        public void HandleMousButtonDownEvent(Event e)
        {
            if(e.InvolvesLeftMouseButton())
            {
                e.DisableInSceneView();
                StartStroke(e);
            }
        }

        public void HandleMouseButtonUpEvent(Event e)
        {
            if(e.InvolvesLeftMouseButton())
            {
                //e.DisableInSceneView();
                EndStroke(e);
            }
        }

        public void HandleMouseDragEvent(Event e)
        {
            if(e.InvolvesLeftMouseButton())
            {
                e.DisableInSceneView();
                DragStroke(e);
            }
        }
        #endregion

        #region Public Abstract Methods
        public abstract void RenderGizmos();
        #endregion

        #region Protected Abstract Methods
        protected abstract void OnMouseMoved(Event e);
        protected abstract void OnStrokeStarted(Event e);
        protected abstract void OnStrokeEnded(Event e);
        protected abstract void OnStrokeDragged(Event e);
        protected abstract Vector3 CalculateRotationAxisForGuideStrokeAlignment();
        protected abstract float CalculateRotationDegreeAngleUsedForStrokeAlignment();
        #endregion

        #region Protected Methods
        protected bool IsOnlyOneStrokePlacementPointAvailable()
        {
            return NumberOfStrokePlacementPoints == 1;
        }

        protected bool HasCursorPickedAnything()
        {
            return IsCursorRayHitAvailable() && _cursorRayHit.WasAnythingHit;
        }

        protected bool HasCursorPickedGridCellButNoObject()
        {
            return IsCursorRayHitAvailable() && _cursorRayHit.WasACellHit && !_cursorRayHit.WasAnObjectHit;
        }

        protected bool HasCursorPickedGameObject()
        {
            return HasCursorPickedAnything() && _cursorRayHit.ClosestObjectRayHit != null;
        }

        protected bool HasCursorPickedTerrainObject()
        {
            return HasCursorPickedGameObject() && _cursorRayHit.ClosestObjectRayHit.WasTerrainHit;
        }

        protected bool IsCursorRayHitAvailable()
        {
            return _cursorRayHit != null;
        }

        protected void RegisterPlacementPoint()
        {
            _strokePlacementPoints.Add(_strokeSurface.MouseCursorPickPoint);
        }

        protected bool IsStrokeDistanceConditionSatisfied()
        {
            if (NumberOfStrokePlacementPoints == 0) return true;
            return (LastStrokePlacementPoint - _strokeSurface.MouseCursorPickPoint).magnitude >= DecorPaintObjectPlacementSettings.Get().StrokeDistance;
        }
        #endregion

        #region Private Methods
        private void AcquireCurorRayHit()
        {
            MouseCursor.Instance.PushObjectPickMaskFlags(GetCursorPickMaskFlags());
            MouseCursor.Instance.PushObjectMask(DecorPaintObjectPlacement.Get().DecorPaintMask);

            _cursorRayHit = MouseCursor.Instance.GetRayHit();

            MouseCursor.Instance.PopObjectMask();
            MouseCursor.Instance.PopObjectPickMaskFlags();
        }

        private void AcquireCursorRayHitForTerrainObject(GameObject terrainObject)
        {
            _cursorRayHit = MouseCursor.Instance.GetCursorRayHitForTerrainObject(terrainObject);
        }

        private void UpdateStrokeSurfaceFromCursorPickInfo()
        {
            if (HasCursorPickedAnything()) _strokeSurface.FromMouseCursorRayHit(_cursorRayHit);
        }

        private void DragStroke(Event e)
        {           
            if (HasCursorPickedTerrainObject()) AcquireCursorRayHitForTerrainObject(_cursorRayHit.ClosestObjectRayHit.HitObject);
            else AcquireCurorRayHit();

            UpdateStrokeSurfaceFromCursorPickInfo();
            OnStrokeDragged(e);
        }
        
        private MouseCursorObjectPickFlags GetCursorPickMaskFlags()
        {
            return MouseCursorObjectPickFlags.ObjectBox;
        }

        private void StartStroke(Event e)
        {
            _isBeingPerformed = true;
            AcquireCursorRayHitOnStrokeStart();
            OnStrokeStarted(e);
        }

        private void AcquireCursorRayHitOnStrokeStart()
        {
            if (HasCursorPickedAnything())
            {
                _strokeGameObjectRayHit = _cursorRayHit.ClosestObjectRayHit;
                _strokeGridCellRayHit = _cursorRayHit.GridCellRayHit;
            }
        }

        private void EndStroke(Event e)
        {
            _isBeingPerformed = false;
            ResetStrokePickInfo();
            OnStrokeEnded(e);

            _strokePlacementPoints.Clear();
            _initialStrokeTravelDirection = Vector3.zero;
        }

        private void ResetStrokePickInfo()
        {
            _strokeGameObjectRayHit = null;
            _strokeGridCellRayHit = null;
        }

        private bool IsStrokeObjectPickInfoAvailable()
        {
            return _strokeGameObjectRayHit != null && _strokeGameObjectRayHit.HitObject != null;
        }

        private bool IsStrokeGridCellPickInfoAvailable()
        {
            return _strokeGridCellRayHit != null;
        }

        private Quaternion CalculateRotationWhichMustBeAppliedForStrokeAlignment()
        {
            Vector3 rotationAxisUsedForAlignment = CalculateRotationAxisForGuideStrokeAlignment();
            if (rotationAxisUsedForAlignment.magnitude < 1e-5f) return Quaternion.identity;

            float angleInDegreesUsedForAlignment = CalculateRotationDegreeAngleUsedForStrokeAlignment();
            return Quaternion.AngleAxis(angleInDegreesUsedForAlignment, rotationAxisUsedForAlignment);
        }
        #endregion
    }
}
#endif