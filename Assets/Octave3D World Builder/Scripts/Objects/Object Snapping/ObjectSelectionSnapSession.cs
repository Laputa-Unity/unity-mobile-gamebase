#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectSelectionSnapSession
    {
        private enum State
        {
            Inactive = 0,
            SelectPivot,
            Snap
        }

        private List<GameObject> _selectedParents = new List<GameObject>();
        private Vector3 _pivot;
        private bool _isPivotAvailable;
        private State _state;

        public bool IsActive { get { return _state != State.Inactive; } }

        public void Begin()
        {
            if (_state != State.Inactive) return;

            _state = State.SelectPivot;
            _selectedParents = GameObjectExtensions.GetParents(ObjectSelection.Get().GetAllSelectedGameObjects());
        }

        public void End()
        {
            _state = State.Inactive;
            _selectedParents.Clear();
        }

        public void RenderGizmos()
        {
            if(_state != State.Inactive && _isPivotAvailable)
            {
                Circle2D circle = new Circle2D(Vector3Extensions.WorldToScreenPoint(_pivot), 6.0f);
                GizmosEx.Render2DCircleBorderLines(circle, Color.black);
                GizmosEx.Render2DFilledCircle(circle, Color.green);

                foreach(var parent in _selectedParents)
                {
                    OrientedBox worldOOBB = parent.GetHierarchyWorldOrientedBox();
                    GizmosEx.RenderOrientedBoxEdges(worldOOBB, Color.yellow);
                }
            }
        }

        public void UpdateForMouseMovement()
        {
            if (_state == State.Inactive) return;

            if (MouseButtonStates.Instance.IsMouseButtonDown(MouseButton.Left)) _state = State.Snap;
            else _state = State.SelectPivot;

            if(_state == State.SelectPivot && _selectedParents.Count != 0)
            {             
                Camera camera = SceneViewCamera.Camera;
                Vector2 mousePos = Event.current.InvMousePos(camera);

                _isPivotAvailable = false;
                float minDistanceSq = float.MaxValue;
                foreach (var parent in _selectedParents)
                {
                    if (parent == null) continue;

                    OrientedBox worldOOBB = parent.GetHierarchyWorldOrientedBox();
                    if(worldOOBB.IsValid())
                    {
                        List<Vector3> centerAndCorners = worldOOBB.GetCenterAndCornerPoints();
                        List<Vector2> oobbScreenPts = Vector2Extensions.GetScreenPoints(centerAndCorners, camera);

                        for (int ptIndex = 0; ptIndex < centerAndCorners.Count; ++ptIndex)
                        {
                            Vector3 worldPt = centerAndCorners[ptIndex];
                            Vector2 screenPt = oobbScreenPts[ptIndex];
                            float distSq = (mousePos - screenPt).sqrMagnitude;
                            if (distSq < minDistanceSq)
                            {
                                minDistanceSq = distSq;
                                _pivot = worldPt;
                                _isPivotAvailable = true;
                            }
                        }
                    }
                }
            }
            else
            if(_state == State.Snap && _isPivotAvailable)
            {
                GameObjectExtensions.RecordObjectTransformsForUndo(_selectedParents);
                MouseCursorRayHit cursorHit = MouseCursor.Instance.GetCursorRayHitForGridCell();
                if (cursorHit.WasACellHit)
                {
                    Camera camera = SceneViewCamera.Camera;
                    GridCellRayHit cellRayHit = cursorHit.GridCellRayHit;
                    Vector3 snapDestination = Vector3Extensions.GetClosestPointToPoint(cellRayHit.HitCell.Quad.GetCenterAndCornerPoints(), cellRayHit.HitPoint);

                    Vector3 moveVector = snapDestination - _pivot;
                    foreach (var parent in _selectedParents)
                    {
                        if (parent != null) parent.transform.position += moveVector;
                    }

                    _pivot = snapDestination;

                    ObjectSelection.Get().ObjectSelectionGizmos.OnObjectSelectionUpdated();
                }
            }
        }
    }
}
#endif