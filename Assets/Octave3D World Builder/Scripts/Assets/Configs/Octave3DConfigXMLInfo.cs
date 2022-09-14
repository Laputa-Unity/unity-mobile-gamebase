#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class Octave3DConfigXMLInfo
    {
        #region Public Static Properties
        public static string RootNode { get { return "Octave3DConfig"; } }

        public static string SnapSettingsNode { get { return "SnapSettings"; } }
        public static string UseOriginalPivotNode { get { return "UseOriginalPivot"; } }
        public static string EnableObjectSurfaceGrid { get { return "EnableObjectSurfaceGrid"; } }
        public static string SnapToCursorHitPointNode { get { return "SnapToCursorHitPoint"; } }
        public static string SnapCenterToCenterGridNode { get { return "SnapCenterToCenter_Grid"; } }
        public static string SnapCenterToCenterObjectSurfaceNode { get { return "SnapCenterToCenter_ObjectSurface"; } }
        public static string EnableObjectToObjectSnapNode { get { return "EnableObjectToObjectSnap"; } }
        public static string ObjectToObjectSnapModeNode { get { return "ObjectToObjectSnapMode"; } }
        public static string ObjectToObjectSnapEpsilonNode { get { return "ObjectToObjectSnapEpsilon"; } }
        public static string SnapGridYOffsetNode { get { return "SnapGridYOffset"; } }
        public static string SnapGridYOffsetStepNode { get { return "SnapGridYOffsetStep"; } }
        public static string SnapGridXOffsetNode { get { return "SnapGridXOffset"; } }
        public static string SnapGridZOffsetNode { get { return "SnapGridZOffset"; } }
        public static string ObjectSurfaceGridDesiredCellSizeNode { get { return "ObjectSurfaceGridDesiredCellSize"; } }
        public static string SnapGridCellSizeXNode { get { return "SnapGridCellSizeX"; } }
        public static string SnapGridCellSizeZNode { get { return "SnapGridCellSizeZ"; } }

        public static string ObjectSelectionSettingsNode { get { return "ObjectSelectionSettings"; } }
        public static string ObjectSelectionAttachToObjectGroupNode { get { return "AttachToObjectGroup"; } }
        public static string ObjectSelectionDestinationObjectGroupNode { get { return "DestinationObjectGroup"; } }
        public static string ObjectSelectionXRotationStepNode { get { return "XRotationStep"; } }
        public static string ObjectSelectionYRotationStepNode { get { return "YRotationStep"; } }
        public static string ObjectSelectionZRotationStepNode { get { return "ZRotationStep"; } }
        public static string ObjectSelectionRotateAroundCenterNode { get { return "RotateAroundCenter"; } }
        public static string ObjectSelectionObject2ObjectSnapSettingsNode { get { return "Object2ObjectSnapSettings"; } }
        public static string ObjectSelectionObj2ObjSnapEpsNode { get { return "SnapEpsilon"; } }
        public static string ObjectSelectionObj2ObjSnapCanHoverObjectsNode { get { return "CanHoverObjects"; } }
        public static string ObjectSelectionAllowPartialOverlapNode { get { return "AllowPartialOverlap"; } }
        public static string ObjectSelectionShapeTypeNode { get { return "ObjectSelectionShapeType"; } }
        public static string ObjectSelectionUpdateModeNode { get { return "ObjectSelectionUpdateMode"; } }
        public static string ObjectSelectionModeNode { get { return "ObjectSelectionMode"; } }
        public static string ObjectSelectionPaint_ShapePixelWidthNode { get { return "ObjectSelectionPaint_ShapePixelWidth"; } }
        public static string ObjectSelectionPaint_ShapePixelHeightNode { get { return "ObjectSelectionPaint_ShapePixelHeight"; } }
        public static string ObjectSelectionPaint_SizeAdjustmentSpeedScrollWheelNode { get { return "ObjectSelectionPaint_SizeAdjustmentSpeedScrollWheel"; } }

        public static string ObjectErasingSettingsNode { get { return "ObjectErasingSettings"; } }
        public static string ObjectErasingAllowUndoRedoNode { get { return "AllowUndo_Redo"; } }
        public static string ObjectEraseDelayNode { get { return "EraseDelay"; } }
        public static string ObjectEraseModeNode { get { return "EraseMode"; } }
        public static string ObjectEraseOnlyMeshObjectsNode { get { return "EraseOnlyMeshObjects"; } }
        public static string ObjectMassErase2D_CircleRadiusNode { get { return "MassErase2D_CircleRadius"; } }
        public static string ObjectMassErase2D_RadiusAdjustmentSpeedScrollWheelNode { get { return "MassErase2D_RadiusAdjustmentSpeedScrollWheel"; } }
        public static string ObjectMassErase2D_AllowPartialOverlap { get { return "MassErase2D_AllowPartialOverlap"; } }
        public static string ObjectMassErase3D_CircleRadiusNode { get { return "MassErase3D_CircleRadius"; } }
        public static string ObjectMassErase3D_RadiusAdjustmentSpeedScrollWheelNode { get { return "MassErase3D_RadiusAdjustmentSpeedScrollWheel"; } }
        public static string ObjectMassErase3D_AllowPartialOverlap { get { return "MassErase3D_AllowPartialOverlap"; } }

        public static string ObjectPlacementMirrorLookAndFeelNode { get { return "ObjectPlacementMirrorLookAndFeel"; } }
        public static string ObjectSelectionMirrorLookAndFeelNode { get { return "ObjectSelectionMirrorLookAndFeel"; } }
        public static string MirrorWidthNode { get { return "MirrorWidth"; } }
        public static string MirrorHeightNode { get { return "MirrorHeight"; } }
        public static string MirrorHasInfiniteWidthNode { get { return "HasInfiniteWidth"; } }
        public static string MirrorHasInfiniteHeightNode { get { return "HasInfiniteHeight"; } }
        public static string MirrorColorNode { get { return "MirrorColor"; } }
        public static string MirrorBorderColorNode { get { return "MirrorBorderColor"; } }
        public static string MirroredBoxColorNode { get { return "MirroredBoxColor"; } }
        public static string MirroredBoxBorderColorNode { get { return "MirroredBoxBorderColor"; } }

        public static string SnapLookAndFeelNode { get { return "SnapLookAndFeel"; } }
        public static string XZGridLookAndFeelNode { get { return "XZGridLookAndFeel"; } }
        public static string IsXZGridVisibleNode { get { return "IsVisible"; } }
        public static string XZGridCellLineColorNode { get { return "CellLineColor"; } }
        public static string XZGridCellLineThicknessNode { get { return "CellLineThickness"; } }
        public static string XZGridPlaneColorNode { get { return "PlaneColor"; } }
        public static string XZGridCoordSystemLookAndFeelNode { get { return "XZGridCoordSystemLookAndFeel"; } }
        public static string IsXZGridCoordSystemVisibleNode { get { return "IsVisible"; } }
        public static string IsXZGridCoordSystemAxisVisibleNode { get { return "IsVisible"; } }
        public static string IsXZGridCoordSystemAxisInfiniteNode { get { return "IsInfinite"; } }
        public static string XZGridCoordSystemAxisFiniteSizeNode { get { return "FiniteSize"; } }
        public static string XZGridCoordSystemAxisColorNode { get { return "Color"; } }
        public static string ObjectSnapSurfaceGridLookAndFeelNode { get { return "ObjectSnapSurfaceGridLookAndFeel"; } }
        public static string IsObjectSnapSurfaceGridVisibleNode { get { return "IsVisible"; } }
        public static string ObjectSnapSurfaceGridCellLineColorNode { get { return "CellLineColor"; } }
        public static string ObjectSnapSurfaceGridCellLineThicknessNode { get { return "CellLineThickness"; } }
        public static string ObjectSnapSurfaceGridPlaneColorNode { get { return "PlaneColor"; } }

        public static string ObjectPlacementLookAndFeelNode { get { return "ObjectPlacementLookAndFeel"; } }
        public static string GuidePivotPointsLookAndFeelNode { get { return "GuidePivotPoints"; } }
        public static string GuidePivotPointsShapeTypeNode { get { return "ShapeType"; } }
        public static string GuidePivotPointsShapeSizeInPixelsNode { get { return "ShapeSizeInPixels"; } }
        public static string DrawPivotPointProjectionLinesNode { get { return "DrawProjectionLines"; } }
        public static string PivotPointProjectionLineColorNode { get { return "ProjectionLineColor"; } }
        public static string DrawPivotPointConnectionLinesNode { get { return "DrawConnectionLines"; } }
        public static string PivotPointConnectionLineColorNode { get { return "ConnectionLineColor"; } }
        public static string ActivePivotPointLookAndFeelNode { get { return "ActivePivotPoint"; } }
        public static string InactivePivotPointLookAndFeelNode { get { return "InactivePivotPoint"; } }
        public static string PivotPointIsVisibleNode { get { return "IsVisible"; } }
        public static string PivotPointFillColorNode { get { return "FillColor"; } }
        public static string PivotPointBorderColorNode { get { return "BorderColor"; } }
        public static string PivotPointScaleNode { get { return "Scale"; } }
        public static string ObjectVertexSnappingLookAndFeelNode { get { return "ObjectVertexSnapping"; } }
        public static string ObjectVertexSnappingDrawSrcVertexNode { get { return "DrawSourceVertex"; } }
        public static string ObjectVertexSnappingSrcVertexFillColorNode { get { return "SourceVertexFillColor"; } }
        public static string ObjectVertexSnappingSrcVertexBorderColorNode { get { return "SourceVertexBorderColor"; } }
        public static string ObjectVertexSnappingSrcVertexPixelRadiusNode { get { return "SourceVertexRadiusInPixels"; } }
        public static string DecorPaintLookAndFeelNode { get { return "DecorPaint"; } }
        public static string DecorPaintBrushCircleLookAndFeelNode { get { return "BrushCircle"; } }
        public static string DecorPaintBrushCircleBorderLineColorNode { get { return "BorderColor"; } }
        public static string PathLookAndFeelNode { get { return "Path"; } }
        public static string PathBorderLineColorNode { get { return "BorderColor"; } }
        public static string ExtensionPlaneLookAndFeelNode { get { return "ExtensionPlane"; } }
        public static string ExtensionPlaneScaleNode { get { return "Scale"; } }
        public static string ExtensionPlaneColorNode { get { return "Color"; } }
        public static string ExtensionPlaneBorderColorNode { get { return "BorderColor"; } }
        public static string ExtensionPlaneNormalLineLengthNode { get { return "NormalLineLength"; } }
        public static string ExtensionPlaneNormalLineColorNode { get { return "NormalLineColor"; } }
        public static string BlockLookAndFeelNode { get { return "Block"; } }
        public static string BlockBorderLineColorNode { get { return "BorderColor"; } }
        public static string BlockTextColorNode { get { return "TextColor"; } }
        public static string BlockFontSizeNode { get { return "FontSize"; } }
        public static string BlockBoldTextNode { get { return "Bold"; } }

        public static string ObjectSelectionLookAndFeelNode { get { return "ObjectSelectionLookAndFeel"; } }
        public static string ObjectSelectionBoxEdgeDrawModeNode { get { return "BoxEdgeDrawMode"; } }
        public static string ObjectSelectionBoxCornerEdgeLengthPercentageNode { get { return "CornerEdgeLengthPercentage"; } }
        public static string ObjectSelectionBoxEdgeColorNode { get { return "EdgeColor"; } }
        public static string ObjectSelectionBoxColorNode { get { return "BoxColor"; } }
        public static string ObjectSelectionBoxScaleNode { get { return "BoxScale"; } }
        public static string ObjectSelectionRectLookAndFeelNode { get { return "SelectionRectangle"; } }
        public static string ObjectSelectionEllipseLookAndFeelNode { get { return "SelectionEllipse"; } }
        public static string ObjectSelectionShapeFillColorNode { get { return "FillColor"; } }
        public static string ObjectSelectionShapeBorderColorNode { get { return "BorderColor"; } }

        public static string ObjectErasingLookAndFeelNode { get { return "ObjectErasingLookAndFeel"; } }
        public static string ObjectErasing2DCircleLookAndFeelNode { get { return "Mass2DEraseCircle"; } }
        public static string ObjectErasing3DCircleLookAndFeelNode { get { return "Mass3DEraseCircle"; } }
        public static string ObjectErasingShapeFillColorNode { get { return "FillColor"; } }
        public static string ObjectErasingShapeBorderColorNode { get { return "BorderColor"; } }
        #endregion
    }
}
#endif