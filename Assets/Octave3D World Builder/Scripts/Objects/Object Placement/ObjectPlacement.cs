#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacement : ScriptableObject, IMessageListener
    {
        #region Private Variables
        private CoordinateSystemAxis _currentGuideAlignmentAxis = CoordinateSystemAxis.PositiveRight;
        private bool _isPlacementLocked = false;

        [SerializeField]
        private InteractableMirror _mirror;

        [SerializeField]
        private ProjectedBoxFacePivotPoints _projectedGuidePivotPoints = new ProjectedBoxFacePivotPoints();
        [SerializeField]
        private ObjectPivotPointsRenderSettings _guidePivotPointsRenderSettings;
        private ProjectedBoxFacePivotPointsRenderer _guidePivotPointsRenderer = new ProjectedBoxFacePivotPointsRenderer();

        private ObjectVertexSnapSession _objectVertexSnapSession = new ObjectVertexSnapSession();
        private ObjectVertexSnapSessionRenderer _objectVertexSnapSessionRenderer = new ObjectVertexSnapSessionRenderer();
        [SerializeField]
        private ObjectVertexSnapSessionRenderSettings _objectVertexSnapSessionRenderSettings;

        [SerializeField]
        private ObjectPlacementSettings _settings;
        [SerializeField]
        private PersistentObjectPlacementGuideData _persistentObjectPlacementGuideData = new PersistentObjectPlacementGuideData();

        [SerializeField]
        private PointAndClickObjectPlacement _pointAndClickObjectPlacement = new PointAndClickObjectPlacement();
        [SerializeField]
        private PathObjectPlacement _pathObjectPlacement = new PathObjectPlacement();
        [SerializeField]
        private BlockObjectPlacement _blockObjectPlacement = new BlockObjectPlacement();
        [SerializeField]
        private DecorPaintObjectPlacement _decorPaintObjectPlacement;
        #endregion

        #region Private Properties
        private InteractableMirror Mirror
        {
            get
            {
                if (_mirror == null) _mirror = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<InteractableMirror>();
                return _mirror;
            }
        }
        #endregion

        #region Public Properties
        public ObjectPlacementSettings Settings
        {
            get
            {
                if (_settings == null) _settings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementSettings>();
                return _settings;
            }
        }
        public ObjectPivotPointsRenderSettings GuidePivotPointsRenderSettings
        {
            get
            {
                if (_guidePivotPointsRenderSettings == null) _guidePivotPointsRenderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPivotPointsRenderSettings>();
                return _guidePivotPointsRenderSettings;
            }
        }
        public ObjectVertexSnapSessionRenderSettings ObjectVertexSnapSessionRenderSettings
        {
            get
            {
                if (_objectVertexSnapSessionRenderSettings == null) _objectVertexSnapSessionRenderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectVertexSnapSessionRenderSettings>();
                return _objectVertexSnapSessionRenderSettings;
            }
        }
        public ProjectedBoxFacePivotPoints ProjectedGuidePivotPoints { get { return _projectedGuidePivotPoints; } }
        public Vector3 CenterProjectedGuidePivotPoint { get { return _projectedGuidePivotPoints.CenterPoint; } }
        public Vector3 ActiveGuidePivotPoint { get { return _projectedGuidePivotPoints.ActivePoint; } }
        public ObjectPlacementMode ObjectPlacementMode { get { return Settings.ObjectPlacementMode; } }
        public bool UsingBrushDecorPaintMode { get { return ObjectPlacementMode == ObjectPlacementMode.DecorPaint && Settings.DecorPaintObjectPlacementSettings.DecorPaintMode == DecorPaintMode.Brush; } }
        public PersistentObjectPlacementGuideData PersistentObjectPlacementGuideData { get { return _persistentObjectPlacementGuideData; } }
        public PointAndClickObjectPlacement PointAndClickObjectPlacement { get { return _pointAndClickObjectPlacement; } }
        public PathObjectPlacement PathObjectPlacement { get { return _pathObjectPlacement; } }
        public BlockObjectPlacement BlockObjectPlacement { get { return _blockObjectPlacement; } }
        public DecorPaintObjectPlacement DecorPaintObjectPlacement
        {
            get
            {
                if (_decorPaintObjectPlacement == null) _decorPaintObjectPlacement = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<DecorPaintObjectPlacement>();
                return _decorPaintObjectPlacement;
            }
        }
        public bool UserWantsToPlaceTileConnections
        {
            get
            {
                return Settings.ObjectPlacementMode == ObjectPlacementMode.Path &&
                       PathObjectPlacement.PathSettings.TileConnectionSettings.UseTileConnections;
            }
        }
        public bool IsObjectVertexSnapSessionActive { get { return _objectVertexSnapSession.IsActive; } }
        public bool IsPlacementLocked { get { return _isPlacementLocked; } }
        public InteractableMirrorSettings MirrorSettings { get { return Mirror.Settings; } }
        public InteractableMirrorRenderSettings MirrorRenderSettings { get { return Mirror.RenderSettings; } }
        public InteractableMirrorView MirrorView { get { return Mirror.View; } }
        #endregion

        #region Constructors
        public ObjectPlacement()
        {
            PrefabCategory.PrefabActivationValidationCallback = PrefabActivationValidationCallback;
        }
        #endregion

        #region Public Static Functions
        public static ObjectPlacement Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ObjectPlacement;
        }
        #endregion

        #region Public Methods
        public void DestroyPlacementGuide()
        {
            if(ObjectPlacementGuide.ExistsInScene)
            {
                PrepareForPlacementGuideDestruction();
                ObjectPlacementGuide.DestroyIfExists();
            }
        }

        public void RenderGizmos()
        {
            if (_objectVertexSnapSession.IsActive) _objectVertexSnapSessionRenderer.RenderGizmos(_objectVertexSnapSession, ObjectVertexSnapSessionRenderSettings);

            if (Settings.ObjectPlacementMode == ObjectPlacementMode.DecorPaint) RenderGizmosForDecorPaintMode();
            else RenderGizmosForNonDecorPaintMode();

            if(Mirror.IsActive)
            {
                Mirror.RenderGizmos();

                if (ObjectPlacementMode == ObjectPlacementMode.PointAndClick &&
                    ObjectPlacementGuide.ExistsInSceneAndIsActive) Mirror.RenderMirroredHierarchyOrientedBox(ObjectPlacementGuide.SceneObject);
                else
                if (ObjectPlacementMode == ObjectPlacementMode.DecorPaint &&
                    DecorPaintObjectPlacement.DecorPaintMode == DecorPaintMode.Single &&
                    ObjectPlacementGuide.ExistsInSceneAndIsActive) Mirror.RenderMirroredHierarchyOrientedBox(ObjectPlacementGuide.SceneObject);
                else
                if (ObjectPlacementMode == ObjectPlacementMode.Path &&
                    PathObjectPlacement.IsPathUnderManualConstruction) Mirror.RenderMirroredEntityOrientedBoxes(PathObjectPlacement.AllOrientedBoxesInPath);
                else
                if (ObjectPlacementMode == ObjectPlacementMode.Block &&
                    BlockObjectPlacement.IsBlockUnderManualConstruction) Mirror.RenderMirroredEntityOrientedBoxes(BlockObjectPlacement.AllOrientedBoxesInBlock);
            }
        }

        public void RenderHandles()
        {
            if (ObjectPlacementGuide.ExistsInScene)
            {
                Handles.BeginGUI();
                var labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.white;
                Rect labelRect = new Rect(2.0f, 0.0f, 1000, 15.0f);
                GUI.Label(labelRect, "Active prefab: " + ObjectPlacementGuide.Instance.SourcePrefab.Name, labelStyle);

                if(ObjectPlacementMode != ObjectPlacementMode.DecorPaint)
                {
                    labelRect.yMin += 15.0f;
                    GUI.Label(labelRect, "Enable object surface grid [W]: " + ObjectSnapping.Get().Settings.EnableObjectSurfaceGrid, labelStyle);

                    labelRect.yMin += 15.0f;
                    GUI.Label(labelRect, "Object to object snap [U]: " + ObjectSnapping.Get().Settings.EnableObjectToObjectSnap, labelStyle);

                    labelRect.yMin += 15.0f;
                    GUI.Label(labelRect, "Toggle original pivot [0]: " + ObjectSnapping.Get().Settings.UseOriginalPivot, labelStyle);
                }
                Handles.EndGUI();
            }

            if (ObjectPlacementMode == ObjectPlacementMode.Block) BlockObjectPlacement.RenderHandles();
        }

        public void HandleRepaintEvent(Event e)
        {
            if(Mirror.IsInteractionSessionActive)
            {
                Mirror.HandleRepaintEvent(e);
                return;
            }
            else
            {
                ObjectPlacementGuide.Active = CanGuideBeActive();
                CancelPlacementGuideSessionsIfNecessary();

                if (!AllShortcutCombos.Instance.ActivateObjectVertexSnapSession_Placement.IsActive()) _objectVertexSnapSession.End();
            }
        }

        public void HandleMouseMoveEvent(Event e)
        {
            if (_isPlacementLocked) return;
            if (Mirror.IsInteractionSessionActive)
            {
                e.DisableInSceneView();
                Mirror.HandleMouseMoveEvent(e);
                return;
            }
            
            ObjectPlacementGuidePrefabUpdate.EnsureGuideUsesCorrectPrefab();
            if(_objectVertexSnapSession.IsActive)
            {
                e.DisableInSceneView();
                _objectVertexSnapSession.UpdateForMouseMovement();
                return;
            }
      
            bool isAnyGuideSessionActive = false;
            if(ObjectPlacementGuide.ExistsInSceneAndIsActive)
            {
                e.DisableInSceneView();

                UpdatePlacementGuideSessions(e);
                isAnyGuideSessionActive = ObjectPlacementGuide.Instance.IsAnyMouseSessionActive;
            }

            if (!isAnyGuideSessionActive && !_objectVertexSnapSession.IsActive)
            {
                if (ObjectPlacementMode == ObjectPlacementMode.PointAndClick) PointAndClickObjectPlacement.HandleMouseMoveEvent(e);
                else if (ObjectPlacementMode == ObjectPlacementMode.Block) BlockObjectPlacement.HandleMouseMoveEvent(e);
                else if (ObjectPlacementMode == ObjectPlacementMode.Path) PathObjectPlacement.HandleMouseMoveEvent(e);
                else if (ObjectPlacementMode == ObjectPlacementMode.DecorPaint) DecorPaintObjectPlacement.HandleMouseMoveEvent(e);
            }

            SceneView.RepaintAll();
        }

        public void HandleMouseDragEvent(Event e)
        {
            if (_isPlacementLocked || Mirror.IsInteractionSessionActive) return;

            if (_objectVertexSnapSession.IsActive)
            {
                e.DisableInSceneView();
                _objectVertexSnapSession.UpdateForMouseMovement();
                return;
            }

            if (ObjectPlacementMode == ObjectPlacementMode.DecorPaint) DecorPaintObjectPlacement.HandleMouseDragEvent(e);
        }

        public void HandleMouseButtonDownEvent(Event e)
        {
            if (e.InvolvesLeftMouseButton()) e.DisableInSceneView();

            if (Mirror.IsInteractionSessionActive && e.InvolvesLeftMouseButton())
            {
                e.DisableInSceneView();
                Mirror.EndInteractionSession();
                return;
            }

            if(_objectVertexSnapSession.IsActive)
            {
                e.DisableInSceneView();
                return;
            }

            if (AllShortcutCombos.Instance.SnapXZGridToCursorPickPointOnLeftClick_Placement.IsActive() && e.InvolvesLeftMouseButton())
            {
                if(!IsConstructingBlock() && !IsConstructingPath())
                {
                    e.DisableInSceneView();

                    ObjectSnapping.Get().SnapXZGridToCursorPickPoint(e.clickCount == 2);
                    return;
                }
            }

            if (_isPlacementLocked)
            {
                // Note: If this is a left click event, we will eat the event so that the tool
                //       object doesn't get deselected in the scene view.
                if (e.InvolvesLeftMouseButton()) e.DisableInSceneView();
                return;
            }

            if (ObjectPlacementMode == ObjectPlacementMode.DecorPaint) DecorPaintObjectPlacement.HandleMouseButtonDownEvent(e);
            if (ObjectPlacementMode == ObjectPlacementMode.PointAndClick) PointAndClickObjectPlacement.HandleMouseButtonDownEvent(e);
            else if (ObjectPlacementMode == ObjectPlacementMode.Path) PathObjectPlacement.HandleMouseButtonDownEvent(e);
            else if (ObjectPlacementMode == ObjectPlacementMode.Block) BlockObjectPlacement.HandleMouseButtonDownEvent(e);
        }

        public void HandleMouseButtonUpEvent(Event e)
        {
            if (_isPlacementLocked) return;

            if (ObjectPlacementGuide.ExistsInSceneAndIsActive && 
                !ObjectPlacementGuide.Instance.IsAnyMouseSessionActive)
            {
                if (ObjectPlacementMode == ObjectPlacementMode.DecorPaint) DecorPaintObjectPlacement.HandleMouseButtonUpEvent(e);
            }
        }

        public void HandleKeyboardButtonDownEvent(Event e)
        {
            e.DisableInSceneView();

            if (AllShortcutCombos.Instance.LockObjectPlacement.IsActive())
            {
                _isPlacementLocked = !_isPlacementLocked;
                Inspector.Get().EditorWindow.Repaint();
                return;
            }

            if (_isPlacementLocked) return;

            if (Mirror.IsInteractionSessionActive)
            {
                Mirror.HandleKeyboardButtonDownEvent(e);
                return;
            }

            if (AllShortcutCombos.Instance.PickPrefabFromScene.IsActive() && CanPickPrefabFromScene())
            {
                e.DisableInSceneView();
                MouseCursor.Instance.PushObjectPickMaskFlags(MouseCursorObjectPickFlags.ObjectBox | MouseCursorObjectPickFlags.ObjectTerrain);
                MouseCursor.Instance.PushObjectMaskEnabledState(false);
                MouseCursorRayHit cursorRayHit = MouseCursor.Instance.GetRayHit();
                MouseCursor.Instance.PopObjectPickMaskFlags();
                MouseCursor.Instance.PopObjectMaskEnabledState();
                if (cursorRayHit.WasAnObjectHit)
                {
                    GameObject sourcePrefab = cursorRayHit.ClosestObjectRayHit.HitObject.GetSourcePrefabRoot();
                    if (sourcePrefab != null)
                    {
                        PrefabCategory categoryWhichContainsPrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(sourcePrefab);
                        if (categoryWhichContainsPrefab != null)
                        {
                            Prefab prefabToActivate = categoryWhichContainsPrefab.GetPrefabByUnityPrefab(sourcePrefab);
                            if (prefabToActivate != null)
                            {
                                UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                                PrefabCategoryDatabase.Get().SetActivePrefabCategory(categoryWhichContainsPrefab);

                                UndoEx.RecordForToolAction(categoryWhichContainsPrefab);
                                categoryWhichContainsPrefab.SetActivePrefab(prefabToActivate);

                                if (AllShortcutCombos.Instance.PickPrefabWithTransform.IsActive())
                                {
                                    if (ObjectPlacementGuide.ExistsInScene)
                                    {
                                        ObjectPlacementGuide.Instance.SetWorldScale(cursorRayHit.ClosestObjectRayHit.HitObject.transform.lossyScale);
                                    }
                                }
                            }
                        }
                        else
                        {
                            PrefabCategory activeCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                            if (activeCategory != null)
                            {
                                UndoEx.RecordForToolAction(activeCategory);
                                Prefab prefab = PrefabFactory.Create(sourcePrefab);
                                activeCategory.AddPrefab(prefab);
                                //activeCategory.SetActivePrefab(prefab);
                            }
                        }
                    }
                }

                return;
            }

            if(AllShortcutCombos.Instance.ToggleDecorSurfaceAlign.IsActive())
            {
                // ...
                Settings.DecorPaintObjectPlacementSettings.SingleDecorPaintModeSettings.PlacementGuideSurfaceAlignmentSettings.IsEnabled =
                    !Settings.DecorPaintObjectPlacementSettings.SingleDecorPaintModeSettings.PlacementGuideSurfaceAlignmentSettings.IsEnabled;
                Octave3DWorldBuilder.ActiveInstance.Inspector.Repaint();
            }
            else
            if(AllShortcutCombos.Instance.StepXZGridUp.IsActive())
            {
                XZGrid grid = ObjectSnapping.Get().XZSnapGrid;
                UndoEx.RecordForToolAction(grid);
                grid.Translate(new Vector3(0.0f, ObjectSnapping.Get().Settings.XZGridYOffsetStep, 0.0f));
                Octave3DWorldBuilder.ActiveInstance.Inspector.Repaint();
            }
            else
            if(AllShortcutCombos.Instance.StepXZGridDown.IsActive())
            {
                XZGrid grid = ObjectSnapping.Get().XZSnapGrid;
                UndoEx.RecordForToolAction(grid);
                grid.Translate(new Vector3(0.0f, -ObjectSnapping.Get().Settings.XZGridYOffsetStep, 0.0f));
                Octave3DWorldBuilder.ActiveInstance.Inspector.Repaint();
            }
            else
            if (AllShortcutCombos.Instance.ActivateObjectVertexSnapSession_Placement.IsActive() && CanActivateObjectVertexSnapSession())
            {
                e.DisableInSceneView();
                DestroyPlacementGuide();
                _objectVertexSnapSession.Begin();
                return;
            }
            else
            if(AllShortcutCombos.Instance.ActivateDecorPaintPlacement.IsActive())
            {
                if(Settings.ObjectPlacementMode != ObjectPlacementMode.DecorPaint)
                {
                    UndoEx.RecordForToolAction(Settings);
                    Settings.ObjectPlacementMode = ObjectPlacementMode.DecorPaint;
                    Inspector.Get().Repaint();
                    return;
                }
            }
            else
            if (AllShortcutCombos.Instance.ActivatePointAndClickPlacement.IsActive())
            {
                if (Settings.ObjectPlacementMode != ObjectPlacementMode.PointAndClick)
                {
                    UndoEx.RecordForToolAction(Settings);
                    Settings.ObjectPlacementMode = ObjectPlacementMode.PointAndClick;
                    Inspector.Get().Repaint();
                    return;
                }
            }
            else
            if (AllShortcutCombos.Instance.ActivatePathPlacement.IsActive())
            {
                if (Settings.ObjectPlacementMode != ObjectPlacementMode.Path)
                {
                    UndoEx.RecordForToolAction(Settings);
                    Settings.ObjectPlacementMode = ObjectPlacementMode.Path;
                    Inspector.Get().Repaint();
                    return;
                }
            }
            else
            if (AllShortcutCombos.Instance.ActivateBlockPlacement.IsActive())
            {
                if (Settings.ObjectPlacementMode != ObjectPlacementMode.Block)
                {
                    UndoEx.RecordForToolAction(Settings);
                    Settings.ObjectPlacementMode = ObjectPlacementMode.Block;
                    Inspector.Get().Repaint();
                    return;
                }
            }
            else
            if (AllShortcutCombos.Instance.ToggleUseOriginalPivotForSnapping.IsActive())
            {
                if (Settings.ObjectPlacementMode != ObjectPlacementMode.DecorPaint)
                {
                    ObjectSnapping.Get().Settings.UseOriginalPivot = !ObjectSnapping.Get().Settings.UseOriginalPivot;
                    Inspector.Get().Repaint();
                    return;
                }
            }

            if (ObjectPlacementGuide.ExistsInSceneAndIsActive)
            {
                if (RotatePlacementGuideWithKeyboardIfNecessary()) e.DisableInSceneView();
                else
                if (AllShortcutCombos.Instance.CycleThroughProjectedBoxFaceGuidePivotPoints.IsActive() && !ObjectSnapping.Get().Settings.UseOriginalPivot)
                {
                    e.DisableInSceneView();
                    _projectedGuidePivotPoints.ActivateNextPivotPoint();
                }
                else 
                if (AllShortcutCombos.Instance.SetPlacementGuideScaleToOriginal.IsActive())
                {
                    e.DisableInSceneView();
                    ObjectPlacementGuide.Instance.SetHierarchyWorldScaleByPivotPoint(ObjectPlacementGuide.Instance.SourcePrefab.InitialWorldScale, CalculateGuideScalePivotPoint());
                    UpdateGuideProjectedPivotPoints();
                }
                else
                if(AllShortcutCombos.Instance.ToggleEnableObjectSurfaceGrid.IsActive())
                {
                    e.DisableInSceneView();
                    UndoEx.RecordForToolAction(ObjectSnapSettings.Get());
                    ObjectSnapSettings.Get().EnableObjectSurfaceGrid = !ObjectSnapSettings.Get().EnableObjectSurfaceGrid;

                    Octave3DWorldBuilder.ActiveInstance.Inspector.EditorWindow.Repaint();
                }
                else
                if (AllShortcutCombos.Instance.SetPlacementGuideRotationToIdentity.IsActive())
                {
                    e.DisableInSceneView();
                    ObjectPlacementGuide.Instance.SetHierarchyWorldRotationAndPreserveHierarchyCenter(Quaternion.identity);
                    UpdateGuideProjectedPivotPoints();
                }
                else
                if (AllShortcutCombos.Instance.AlignPlacementGuideToNextAxis.IsActive())
                {
                    e.DisableInSceneView();

                    AxisAlignment.AlignObjectAxis(ObjectPlacementGuide.SceneObject, _currentGuideAlignmentAxis, ObjectPlacementSurfaceInfo.GetSurfaceNormal());
                    UpdateGuideProjectedPivotPoints();
                    AdjustPlacementGuidePositionOnCurrentPlacementSurface();

                    _currentGuideAlignmentAxis = CoordinateSystemAxes.GetNext(_currentGuideAlignmentAxis);
                }
                else 
                if(AllShortcutCombos.Instance.AdjustXZGridCellSizeToGuideSize.IsActive() && ObjectPlacementGuide.ExistsInSceneAndIsActive)
                {
                    e.DisableInSceneView();

                    Plane xzGridPlane = ObjectSnapping.Get().XZSnapGrid.Plane;
                    OrientedBox guideWorldOrientedBox = ObjectPlacementGuide.SceneObject.GetHierarchyWorldOrientedBox();
                    BoxFace faceWhichFacesGrid = guideWorldOrientedBox.GetBoxFaceWhichFacesNormal(xzGridPlane.normal);
                    List<Vector3> boxFacePoints = guideWorldOrientedBox.GetBoxFaceCornerPoints(faceWhichFacesGrid);
                    List<Vector3> projectedBoxFacePoints = xzGridPlane.ProjectAllPoints(boxFacePoints);
                    Box projectedPointsBox = Vector3Extensions.GetPointCloudBox(projectedBoxFacePoints);

                    UndoEx.RecordForToolAction(ObjectSnapping.Get().XZSnapGrid.CellSizeSettings);
                    TransformMatrix gridTransformMatrix = ObjectSnapping.Get().XZSnapGrid.TransformMatrix;
                    ObjectSnapping.Get().XZSnapGrid.CellSizeSettings.CellSizeX = projectedPointsBox.Size.GetAbsDot(gridTransformMatrix.GetNormalizedRightAxis());
                    ObjectSnapping.Get().XZSnapGrid.CellSizeSettings.CellSizeZ = projectedPointsBox.Size.GetAbsDot(gridTransformMatrix.GetNormalizedLookAxis());

                    SceneView.RepaintAll();
                }
                else
                if(AllShortcutCombos.Instance.TogglePlacementObject2ObjectSnap.IsActive())
                {
                    e.DisableInSceneView();
                    UndoEx.RecordForToolAction(ObjectSnapSettings.Get());
                    ObjectSnapping.Get().Settings.EnableObjectToObjectSnap = !ObjectSnapping.Get().Settings.EnableObjectToObjectSnap;

                    Octave3DWorldBuilder.ActiveInstance.Inspector.EditorWindow.Repaint();
                }
            }

            if (_settings.ObjectPlacementMode == ObjectPlacementMode.Path) PathObjectPlacement.HandleKeyboardButtonDownEvent(e);
            else if (_settings.ObjectPlacementMode == ObjectPlacementMode.Block) BlockObjectPlacement.HandleKeyboardButtonDownEvent(e);
        }

        public void HandleKeyboardButtonUpEvent(Event e)
        {
        }

        public void HandleMouseScrollWheelEvent(Event e)
        {
            if (_isPlacementLocked) return;

            if (AllShortcutCombos.Instance.CycleThroughPrefabsInActiveCategoryUsingMouseScrollWheel.IsActive() && ObjectPlacementGuide.ExistsInScene)
            {
                e.DisableInSceneView();
                if (Mathf.Abs(e.delta.y) <= 3.0f)
                {
                    OrientedBox guideWorldOrientedBox = ObjectPlacementGuide.SceneObject.GetHierarchyWorldOrientedBox();
                    Quaternion currentWorldRotation = ObjectPlacementGuide.Instance.WorldRotation;

                    UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get().ActivePrefabCategory);
                    if (e.delta.y > 0.0f) PrefabCategoryActions.ActivateNextPrefabInPrefabCategory(PrefabCategoryDatabase.Get().ActivePrefabCategory);
                    else if (e.delta.y < 0.0f) PrefabCategoryActions.ActivatePreviousPrefabInPrefabCategory(PrefabCategoryDatabase.Get().ActivePrefabCategory);

                    ObjectPlacementGuide.Instance.SetWorldPosition(ObjectPositionCalculator.CalculateObjectHierarchyPosition(ObjectPlacementGuide.SceneObject, guideWorldOrientedBox.Center, ObjectPlacementGuide.Instance.WorldScale, ObjectPlacementGuide.Instance.WorldRotation));
                    UpdateGuideProjectedPivotPoints();
                    AdjustPlacementGuidePositionOnCurrentPlacementSurface();

                    if (Settings.InheritRotationOnPrefabScroll) ObjectPlacementGuide.Instance.SetWorldRotation(currentWorldRotation);
                    Octave3DWorldBuilder.ActiveInstance.PrefabManagementWindow.Repaint();
                }
            }
            else
                if (AllShortcutCombos.Instance.CycleThroughPrefabCategoriesUsingMouseScrollWheel.IsActive())
                {
                    e.DisableInSceneView();
                    if (Mathf.Abs(e.delta.y) <= 3.0f && PrefabCategoryDatabase.Get().NumberOfCategories > 1)
                    {
                        if (ObjectPlacementGuide.ExistsInScene)
                        {
                            OrientedBox guideWorldOrientedBox = ObjectPlacementGuide.SceneObject.GetHierarchyWorldOrientedBox();

                            UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                            if (e.delta.y > 0.0f) PrefabCategoryDatabase.Get().ActivateNextPrefabCategory();
                            else if (e.delta.y < 0.0f) PrefabCategoryDatabase.Get().ActivatePreviousPrefabCategory();

                            if (PrefabCategoryDatabase.Get().ActivePrefabCategory.ActivePrefab != null)
                            {
                                DestroyPlacementGuide();
                                ObjectPlacementGuide.CreateFromActivePrefabIfNotExists();

                                ObjectPlacementGuide.Instance.SetWorldPosition(ObjectPositionCalculator.CalculateObjectHierarchyPosition(ObjectPlacementGuide.SceneObject, guideWorldOrientedBox.Center, ObjectPlacementGuide.Instance.WorldScale, ObjectPlacementGuide.Instance.WorldRotation));
                                UpdateGuideProjectedPivotPoints();
                                AdjustPlacementGuidePositionOnCurrentPlacementSurface();
                            }

                            Octave3DWorldBuilder.ActiveInstance.PrefabManagementWindow.Repaint();
                        }
                        else
                        {
                            UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                            if (e.delta.y > 0.0f) PrefabCategoryDatabase.Get().ActivateNextPrefabCategory();
                            else if (e.delta.y < 0.0f) PrefabCategoryDatabase.Get().ActivatePreviousPrefabCategory();

                            Octave3DWorldBuilder.ActiveInstance.PrefabManagementWindow.Repaint();
                        }
                    }
                }
                else
                if (DoesCurrentPlacementModeRequireSnapping() &&
                    AllShortcutCombos.Instance.EnableScrollWheelDesiredCellSizeAdjustmentForObjectColliderSnapSurfaceGrid.IsActive())
                {
                    e.DisableInSceneView();
                    if (Mathf.Abs(e.delta.y) <= 3.0f)
                    {
                        float adjustmentSpeed = ObjectSnapSettings.Get().ObjectColliderSnapSurfaceGridSettings.DesiredCellSize * 0.1f;
                        float sizeAdjustAmount = -e.delta.y * adjustmentSpeed;

                        UndoEx.RecordForToolAction(ObjectSnapSettings.Get().ObjectColliderSnapSurfaceGridSettings);
                        ObjectSnapSettings.Get().ObjectColliderSnapSurfaceGridSettings.DesiredCellSize += sizeAdjustAmount;
                        ObjectSnapping.Get().RefreshSnapSurface();
                    }
                }
                else
                if (Settings.ObjectPlacementMode == ObjectPlacementMode.DecorPaint) DecorPaintObjectPlacement.HandleMouseScrollWheelEvent(e);
        }
        #endregion

        #region Message Handlers
        public void RespondToMessage(Message message)
        {
            switch(message.Type)
            {
                case MessageType.PrefabWasRemovedFromCategory:

                    RespondToMessage(message as PrefabWasRemovedFromCategoryMessage);
                    break;

                case MessageType.PrefabCategoryWasRemovedFromDatabase:

                    RespondToMessage(message as PrefabCategoryWasRemovedFromDatabaseMessage);
                    break;

                case MessageType.ObjectPlacementModeWasChanged:

                    RespondToMessage(message as ObjectPlacementModeWasChangedMessage);
                    break;

                case MessageType.UndoRedoWasPerformed:

                    RespondToMessage(message as UndoRedoWasPerformedMessage);
                    break;

                case MessageType.InspectorGUIWasChanged:
                    
                    RespondToMessage(message as InspectorGUIWasChangedMessage);
                    break;

                case MessageType.ObjectHierarchyRootsWerePlacedInScene:

                    RespondToMessage(message as ObjectHierarchyRootsWerePlacedInSceneMessage);
                    break;

                case MessageType.ObjectPlacementGuideWasInstantiated:

                    RespondToMessage(message as ObjectPlacementGuideWasInstantiatedMessage);
                    break;

                case MessageType.ToolWasReset:

                    RespondToMessage(message as ToolWasResetMessage);
                    break;
            }
        }

        private void RespondToMessage(PrefabWasRemovedFromCategoryMessage message)
        {
            PathObjectPlacement.PathSettings.TileConnectionSettings.RecordAllTileConnectionTypeSettingsForUndo();
            PathObjectPlacement.PathSettings.TileConnectionSettings.RemovePrefabAssociation(message.PrefabWhichWasRemoved);

            ObjectPlacementPathTileConnectionConfigurationDatabase.Get().RecordAllConfigurationsForUndo();
            ObjectPlacementPathTileConnectionConfigurationDatabase.Get().RemovePrefabAssociationForAllConfigurations(message.PrefabWhichWasRemoved);

            // NOTE: This is now handled inside the brush class
            //DecorPaintObjectPlacementBrushDatabase.Get().RecordAllBrushesForUndo();
           // DecorPaintObjectPlacementBrushDatabase.Get().RemovePrefabAssociationForAllBrushElements(message.PrefabWhichWasRemoved);
        }

        private void RespondToMessage(PrefabCategoryWasRemovedFromDatabaseMessage message)
        {
            List<Prefab> allPrefabsInCategory = message.PrefabCategoryWhichWasRemoved.GetAllPrefabs();
            PathObjectPlacement.PathSettings.TileConnectionSettings.RecordAllTileConnectionTypeSettingsForUndo();
            PathObjectPlacement.PathSettings.TileConnectionSettings.RemovePrefabAssociations(allPrefabsInCategory);

            ObjectPlacementPathTileConnectionConfigurationDatabase.Get().RecordAllConfigurationsForUndo();
            ObjectPlacementPathTileConnectionConfigurationDatabase.Get().RemovePrefabAssociationForAllConfigurations(allPrefabsInCategory);

            // NOTE: This is now handled inside the brush class
            //DecorPaintObjectPlacementBrushDatabase.Get().RecordAllBrushesForUndo();
           // DecorPaintObjectPlacementBrushDatabase.Get().RemovePrefabAssociationForAllBrushElements(allPrefabsInCategory);
        }

        private void RespondToMessage(ObjectPlacementModeWasChangedMessage message)
        {
            BlockObjectPlacement.CancelManualBlockConstruction();
            PathObjectPlacement.CancelManualPathConstruction();
        }

        private void RespondToMessage(UndoRedoWasPerformedMessage message)
        {
            if (ShouldPlacementGuideBeDestroyed()) DestroyPlacementGuide();

            if (Settings.ObjectPlacementMode != ObjectPlacementMode.Block) BlockObjectPlacement.CancelManualBlockConstruction();
            if (Settings.ObjectPlacementMode != ObjectPlacementMode.Path) PathObjectPlacement.CancelManualPathConstruction();

            if(Settings.ObjectPlacementMode != ObjectPlacementMode.DecorPaint) ObjectSnapping.Get().RefreshSnapSurface();
            UpdateGuideProjectedPivotPoints();

            if (BlockObjectPlacement.IsBlockUnderManualConstruction || PathObjectPlacement.IsPathUnderManualConstruction) ObjectPlacementGuide.Active = false;
        }

        private void RespondToMessage(InspectorGUIWasChangedMessage message)
        {
            if (ShouldPlacementGuideBeDestroyed()) DestroyPlacementGuide();
        }

        private void RespondToMessage(ObjectHierarchyRootsWerePlacedInSceneMessage message)
        {
            List<GameObject> placedObjects = message.PlacedRoots;
            if(placedObjects != null && placedObjects.Count != 0)
            {
                if (message.ObjectPlacementType == ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.ObjectPlacement)
                {
                    // Assign objects to custom layer if necessary
                    if (!Settings.SpawnInPrefabLayer)
                    {
                        foreach(var parent in placedObjects)
                        {
                            parent.SetHierarchyLayer(Settings.CustomSpawnLayer, true);
                        }
                    }

                    // Mirror placed objects
                    List<GameObject> mirroredObjects = Mirror.MirrorGameObjectHierarchies(placedObjects);
                    placedObjects.AddRange(mirroredObjects);
                }

                // Check if the objects need to be attached to the active group. If not,
                // check if they must be attached as children of the currently hovered object.
                if (CanAttachPlacedRootsToObjectGroup(message.ObjectPlacementType))
                {
                    if(message.ObjectPlacementType == ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.Selection &&
                        ObjectSelection.Get().Settings.ObjectGroupSettings.DestinationGroup != null)
                    {
                        GameObject selectionObjectGroup = ObjectSelection.Get().Settings.ObjectGroupSettings.DestinationGroup.GroupObject;
                        if (selectionObjectGroup != null) selectionObjectGroup.AttachChildren(placedObjects, true);
                    }
                    else
                    {
                        if (Settings.UseActivePrefabCategoryGroup)
                        {
                            GameObject categoryGroupParent = PrefabCategoryDatabase.Get().ActivePrefabCategory.ObjectGroup.GroupObject;
                            categoryGroupParent.AttachChildren(placedObjects, true);
                        }
                        else
                        {
                            GameObject activeGroupParent = GetActiveObjectGroup().GroupObject;
                            activeGroupParent.AttachChildren(placedObjects, true);
                        }
                    }
                }
                else
                if (CanAttachPlacedRootsAsChildrenOfHoveredObject(message.ObjectPlacementType))
                {
                    GameObject surfaceObject = ObjectPlacementSurfaceInfo.GetSurfaceObject();
                    surfaceObject.AttachChildren(placedObjects, true);
                }
            }
        }

        private void RespondToMessage(ObjectPlacementGuideWasInstantiatedMessage message)
        {
            // Note: Only handle this in case the focus is on the Scene View window. On a couple of
            //       occasions exceptions were thrown regarding the 'GUIPointToWorldRay' function.
            if (SceneViewExtensions.IsSceneViewWindowFocused())
            {
                UpdateGuideProjectedPivotPoints();
                AdjustPlacementGuidePositionOnCurrentPlacementSurface();
            }
        }

        private void RespondToMessage(ToolWasResetMessage message)
        {
            DestroyPlacementGuide();
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            MessageListenerRegistration.PerformRegistrationForObjectPlacementModule(this);
        }

        private ObjectGroup GetActiveObjectGroup()
        {
            return Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase.ActiveGroup;
        }

        private bool CanPickPrefabFromScene()
        {
            return !PathObjectPlacement.IsPathUnderManualConstruction && !BlockObjectPlacement.IsBlockUnderManualConstruction;
        }

        private bool CanAttachPlacedRootsAsChildrenOfHoveredObject(ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType objectPlacementType)
        {
            if (objectPlacementType == ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.Selection) return false;

            GameObject surfaceObject = ObjectPlacementSurfaceInfo.GetSurfaceObject();
            return Settings.MakePlacedObjectsChildrenOfHoveredObject && surfaceObject != null && Octave3DWorldBuilder.ActiveInstance.IsWorkingObject(surfaceObject);
        }

        private bool CanAttachPlacedRootsToObjectGroup(ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType objectPlacementType)
        {
            if(objectPlacementType == ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.ObjectPlacement)
            {
                if(Settings.UseActivePrefabCategoryGroup)
                {
                    PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                    ObjectGroup categoryGroup = activePrefabCategory.ObjectGroup;
                    if (categoryGroup == null || categoryGroup.GroupObject == null) return false;
                }
                else
                {
                    ObjectGroup activeObjectGroup = GetActiveObjectGroup();
                    if (activeObjectGroup == null || activeObjectGroup.GroupObject == null) return false;
                }
            }
            else
            {
                ObjectGroup activeObjectGroup = GetActiveObjectGroup();
                if (activeObjectGroup == null || activeObjectGroup.GroupObject == null) return false;
            }

            if (objectPlacementType == ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.Selection)
                return ObjectSelection.Get().Settings.ObjectGroupSettings.AttachToObjectGroup;
            return Settings.AttachPlacedObjectsToObjectGroup;
        }

        private bool DoesCurrentPlacementModeRequireSnapping()
        {
            return ObjectPlacementMode != O3DWB.ObjectPlacementMode.DecorPaint;
        }

        private bool ShouldPlacementGuideBeDestroyed()
        {
            return Inspector.Get().ActiveInspectorGUIIdentifier != InspectorGUIIdentifier.ObjectPlacement &&
                   Inspector.Get().ActiveInspectorGUIIdentifier != InspectorGUIIdentifier.ObjectSnapping;
        }

        private bool PrefabActivationValidationCallback(Prefab prefabToActivate)
        {
            if (UserWantsToPlaceTileConnections)
            {
                ObjectPlacementPathTileConnectionSettings tileConnectionSettings = PathObjectPlacement.PathSettings.TileConnectionSettings;
                ObjectPlacementPathTileConnectionTypeSettings beginTileConnectionSettings = tileConnectionSettings.GetSettingsForTileConnectionType(ObjectPlacementPathTileConnectionType.Begin);
                if (beginTileConnectionSettings.Prefab != prefabToActivate) return false;
            }
         
            /*if (Octave3DWorldBuilder.Instance.Inspector.ActiveInspectorGUIIdentifier != InspectorGUIIdentifier.ObjectPlacement &&
                Octave3DWorldBuilder.Instance.Inspector.ActiveInspectorGUIIdentifier != InspectorGUIIdentifier.ObjectSnapping) return false;*/
            return true;
        }

        private void PrepareForPlacementGuideDestruction()
        {
            BlockObjectPlacement.CancelManualBlockConstruction();
            PathObjectPlacement.CancelManualPathConstruction();
        }

        private void UpdatePlacementGuideSessions(Event e)
        {
            if(ObjectPlacementGuide.ExistsInSceneAndIsActive)
            {
                ObjectPlacementGuide guide = ObjectPlacementGuide.Instance;

                if (CanBeginGuideMouseScaleSession()) guide.BeginMouseScaleSession(CalculateGuideScalePivotPoint());
                if (CanBeginGuideMouseRotationSession()) BeginGuideMouseRotationSession();
                if (CanBeginOffsetGuideFromPlacementSession()) guide.BeginMouseMoveAlongDirectionSession(ObjectPlacementSurfaceInfo.GetSurfaceNormal());

                CancelPlacementGuideSessionsIfNecessary();

                guide.UpdateActiveMouseSessionsForMouseMovement(e);
                UpdateGuideProjectedPivotPoints(true);
            }
        }

        private void CancelPlacementGuideSessionsIfNecessary()
        {
            if (ObjectPlacementGuide.ExistsInScene)
            {
                ObjectPlacementGuide guide = ObjectPlacementGuide.Instance;
                if (MustEndGuideMouseScaleSession()) guide.EndMouseScaleSession();
                if (MustEndGuideMouseRotationSession()) guide.EndMouseRotationSession();
                if (MustEndOffsetGuideFromPlacementSession()) guide.EndMouseMoveAlongDirectionSession();
            }
        }

        private bool CanBeginGuideMouseRotationSession()
        {
            if (!ObjectPlacementGuide.ExistsInSceneAndIsActive) return false;
            if (PathObjectPlacement.IsPathUnderManualConstruction) return false;
            if (BlockObjectPlacement.IsBlockUnderManualConstruction) return false;
            if (ObjectPlacementMode == ObjectPlacementMode.DecorPaint && DecorPaintObjectPlacement.DecorPaintMode == DecorPaintMode.Brush) return false;
            if (DecorPaintObjectPlacement.IsStroking) return false;
            if (ObjectPlacementGuide.Instance.IsAnyMouseSessionActive) return false;

            return true;
        }

        private void BeginGuideMouseRotationSession()
        {
            ObjectPlacementGuide guide = ObjectPlacementGuide.Instance;
            if (AllShortcutCombos.Instance.MouseRotatePlacementGuideAroundPlacementSurfaceNormal.IsActive())
                guide.BeginMouseRotationSession(ObjectPlacementSurfaceInfo.GetSurfaceNormal());
            else
            if (AllShortcutCombos.Instance.MouseRotatePlacementGuideAroundX.IsActive())
                guide.BeginMouseRotationSession(TransformAxis.X);
            else
            if (AllShortcutCombos.Instance.MouseRotatePlacementGuideAroundY.IsActive())
                guide.BeginMouseRotationSession(TransformAxis.Y);
            else
            if (AllShortcutCombos.Instance.MouseRotatePlacementGuideAroundZ.IsActive())
                guide.BeginMouseRotationSession(TransformAxis.Z);
        }

        private bool MustEndGuideMouseRotationSession()
        {
            if (_pathObjectPlacement.IsPathUnderManualConstruction || _blockObjectPlacement.IsBlockUnderManualConstruction) return true;

            ObjectPlacementGuide guide = ObjectPlacementGuide.Instance;
            if (guide.IsMouseRotationSessionForCustomAxis)
            {
                if(!AllShortcutCombos.Instance.MouseRotatePlacementGuideAroundPlacementSurfaceNormal.IsActive()) return true;
            }
            else
            {
                if (guide.MouseRotationSessionAxis == TransformAxis.X && !AllShortcutCombos.Instance.MouseRotatePlacementGuideAroundX.IsActive()) return true;
                if (guide.MouseRotationSessionAxis == TransformAxis.Y && !AllShortcutCombos.Instance.MouseRotatePlacementGuideAroundY.IsActive()) return true;
                if (guide.MouseRotationSessionAxis == TransformAxis.Z && !AllShortcutCombos.Instance.MouseRotatePlacementGuideAroundZ.IsActive()) return true;
            }

            return false;
        }

        private bool CanBeginGuideMouseScaleSession()
        {
            if (UserWantsToPlaceTileConnections) return false;
            if (!ObjectPlacementGuide.ExistsInSceneAndIsActive) return false;
            if (PathObjectPlacement.IsPathUnderManualConstruction) return false;
            if (BlockObjectPlacement.IsBlockUnderManualConstruction) return false;
            if (ObjectPlacementMode == ObjectPlacementMode.DecorPaint && DecorPaintObjectPlacement.DecorPaintMode == DecorPaintMode.Brush) return false;
            if (DecorPaintObjectPlacement.IsStroking) return false;
            if (ObjectPlacementGuide.Instance.IsAnyMouseSessionActive) return false;
         
            return AllShortcutCombos.Instance.MousePlacementGuideUniformScale.IsActive();
        }

        private bool MustEndGuideMouseScaleSession()
        {
            return _pathObjectPlacement.IsPathUnderManualConstruction || 
                   _blockObjectPlacement.IsBlockUnderManualConstruction || 
                   !AllShortcutCombos.Instance.MousePlacementGuideUniformScale.IsActive();
        }

        private bool RotatePlacementGuideWithKeyboardIfNecessary()
        {
            if (!CanPlacementGuideBeRotatedWithKeyboard()) return false;

            ObjectPlacementGuide objectPlacementGuide = ObjectPlacementGuide.Instance;
            AllShortcutCombos allShortcutCombos = AllShortcutCombos.Instance;
            bool wasRotated = false;

            if (allShortcutCombos.KeyboardRotatePlacementGuideAroundX.IsActive())
            {
                objectPlacementGuide.RotateUsingKeyboardSettings(TransformAxis.X);
                wasRotated = true;
            }
            if (allShortcutCombos.KeyboardRotatePlacementGuideAroundY.IsActive())
            {
                objectPlacementGuide.RotateUsingKeyboardSettings(TransformAxis.Y);
                wasRotated = true;
            }
            if (allShortcutCombos.KeyboardRotatePlacementGuideAroundZ.IsActive())
            {
                objectPlacementGuide.RotateUsingKeyboardSettings(TransformAxis.Z);
                wasRotated = true;
            }
            if (allShortcutCombos.KeyboardRotatePlacementGuideAroundPlacementSurfaceNormal.IsActive())
            {
                objectPlacementGuide.RotateUsingKeyboardSettings(ObjectPlacementSurfaceInfo.GetSurfaceNormal());
                wasRotated = true;
            }

            if (wasRotated) UpdateGuideProjectedPivotPoints();
            return wasRotated;
        }

        private bool CanActivateObjectVertexSnapSession()
        {
            return !PathObjectPlacement.IsPathUnderManualConstruction && !BlockObjectPlacement.IsBlockUnderManualConstruction &&
                   !Mirror.IsInteractionSessionActive && !(DecorPaintObjectPlacement.IsStroking);
        }

        private bool CanPlacementGuideBeRotatedWithKeyboard()
        {
            if (!ObjectPlacementGuide.ExistsInSceneAndIsActive) return false;
            if (PathObjectPlacement.IsPathUnderManualConstruction) return false;
            if (BlockObjectPlacement.IsBlockUnderManualConstruction) return false;
            if (ObjectPlacementMode == ObjectPlacementMode.DecorPaint && DecorPaintObjectPlacement.DecorPaintMode == DecorPaintMode.Brush) return false;
            if (DecorPaintObjectPlacement.IsStroking) return false;

            return !ObjectPlacementGuide.Instance.IsAnyMouseSessionActive;
        }

        private bool CanBeginOffsetGuideFromPlacementSession()
        {
            if (!ObjectPlacementGuide.ExistsInSceneAndIsActive) return false;
            if (PathObjectPlacement.IsPathUnderManualConstruction) return false;
            if (BlockObjectPlacement.IsBlockUnderManualConstruction) return false;
            if (ObjectPlacementMode == ObjectPlacementMode.DecorPaint && DecorPaintObjectPlacement.DecorPaintMode == DecorPaintMode.Brush) return false;
            if (DecorPaintObjectPlacement.IsStroking) return false;
            if (ObjectPlacementGuide.Instance.IsAnyMouseSessionActive) return false;
    
            return AllShortcutCombos.Instance.OffsetGuideFromPlacementSurface.IsActive();
        }

        private bool MustEndOffsetGuideFromPlacementSession()
        {
            return _pathObjectPlacement.IsPathUnderManualConstruction ||
                   _blockObjectPlacement.IsBlockUnderManualConstruction ||
                   !AllShortcutCombos.Instance.OffsetGuideFromPlacementSurface.IsActive();
        }

        private bool CanGuideBeActive()
        {
            if (AllShortcutCombos.Instance.SnapXZGridToCursorPickPointOnLeftClick_Placement.IsActive()) return false;
            if (IsConstructingPath()) return false;
            if (IsConstructingBlock()) return false;

            return true;
        }

        private bool IsConstructingPath()
        {
            return ObjectPlacementMode == ObjectPlacementMode.Path && PathObjectPlacement.IsPathUnderManualConstruction;
        }

        private bool IsConstructingBlock()
        {
            return ObjectPlacementMode == ObjectPlacementMode.Block && BlockObjectPlacement.IsBlockUnderManualConstruction;
        }

        private void UpdateGuideProjectedPivotPoints(bool keepPlacementSurface = false)
        {
            if(ObjectPlacementGuide.ExistsInScene)
            {
                if (ObjectPlacementMode != ObjectPlacementMode.DecorPaint) ObjectSnapping.Get().UpdateProjectedBoxFacePivotPoints(ObjectPlacementGuide.SceneObject, _projectedGuidePivotPoints, keepPlacementSurface);
                else DecorPaintObjectPlacement.UpdateGuidePivotPoints();
            }
        }

        private Vector3 CalculateGuideScalePivotPoint()
        {
            Plane pivotSurfacePlane = new Plane(ObjectPlacementSurfaceInfo.GetSurfaceNormal(), _projectedGuidePivotPoints.ActivePoint);
            return pivotSurfacePlane.ProjectPoint(ObjectPlacementGuide.SceneObject.GetHierarchyWorldOrientedBox().Center);
        }

        private void RenderGizmosForDecorPaintMode()
        {
            ObjectSnapping.Get().XZSnapGrid.RenderGizmos();

            if(Settings.DecorPaintObjectPlacementSettings.DecorPaintMode == DecorPaintMode.Single) _guidePivotPointsRenderer.RenderGizmos(_projectedGuidePivotPoints, GuidePivotPointsRenderSettings);
            DecorPaintObjectPlacement.RenderGizmos();
        }

        private void RenderGizmosForNonDecorPaintMode()
        {
            if(!_objectVertexSnapSession.IsActive && !AllShortcutCombos.Instance.SnapXZGridToCursorPickPointOnLeftClick_Placement.IsActive())
            {
                ObjectSnapping.Get().RenderGizmos();
                if (!ObjectSnapping.Get().Settings.UseOriginalPivot) _guidePivotPointsRenderer.RenderGizmos(_projectedGuidePivotPoints, GuidePivotPointsRenderSettings);
                else
                if(ObjectPlacementGuide.ExistsInSceneAndIsActive)
                {
                    // Note: Just us the projected pivot points settings for rendering the object pivot
                    Camera camera = SceneViewCamera.Camera;
                    Vector2 screenPoint = Vector3Extensions.WorldToScreenPoint(ObjectPlacementGuide.Instance.transform.position);

                    Circle2D circle = new Circle2D(screenPoint, GuidePivotPointsRenderSettings.PivotPointSizeInPixels * 0.5f);
                    GizmosEx.Render2DFilledCircle(circle, GuidePivotPointsRenderSettings.ProjectedBoxFacePivotPointsRenderSettings.ActivePivotPointRenderSettings.FillColor);
                    GizmosEx.Render2DCircleBorderLines(circle, GuidePivotPointsRenderSettings.ProjectedBoxFacePivotPointsRenderSettings.ActivePivotPointRenderSettings.BorderLineColor);
                }

                if (Settings.ObjectPlacementMode == ObjectPlacementMode.Path) _pathObjectPlacement.RenderGizmos();
                else if (Settings.ObjectPlacementMode == ObjectPlacementMode.Block) _blockObjectPlacement.RenderGizmos();
            }
            else
            {
                ObjectSnapping.Get().XZSnapGrid.RenderGizmos();
            }
        }

        private void AdjustPlacementGuidePositionOnCurrentPlacementSurface()
        {
            if (DoesCurrentPlacementModeRequireSnapping()) ObjectPlacementGuide.Instance.Snap();
            else ObjectPlacementGuide.Instance.SetWorldPosition(DecorPaintObjectPlacement.CalculatePlacementGuidePosition());
        
            UpdateGuideProjectedPivotPoints();
        }
        #endregion
    }
}
#endif