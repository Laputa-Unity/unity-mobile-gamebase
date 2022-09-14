#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class InteractableMirror : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private GameObject _sessionMirrorObject;
        [SerializeField]
        private Transform _sessionMirrorObjectTransform;

        [SerializeField]
        private Mirror _mirror = new Mirror();
        [SerializeField]
        private TransformMatrix _mirrorTransformMatrix = TransformMatrix.GetIdentity();
        private bool _isInteractionSessionActive = false;

        private ObjectMouseRotationSession _mouseRotationSession = new ObjectMouseRotationSession();
        private ObjectMouseMoveAlongDirectionSession _offsetFromHoverSurfaceSession = new ObjectMouseMoveAlongDirectionSession();
        private MouseCursorRayHit _cursorRayHit;
        private MirroredEntityRenderer _mirroredEntityRenderer = new MirroredEntityRenderer();

        [SerializeField]
        private bool _isActive = false;
        [SerializeField]
        private bool _alignToSurface = false;
        [SerializeField]
        private bool _mirrorRotation = true;
        [SerializeField]
        private bool _mirrorSpanningObjects = true;
        [SerializeField]
        private bool _allowIntersectionForMirroredObjects = true;
        [SerializeField]
        private InteractableMirrorSettings _settings;
        [SerializeField]
        private InteractableMirrorRenderSettings _renderSettings;

        [SerializeField]
        private InteractableMirrorView _view;
        #endregion

        #region Private Static Properties
        private static string MirrorObjectName { get { return "Octave3D Mirror Object"; } }
        #endregion

        #region Public Properties
        public Plane WorldPlane { get { return _mirror.WorldPlane; } }
        public Vector3 WorldCenter 
        {
            get { return _mirror.WorldCenter; } 
            set 
            {
                SetPosition(value, true);
                if (_sessionMirrorObjectTransform != null)
                {
                    UndoEx.RecordForToolAction(_sessionMirrorObjectTransform);
                    _sessionMirrorObjectTransform.position = value;
                }
            } 
        }
        public Vector3 WorldRotation
        {
            get { return _mirrorTransformMatrix.Rotation.eulerAngles; }
            set
            {
                Quaternion rotation = Quaternion.Euler(value);
                UndoEx.RecordForToolAction(this);
                SetRotation(rotation);

                if (_sessionMirrorObjectTransform != null)
                {
                    UndoEx.RecordForToolAction(_sessionMirrorObjectTransform);
                    _sessionMirrorObjectTransform.rotation = rotation;
                }
            }
        }
        public bool IsInteractionSessionActive { get { return _isInteractionSessionActive; } }
        public bool IsActive 
        { 
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (!_isActive) _isInteractionSessionActive = false;
            } 
        }
        public bool AlignToSurface { get { return _alignToSurface; } set { _alignToSurface = value; } }
        public bool MirrorRotation { get { return _mirrorRotation; } set { _mirrorRotation = value; } }
        public bool MirrorSpanningObjects { get { return _mirrorSpanningObjects; } set { _mirrorSpanningObjects = value; } }
        public bool AllowIntersectionForMirroredObjects { get { return _allowIntersectionForMirroredObjects; } set { _allowIntersectionForMirroredObjects = value; } }
        public bool IsAnyTransformSessionActive
        {
            get { return _mouseRotationSession.IsActive || _offsetFromHoverSurfaceSession.IsActive; }
        }
        public InteractableMirrorSettings Settings
        {
            get
            {
                if (_settings == null) _settings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<InteractableMirrorSettings>();
                return _settings;
            }
        }
        public InteractableMirrorRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<InteractableMirrorRenderSettings>();
                return _renderSettings;
            }
        }
        public InteractableMirrorView View { get { return _view; } }
        #endregion

        #region Constructors
        private InteractableMirror()
        {
            _view = new InteractableMirrorView(this);
        }
        #endregion

        #region Public Methods
        public void RenderMirroredHierarchyOrientedBox(GameObject hierarchyRoot)
        {
            _mirroredEntityRenderer.RenderMirroredHierarchyOrientedBox(_mirror.WorldPlane, hierarchyRoot, _mirrorRotation, RenderSettings.MirroredBoxColor, RenderSettings.MirroredBoxBorderLineColor);
        }

        public void RenderMirroredHierarchyOrientedBoxes(List<GameObject> hierarchyRoots)
        {
            _mirroredEntityRenderer.RenderMirroredHierarchyOrientedBoxes(_mirror.WorldPlane, hierarchyRoots, _mirrorRotation, RenderSettings.MirroredBoxColor, RenderSettings.MirroredBoxBorderLineColor);
        }

        public void RenderMirroredEntityOrientedBox(OrientedBox entityBox)
        {
            _mirroredEntityRenderer.RenderMirroredEntityOrientedBox(_mirror.WorldPlane, entityBox, _mirrorRotation, RenderSettings.MirroredBoxColor, RenderSettings.MirroredBoxBorderLineColor);
        }

        public void RenderMirroredEntityOrientedBoxes(List<OrientedBox> entityBoxes)
        {
            _mirroredEntityRenderer.RenderMirroredEntityOrientedBoxes(_mirror.WorldPlane, entityBoxes, _mirrorRotation, RenderSettings.MirroredBoxColor, RenderSettings.MirroredBoxBorderLineColor);
        }

        public GameObject MirrorGameObjectHierarchy(GameObject hierarchyRoot, Prefab sourcePrefab)
        {
            if (!IsActive) return null;

            GameObject mirroredHierarchyRoot = null;
            OrientedBox hierarchyWorldOrientedBox = hierarchyRoot.GetHierarchyWorldOrientedBox();
            if (!hierarchyWorldOrientedBox.IsValid()) return null;

            OrientedBox mirroredBox = Mirroring.MirrorOrientedBox(_mirror.WorldPlane, hierarchyWorldOrientedBox, _mirrorRotation);
            if (!_allowIntersectionForMirroredObjects && ObjectQueries.IntersectsAnyObjectsInScene(mirroredBox, true)) return null;
            if (!_mirrorSpanningObjects && _mirror.WorldPlane.ClassifyOrientedBox(hierarchyWorldOrientedBox) == BoxPlaneClassificationResult.Spanning) return null;

            Transform hierarchyRootTransform = hierarchyRoot.transform;
            if (sourcePrefab != null) mirroredHierarchyRoot = Octave3DScene.Get().InstantiateObjectHierarchyFromPrefab(sourcePrefab, hierarchyRootTransform.position, hierarchyRootTransform.rotation, hierarchyRootTransform.lossyScale);
            else mirroredHierarchyRoot = hierarchyRoot.CloneAsWorkingObject(hierarchyRoot.transform.parent);

            mirroredHierarchyRoot.ApplyTransformDataToRootChildren(hierarchyRoot);

            Mirroring.MirrorObjectHierarchyTransform(_mirror.WorldPlane, mirroredHierarchyRoot, _mirrorRotation);

            return mirroredHierarchyRoot;
        }

        public List<GameObject> MirrorGameObjectHierarchies(List<GameObject> sourceHierarchyRoots)
        {
            if(sourceHierarchyRoots.Count == 0) return new List<GameObject>();

            var mirroredHierarchies = new List<GameObject>(sourceHierarchyRoots.Count);
            if(IsActive)
            {
                Dictionary<GameObject, Prefab> unityPrefabsToOctavePrefabs = new Dictionary<GameObject, Prefab>();
                foreach(var root in sourceHierarchyRoots)
                {
                    GameObject unityPrefab = root.GetSourcePrefab();
                    Prefab octavePrefab = null;

                    if(unityPrefab != null)
                    {
                        if (!unityPrefabsToOctavePrefabs.ContainsKey(unityPrefab))
                        {
                            PrefabCategory prefabCategory = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(unityPrefab);
                            if (prefabCategory != null)
                            {
                                octavePrefab = prefabCategory.GetPrefabByUnityPrefab(unityPrefab);
                                unityPrefabsToOctavePrefabs.Add(unityPrefab, octavePrefab);
                            }
                        }
                        else octavePrefab = unityPrefabsToOctavePrefabs[unityPrefab];
                    }

                    GameObject mirroredGameObjectHierarchy = MirrorGameObjectHierarchy(root, octavePrefab);
                    if (mirroredGameObjectHierarchy != null) mirroredHierarchies.Add(mirroredGameObjectHierarchy);
                }
            }

            return mirroredHierarchies;
        }

        public void RenderGizmos()
        {
            if(_isActive)
            {
                XZOrientedQuad3D mirrorQuad = GetMirrorQuad();
                GizmosEx.RenderXZOrientedQuad(mirrorQuad, RenderSettings.MirrorPlaneColor);
                GizmosEx.RenderXZOrientedQuadBorderLines(mirrorQuad, RenderSettings.MirrorPlaneBorderLineColor);
            }
        }

        public OrientedBox MirrorOrientedBox(OrientedBox orientedBox)
        {
            return Mirroring.MirrorOrientedBox(_mirror.WorldPlane, orientedBox, _mirrorRotation);
        }

        public void ToggleInteractionSession()
        {
            if (_isInteractionSessionActive) EndInteractionSession();
            else BeginInteractionSession();
        }

        public void BeginInteractionSession()
        {
            if (_isInteractionSessionActive) return;

            IsActive = true;
            _isInteractionSessionActive = true;

            CreateSessionMirrorObject();
        }

        public void HandleRepaintEvent(Event e)
        {
            if (_isInteractionSessionActive) Octave3DWorldBuilder.ActiveInstance.Inspector.EditorWindow.Repaint();

            if(_mouseRotationSession.IsActive)
            {
                if (_mouseRotationSession.RotatingAroundCustomAxis)
                {
                    if (!AllShortcutCombos.Instance.MouseRotateMirrorAroundHoverSurfaceNormal.IsActive()) _mouseRotationSession.End();
                }
                else if (_mouseRotationSession.RotationAxis == TransformAxis.X && !AllShortcutCombos.Instance.MouseRotateMirrorAroundX.IsActive()) _mouseRotationSession.End();
                else if (_mouseRotationSession.RotationAxis == TransformAxis.Y && !AllShortcutCombos.Instance.MouseRotateMirrorAroundY.IsActive()) _mouseRotationSession.End();
                else if (_mouseRotationSession.RotationAxis == TransformAxis.Z && !AllShortcutCombos.Instance.MouseRotateMirrorAroundZ.IsActive()) _mouseRotationSession.End();
            }

            if(_offsetFromHoverSurfaceSession.IsActive)
            {
                if (!AllShortcutCombos.Instance.OffsetMirrorFromHoverSurface.IsActive()) _offsetFromHoverSurfaceSession.End();
            }
        }

        public void HandleMouseMoveEvent(Event e)
        {
            if(_isInteractionSessionActive)
            {
                if(IsAnyTransformSessionActive)
                {
                    _mouseRotationSession.UpdateForMouseMovement(e);
                    if (_mirrorTransformMatrix.Rotation.eulerAngles != _sessionMirrorObjectTransform.rotation.eulerAngles)
                    {
                        UndoEx.RecordForToolAction(this);
                        SetRotation(_sessionMirrorObjectTransform.rotation);
                    }

                    _offsetFromHoverSurfaceSession.UpdateForMouseMovement(e);
                    if (_mirrorTransformMatrix.Translation != _sessionMirrorObjectTransform.position)
                    {
                        UndoEx.RecordForToolAction(this);
                        SetPosition(_sessionMirrorObjectTransform.position);
                    }
                }
                else
                {
                    AcquireMouseCursorRayHit();
                    AdjustPositionBasedOnCursorHit();
                    if (_alignToSurface) AlignToHoverSurface();
                }
            }
        }

        public void HandleKeyboardButtonDownEvent(Event e)
        {
            if (!_isInteractionSessionActive) return;

            if(AllShortcutCombos.Instance.ResetMirrorRotationToIdentity.IsActive())
            {
                e.DisableInSceneView();
                UndoEx.RecordForToolAction(this);
                SetRotation(Quaternion.identity);
            }
            if(AllShortcutCombos.Instance.AlignMirrorWithHoverSurface.IsActive())
            {
                e.DisableInSceneView();
                UndoEx.RecordForToolAction(this);
                AlignToHoverSurface();
                return;
            }
            else
            if(AllShortcutCombos.Instance.MouseRotateMirrorAroundX.IsActive())
            {
                e.DisableInSceneView();
                if(!IsAnyTransformSessionActive)
                {
                    _mouseRotationSession.BeginRotationAroundAxis(_sessionMirrorObject, Settings.MouseRotationSettings, TransformAxis.X);
                    return;
                }
            }
            else
            if (AllShortcutCombos.Instance.MouseRotateMirrorAroundY.IsActive())
            {
                e.DisableInSceneView();
                if (!IsAnyTransformSessionActive)
                {
                    _mouseRotationSession.BeginRotationAroundAxis(_sessionMirrorObject, Settings.MouseRotationSettings, TransformAxis.Y);
                    return;
                }
            }
            else
            if (AllShortcutCombos.Instance.MouseRotateMirrorAroundZ.IsActive())
            {
                e.DisableInSceneView();
                if (!IsAnyTransformSessionActive)
                {
                    _mouseRotationSession.BeginRotationAroundAxis(_sessionMirrorObject, Settings.MouseRotationSettings, TransformAxis.Z);
                    return;
                }
            }
            else
            if (AllShortcutCombos.Instance.MouseRotateMirrorAroundHoverSurfaceNormal.IsActive())
            {
                e.DisableInSceneView();
                if (!IsAnyTransformSessionActive)
                {
                    _mouseRotationSession.BeginRotationAroundCustomAxis(_sessionMirrorObject, Settings.MouseRotationSettings, GetHoverSurfaceNormal());
                    return;
                }
            }
            else 
            if(AllShortcutCombos.Instance.OffsetMirrorFromHoverSurface.IsActive())
            {
                e.DisableInSceneView();
                if (!IsAnyTransformSessionActive)
                {
                    _offsetFromHoverSurfaceSession.Begin(_sessionMirrorObject, GetHoverSurfaceNormal(), Settings.MouseOffsetFromHoverSurfaceSettings);
                    return;
                }
            }
            else
            if(!IsAnyTransformSessionActive)
            {
                if(AllShortcutCombos.Instance.KeyboardRotateMirrorAroundX.IsActive())
                {
                    e.DisableInSceneView();
                    UndoEx.RecordForToolAction(_sessionMirrorObjectTransform);
                    UndoEx.RecordForToolAction(this);
                    _sessionMirrorObjectTransform.Rotate(Vector3.right, Settings.KeyboardRotationSettings.XAxisRotationSettings.RotationAmountInDegrees, Space.World);
                    SetRotation(_sessionMirrorObjectTransform.rotation);
                    return;
                }
                else
                if(AllShortcutCombos.Instance.KeyboardRotateMirrorAroundY.IsActive())
                {
                    e.DisableInSceneView();
                    UndoEx.RecordForToolAction(_sessionMirrorObjectTransform);
                    UndoEx.RecordForToolAction(this);
                    _sessionMirrorObjectTransform.Rotate(Vector3.up, Settings.KeyboardRotationSettings.YAxisRotationSettings.RotationAmountInDegrees, Space.World);
                    SetRotation(_sessionMirrorObjectTransform.rotation);
                    return;
                }
                else
                if(AllShortcutCombos.Instance.KeyboardRotateMirrorAroundZ.IsActive())
                {
                    e.DisableInSceneView();
                    UndoEx.RecordForToolAction(_sessionMirrorObjectTransform);
                    UndoEx.RecordForToolAction(this);
                    _sessionMirrorObjectTransform.Rotate(Vector3.forward, Settings.KeyboardRotationSettings.ZAxisRotationSettings.RotationAmountInDegrees, Space.World);
                    SetRotation(_sessionMirrorObjectTransform.rotation);
                    return;
                }
                else
                if(AllShortcutCombos.Instance.KeyboardRotateMirrorAroundHoverSurfaceNormal.IsActive())
                {
                    e.DisableInSceneView();
                    UndoEx.RecordForToolAction(_sessionMirrorObjectTransform);
                    UndoEx.RecordForToolAction(this);
                    _sessionMirrorObjectTransform.Rotate(GetHoverSurfaceNormal(), Settings.KeyboardRotationSettings.CustomAxisRotationSettings.RotationAmountInDegrees, Space.World);
                    SetRotation(_sessionMirrorObjectTransform.rotation);
                    return;
                }
            }
        }

        public void EndInteractionSession()
        {
            DestroySessionMirrorObject();
            _isInteractionSessionActive = false;
            EndAllTransformSessions();

            Octave3DWorldBuilder.ActiveInstance.Inspector.EditorWindow.Repaint();
        }
        #endregion

        #region Private Methods
        private void AdjustPositionBasedOnCursorHit()
        {
            UndoEx.RecordForToolAction(this);
            if (AllShortcutCombos.Instance.DisableMirrorSnapping.IsActive() && _cursorRayHit.WasAnythingHit) SetPosition(GetHoverSurfaceHitPoint());
            else
            {
                if (AllShortcutCombos.Instance.SnapMirrorToCenterOfSnapSurface.IsActive()) ObjectSnapping.Get().SnapObjectPositionToSnapSurfaceCenter(_sessionMirrorObject);
                else ObjectSnapping.Get().SnapObjectPosition(_sessionMirrorObject);
                SetPosition(_sessionMirrorObjectTransform.position);
            }
        }

        private Vector3 GetHoverSurfaceHitPoint()
        {
            if (_cursorRayHit.WasAnObjectHit) return _cursorRayHit.ClosestObjectRayHit.HitPoint;
            else return _cursorRayHit.GridCellRayHit.HitPoint;
        }

        private Vector3 GetHoverSurfaceNormal()
        {
            if (_cursorRayHit.WasAnObjectHit) return _cursorRayHit.ClosestObjectRayHit.HitNormal;
            else return _cursorRayHit.GridCellRayHit.HitNormal;
        }

        private void EndAllTransformSessions()
        {
            _mouseRotationSession.End();
            _offsetFromHoverSurfaceSession.End();
        }

        private void SetPosition(Vector3 position, bool allowUndoRedo = false)
        {
            if (allowUndoRedo) UndoEx.RecordForToolAction(this);
            _mirror.SetWorldCenter(position);
            _mirrorTransformMatrix.Translation = position;
        }

        private void SetNormal(Vector3 normal)
        {
            normal.Normalize();
            _mirror.SetWorldPlaneNormal(normal);
        }

        private void SetRotation(Quaternion rotation)
        {
            _mirrorTransformMatrix.Rotation = rotation;
            SetNormal(_mirrorTransformMatrix.GetNormalizedRightAxis());
        }

        private void AlignToHoverSurface()
        {
            if (_cursorRayHit.WasAnythingHit)
            {
                _sessionMirrorObjectTransform.right = GetHoverSurfaceNormal();
                SetRotation(_sessionMirrorObjectTransform.rotation);
            }
        }

        private void AcquireMouseCursorRayHit()
        {
            MouseCursor.Instance.PushObjectPickMaskFlags(MouseCursorObjectPickFlags.ObjectMesh | MouseCursorObjectPickFlags.ObjectTerrain);
            _cursorRayHit = MouseCursor.Instance.GetRayHit();
            MouseCursor.Instance.PopObjectPickMaskFlags();
        }

        private XZOrientedQuad3D GetMirrorQuad()
        {
            const float infiniteSize = 9999999.9999999f;
            XZOrientedQuad3D mirrorQuad = new XZOrientedQuad3D(_mirrorTransformMatrix.Translation, Vector3.one);
            mirrorQuad.Rotation = Quaternion.AngleAxis(-90.0f, Vector3.forward);
            mirrorQuad.Rotation = _mirrorTransformMatrix.Rotation * mirrorQuad.Rotation;
            mirrorQuad.SetScale(new Vector3(RenderSettings.UseInfiniteHeight ? infiniteSize : RenderSettings.MirrorHeight, 1.0f, RenderSettings.UseInfiniteWidth ? infiniteSize : RenderSettings.MirrorWidth));
            return mirrorQuad;
        }

        private void OnDestroy()
        {
            DestroySessionMirrorObject();
        }

        private void CreateSessionMirrorObject()
        {
            DestroySessionMirrorObject();
            _sessionMirrorObject = new GameObject(MirrorObjectName);
            //_sessionMirrorObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            _sessionMirrorObjectTransform = _sessionMirrorObject.transform;

            // Note: It is important to not attach as a child of the Octave3D tool object. Otherwise
            //       serialization hell happens when: 'Remove Component' for Octave3D and then Undo.
            _sessionMirrorObjectTransform.position = _mirrorTransformMatrix.Translation;
            _sessionMirrorObjectTransform.rotation = _mirrorTransformMatrix.Rotation;
            _sessionMirrorObjectTransform.localScale = _mirrorTransformMatrix.Scale;
        }

        private void DestroySessionMirrorObject()
        {
            if (_sessionMirrorObject != null) DestroyImmediate(_sessionMirrorObject);
            _sessionMirrorObject = null;
            _sessionMirrorObjectTransform = null;
        }
        #endregion
    }
}
#endif