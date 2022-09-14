#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectGrabSession
    {
        private enum State
        {
            Inactive = 0,
            Moving,
            Rotating,
            Scaling
        }

        private struct ObjectSurfaceInfo
        {
            public Plane SurfacePlane;
            public Vector3 SitPoint;
        }

        private State _state;
        private List<GameObject> _grabbedObjects;
        private Dictionary<GameObject, Vector3> _objectToPivotDir = new Dictionary<GameObject, Vector3>();
        private Dictionary<GameObject, ObjectSurfaceInfo> _objectToSurfaceInfo = new Dictionary<GameObject, ObjectSurfaceInfo>();
        private Dictionary<GameObject, Vector3> _objectToWorldScale = new Dictionary<GameObject, Vector3>();
        private Vector2 _cursorPosAtScaleBegin;
        private ObjectMask _rayHitMask = new ObjectMask();
        private MouseCursorRayHit _currentCursorRayHit;
        private Vector3 _surfaceHitPoint;
        private ObjectGrabSettings _grabSettings;

        public bool IsActive { get { return _state != State.Inactive; } }
        public ObjectGrabSettings Settings { set { if (value != null) _grabSettings = value; } }

        public void Begin(List<GameObject> grabbedObjects)
        {
            if (_grabSettings == null || grabbedObjects == null || grabbedObjects.Count == 0 || IsActive) return;

            _grabbedObjects = Octave3DWorldBuilder.ActiveInstance.GetRoots(grabbedObjects);
            foreach(var grabbedObj in _grabbedObjects) _rayHitMask.ObjectCollectionMask.Mask(grabbedObj.GetAllChildrenIncludingSelf());

            MouseCursorRayHit cursorRayHit = GetCursorRayHit();
            if (!cursorRayHit.WasAnythingHit) return;

            _surfaceHitPoint = cursorRayHit.WasAnObjectHit ? cursorRayHit.ClosestObjectRayHit.HitPoint : cursorRayHit.GridCellRayHit.HitPoint;
            _state = State.Moving;

            foreach(var grabbedObject in _grabbedObjects)
            {
                if(grabbedObject != null)
                {
                    _objectToPivotDir.Add(grabbedObject, grabbedObject.transform.position - _surfaceHitPoint);
                }
            }
        }

        public void End()
        {
            _state = State.Inactive;
            if (_grabbedObjects != null) _grabbedObjects.Clear();
            _rayHitMask.ObjectCollectionMask.UnmaskAll();
            _objectToPivotDir.Clear();
            _objectToSurfaceInfo.Clear();
            _objectToWorldScale.Clear();
        }

        public void RenderGizmos()
        {
            if(IsActive && _grabSettings.ShowGrabLines)
            {
                foreach(var grabbedObject in _grabbedObjects)
                {
                    GizmosEx.RenderLine(grabbedObject.GetHierarchyWorldOrientedBox().Center, _surfaceHitPoint, _grabSettings.GrabLineColor);
                }
            }
        }

        public void Update()
        {
            if(IsActive)
            {
                if (AllShortcutCombos.Instance.GrabRotateSelection.IsActive()) _state = State.Rotating;
                else if (AllShortcutCombos.Instance.GrabScaleSelection.IsActive())
                {
                    if(_state != State.Scaling)
                    {
                        _objectToWorldScale.Clear();
                        foreach (var grabbedObject in _grabbedObjects)
                        {
                            if (grabbedObject != null)
                            {
                                _objectToWorldScale.Add(grabbedObject, grabbedObject.transform.lossyScale);
                            }
                        }
                        _cursorPosAtScaleBegin = MouseCursor.Instance.Position;
                        _state = State.Scaling;
                    }
                }
                else
                {
                    // Need to reset the anchor relationships because the cursor was moved without
                    // the objects following it.
                    if (_state == State.Rotating || _state == State.Scaling)
                    {
                        _objectToPivotDir.Clear();
                        foreach (var grabbedObject in _grabbedObjects)
                        {
                            if (grabbedObject != null)
                            {
                                _objectToPivotDir.Add(grabbedObject, grabbedObject.transform.position - _surfaceHitPoint);
                            }
                        }
                    }
                    _state = State.Moving;
                }

                _currentCursorRayHit = GetCursorRayHit();
                if (!_currentCursorRayHit.WasAnythingHit) return;

                if(_currentCursorRayHit.WasAnythingHit)
                {
                    if (_currentCursorRayHit.WasAnObjectHit) _surfaceHitPoint = _currentCursorRayHit.ClosestObjectRayHit.HitPoint;
                    else _surfaceHitPoint = _currentCursorRayHit.GridCellRayHit.HitPoint;
                }

                if (_state == State.Moving || _objectToSurfaceInfo.Count == 0)
                {
                    if (_currentCursorRayHit.WasAnObjectHit)
                    {
                        GameObjectExtensions.RecordObjectTransformsForUndo(_grabbedObjects);
                        GameObjectRayHit objectRayHit = _currentCursorRayHit.ClosestObjectRayHit;
                        foreach (var grabbedObject in _grabbedObjects)
                        {
                            if (grabbedObject == null) continue;

                            Transform objectTransform = grabbedObject.transform;
                            objectTransform.position = objectRayHit.HitPoint + _objectToPivotDir[grabbedObject];
                            if (objectRayHit.WasTerrainHit)
                            {
                                Ray ray = new Ray(grabbedObject.GetHierarchyWorldOrientedBox().Center + Vector3.up, -Vector3.up);
                                GameObjectRayHit sitPointHit = null;
                                if (objectRayHit.HitObject.RaycastTerrainReverseIfFail(ray, out sitPointHit))
                                {
                                    Plane surfacePlane = new Plane(sitPointHit.HitNormal, sitPointHit.HitPoint);
                                    if (_grabSettings.AlignAxis) AxisAlignment.AlignObjectAxis(grabbedObject, _grabSettings.AlignmentAxis, sitPointHit.HitNormal);
                                    grabbedObject.PlaceHierarchyOnPlane(surfacePlane);
                                    if (!_grabSettings.EmbedInSurfaceWhenNoAlign || _grabSettings.AlignAxis) objectTransform.position += _grabSettings.OffsetFromSurface * sitPointHit.HitNormal;
                                    if (_grabSettings.EmbedInSurfaceWhenNoAlign && !_grabSettings.AlignAxis) grabbedObject.EmbedInSurfaceByVertex(-Vector3.up, objectRayHit.HitObject);

                                    ObjectSurfaceInfo surfaceInfo = new ObjectSurfaceInfo();
                                    surfaceInfo.SurfacePlane = surfacePlane;
                                    surfaceInfo.SitPoint = sitPointHit.HitPoint;
                                    SetObjectSurfaceInfo(grabbedObject, surfaceInfo);
                                }
                            }
                            else
                            if (objectRayHit.WasMeshHit)
                            {
                                Ray ray = new Ray(grabbedObject.GetHierarchyWorldOrientedBox().Center + objectRayHit.HitNormal * 2.0f, -objectRayHit.HitNormal);
                                GameObjectRayHit sitPointHit = null;
                                if (objectRayHit.HitObject.RaycastMeshReverseIfFail(ray, out sitPointHit))
                                {
                                    Plane surfacePlane = new Plane(sitPointHit.HitNormal, sitPointHit.HitPoint);
                                    if (_grabSettings.AlignAxis) AxisAlignment.AlignObjectAxis(grabbedObject, _grabSettings.AlignmentAxis, sitPointHit.HitNormal);
                                    grabbedObject.PlaceHierarchyOnPlane(surfacePlane);
                                    if (!_grabSettings.EmbedInSurfaceWhenNoAlign || _grabSettings.AlignAxis) objectTransform.position += _grabSettings.OffsetFromSurface * sitPointHit.HitNormal;
                                    if (_grabSettings.EmbedInSurfaceWhenNoAlign && !_grabSettings.AlignAxis) grabbedObject.EmbedInSurfaceByVertex(-sitPointHit.HitNormal, objectRayHit.HitObject);

                                    ObjectSurfaceInfo surfaceInfo = new ObjectSurfaceInfo();
                                    surfaceInfo.SurfacePlane = surfacePlane;
                                    surfaceInfo.SitPoint = sitPointHit.HitPoint;
                                    SetObjectSurfaceInfo(grabbedObject, surfaceInfo);
                                }
                            }
                        }
                    }
                    else
                    if (_currentCursorRayHit.WasACellHit)
                    {
                        GameObjectExtensions.RecordObjectTransformsForUndo(_grabbedObjects);
                        GridCellRayHit cellRayHit = _currentCursorRayHit.GridCellRayHit;
                        foreach (var grabbedObject in _grabbedObjects)
                        {
                            if (grabbedObject == null) continue;

                            Transform objectTransform = grabbedObject.transform;
                            objectTransform.position = cellRayHit.HitPoint + _objectToPivotDir[grabbedObject];

                            Plane surfacePlane = new Plane(cellRayHit.HitNormal, cellRayHit.HitPoint);
                            Vector3 sitPoint = surfacePlane.ProjectPoint(grabbedObject.GetWorldOrientedBox().Center);

                            if (_grabSettings.AlignAxis) AxisAlignment.AlignObjectAxis(grabbedObject, _grabSettings.AlignmentAxis, cellRayHit.HitNormal);
                            grabbedObject.PlaceHierarchyOnPlane(surfacePlane);
                            if (!_grabSettings.EmbedInSurfaceWhenNoAlign || _grabSettings.AlignAxis) objectTransform.position += _grabSettings.OffsetFromSurface * cellRayHit.HitNormal;

                            ObjectSurfaceInfo surfaceInfo = new ObjectSurfaceInfo();
                            surfaceInfo.SurfacePlane = surfacePlane;
                            surfaceInfo.SitPoint = sitPoint;
                            SetObjectSurfaceInfo(grabbedObject, surfaceInfo);
                        }
                    }
                }
                else
                if (_state == State.Rotating)
                {
                    GameObjectExtensions.RecordObjectTransformsForUndo(_grabbedObjects);
                    float rotationSensitivity = _grabSettings.RotationSensitivity;
                    foreach (var grabbedObject in _grabbedObjects)
                    {
                        if (!_objectToSurfaceInfo.ContainsKey(grabbedObject)) continue;

                        var surfaceInfo = _objectToSurfaceInfo[grabbedObject];
                        OrientedBox worldOOBB = grabbedObject.GetHierarchyWorldOrientedBox();
                        grabbedObject.RotateHierarchyBoxAroundPoint(MouseCursor.Instance.OffsetSinceLastMouseMove.x * rotationSensitivity, _grabSettings.AlignAxis ? surfaceInfo.SurfacePlane.normal : Vector3.up, worldOOBB.Center);
                    }
                }
                else
                if(_state == State.Scaling)
                {
                    GameObjectExtensions.RecordObjectTransformsForUndo(_grabbedObjects);
                    Vector2 currentCursorPos = MouseCursor.Instance.Position;
                    Vector2 cursorOffsetFromScaleBegin = currentCursorPos - _cursorPosAtScaleBegin;

                    float scaleFactor = 1.0f + _grabSettings.ScaleSensitivity * cursorOffsetFromScaleBegin.x;
                    foreach (var grabbedObject in _grabbedObjects)
                    {
                        if (!_objectToSurfaceInfo.ContainsKey(grabbedObject) ||
                            !_objectToWorldScale.ContainsKey(grabbedObject)) continue;

                        var surfaceInfo = _objectToSurfaceInfo[grabbedObject];
                        grabbedObject.SetHierarchyWorldScaleByPivotPoint(_objectToWorldScale[grabbedObject] * scaleFactor, surfaceInfo.SitPoint);
                    }
                }
            }
        }

        private void SetObjectSurfaceInfo(GameObject gameObject, ObjectSurfaceInfo surfaceInfo)
        {
            if (gameObject == null) return;
            if (_objectToSurfaceInfo.ContainsKey(gameObject)) _objectToSurfaceInfo[gameObject] = surfaceInfo;
            else _objectToSurfaceInfo.Add(gameObject, surfaceInfo);
        }

        private MouseCursorRayHit GetCursorRayHit()
        {
            MouseCursor.Instance.PushObjectPickMaskFlags(MouseCursorObjectPickFlags.ObjectBox | MouseCursorObjectPickFlags.ObjectSprite);
            MouseCursor.Instance.PushObjectMask(_rayHitMask);
            MouseCursorRayHit cursorRayHit = MouseCursor.Instance.GetRayHit();
            MouseCursor.Instance.PopObjectPickMaskFlags();
            MouseCursor.Instance.PopObjectMask();

            return cursorRayHit;
        }
    }
}
#endif