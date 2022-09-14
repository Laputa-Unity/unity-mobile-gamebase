#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class AllShortcutCombos : Singleton<AllShortcutCombos>
    {
        #region Private Variables
        private List<ShortcutCombo> _generalShortcutCombos = new List<ShortcutCombo>();
        private List<ShortcutCombo> _shortcutCombosForMirroring = new List<ShortcutCombo>();
        private List<ShortcutCombo> _shortcutCombosForObjectPlacement = new List<ShortcutCombo>();
        private List<ShortcutCombo> _shortcutCombosForObjectSnapping = new List<ShortcutCombo>();
        private List<ShortcutCombo> _shortcutCombosForObjectErasing = new List<ShortcutCombo>();
        private List<ShortcutCombo> _shortcutCombosForObjectSelection = new List<ShortcutCombo>();

        /// <summary>
        /// General shortcuts.
        /// </summary>
        private ShortcutCombo _activateObjectPlacementGUI;
        private ShortcutCombo _activateObjectSelectionGUI;
        private ShortcutCombo _activateObjectErasingGUI;

        /// <summary>
        /// Shortcuts related to mirroring.
        /// </summary>
        private ShortcutCombo _mirrorSelectedObjects;
        private ShortcutCombo _snapMirrorToCenterOfSnapSurface;
        private ShortcutCombo _disableMirrorSnapping;
        private ShortcutCombo _resetMirrorRotationToIdentity;
        private ShortcutCombo _mouseRotateMirrorAroundX;
        private ShortcutCombo _mouseRotateMirrorAroundY;
        private ShortcutCombo _mouseRotateMirrorAroundZ;
        private ShortcutCombo _mouseRotateMirrorAroundHoverSurfaceNormal;
        private ShortcutCombo _keyboardRotateMirrorAroundX;
        private ShortcutCombo _keyboardRotateMirrorAroundY;
        private ShortcutCombo _keyboardRotateMirrorAroundZ;
        private ShortcutCombo _keyboardRotateMirrorAroundHoverSurfaceNormal;
        private ShortcutCombo _offsetMirrorFromHoverSurface;
        private ShortcutCombo _alignMirrorWithHoverSurface;

        /// <summary>
        /// Shortcuts related to object placement.
        /// </summary>
        private ShortcutCombo _toggleDecorSurfaceAlign;
        private ShortcutCombo _pickPrefabFromScene;
        private ShortcutCombo _pickPrefabWithTransform;
        private ShortcutCombo _activateObjectVertexSnapSession_Placement;
        private ShortcutCombo _placeGuideBehindSurfacePlane;
        private ShortcutCombo _activateDecorPaintPlacement;
        private ShortcutCombo _activatePointAndClickPlacement;
        private ShortcutCombo _activatePathPlacement;
        private ShortcutCombo _activateBlockPlacement;
        private ShortcutCombo _offsetGuideFromPlacementSurface;
        private ShortcutCombo _lockObjectPlacement;
        private ShortcutCombo _alignPlacementGuideToNextAxis;
        private ShortcutCombo _setPlacementGuideRotationToIdentity;
        private ShortcutCombo _setPlacementGuideScaleToOriginal;
        private ShortcutCombo _cycleThroughPrefabsInActiveCategoryUsingMouseScrollWheel;
        private ShortcutCombo _cycleThroughPrefabCategoriesUsingMouseScrollWheel;
        private ShortcutCombo _cancelManualPathConstruction;
        private ShortcutCombo _placePathOnClick;
        private ShortcutCombo _manualRaisePath;
        private ShortcutCombo _manualLowerPath;
        private ShortcutCombo _manualRemoveLast2SegmentsInPath;
        private ShortcutCombo _nextPathExtensionPlane;
        private ShortcutCombo _beginManualPathConstruction;
        private ShortcutCombo _manualAttach2NewSegmentsToPath;
        private ShortcutCombo _beginManualBlockConstruction;
        private ShortcutCombo _cancelManualBlockConstruction;
        private ShortcutCombo _enable1To1RatioBlockAdjustment;
        private ShortcutCombo _placeBlockOnClick;
        private ShortcutCombo _manualRaiseBlock;
        private ShortcutCombo _manualLowerBlock;
        private ShortcutCombo _nextBlockExtensionPlane;
        private ShortcutCombo _cycleThroughProjectedBoxFaceGuidePivotPoints;
        private ShortcutCombo _toggleUseOriginalPivotForSnapping;

        private ShortcutCombo _keyboardRotatePlacementGuideAroundX;
        private ShortcutCombo _keyboardRotatePlacementGuideAroundY;
        private ShortcutCombo _keyboardRotatePlacementGuideAroundZ;
        private ShortcutCombo _keyboardRotatePlacementGuideAroundPlacementSurfaceNormal;
        private ShortcutCombo _mouseRotatePlacementGuideAroundX;
        private ShortcutCombo _mouseRotatePlacementGuideAroundY;
        private ShortcutCombo _mouseRotatePlacementGuideAroundZ;
        private ShortcutCombo _mouseRotatePlacementGuideAroundPlacementSurfaceNormal;
        private ShortcutCombo _mousePlacementGuideUniformScale;

        private ShortcutCombo _enableScrollWheelSizeAdjustmentForDecorPaintBrush;
        private ShortcutCombo _scrollWheelTilePaintBrushSize;

        /// <summary>
        /// Shortcuts related to object snapping.
        /// </summary>
        private ShortcutCombo _toggleEnableObjectSurfaceGrid;
        private ShortcutCombo _enableScrollWheelDesiredCellSizeAdjustmentForObjectColliderSnapSurfaceGrid;
        private ShortcutCombo _snapCenterToCenter;
        private ShortcutCombo _keepSnappedHierarchyInSnapSurfaceArea;
        private ShortcutCombo _snapXZGridToCursorPickPointOnLeftClick_Placement;
        private ShortcutCombo _adjustXZGridCellSizeToGuideSize;
        private ShortcutCombo _stepXZGridUp;
        private ShortcutCombo _stepXZGridDown;
        private ShortcutCombo _togglePlacementObject2ObjectSnap;

        /// <summary>
        /// Shortcuts related to object erasing.
        /// </summary>
        private ShortcutCombo _enableScrollWheelSizeAdjustmentForMassEraseShape;

        /// <summary>
        /// Shortcuts related to object selection.
        /// </summary>
        private ShortcutCombo _activateObjectVertexSnapSession_Selection;
        private ShortcutCombo _replacePrefabsForSelectedObjects_Preview;
        private ShortcutCombo _replacePrefabsForSelectedObjects_Scene;
        private ShortcutCombo _activateMoveGizmo;
        private ShortcutCombo _activateRotationGizmo;
        private ShortcutCombo _activateScaleGizmo;
        private ShortcutCombo _activateObjectSelectionExtrudeGizmo;
        private ShortcutCombo _enableScrollWheelSizeAdjustmentForSelectionShape;
        private ShortcutCombo _enableAppendObjectsToSelection;
        private ShortcutCombo _enableDeselectObjectWithSelectionShape;
        private ShortcutCombo _deleteSelectedObjects;
        private ShortcutCombo _selectAllObjectsWithSamePrefabAsCurrentSelection;
        private ShortcutCombo _projectSelectedObjects;
        private ShortcutCombo _selectionGridSnap;
        private ShortcutCombo _grabSelection;
        private ShortcutCombo _grabRotateSelection;
        private ShortcutCombo _grabScaleSelection;
        private ShortcutCombo _setRotationToIdentity;
        private ShortcutCombo _selectionRotateWorldX;
        private ShortcutCombo _selectionRotateWorldY;
        private ShortcutCombo _selectionRotateWorldZ;
        private ShortcutCombo _toggleSelectionObject2ObjectSnap;
        private ShortcutCombo _endSelectionObject2ObjectSnap;
        private ShortcutCombo _snapXZGridToCursorPickPointOnLeftClick_Selection;
        #endregion

        #region Public Properties
        /// <summary>
        /// General.
        /// </summary>
        public ShortcutCombo ActivateObjectPlacementGUI { get { return _activateObjectPlacementGUI; } }
        public ShortcutCombo ActivateObjectSelectionGUI { get { return _activateObjectSelectionGUI; } }
        public ShortcutCombo ActivateObjectErasingGUI { get { return _activateObjectErasingGUI; } }

        /// <summary>
        /// Mirroring.
        /// </summary>
        public ShortcutCombo MirrorSelectedObjects { get { return _mirrorSelectedObjects; } }
        public ShortcutCombo SnapMirrorToCenterOfSnapSurface { get { return _snapMirrorToCenterOfSnapSurface; } }
        public ShortcutCombo DisableMirrorSnapping { get { return _disableMirrorSnapping; } }
        public ShortcutCombo ResetMirrorRotationToIdentity { get { return _resetMirrorRotationToIdentity; } }
        public ShortcutCombo MouseRotateMirrorAroundX { get { return _mouseRotateMirrorAroundX; } }
        public ShortcutCombo MouseRotateMirrorAroundY { get { return _mouseRotateMirrorAroundY; } }
        public ShortcutCombo MouseRotateMirrorAroundZ { get { return _mouseRotateMirrorAroundZ; } }
        public ShortcutCombo MouseRotateMirrorAroundHoverSurfaceNormal { get { return _mouseRotateMirrorAroundHoverSurfaceNormal; } }
        public ShortcutCombo KeyboardRotateMirrorAroundX { get { return _keyboardRotateMirrorAroundX; } }
        public ShortcutCombo KeyboardRotateMirrorAroundY { get { return _keyboardRotateMirrorAroundY; } }
        public ShortcutCombo KeyboardRotateMirrorAroundZ { get { return _keyboardRotateMirrorAroundZ; } }
        public ShortcutCombo KeyboardRotateMirrorAroundHoverSurfaceNormal { get { return _keyboardRotateMirrorAroundHoverSurfaceNormal; } }
        public ShortcutCombo OffsetMirrorFromHoverSurface { get { return _offsetMirrorFromHoverSurface; } }
        public ShortcutCombo AlignMirrorWithHoverSurface { get { return _alignMirrorWithHoverSurface; } }

        /// <summary>
        /// Object placement.
        /// </summary>
        public ShortcutCombo ToggleDecorSurfaceAlign { get { return _toggleDecorSurfaceAlign; } }
        public ShortcutCombo PickPrefabFromScene { get { return _pickPrefabFromScene; } }
        public ShortcutCombo PickPrefabWithTransform { get { return _pickPrefabWithTransform; } }
        public ShortcutCombo ActivateObjectVertexSnapSession_Placement { get { return _activateObjectVertexSnapSession_Placement; } }
        public ShortcutCombo PlaceGuideBehindSurfacePlane { get { return _placeGuideBehindSurfacePlane; } }
        public ShortcutCombo ActivateDecorPaintPlacement { get { return _activateDecorPaintPlacement; } }
        public ShortcutCombo ActivatePointAndClickPlacement { get { return _activatePointAndClickPlacement; } }
        public ShortcutCombo ActivatePathPlacement { get { return _activatePathPlacement; } }
        public ShortcutCombo ActivateBlockPlacement { get { return _activateBlockPlacement; } }
        public ShortcutCombo OffsetGuideFromPlacementSurface { get { return _offsetGuideFromPlacementSurface; } }
        public ShortcutCombo LockObjectPlacement { get { return _lockObjectPlacement; } }
        public ShortcutCombo AlignPlacementGuideToNextAxis { get { return _alignPlacementGuideToNextAxis; } }
        public ShortcutCombo SetPlacementGuideRotationToIdentity { get { return _setPlacementGuideRotationToIdentity; } }
        public ShortcutCombo SetPlacementGuideScaleToOriginal { get { return _setPlacementGuideScaleToOriginal; } }
        public ShortcutCombo CycleThroughPrefabsInActiveCategoryUsingMouseScrollWheel { get { return _cycleThroughPrefabsInActiveCategoryUsingMouseScrollWheel; } }
        public ShortcutCombo CycleThroughPrefabCategoriesUsingMouseScrollWheel { get { return _cycleThroughPrefabCategoriesUsingMouseScrollWheel; } }
        public ShortcutCombo CancelManualPathConstruction { get { return _cancelManualPathConstruction; } }
        public ShortcutCombo PlacePathOnClick { get { return _placePathOnClick; } }
        public ShortcutCombo ManualRaisePath { get { return _manualRaisePath; } }
        public ShortcutCombo ManualLowerPath { get { return _manualLowerPath; } }
        public ShortcutCombo ManualRemoveLast2SegmentsInPath { get { return _manualRemoveLast2SegmentsInPath; } }
        public ShortcutCombo NextPathExtensionPlane { get { return _nextPathExtensionPlane; } }
        public ShortcutCombo BeginManualPathConstruction { get { return _beginManualPathConstruction; } }
        public ShortcutCombo ManualAttach2NewSegmentsToPath { get { return _manualAttach2NewSegmentsToPath; } }
        public ShortcutCombo BeginManualBlockConstruction { get { return _beginManualBlockConstruction; } }
        public ShortcutCombo CancelManualBlockConstruction { get { return _cancelManualBlockConstruction; } }
        public ShortcutCombo Enable1To1RatioBlockAdjustment { get { return _enable1To1RatioBlockAdjustment; } }
        public ShortcutCombo PlaceBlockOnClick { get { return _placeBlockOnClick; } }
        public ShortcutCombo ManualRaiseBlock { get { return _manualRaiseBlock; } }
        public ShortcutCombo ManualLowerBlock { get { return _manualLowerBlock; } }
        public ShortcutCombo NextBlockExtensionPlane { get { return _nextBlockExtensionPlane; } }
        public ShortcutCombo CycleThroughProjectedBoxFaceGuidePivotPoints { get { return _cycleThroughProjectedBoxFaceGuidePivotPoints; } }
        public ShortcutCombo ToggleUseOriginalPivotForSnapping { get { return _toggleUseOriginalPivotForSnapping; } }

        public ShortcutCombo KeyboardRotatePlacementGuideAroundX { get { return _keyboardRotatePlacementGuideAroundX; } }
        public ShortcutCombo KeyboardRotatePlacementGuideAroundY { get { return _keyboardRotatePlacementGuideAroundY; } }
        public ShortcutCombo KeyboardRotatePlacementGuideAroundZ { get { return _keyboardRotatePlacementGuideAroundZ; } }
        public ShortcutCombo KeyboardRotatePlacementGuideAroundPlacementSurfaceNormal { get { return _keyboardRotatePlacementGuideAroundPlacementSurfaceNormal; } }
        public ShortcutCombo MouseRotatePlacementGuideAroundX { get { return _mouseRotatePlacementGuideAroundX; } }
        public ShortcutCombo MouseRotatePlacementGuideAroundY { get { return _mouseRotatePlacementGuideAroundY; } }
        public ShortcutCombo MouseRotatePlacementGuideAroundZ { get { return _mouseRotatePlacementGuideAroundZ; } }
        public ShortcutCombo MouseRotatePlacementGuideAroundPlacementSurfaceNormal { get { return _mouseRotatePlacementGuideAroundPlacementSurfaceNormal; } }
        public ShortcutCombo MousePlacementGuideUniformScale { get { return _mousePlacementGuideUniformScale; } }

        public ShortcutCombo EnableScrollWheelSizeAdjustmentForDecorPaintBrush { get { return _enableScrollWheelSizeAdjustmentForDecorPaintBrush; } }
        public ShortcutCombo ScrollWheelTilePaintBrushSize { get { return _scrollWheelTilePaintBrushSize; } }

        /// <summary>
        /// Object snapping.
        /// </summary>
        public ShortcutCombo ToggleEnableObjectSurfaceGrid { get { return _toggleEnableObjectSurfaceGrid; } }
        public ShortcutCombo EnableScrollWheelDesiredCellSizeAdjustmentForObjectColliderSnapSurfaceGrid { get { return _enableScrollWheelDesiredCellSizeAdjustmentForObjectColliderSnapSurfaceGrid; } }
        public ShortcutCombo SnapCenterToCenter { get { return _snapCenterToCenter; } }
        public ShortcutCombo KeepSnappedHierarchyInSnapSurfaceArea { get { return _keepSnappedHierarchyInSnapSurfaceArea; } }
        public ShortcutCombo SnapXZGridToCursorPickPointOnLeftClick_Placement { get { return _snapXZGridToCursorPickPointOnLeftClick_Placement; } }
        public ShortcutCombo AdjustXZGridCellSizeToGuideSize { get { return _adjustXZGridCellSizeToGuideSize; } }
        public ShortcutCombo StepXZGridUp { get { return _stepXZGridUp; } }
        public ShortcutCombo StepXZGridDown { get { return _stepXZGridDown; } }
        public ShortcutCombo TogglePlacementObject2ObjectSnap { get { return _togglePlacementObject2ObjectSnap; } }

        /// <summary>
        /// Object erasing.
        /// </summary>
        public ShortcutCombo EnableScrollWheelSizeAdjustmentForMassEraseShape { get { return _enableScrollWheelSizeAdjustmentForMassEraseShape; } }

        /// <summary>
        /// Object selection.
        /// </summary>
        public ShortcutCombo ActivateObjectVertexSnapSession_Selection { get { return _activateObjectVertexSnapSession_Selection; } }
        public ShortcutCombo ReplacePrefabsForSelectedObjects_Preview { get { return _replacePrefabsForSelectedObjects_Preview; } }
        public ShortcutCombo ReplacePrefabsForSelectedObjects_Scene { get { return _replacePrefabsForSelectedObjects_Scene; } }
        public ShortcutCombo ActivateMoveGizmo { get { return _activateMoveGizmo; } }
        public ShortcutCombo ActivateRotationGizmo { get { return _activateRotationGizmo; } }
        public ShortcutCombo ActivateScaleGizmo { get { return _activateScaleGizmo; } }
        public ShortcutCombo ActivateObjectSelectionExtrudeGizmo { get { return _activateObjectSelectionExtrudeGizmo; } }
        public ShortcutCombo EnableScrollWheelSizeAdjustmentForSelectionShape { get { return _enableScrollWheelSizeAdjustmentForSelectionShape; } }
        public ShortcutCombo EnableAppendObjectsToSelection { get { return _enableAppendObjectsToSelection; } }
        public ShortcutCombo EnableDeselectObjectsWithSelectionShape { get { return _enableDeselectObjectWithSelectionShape; } }
        public ShortcutCombo DeleteSelectedObjects { get { return _deleteSelectedObjects; } }
        public ShortcutCombo SelectAllObjectsWithSamePrefabAsCurrentSelection { get { return _selectAllObjectsWithSamePrefabAsCurrentSelection; } }
        public ShortcutCombo ProjectSelectedObjects { get { return _projectSelectedObjects; } }
        public ShortcutCombo SelectionGridSnap { get { return _selectionGridSnap; } }
        public ShortcutCombo GrabSelection { get { return _grabSelection; } }
        public ShortcutCombo GrabRotateSelection { get { return _grabRotateSelection; } }
        public ShortcutCombo GrabScaleSelection { get { return _grabScaleSelection; } }
        public ShortcutCombo SetRotationToIdentity { get { return _setRotationToIdentity; } }
        public ShortcutCombo SelectionRotateWorldX { get { return _selectionRotateWorldX; } }
        public ShortcutCombo SelectionRotateWorldY { get { return _selectionRotateWorldY; } }
        public ShortcutCombo SelectionRotateWorldZ { get { return _selectionRotateWorldZ; } }
        public ShortcutCombo ToggleSelectionObject2ObjectSnap { get { return _toggleSelectionObject2ObjectSnap; } }
        public ShortcutCombo EndSelectionObject2ObjectSnap { get { return _endSelectionObject2ObjectSnap; } }
        public ShortcutCombo SnapXZGridToCursorPickPointOnLeftClick_Selection { get { return _snapXZGridToCursorPickPointOnLeftClick_Selection; } }
        #endregion

        #region Constructors
        public AllShortcutCombos()
        {
            CreateCombos();
            EstablishPossibleComboOverlapsForShortcutCollection(_generalShortcutCombos);
            EstablishPossibleComboOverlapsForShortcutCollection(_shortcutCombosForObjectPlacement);
            EstablishPossibleComboOverlapsForShortcutCollection(_shortcutCombosForObjectSnapping);
            EstablishPossibleComboOverlapsForShortcutCollection(_shortcutCombosForObjectSelection);
            EstablishPossibleComboOverlapsForShortcutCollection(_shortcutCombosForObjectErasing);
        }
        #endregion

        #region Private Methods
        private void CreateCombos()
        {
            // General 
            _activateObjectPlacementGUI = CreateGeneralShortcutCombo();
            _activateObjectPlacementGUI.AddKey(KeyCode.A);
            _activateObjectPlacementGUI.NotActiveWhenMouseButtonsPressed = false;

            _activateObjectSelectionGUI = CreateGeneralShortcutCombo();
            _activateObjectSelectionGUI.AddKey(KeyCode.S);
            _activateObjectSelectionGUI.NotActiveWhenMouseButtonsPressed = false;

            _activateObjectErasingGUI = CreateGeneralShortcutCombo();
            _activateObjectErasingGUI.AddKey(KeyCode.D);
            _activateObjectErasingGUI.NotActiveWhenMouseButtonsPressed = false;

            // Mirroring
            _mirrorSelectedObjects = CreateShortcutComboForMirroring();
            _mirrorSelectedObjects.AddKey(KeyCode.M);

            _snapMirrorToCenterOfSnapSurface = CreateShortcutComboForMirroring();
            _snapMirrorToCenterOfSnapSurface.AddKey(KeyCode.Space);

            _disableMirrorSnapping = CreateShortcutComboForMirroring();
            _disableMirrorSnapping.AddKey(KeyCode.LeftControl);

            _resetMirrorRotationToIdentity = CreateShortcutComboForMirroring();
            _resetMirrorRotationToIdentity.AddKey(KeyCode.I);

            _mouseRotateMirrorAroundX = CreateShortcutComboForMirroring();
            _mouseRotateMirrorAroundX.AddKey(KeyCode.X);
            _mouseRotateMirrorAroundX.AddKey(KeyCode.LeftShift);

            _mouseRotateMirrorAroundY = CreateShortcutComboForMirroring();
            _mouseRotateMirrorAroundY.AddKey(KeyCode.Y);
            _mouseRotateMirrorAroundY.AddKey(KeyCode.LeftShift);

            _mouseRotateMirrorAroundZ = CreateShortcutComboForMirroring();
            _mouseRotateMirrorAroundZ.AddKey(KeyCode.Z);
            _mouseRotateMirrorAroundZ.AddKey(KeyCode.LeftShift);

            _mouseRotateMirrorAroundHoverSurfaceNormal = CreateShortcutComboForMirroring();
            _mouseRotateMirrorAroundHoverSurfaceNormal.AddKey(KeyCode.C);
            _mouseRotateMirrorAroundHoverSurfaceNormal.AddKey(KeyCode.LeftShift);

            _keyboardRotateMirrorAroundX = CreateShortcutComboForMirroring();
            _keyboardRotateMirrorAroundX.AddKey(KeyCode.X);

            _keyboardRotateMirrorAroundY = CreateShortcutComboForMirroring();
            _keyboardRotateMirrorAroundY.AddKey(KeyCode.Y);

            _keyboardRotateMirrorAroundZ = CreateShortcutComboForMirroring();
            _keyboardRotateMirrorAroundZ.AddKey(KeyCode.Z);

            _keyboardRotateMirrorAroundHoverSurfaceNormal = CreateShortcutComboForMirroring();
            _keyboardRotateMirrorAroundHoverSurfaceNormal.AddKey(KeyCode.C);

            _offsetMirrorFromHoverSurface = CreateShortcutComboForMirroring();
            _offsetMirrorFromHoverSurface.AddKey(KeyCode.Q);

            _alignMirrorWithHoverSurface = CreateShortcutComboForMirroring();
            _alignMirrorWithHoverSurface.AddKey(KeyCode.B);

            // Object placement
            _toggleDecorSurfaceAlign = CreateShortcutComboForObjectPlacement();
            _toggleDecorSurfaceAlign.AddKey(KeyCode.W);
            _toggleDecorSurfaceAlign.AddKey(KeyCode.LeftShift);

            _pickPrefabFromScene = CreateShortcutComboForObjectPlacement();
            _pickPrefabFromScene.AddKey(KeyCode.R);

            _pickPrefabWithTransform = CreateShortcutComboForObjectPlacement();
            _pickPrefabWithTransform.AddKey(KeyCode.LeftShift);

            _activateObjectVertexSnapSession_Placement = CreateShortcutComboForObjectPlacement();
            _activateObjectVertexSnapSession_Placement.AddKey(KeyCode.V);
            _activateObjectVertexSnapSession_Placement.NotActiveWhenRightMouseButtonPressed = false;

            _placeGuideBehindSurfacePlane = CreateShortcutComboForObjectPlacement();
            _placeGuideBehindSurfacePlane.AddKey(KeyCode.N);

            _activateDecorPaintPlacement = CreateShortcutComboForObjectPlacement();
            _activateDecorPaintPlacement.AddKey(KeyCode.Alpha1);

            _activatePointAndClickPlacement = CreateShortcutComboForObjectPlacement();
            _activatePointAndClickPlacement.AddKey(KeyCode.Alpha2);

            _activatePathPlacement = CreateShortcutComboForObjectPlacement();
            _activatePathPlacement.AddKey(KeyCode.Alpha3);

            _activateBlockPlacement = CreateShortcutComboForObjectPlacement();
            _activateBlockPlacement.AddKey(KeyCode.Alpha4);

            _offsetGuideFromPlacementSurface = CreateShortcutComboForObjectPlacement();
            _offsetGuideFromPlacementSurface.AddKey(KeyCode.Q);

            _lockObjectPlacement = CreateShortcutComboForObjectPlacement();
            _lockObjectPlacement.AddKey(KeyCode.L);

            _alignPlacementGuideToNextAxis = CreateShortcutComboForObjectPlacement();
            _alignPlacementGuideToNextAxis.AddKey(KeyCode.B);

            _setPlacementGuideRotationToIdentity = CreateShortcutComboForObjectPlacement();
            _setPlacementGuideRotationToIdentity.AddKey(KeyCode.I);

            _setPlacementGuideScaleToOriginal = CreateShortcutComboForObjectPlacement();
            _setPlacementGuideScaleToOriginal.AddKey(KeyCode.O);

            _cycleThroughPrefabsInActiveCategoryUsingMouseScrollWheel = CreateShortcutComboForObjectPlacement();
            _cycleThroughPrefabsInActiveCategoryUsingMouseScrollWheel.AddKey(KeyCode.LeftShift);

            _cycleThroughPrefabCategoriesUsingMouseScrollWheel = CreateShortcutComboForObjectPlacement();
            _cycleThroughPrefabCategoriesUsingMouseScrollWheel.AddKey(KeyCode.LeftShift);
            _cycleThroughPrefabCategoriesUsingMouseScrollWheel.AddKey(KeyCode.LeftAlt);

            _cancelManualPathConstruction = CreateShortcutComboForObjectPlacement();
            _cancelManualPathConstruction.AddKey(KeyCode.Escape);

            _placePathOnClick = CreateShortcutComboForObjectPlacement();
            _placePathOnClick.AddKey(KeyCode.LeftShift);
            _placePathOnClick.AddMouseButton(MouseButton.Left);

            _manualRaisePath = CreateShortcutComboForObjectPlacement();
            _manualRaisePath.AddKey(KeyCode.G);

            _manualLowerPath = CreateShortcutComboForObjectPlacement();
            _manualLowerPath.AddKey(KeyCode.H);

            _manualRemoveLast2SegmentsInPath = CreateShortcutComboForObjectPlacement();
            _manualRemoveLast2SegmentsInPath.AddKey(KeyCode.R);

            _nextPathExtensionPlane = CreateShortcutComboForObjectPlacement();
            _nextPathExtensionPlane.AddKey(KeyCode.E);

            _beginManualPathConstruction = CreateShortcutComboForObjectPlacement();
            _beginManualPathConstruction.AddMouseButton(MouseButton.Left);

            _manualAttach2NewSegmentsToPath = CreateShortcutComboForObjectPlacement();
            _manualAttach2NewSegmentsToPath.AddMouseButton(MouseButton.Left);

            _beginManualBlockConstruction = CreateShortcutComboForObjectPlacement();
            _beginManualBlockConstruction.AddMouseButton(MouseButton.Left);

            _cancelManualBlockConstruction = CreateShortcutComboForObjectPlacement();
            _cancelManualBlockConstruction.AddKey(KeyCode.Escape);

            _enable1To1RatioBlockAdjustment = CreateShortcutComboForObjectPlacement();
            _enable1To1RatioBlockAdjustment.AddKey(KeyCode.LeftShift);

            _placeBlockOnClick = CreateShortcutComboForObjectPlacement();
            _placeBlockOnClick.AddMouseButton(MouseButton.Left);

            _manualRaiseBlock = CreateShortcutComboForObjectPlacement();
            _manualRaiseBlock.AddKey(KeyCode.G);

            _manualLowerBlock = CreateShortcutComboForObjectPlacement();
            _manualLowerBlock.AddKey(KeyCode.H);

            _nextBlockExtensionPlane = CreateShortcutComboForObjectPlacement();
            _nextBlockExtensionPlane.AddKey(KeyCode.E);

            _cycleThroughProjectedBoxFaceGuidePivotPoints = CreateShortcutComboForObjectPlacement();
            _cycleThroughProjectedBoxFaceGuidePivotPoints.AddKey(KeyCode.J);

            _toggleUseOriginalPivotForSnapping = CreateShortcutComboForObjectPlacement();
            _toggleUseOriginalPivotForSnapping.AddKey(KeyCode.Alpha0);

            _enableScrollWheelSizeAdjustmentForDecorPaintBrush = CreateShortcutComboForObjectPlacement();
            _enableScrollWheelSizeAdjustmentForDecorPaintBrush.AddKey(KeyCode.LeftControl);

            _scrollWheelTilePaintBrushSize = CreateShortcutComboForObjectPlacement();
            _scrollWheelTilePaintBrushSize.AddKey(KeyCode.LeftControl);

            _keyboardRotatePlacementGuideAroundX = CreateShortcutComboForObjectPlacement();
            _keyboardRotatePlacementGuideAroundX.AddKey(KeyCode.X);

            _keyboardRotatePlacementGuideAroundY = CreateShortcutComboForObjectPlacement();
            _keyboardRotatePlacementGuideAroundY.AddKey(KeyCode.Y);

            _keyboardRotatePlacementGuideAroundZ = CreateShortcutComboForObjectPlacement();
            _keyboardRotatePlacementGuideAroundZ.AddKey(KeyCode.Z);

            _keyboardRotatePlacementGuideAroundPlacementSurfaceNormal = CreateShortcutComboForObjectPlacement();
            _keyboardRotatePlacementGuideAroundPlacementSurfaceNormal.AddKey(KeyCode.C);

            _mouseRotatePlacementGuideAroundX = CreateShortcutComboForObjectPlacement();
            _mouseRotatePlacementGuideAroundX.AddKey(KeyCode.X);
            _mouseRotatePlacementGuideAroundX.AddKey(KeyCode.LeftShift);

            _mouseRotatePlacementGuideAroundY = CreateShortcutComboForObjectPlacement();
            _mouseRotatePlacementGuideAroundY.AddKey(KeyCode.Y);
            _mouseRotatePlacementGuideAroundY.AddKey(KeyCode.LeftShift);

            _mouseRotatePlacementGuideAroundZ = CreateShortcutComboForObjectPlacement();
            _mouseRotatePlacementGuideAroundZ.AddKey(KeyCode.Z);
            _mouseRotatePlacementGuideAroundZ.AddKey(KeyCode.LeftShift);

            _mouseRotatePlacementGuideAroundPlacementSurfaceNormal = CreateShortcutComboForObjectPlacement();
            _mouseRotatePlacementGuideAroundPlacementSurfaceNormal.AddKey(KeyCode.C);
            _mouseRotatePlacementGuideAroundPlacementSurfaceNormal.AddKey(KeyCode.LeftShift);

            _mousePlacementGuideUniformScale = CreateShortcutComboForObjectPlacement();
            _mousePlacementGuideUniformScale.AddKey(KeyCode.LeftControl);
            _mousePlacementGuideUniformScale.AddKey(KeyCode.LeftShift);

            // Object snapping
            _toggleEnableObjectSurfaceGrid = CreateShortcutComboForObjectSnapping();
            _toggleEnableObjectSurfaceGrid.AddKey(KeyCode.W);
            _toggleEnableObjectSurfaceGrid.NotActiveWhenMouseButtonsPressed = true;

            _enableScrollWheelDesiredCellSizeAdjustmentForObjectColliderSnapSurfaceGrid = CreateShortcutComboForObjectSnapping();
            _enableScrollWheelDesiredCellSizeAdjustmentForObjectColliderSnapSurfaceGrid.AddKey(KeyCode.LeftControl);
            _enableScrollWheelDesiredCellSizeAdjustmentForObjectColliderSnapSurfaceGrid.AddKey(KeyCode.Space);

            _snapCenterToCenter = CreateShortcutComboForObjectSnapping();
            _snapCenterToCenter.AddKey(KeyCode.Space);

            _keepSnappedHierarchyInSnapSurfaceArea = CreateShortcutComboForObjectSnapping();
            _keepSnappedHierarchyInSnapSurfaceArea.AddKey(KeyCode.LeftControl);

            _snapXZGridToCursorPickPointOnLeftClick_Placement = CreateShortcutComboForObjectSnapping();
            _snapXZGridToCursorPickPointOnLeftClick_Placement.AddKey(KeyCode.T);

            _adjustXZGridCellSizeToGuideSize = CreateShortcutComboForObjectSnapping();
            _adjustXZGridCellSizeToGuideSize.AddKey(KeyCode.K);

            _stepXZGridUp = CreateShortcutComboForObjectSnapping();
            _stepXZGridUp.AddKey(KeyCode.Alpha6);

            _stepXZGridDown = CreateShortcutComboForObjectSnapping();
            _stepXZGridDown.AddKey(KeyCode.Alpha5);

            _togglePlacementObject2ObjectSnap = CreateShortcutComboForObjectSnapping();
            _togglePlacementObject2ObjectSnap.AddKey(KeyCode.U);

            // Object erasing
            _enableScrollWheelSizeAdjustmentForMassEraseShape = CreateShortcutComboForObjectErasing();
            _enableScrollWheelSizeAdjustmentForMassEraseShape.AddKey(KeyCode.LeftControl);

            // Object selection
            _activateObjectVertexSnapSession_Selection = CreateShortcutComboForObjectSelection();
            _activateObjectVertexSnapSession_Selection.AddKey(KeyCode.V);

            _replacePrefabsForSelectedObjects_Preview = CreateShortcutComboForObjectSelection();
            _replacePrefabsForSelectedObjects_Preview.AddKey(KeyCode.LeftShift);

            _replacePrefabsForSelectedObjects_Scene = CreateShortcutComboForObjectSelection();
            _replacePrefabsForSelectedObjects_Scene.AddKey(KeyCode.Space);

            _activateMoveGizmo = CreateShortcutComboForObjectSelection();
            _activateMoveGizmo.AddKey(KeyCode.W);
            _activateMoveGizmo.NotActiveWhenMouseButtonsPressed = false;

            _activateRotationGizmo = CreateShortcutComboForObjectSelection();
            _activateRotationGizmo.AddKey(KeyCode.E);
            _activateRotationGizmo.NotActiveWhenMouseButtonsPressed = false;

            _activateScaleGizmo = CreateShortcutComboForObjectSelection();
            _activateScaleGizmo.AddKey(KeyCode.R);
            _activateScaleGizmo.NotActiveWhenMouseButtonsPressed = false;

            _activateObjectSelectionExtrudeGizmo = CreateShortcutComboForObjectSelection();
            _activateObjectSelectionExtrudeGizmo.AddKey(KeyCode.Q);
            _activateObjectSelectionExtrudeGizmo.NotActiveWhenMouseButtonsPressed = false;

            _enableScrollWheelSizeAdjustmentForSelectionShape = CreateShortcutComboForObjectSelection();
            _enableScrollWheelSizeAdjustmentForSelectionShape.AddKey(KeyCode.LeftControl);

            _enableAppendObjectsToSelection = CreateShortcutComboForObjectSelection();
            _enableAppendObjectsToSelection.AddKey(KeyCode.LeftControl);

            _enableDeselectObjectWithSelectionShape = CreateShortcutComboForObjectSelection();
            _enableDeselectObjectWithSelectionShape.AddKey(KeyCode.LeftShift);

            _deleteSelectedObjects = CreateShortcutComboForObjectSelection();
            _deleteSelectedObjects.AddKey(KeyCode.Delete);

            _selectAllObjectsWithSamePrefabAsCurrentSelection = CreateShortcutComboForObjectSelection();
            _selectAllObjectsWithSamePrefabAsCurrentSelection.AddKey(KeyCode.G);

            _projectSelectedObjects = CreateShortcutComboForObjectSelection();
            _projectSelectedObjects.AddKey(KeyCode.P);

            _selectionGridSnap = CreateShortcutComboForObjectSelection();
            _selectionGridSnap.AddKey(KeyCode.B);

            _grabSelection = CreateShortcutComboForObjectSelection();
            _grabSelection.AddKey(KeyCode.C);

            _grabRotateSelection = CreateShortcutComboForObjectSelection();
            _grabRotateSelection.AddKey(KeyCode.LeftShift);

            _grabScaleSelection = CreateShortcutComboForObjectSelection();
            _grabScaleSelection.AddKey(KeyCode.LeftControl);

            _setRotationToIdentity = CreateShortcutComboForObjectSelection();
            _setRotationToIdentity.AddKey(KeyCode.I);

            _selectionRotateWorldX = CreateShortcutComboForObjectSelection();
            _selectionRotateWorldX.AddKey(KeyCode.X);

            _selectionRotateWorldY = CreateShortcutComboForObjectSelection();
            _selectionRotateWorldY.AddKey(KeyCode.Y);

            _selectionRotateWorldZ = CreateShortcutComboForObjectSelection();
            _selectionRotateWorldZ.AddKey(KeyCode.Z);

            _toggleSelectionObject2ObjectSnap = CreateShortcutComboForObjectSelection();
            _toggleSelectionObject2ObjectSnap.AddKey(KeyCode.U);

            _endSelectionObject2ObjectSnap = CreateShortcutComboForObjectSelection();
            _endSelectionObject2ObjectSnap.AddMouseButton(MouseButton.Left);

            _snapXZGridToCursorPickPointOnLeftClick_Selection = CreateShortcutComboForObjectSelection();
            _snapXZGridToCursorPickPointOnLeftClick_Selection.AddKey(KeyCode.T);
        }

        private ShortcutCombo CreateGeneralShortcutCombo()
        {
            var shortcutCombo = new ShortcutCombo();
            _generalShortcutCombos.Add(shortcutCombo);

            return shortcutCombo;
        }

        private ShortcutCombo CreateShortcutComboForMirroring()
        {
            var shortcutCombo = new ShortcutCombo();
            _shortcutCombosForMirroring.Add(shortcutCombo);

            return shortcutCombo;
        }

        private ShortcutCombo CreateShortcutComboForObjectPlacement()
        {
            var shortcutCombo = new ShortcutCombo();
            _shortcutCombosForObjectPlacement.Add(shortcutCombo);

            return shortcutCombo;
        }

        private ShortcutCombo CreateShortcutComboForObjectSnapping()
        {
            var shortcutCombo = new ShortcutCombo();
            _shortcutCombosForObjectSnapping.Add(shortcutCombo);

            return shortcutCombo;
        }

        private ShortcutCombo CreateShortcutComboForObjectErasing()
        {
            var shortcutCombo = new ShortcutCombo();
            _shortcutCombosForObjectErasing.Add(shortcutCombo);

            return shortcutCombo;
        }

        private ShortcutCombo CreateShortcutComboForObjectSelection()
        {
            var shortcutCombo = new ShortcutCombo();
            _shortcutCombosForObjectSelection.Add(shortcutCombo);

            return shortcutCombo;
        }

        private void EstablishPossibleComboOverlapsForShortcutCollection(List<ShortcutCombo> shortcutCombos)
        {
            foreach (ShortcutCombo combo in shortcutCombos)
            {
                foreach (ShortcutCombo possibleOverlap in shortcutCombos)
                {
                    if (ReferenceEquals(combo, possibleOverlap)) continue;
                    if (combo.CanOverlapWith(possibleOverlap)) combo.AddPossibleOverlap(possibleOverlap);
                }
            }
        }
        #endregion
    }
}
#endif