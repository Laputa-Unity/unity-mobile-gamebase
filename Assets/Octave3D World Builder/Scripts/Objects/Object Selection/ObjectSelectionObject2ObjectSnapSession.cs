#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectSelectionObject2ObjectSnapSession
    {
        private bool _isActive;
        private float _snapEpsilon = 0.5f;
        private List<GameObject> _selectedParents = new List<GameObject>();
        private List<GameObject> _allSelectedObjects = new List<GameObject>();
        private ObjectMask _rayCastObjectMask = new ObjectMask();

        public bool IsActive { get { return _isActive; } }
        public float SnapEpsilon { get { return _snapEpsilon; } set { _snapEpsilon = Mathf.Max(1e-5f, value); } }

        public void Begin()
        {
            if (_isActive) return;

            _allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
            _selectedParents = GameObjectExtensions.GetParents(_allSelectedObjects);
            _rayCastObjectMask.ObjectCollectionMask.Mask(_allSelectedObjects);
            _isActive = true;
        }

        public void End()
        {
            if(_isActive)
            {
                _isActive = false;
                _selectedParents.Clear();
                _allSelectedObjects.Clear();
                _rayCastObjectMask.ObjectCollectionMask.UnmaskAll();
            }
        }

        public void UpdateOnMouseMove()
        {
            if(_isActive)
            {
                MouseCursorRayHit cursorRayHit = GetCursorRayHit();
                if (cursorRayHit == null || !cursorRayHit.WasAnythingHit) return;

                Vector3 hitPoint = Vector3.zero;
                Vector3 hitNormal = Vector3.zero;

                if (!cursorRayHit.WasAnObjectHit && cursorRayHit.WasACellHit)
                {
                    hitPoint = cursorRayHit.GridCellRayHit.HitPoint;
                    hitNormal = cursorRayHit.GridCellRayHit.HitNormal;
                }
                else
                if (cursorRayHit.WasAnObjectHit && !cursorRayHit.WasACellHit)
                {
                    hitPoint = cursorRayHit.ClosestObjectRayHit.HitPoint;
                    hitNormal = cursorRayHit.ClosestObjectRayHit.HitNormal;
                }
                else
                if(cursorRayHit.WasAnObjectHit && cursorRayHit.WasACellHit)
                {
                    if(cursorRayHit.ClosestObjectRayHit.HitEnter < cursorRayHit.GridCellRayHit.HitEnter)
                    {
                        hitPoint = cursorRayHit.ClosestObjectRayHit.HitPoint;
                        hitNormal = cursorRayHit.ClosestObjectRayHit.HitNormal;
                    }
                    else
                    {
                        hitPoint = cursorRayHit.GridCellRayHit.HitPoint;
                        hitNormal = cursorRayHit.GridCellRayHit.HitNormal;
                    }
                }

                Plane hitPlane = new Plane(hitNormal, hitPoint);
                Box selectionWorldAABB = ObjectSelection.Get().GetWorldBox();
                if (selectionWorldAABB.IsInvalid()) return;

                Vector3 oldCenter = selectionWorldAABB.Center;
                selectionWorldAABB.Center = hitPoint;
                selectionWorldAABB.MoveInFrontOfPlane(hitPlane);
                Vector3 moveVector = selectionWorldAABB.Center - oldCenter;

                float snapEps = ObjectSelection.Get().Settings.Object2ObjectSnapSettings.SnapEps;

                GameObjectExtensions.RecordObjectTransformsForUndo(_selectedParents);
                foreach(var parent in _selectedParents)
                {
                    parent.transform.position += moveVector;
                }

                var ignoreObjects = new List<GameObject>(_allSelectedObjects);
                ignoreObjects.AddRange(ObjectSnapping.Get().ObjectSnapMask.ObjectCollectionMask.GetAllMaskedGameObjects());
                Object2ObjectSnap.Snap(_selectedParents, snapEps, ignoreObjects);
            }
        }

        private MouseCursorRayHit GetCursorRayHit()
        {
            if (!ObjectSelection.Get().Settings.Object2ObjectSnapSettings.CanHoverObjects)
                MouseCursor.Instance.PushObjectPickMaskFlags(MouseCursorObjectPickFlags.ObjectTerrain | MouseCursorObjectPickFlags.ObjectMesh | MouseCursorObjectPickFlags.ObjectBox | MouseCursorObjectPickFlags.ObjectSprite);
            else
                MouseCursor.Instance.PushObjectPickMaskFlags(MouseCursorObjectPickFlags.ObjectTerrain | MouseCursorObjectPickFlags.ObjectMesh);

            MouseCursor.Instance.PushObjectMask(_rayCastObjectMask);
            MouseCursorRayHit cursorRayHit = MouseCursor.Instance.GetRayHit();
            MouseCursor.Instance.PopObjectMask();
            MouseCursor.Instance.PopObjectPickMaskFlags();

            return cursorRayHit;
        }
    }
}
#endif